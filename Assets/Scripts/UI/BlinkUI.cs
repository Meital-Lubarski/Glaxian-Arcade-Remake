using UnityEngine;
using System.Collections;

public class BlinkUI : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    [SerializeField] private float blinkTime = 0.5f;
    
    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Blink), 0f, blinkTime);
    }


    void Blink()
    {
        if (SessionManager.Instance != null)
        {
            if (SessionManager.Instance.GetCurrentState() == GameState.Playing)
            {
                _canvasGroup.alpha = (Mathf.Approximately(_canvasGroup.alpha, 1)) ? 0 : 1;
            }
            else
            {
                _canvasGroup.alpha = 1; 
            }
        }
    }
    
}
