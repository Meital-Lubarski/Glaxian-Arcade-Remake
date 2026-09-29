using UnityEngine;
using System;


[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    public event Action Died;
    [SerializeField] private int hitPoints = 1;
    [SerializeField] private float deathDelay = 0.5f;
    [SerializeField] private float deathAnimSpeed = 1f;

    [Header("Compatibility")]
    [Tooltip("Old behavior: Destroy immediately. Turn OFF when using EnemyDeathSequence.")]
    [SerializeField] private bool destroyImmediately = true;
    
    private EnemyStateController _state;
    private Collider2D _collider;
    private bool _isDead;
    private const int MIN_DAMAGE = 1;
    
    private void Awake()
    {
        _state = GetComponent<EnemyStateController>();
        _collider = GetComponent<Collider2D>();
    }

    public void TakeHit(int damage = MIN_DAMAGE)
    {
        if (_isDead) return;
        hitPoints -= Mathf.Max(MIN_DAMAGE, damage);

        if (hitPoints > 0) return;
    
        _isDead = true;
        EnemyIdentity identity = GetComponent<EnemyIdentity>();
        if (identity != null && identity.enemyData != null)
        {
            if (identity.enemyData.enemyName == "Yellow") 
            {
                SoundManager.Instance.PlayHitBoss();
            }
            else
            {
                SoundManager.Instance.PlayEnemyExplosion();
            }
        }
        if (_collider != null)
            _collider.enabled = false;

        if (_state != null)
            _state.SetState(EnemyState.Dead);
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.speed = deathAnimSpeed;
        }

        Died?.Invoke();

        if (destroyImmediately)
            Destroy(gameObject, deathDelay);
    }
}