using System;
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyDive : MonoBehaviour
{
    public static List<EnemyDive> AllDivers = new List<EnemyDive>();
    
    private EnemyStateController _state;
    private EnemySlot _slot;
    private Animator _animator;
    private Camera _worldCamera;
    private ReturnToGroup _returnToGroup;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private Transform visualTransform;

    [Header("Dive Rotation")] [SerializeField]
    private float tiltStartT = 0.45f;

    [SerializeField] private float tiltBlendSeconds = 0.25f;
    [SerializeField] private float diveUprightZ = -180f;
    [SerializeField] private float diveRotationOffsetDegrees;

    [Header("Dive Motion")] [SerializeField]
    private float diveSpeedUnitsPerSecond = 5.5f;

    [Header("Turn Animation")] [SerializeField]
    private float loopEndT = 0.22f;

    [SerializeField] private float loopHoldZ;

    [Header("Second Turn During Dive")] [SerializeField]
    private float secondTurnTriggerT = 0.45f;

    [Header("Bounds")] [SerializeField] private float bottomExitPaddingUnits = 0.5f;
    [SerializeField] private float sideExitPaddingUnits = 0.5f;

    [Header("Dive Shooting")] 
    [SerializeField] private int minShotsPerDive = 1;
    [SerializeField] private int maxShotsPerDive = 3;
    [SerializeField] private float timeBetweenShots = 0.5f;
    [SerializeField] private float shootStart = 0.24f;
    private int _currentDiveShotCount; 
    private int _shotsFiredCounter;   
    private float _shootTimer;
    
    private SplineContainer _activeTemplateSpline;
    private float _t;
    private float _splineLength;
    private float _angleTransition;
    private Vector3 _diveStartPoint;

    private bool _isRightFormation;
    private bool _loopFinished;
    private bool _secondTurnTriggered;

    private bool _savedFlipX;
    private bool _flipSaved;
    private bool _isSpriteFlipped;
    
    private bool _isReturningStarted;
    private float _currentSpeed;
    
    private const float MIN_SPLINE_THRESHOLD = 0.0001f;
    private const float MIN_TANGENT_THRESHOLD = 0.000001f;
    private const float ROTATION_DEGREES_OFFSET = 90f;
    private const float SPLINE_END_T = 1f;

    private void OnEnable()
    {
        if (!AllDivers.Contains(this)) 
        {
            AllDivers.Add(this);
        }
    }
    
    private void OnDisable()
    {
        AllDivers.Remove(this);
    }

    private void Awake()
    {
        _state = GetComponent<EnemyStateController>();
        _slot = GetComponent<EnemySlot>();
        _animator = GetComponentInChildren<Animator>(true);
        _worldCamera = Camera.main;
        _returnToGroup = GetComponent<ReturnToGroup>();

        if (visualTransform == null && _animator != null)
            visualTransform = _animator.transform;

        if (visualTransform == null)
            visualTransform = transform;

        _spriteRenderer = visualTransform.GetComponentInChildren<SpriteRenderer>(true);
        if (_spriteRenderer == null)
            _spriteRenderer = visualTransform.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_state == null)
        {
            return;
        }
        if (_state.CurrentState != EnemyState.Diving)
        {
            return; 
        }
        UpdateDivingOnSpline();
    }

    private bool BeginDive(SplineContainer templateSpline)
    {
        if (templateSpline == null)
        {
            return false;
        }
        if (_state == null)
        {
            return false;
        }
        if (_state.CurrentState != EnemyState.InFormation)
        {
            return false; 
        }
        if (_slot == null)
        {
            return false;
        }
        float homeLocalX = _slot.HomeLocalPosition.x;
        _isRightFormation = homeLocalX > 0f;
        SoundManager.Instance.PlayDivingSound();
        _activeTemplateSpline = templateSpline;
        _diveStartPoint = transform.position;
        _t = 0f;
        _splineLength = 0f;
        _angleTransition = 0f;
        _loopFinished = false;
        _secondTurnTriggered = false;
        _flipSaved = false;
        _isSpriteFlipped = false;
        _isReturningStarted = false;
        if (_activeTemplateSpline != null)
        {
            _splineLength = DiveSplineUtils.ApproxLength(_activeTemplateSpline);
        }
        ApplyLeftFlip();

        if (visualTransform != null)
            visualTransform.rotation = Quaternion.Euler(0f, 0f, loopHoldZ);
        if (_animator != null)
        {
            _animator.speed = 0f;
        }
        _currentSpeed = diveSpeedUnitsPerSecond * 0.3f;
        _currentDiveShotCount = Random.Range(minShotsPerDive, maxShotsPerDive +1);
        _shotsFiredCounter = 0;
        _shootTimer = 0f;
        _state.SetState(EnemyState.Diving);
        transform.SetParent(null);
        return true; 
    }
    
    public bool TryStartDive()
    {
        float homeLocalX = (_slot != null) ? _slot.HomeLocalPosition.x : 0f;
        SplineContainer chosen =
            (DiveSplineLibrary.Instance != null) ? DiveSplineLibrary.Instance.Pick(homeLocalX) : null;
        return BeginDive(chosen);
    }

    public bool TryStartDiveOn(SplineContainer sharedTemplate)
    {
        return BeginDive(sharedTemplate);
    }

    public SplineContainer PickDiveTemplate()
    {
        if (_slot == null || DiveSplineLibrary.Instance == null)
        {
            return null;
        }

        return DiveSplineLibrary.Instance.Pick(_slot.HomeLocalPosition.x);
    }
    
    private void ApplyLeftFlip()
    {
        if (_isRightFormation)
            return;

        if (_spriteRenderer == null)
            return;
        if (!_flipSaved)
        {
            _savedFlipX = _spriteRenderer.flipX;
            _flipSaved = true;
        }

        _spriteRenderer.flipX = !_savedFlipX;
        _isSpriteFlipped = true;
    }

    private void RestoreFlip()
    {
        if (!_isSpriteFlipped)
            return;

        if (_spriteRenderer == null)
            return;

        _spriteRenderer.flipX = _savedFlipX;
        _isSpriteFlipped = false;
    }
    
    private void UpdateDivingOnSpline()
    {
        if (_state == null || _state.CurrentState != EnemyState.Diving)
            return;
        HandleSplineMovement();
        DiveShooting();
        DiveRotation();
        if (_t >= SPLINE_END_T || HasExitedDiveBounds())
            BeginReturn();
    }

