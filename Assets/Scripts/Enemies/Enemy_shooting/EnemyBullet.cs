using System;
using UnityEngine;
using DG.Tweening;

public class EnemyBullet : MonoBehaviour
{
    [Header("Movement")] [SerializeField] private float speed = 0.5f;
    
    [Header("Shake Settings")] 
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float shakeStrength = 0.3f;
    [SerializeField] private int shakeVib = 1;
    private Tween _shakeTween;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(visualTransform != null)
        {
            _shakeTween = visualTransform.DOShakePosition(1f, shakeStrength, shakeVib, 90, false, false)
                .SetLoops(-1, LoopType.Restart);
        }
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(Vector3.down * (speed * Time.deltaTime));
    }

    private void OnDestroy()
    {
        if (_shakeTween != null)
        {
            _shakeTween.Kill();
        }
    }
}
