using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject[] lifeIcons;
    
    
    private string assetName = "Arcade - Galaxian - Miscellaneous - General Sprites (2)";
    private int _currentScore;
    private PlayerHealth _playerHealth;
    
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
    
    
    private string GetSpriteName(char digit)
    {
        if (digit == '0') return "clean_red";
        return assetName + "_" + digit;
    }
    
    
    private void OnEnable()
    {
        _currentScore = 0;
        UpdateScoreDisplay();
            //StopAllCoroutines();
        StartCoroutine(KeepSearchingForPlayer());
    }

    private IEnumerator KeepSearchingForPlayer()
    {
        while (_playerHealth == null) 
        {
            if (SessionManager.Instance != null && SessionManager.Instance.GetCurrentState() == GameState.Playing)
            {
                if (PlayerHealth.Instance != null)
                {
                    _playerHealth = PlayerHealth.Instance;
                    _playerHealth.OnLivesChanged += UpdateLives;
                    UpdateLives(_playerHealth.CurrentLives);
                    
                    yield break; 
                }
            }
            yield return new WaitForSeconds(0.5f); 
        }
    }

    private IEnumerator DelayedInitialUpdate()
    {
        yield return null;
        if (_playerHealth != null)
        {
            UpdateLives(_playerHealth.CurrentLives);
        }
    }

    private void OnDisable()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnLivesChanged -= UpdateLives;
        }
    }

    public void ShowFullLivesAtStart()
    {
        if (lifeIcons == null)
        {
            return;
        }
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null) lifeIcons[i].SetActive(true);
        }
    }

    public void UpdateLives(int lives)
    {
        if (lifeIcons == null || lifeIcons.Length == 0)
        {
            return;
        }
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
            {
                lifeIcons[i].SetActive(i < lives - 1);
            }
        }
    }

    public void UpdateScore(int score)
    {
        _currentScore += score;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText == null)
        {
            return;
        }

        string scoreStr = _currentScore.ToString();
        if (_currentScore == 0)
        {
            scoreStr = "00";
        }
        string visualScore = "";
        foreach (char c in scoreStr)
        {
            visualScore += $"<sprite name=\"{GetSpriteName(c)}\">";
        }
        scoreText.text = visualScore + "                               " + visualScore;
    }

    public void AddScoreFromEnemy(EnemyIdentity enemy, bool isDiving)
    {
        if (enemy != null && enemy.enemyData != null)
        {
            int finalScore = isDiving ? enemy.enemyData.divingScoreValue : enemy.enemyData.scoreValue;
            UpdateScore(finalScore);
        }
    }
}