private void HandleSplineMovement()
{
    _currentSpeed = Mathf.MoveTowards(_currentSpeed, diveSpeedUnitsPerSecond, Time.deltaTime * 2.5f);

    if (_splineLength <= MIN_SPLINE_THRESHOLD)
        _splineLength = DiveSplineUtils.ApproxLength(_activeTemplateSpline);
    float deltaT = (_currentSpeed * Time.deltaTime) / _splineLength;
    _t = Mathf.Clamp01(_t + deltaT);
    
    Vector3 offset = DiveSplineUtils.EvaluateOffsetLocal(_activeTemplateSpline, _t);
    transform.position = new Vector3(
        _diveStartPoint.x + offset.x,
        _diveStartPoint.y + offset.y,
        transform.position.z
    );
}

private void DiveShooting()
{
    if (_t >= shootStart && _shotsFiredCounter < _currentDiveShotCount)
    {
        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0f)
        {
            IEnemyShooter shooter = GetComponent<IEnemyShooter>();
            if (shooter != null)
            {
                shooter.ExecuteShoot();
                _shotsFiredCounter++;
                _shootTimer = timeBetweenShots; 
            }
        }
    }
}

private void DiveRotation()
{
    if (!_loopFinished)
    {
        Vector3 tanLoop = DiveSplineUtils.EvaluateTangentLocal(_activeTemplateSpline, _t);
        float zLoop = loopHoldZ;
        if (tanLoop.sqrMagnitude > MIN_TANGENT_THRESHOLD)
            zLoop = Mathf.Atan2(tanLoop.y, tanLoop.x) * Mathf.Rad2Deg - ROTATION_DEGREES_OFFSET + diveRotationOffsetDegrees;
        if (visualTransform != null)
            visualTransform.rotation = Quaternion.Euler(0f, 0f, zLoop);
        if (_t >= loopEndT)
        {
            _loopFinished = true;
            _angleTransition = 0f;
            RestoreFlip();
            if (visualTransform != null)
                visualTransform.rotation = Quaternion.Euler(0f, 0f, diveUprightZ);
        }
        return;
    }
    if (!_secondTurnTriggered && _t >= secondTurnTriggerT)
    {
        _secondTurnTriggered = true;
    }
    Vector3 tan = DiveSplineUtils.EvaluateTangentLocal(_activeTemplateSpline, _t);
    float targetZ = diveUprightZ;
    if (tan.sqrMagnitude > MIN_TANGENT_THRESHOLD)
        targetZ = Mathf.Atan2(tan.y, tan.x) * Mathf.Rad2Deg - ROTATION_DEGREES_OFFSET + diveRotationOffsetDegrees;
    float wantedBlend = (_t >= tiltStartT) ? 1f : 0f;

    _angleTransition = (tiltBlendSeconds <= MIN_SPLINE_THRESHOLD)
        ? wantedBlend
        : Mathf.MoveTowards(_angleTransition, wantedBlend, Time.deltaTime / tiltBlendSeconds);
    float z = Mathf.LerpAngle(diveUprightZ, targetZ, _angleTransition);
    if (visualTransform != null)
        visualTransform.rotation = Quaternion.Euler(0f, 0f, z);
}
    private void BeginReturn()
    {
        if (_isReturningStarted)
            return;

        _isReturningStarted = true;

        RestoreFlip();

        if (_returnToGroup == null)
            return;

        _returnToGroup.BeginReturn();
    }
    
    private bool HasExitedDiveBounds()
    {
        if (_worldCamera == null)
            return false;

        Vector3 left = _worldCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        Vector3 right = _worldCamera.ViewportToWorldPoint(new Vector3(1f, 0f, 0f));
        float bottomY = left.y;

        Vector3 p = transform.position;

        if (p.y < bottomY - bottomExitPaddingUnits)
            return true;

        if (p.x < left.x - sideExitPaddingUnits)
            return true;

        if (p.x > right.x + sideExitPaddingUnits)
            return true;

        return false;
    }
}
 