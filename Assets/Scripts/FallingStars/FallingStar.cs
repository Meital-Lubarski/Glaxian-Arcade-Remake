using UnityEngine;
using System.Collections.Generic;

public class FallingStar : MonoBehaviour
{
    [SerializeField] private bool isBonusStar = false;
    [SerializeField] private int scorePerHit = 50;
    [SerializeField] private float waitBeforeGrow = 2.1f;
    [SerializeField] private float growSpeed = 0.4f;
    [SerializeField] private float fallSpeed =  1.5f;
    [SerializeField] private float maxScale = 0.27f;
    [SerializeField] private float blinkSpeed = 10f;
    [SerializeField] private float minBlinkAlpha = 0.3f;
    [SerializeField] private float maxBlinkAlpha = 1.0f;
    
    [Header("Colors")]
    [SerializeField] private bool useRandomColors = false; 
    [SerializeField] private Color[] possibleColors = { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow };
    
    [Header("Shrink Settings")]
    [SerializeField] private float timeBeforeShrink = 3.0f;
    [SerializeField] private float shrinkSpeed = 1.2f;


    private float _fallTimer = 0f;
    private bool _isShrinking = false;
    private SpriteRenderer _starPrefab;
    private bool _isFalling;
    private float _timer;
    private Vector3 _originalPrefabScale;
    private Color _starColor;
    
    // ׂ[SerializeField] private Color[] possibleColors = { 
        // Color.red, 
        // Color.blue, 
        // Color.green, 
        // Color.yellow, 
        // new Color(1f, 0f, 1f)
    // };

    public static List<FallingStar> AllFallingStars = new List<FallingStar>();

    private void OnEnable() { AllFallingStars.Add(this); }
    private void OnDisable() { AllFallingStars.Remove(this); }
    
    private void Awake()
    {
        _starPrefab = GetComponent<SpriteRenderer>();
        _originalPrefabScale = transform.localScale;
        if (useRandomColors && possibleColors.Length > 0)
            _starColor = possibleColors[Random.Range(0, possibleColors.Length)];
        else
            _starColor = Color.white;
    }

    private void Start()
    {
        if (_starPrefab == null) _starPrefab = GetComponent<SpriteRenderer>();
    
        if (_starPrefab != null)
        {
            _starPrefab.color = _starColor;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (SessionManager.Instance == null) return;

        GameState currentState = SessionManager.Instance.GetCurrentState();

        if (currentState == GameState.GameOver || currentState == GameState.Victory) 
        { 
            Destroy(gameObject); 
            return; 
        }

        if (currentState != GameState.Playing) return;

        float alpha = Mathf.Lerp(minBlinkAlpha, maxBlinkAlpha, Mathf.PingPong(Time.time * blinkSpeed, 1.0f));
        if (_starPrefab != null)
        {
            Color c = _starColor;
            c.a = alpha;
            _starPrefab.color = c;
        }

        _timer += Time.deltaTime;
        if (_timer < waitBeforeGrow) return;

        if (transform.localScale.x < maxScale && !_isFalling && !_isShrinking)
        {
            transform.localScale += Vector3.one * (growSpeed * Time.deltaTime);
        }
        else if (!_isFalling && !_isShrinking)
        {
            _isFalling = true;
            if (SoundManager.Instance != null) SoundManager.Instance.PlayFallingStar();
        }
    
        if (_isFalling)
        {
            transform.Translate(Vector2.down * (fallSpeed * Time.deltaTime));
            if (isBonusStar)
            {
                _fallTimer += Time.deltaTime;
                if (_fallTimer >= timeBeforeShrink) _isShrinking = true;
            }
        }

        if (_isShrinking)
        {
            transform.localScale -= Vector3.one * (shrinkSpeed * Time.deltaTime);
            if (transform.localScale.x <= 0.01f) Destroy(gameObject);
        }

        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
    
    public void GetHitByPlayer()
    {
        if (isBonusStar)
        {
            if (HudManager.Instance != null)
            {
                HudManager.Instance.UpdateScore(scorePerHit);
            }
            transform.localScale += Vector3.one * 0.05f; 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                SoundManager.Instance.PlayStarExplosion();
                player.TakeHit();
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("PlayerBullet")) 
        {
            GetHitByPlayer();
            if (!isBonusStar) Destroy(gameObject); 
            //Destroy(other.gameObject); 
        }
    }
    
    
    
    
}
