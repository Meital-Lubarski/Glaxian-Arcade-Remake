using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PlayerBullet : MonoBehaviour
{
    public event Action<PlayerBullet> OnBulletDestroyed;

    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float topScreenBoundary = 7f;

    private bool _destroyed;

    private void Start()
    {
        Invoke(nameof(Kill), lifetime);
        
    }

    private void Update()
    {
        transform.Translate(Vector2.up * (speed * Time.deltaTime));
        if (transform.position.y > topScreenBoundary)
        {
            Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_destroyed)
        {
            return;
        }
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeHit(1);
            Kill();
            return; 
        }
        FallingStar star = other.GetComponent<FallingStar>();
        if (star != null)
        {
            star.GetHitByPlayer();
            Kill();
            return; 
        }
        if (enemy == null)
        {
            return; 
        }
    }

    private void Kill()
    {
        if (_destroyed)
        {
            return; 
        }
        _destroyed = true;
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider != null)
        {
            myCollider.enabled = false; 
        }
        OnBulletDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}