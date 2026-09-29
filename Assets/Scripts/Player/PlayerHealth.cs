using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance  { get; private set; }
    
    [Header("Lives")]
    [SerializeField] private int maxLives = 3;

    //[Header("Invulnerability")]
    //[SerializeField] private float invulnerabilityDuration = 1.0f;

    [Header("State")]
    [SerializeField] private PlayerStateController state;

    [Header("Visuals")]
    [SerializeField] private PlayerVisual playerVisual;
    
    public int CurrentLives { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public event Action<int> OnLivesChanged;
    public event Action OnPlayerDied;
    public event Action OnPlayerHit;

    private float _invulnerabilityTimer;
    private Vector3 _startPosition;
    private Collider2D _playerCollider;
    //private PlayerControls _controls;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        _startPosition = transform.position;
        CurrentLives = maxLives;
        _playerCollider = GetComponent<Collider2D>();
        OnLivesChanged?.Invoke(CurrentLives);
        if (playerVisual == null)
        {
            playerVisual = GetComponentInChildren<PlayerVisual>();
        }
    }

// Called by enemy bullets / collisions
    public void TakeHit()
    {
        if (IsInvulnerable)
        {
            return; 
        }
        if (state != null && state.CurrentPlayerState == PlayerState.Dead)
        {
            return; 
        }
        HandleHit();    
    }

    public void HandleHit()
    {
        if (state != null)
        {
            state.SetPlayerState(PlayerState.Dead);
        }

        CurrentLives--;
    
        if (playerVisual != null)
        {
            playerVisual.TriggerExplosion();
        }

        SoundManager.Instance.PlayPlayerDeath();
        OnPlayerHit?.Invoke();
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives <= 0)
        {
            StopAllCoroutines(); 
            StartCoroutine(FinalDeathSequence());
        }
        else
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    private IEnumerator RespawnPlayer()
    {
        IsInvulnerable = true;
        if (state != null)
        {
            state.SetPlayerState(PlayerState.Dead);
        }
        if (_playerCollider != null)
        {
            _playerCollider.enabled = false;
        }
        yield return new WaitForSeconds(0.55f);
        if (playerVisual != null)
        {
            playerVisual.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(4.45f);
        transform.position = _startPosition;
        if (playerVisual != null)
        {
            playerVisual.gameObject.SetActive(true);
            Animator anim = playerVisual.GetComponent<Animator>();
            if (anim != null)
            {
                anim.ResetTrigger("Explode");
                anim.Play("Player_Idle", 0, 0f);
            }
            playerVisual.SetLoaded(true);
        }

        if (state != null)
        {
            state.SetPlayerState(PlayerState.Walking);
        }

        if (_playerCollider != null)
        {
            _playerCollider.enabled = true;
        }
        PlayerShooter shooter = GetComponent<PlayerShooter>();
        if (shooter != null)
        {
            shooter.ResetShooting(); 
        }
        IsInvulnerable = false;
    }


    private IEnumerator FinalDeathSequence()
    {
        if (state != null) state.SetPlayerState(PlayerState.Dead);

        yield return new WaitForSeconds(0.55f);
        if (playerVisual != null) playerVisual.gameObject.SetActive(false);

        if (EnemyIdentity.AllEnemies != null)
        {
            List<EnemyIdentity> enemiesToReturn = new List<EnemyIdentity>(EnemyIdentity.AllEnemies);
        
            foreach (var enemyIdentity in enemiesToReturn)
            {
                if (enemyIdentity != null)
                {
                    var collisions = enemyIdentity.GetComponent<EnemyCollisions>();
                    if (collisions != null)
                    {
                        collisions.ReturnWhenGameOver();
                    }
                }
            }
        }

        yield return new WaitForSeconds(2f); 

        if (SessionManager.Instance != null)
        {
            SessionManager.Instance.SetState(GameState.GameOver);
        }
    }
        //For the next steps
        public void ResetLives()
        {
            //StopAllCoroutines();
            CurrentLives = maxLives;
            IsInvulnerable = false;
            _invulnerabilityTimer = 0f;
            if (playerVisual != null)
            {
                playerVisual.gameObject.SetActive(true);
                playerVisual.enabled = true;
                playerVisual.SetLoaded(true);
            }

            if (_playerCollider != null)
            {
                _playerCollider.enabled = true;
            }

            if (state != null)
            {
                state.SetPlayerState(PlayerState.Walking);
            }

            // if (SessionManager.Instance != null)
            // {
            //     SessionManager.Instance.SetState(GameState.Playing);
            // }
            OnLivesChanged?.Invoke(CurrentLives);
        }
}
