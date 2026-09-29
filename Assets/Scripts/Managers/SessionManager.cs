using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { Attract, Playing, Victory, GameOver }

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private GameState currentState = GameState.Attract;
    [SerializeField] private float delayBetweenLines = 1.2f;
    [SerializeField] private float moveDuration = 1.2f;
    [SerializeField] private float delayBeforePlayerAppears = 1.5f;
    [SerializeField] private int targetFPS = 30;
    
    [Header("Screens")]
    [SerializeField] private GameObject attractScreen1;
    [SerializeField] private GameObject attractScreen2;
    [SerializeField] private GameObject hudScreen;
    [SerializeField] private GameObject gameOverScreen;

    [Header("Logic References")]
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] public WaveManager waveManager;
    [SerializeField] private GameObject groupOrderParent;
    
    [Header("Attract Animation Sequence")]
    [SerializeField] private List<GameObject> attractLines;

    [SerializeField] private GameObject playerOneText;
    [SerializeField] private UnityEngine.UI.Image blackOverlay;
    
  
    
    [SerializeField] private MonoBehaviour attackDirector;
    [SerializeField] private GameObject victoryScreen;
    private float _victoryCheckTimer = 0.5f;
    
    
    private void Awake()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (hudScreen != null) hudScreen.SetActive(false);
        if (attractScreen2 != null) attractScreen2.SetActive(false);
        if (attackDirector != null) attackDirector.enabled = false;
        if (blackOverlay != null)
        {
            blackOverlay.GetComponent<RectTransform>().anchoredPosition = new Vector2(-2500f, 0f);
        }
        StartAttractSequence();
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }
        if (currentState == GameState.Attract)
        {
            
            if (keyboard.enterKey.wasPressedThisFrame)
            {
                StartGame();
            }
        }
        else if (currentState == GameState.GameOver || currentState == GameState.Victory)
        {
            if (keyboard.enterKey.wasPressedThisFrame)
            {
                RestartSession();
            }
        }
    }

    public void StartAttractSequence()
    {
        SetState(GameState.Attract);
        StartCoroutine(PlayAttractAnimation());
    }

    public GameState GetCurrentState()
    {
        return currentState;
    }
    
    private IEnumerator PlayAttractAnimation()
    {
        float startX = 1200f; 
        float endX = 0f;
        int startIndex = 4; 
        int stopIndex = attractLines.Count - 2;

        foreach (GameObject line in attractLines)
        {
            if (line != null)
            {
                int index = attractLines.IndexOf(line);
                if (index >= startIndex && index < stopIndex) 
                {
                    RectTransform rect = line.GetComponent<RectTransform>();
                    if (rect != null) rect.anchoredPosition = new Vector2(startX, rect.anchoredPosition.y);
                }
                line.SetActive(false);
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < attractLines.Count; i++)
        {
            if (attractLines[i] != null)
            {
                attractLines[i].SetActive(true);
                if (i >= startIndex && i < stopIndex)
                {
                    RectTransform rect = attractLines[i].GetComponent<RectTransform>();
                    if (rect != null) StartCoroutine(MoveLineSmoothly(rect, startX, endX, moveDuration));
                }
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }

        yield return new WaitForSeconds(1.5f);
        float screenWidth = 2000f;
        blackOverlay.GetComponent<RectTransform>().anchoredPosition = new Vector2(screenWidth, 0);
        yield return StartCoroutine(DoFadeEffect(0f, 0.6f));
        ShowNextAttractScreen();
        yield return StartCoroutine(DoFadeEffect(-screenWidth, 0.6f));
    }

    private IEnumerator MoveLineSmoothly(RectTransform rect, float fromX, float toX, float duration)
    {
        float elapsed = 0f;
        float currentY = rect.anchoredPosition.y;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float steppedProgress = progress * progress * (3f - 2f * progress);
            rect.anchoredPosition = new Vector2(Mathf.Lerp(fromX, toX, steppedProgress), currentY);
            yield return null;
        }
        rect.anchoredPosition = new Vector2(toX, currentY);
    }

    public void ShowNextAttractScreen()
    {
        if (attractScreen1 != null) attractScreen1.SetActive(false);
        if (attractScreen2 != null) attractScreen2.SetActive(true);
    }

    public void StartGame()
    {
        StopAllCoroutines();
        if (attractScreen1 != null)
        {
            attractScreen1.SetActive(false);
        }

        if (attractScreen2 != null)
        {
            attractScreen2.SetActive(false);
        }
        StartCoroutine(PlayStartSequence());
    }

    private IEnumerator PlayStartSequence()
    {
        float screenWidth = 2000f;
        blackOverlay.GetComponent<RectTransform>().anchoredPosition = new Vector2(screenWidth, 0);
        yield return StartCoroutine(DoFadeEffect(0f, 0.6f));

        if (attractScreen2 != null) attractScreen2.SetActive(false);
    
        if (groupOrderParent != null) groupOrderParent.SetActive(true);
        if (waveManager != null) 
        {
            waveManager.gameObject.SetActive(true);
            waveManager.enabled = true;
        }
        if (hudScreen != null) 
        {
            hudScreen.SetActive(true);
            HudManager hud = hudScreen.GetComponent<HudManager>();
            if (hud != null) hud.ShowFullLivesAtStart();
        }

        if (playerOneText != null) playerOneText.SetActive(true);

        yield return StartCoroutine(DoFadeEffect(-screenWidth, 0.6f));

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBackgroundMusic();
            SoundManager.Instance.PlayStartGame();
        
            float duration = SoundManager.Instance.GetStartGameDuration();
            if (duration <= 0) duration = 2.0f;
            yield return new WaitForSeconds(duration);
        }

        if (playerOneText != null) playerOneText.SetActive(false);

        if (SoundManager.Instance != null)
        {
            var lib = SoundManager.Instance.GetLibrary();
            SoundManager.Instance.PlayBackgroundMusic(lib.backgroundMusic, true);
        }

        SetState(GameState.Playing); 
    
        if (HudManager.Instance != null && playerStateController != null)
        {
            HudManager.Instance.UpdateLives(3);
        }
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        bool isPlaying = (currentState == GameState.Playing);
        bool isGameOver = (currentState == GameState.GameOver);
        bool isVictory = (currentState == GameState.Victory);

        if (isGameOver || isVictory)
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopBackgroundMusic();
            }
            var starsToDestroy = new List<FallingStar>(FallingStar.AllFallingStars);
            foreach (FallingStar star in starsToDestroy)
            {
                if (star != null) Destroy(star.gameObject);
            }
        }

        if (attractScreen1 != null) 
            attractScreen1.SetActive(currentState == GameState.Attract);

        if (attractScreen2 != null) 
            attractScreen2.SetActive(currentState == GameState.Attract && !attractScreen1.activeSelf);

        if (hudScreen != null) 
            hudScreen.SetActive(isPlaying || isGameOver);

        if (attackDirector != null)
            attackDirector.enabled = isPlaying;

        if (playerStateController != null) 
        {
            playerStateController.gameObject.SetActive(isPlaying);
            playerStateController.enabled = isPlaying;
        }
        if (isPlaying) 
        {
            PlayerVisual pv = playerStateController.GetComponentInChildren<PlayerVisual>();
            if (pv != null) pv.SetLoaded(true);
        }

        if (groupOrderParent != null) 
            groupOrderParent.SetActive(isPlaying || isGameOver);

        if (waveManager != null) 
        {
            if (isPlaying)
            {
                waveManager.gameObject.SetActive(true);
                waveManager.enabled = true;
            }
            else
            {
                waveManager.enabled = false;
                if (!isGameOver) waveManager.gameObject.SetActive(false);
            }
        }

        if (gameOverScreen != null) gameOverScreen.SetActive(isGameOver);
        if (victoryScreen != null) victoryScreen.SetActive(isVictory);
    }


    public void CheckForVictory()
    {
        if (currentState != GameState.Playing) return;

        if (EnemyIdentity.AllEnemies.Count <= 0)
        {
            SetState(GameState.Victory);
        }
    }

    private IEnumerator DoFadeEffect(float targetAlpha, float duration)
    {
        if (blackOverlay == null)
        {
            yield break;
        }
        RectTransform rect = blackOverlay.GetComponent<RectTransform>();
        Vector2 start = rect.anchoredPosition;
        Vector2 end = new Vector2(targetAlpha, 0f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float smoothProgress = progress * progress * (3f - 2f * progress);
            rect.anchoredPosition = Vector2.Lerp(start, end, smoothProgress);
            yield return null;
        }
        rect.anchoredPosition = end;
    }
    
    public void PlayerVictory() { SetState(GameState.Victory); }

    public void RestartSession()
    {
        if (EnemyIdentity.AllEnemies != null)
        {
            EnemyIdentity.AllEnemies.Clear();
        }

        if (EnemyDive.AllDivers != null)
        {
            EnemyDive.AllDivers.Clear();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
}