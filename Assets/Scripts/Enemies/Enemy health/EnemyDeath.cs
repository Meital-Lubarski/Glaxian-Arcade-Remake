using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyDeath : MonoBehaviour
{
    
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hitCollider;
    [FormerlySerializedAs("detachFromParentOnDeath")] [SerializeField] private bool detachFromGroupOnDeath = true;
    
    [SerializeField] private MonoBehaviour[] disableOnDeath;

    private EnemyHealth _health;
    private bool _started;
    
    private void Awake()
    {
        _health = GetComponent<EnemyHealth>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (hitCollider == null)
        {
            hitCollider = GetComponent<Collider2D>();
        }
    }
    
    private void OnEnable()
    {
        _health.Died += OnDied;
    }

    private void OnDisable()
    {
        _health.Died -= OnDied;
    }

    private void OnDied()
    {
        if (_started)
        {
            return;
        }

        _started = true;
        if (detachFromGroupOnDeath)
        {
            transform.SetParent(null, true);
        }

        if (hitCollider != null)
        {
            hitCollider.enabled = false;
        }

        if (disableOnDeath != null)
        {
            foreach (var c in disableOnDeath)
            {
                if (c != null)
                {
                    c.enabled = false;
                }
            }
        }

        if (animator != null)
        {
            animator.SetTrigger("Explode");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
