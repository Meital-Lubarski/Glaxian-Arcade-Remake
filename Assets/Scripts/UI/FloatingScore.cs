using UnityEngine;

public class FloatingScore : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 1;
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector3.up * (floatSpeed * Time.deltaTime));
        if (_spriteRenderer != null)
        {
            Color c = _spriteRenderer.color;
            c.a -= (1f / lifetime) * Time.deltaTime;
            _spriteRenderer.color = c;
        }
    }

    public void Setup(Sprite scoreSprite, int scoreValue)
    {
        if (_spriteRenderer != null && scoreSprite != null)
        {
            _spriteRenderer.sprite = scoreSprite;
        }
        if (HudManager.Instance != null)
        {
            HudManager.Instance.UpdateScore(scoreValue);
        }
        Destroy(gameObject, lifetime);
    }
}
