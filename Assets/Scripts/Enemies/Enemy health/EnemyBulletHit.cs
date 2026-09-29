using UnityEngine;

public class EnemyBulletHit : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 3f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector2.down * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();
        if (player == null)
        {
            return;
        }
        player.TakeHit();
        Destroy(gameObject);
    }
}
