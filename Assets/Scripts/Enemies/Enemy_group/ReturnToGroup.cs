using UnityEngine;

public class ReturnToGroup : MonoBehaviour
{
    private EnemyStateController _state;
    private EnemySlot _slot;
    private Animator _animator;
    private Camera _worldCamera;

    [SerializeField] private Transform visualTransform;

    [SerializeField] private float diveUprightZ = -180f;

    [Header("Return Arc")]
    [SerializeField] private float returnArcDurationSeconds = 0.9f;
    [SerializeField] private float returnArcHeightUnits = 1.8f;
    [SerializeField] private float returnSpawnAboveFormationUnits = 0.6f;

    [Header("Return Spin (end of return)")]
    [Range(0f, 0.99f)]
    [SerializeField] private float returnSpinStartT = 0.78f;
    [SerializeField] private float returnSpinDegrees = 180f;
    [SerializeField] private float formationUprightZ;

    private float _returnT;
    private Vector3 _returnStart;
    private bool _returnIsLeftSide;
    private bool _isSnapped = false;

    private void Awake()
    {
        _state = GetComponent<EnemyStateController>();
        _slot = GetComponent<EnemySlot>();
        _animator = GetComponentInChildren<Animator>(true);
        _worldCamera = Camera.main;

        if (visualTransform == null && _animator != null)
            visualTransform = _animator.transform;

        if (visualTransform == null)
            visualTransform = transform;
    }

    private void Update()
    {
        if (_state == null || SessionManager.Instance == null) return;

        GameState currentState = SessionManager.Instance.GetCurrentState();
        bool isGameOver = (currentState == GameState.GameOver);

        if (isGameOver)
        {
            if (_state.CurrentState == EnemyState.InFormation || _state.CurrentState == EnemyState.Dead)
            {
                this.enabled = false; 
                return;
            }

            if (_state.CurrentState == EnemyState.Diving)
            {
                BeginReturn(); 
                return; 
            }
        }
    
        if (_state.CurrentState == EnemyState.Returning)
        {
            UpdateReturning();
        }
    }
    
    private void SnapBackToFormation()
    {
        if (_slot == null || _slot.FormationRoot == null) return;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = false; 
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }
        if (transform.parent != _slot.FormationRoot)
        {
            transform.SetParent(_slot.FormationRoot, false); 
        }
        transform.localPosition = _slot.HomeLocalPosition;
        transform.localRotation = Quaternion.identity;

        if (visualTransform != null)
            visualTransform.localRotation = Quaternion.Euler(0f, 0f, formationUprightZ);
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.speed = 1f;
            _animator.Play(0, -1, 0f);
        }
        _state.SetState(EnemyState.InFormation);
        this.enabled = false; 
    }

    public void BeginReturn()
    {
        if (_state == null || _slot == null || _slot.FormationRoot == null || _worldCamera == null)
        {
            return;
        }

        if (_state.CurrentState == EnemyState.InFormation || _state.CurrentState == EnemyState.Returning)
            return;

        _state.SetState(EnemyState.Returning);

        Vector3 end = _slot.FormationRoot.TransformPoint(_slot.HomeLocalPosition);
        float homeWorldX = end.x;
        float formationCenterWorldX = _slot.FormationRoot.position.x;
        _returnIsLeftSide = homeWorldX < formationCenterWorldX;

        float topY = _worldCamera.ViewportToWorldPoint(new Vector3(0f, 1f, 0f)).y;
        _returnStart = new Vector3(end.x, topY + returnSpawnAboveFormationUnits, transform.position.z);

        transform.position = _returnStart;
        _returnT = 0f;
    }

    private void UpdateReturning()
    {
        if (_slot == null || _slot.FormationRoot == null)
            return;

        _returnT += Time.deltaTime / Mathf.Max(0.01f, returnArcDurationSeconds);
        float t = Mathf.Clamp01(_returnT);

        Vector3 end = _slot.FormationRoot.TransformPoint(_slot.HomeLocalPosition);
        float side = _returnIsLeftSide ? -1f : 1f;

        Vector3 control = (_returnStart + end) * 0.5f + new Vector3(side * returnArcHeightUnits, returnArcHeightUnits, 0f);
        transform.position = EvaluateQuadraticBezier(_returnStart, control, end, t);

        float z;
        if (t < returnSpinStartT)
        {
            z = diveUprightZ;
        }
        else
        {
            float spin01 = Mathf.InverseLerp(returnSpinStartT, 1f, t);
            float spinSign = _returnIsLeftSide ? -1f : 1f;
            z = diveUprightZ + (returnSpinDegrees * spinSign * spin01);
        }

        visualTransform.rotation = Quaternion.Euler(0f, 0f, z);

        if (t >= 1f)
        {
            transform.SetParent(_slot.FormationRoot);
            transform.localPosition = _slot.HomeLocalPosition;
            transform.rotation = Quaternion.identity;
            visualTransform.rotation = Quaternion.Euler(0f, 0f, formationUprightZ);
        
            if (_animator != null)
            {
                _animator.speed = 1f;
            }
        
            _state.SetState(EnemyState.InFormation);
        }
    }

    private static Vector3 EvaluateQuadraticBezier(Vector3 a, Vector3 c, Vector3 b, float t)
    {
        float u = 1f - t;
        return (u * u) * a + (2f * u * t) * c + (t * t) * b;
    }
}
