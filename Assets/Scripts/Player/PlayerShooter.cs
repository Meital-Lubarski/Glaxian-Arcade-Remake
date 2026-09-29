using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private PlayerVisual visual;
    [SerializeField] private Transform firePoint;
    [SerializeField] private PlayerBullet bulletPrefab;
    [SerializeField] private PlayerStateController state;
    
    [SerializeField] private KeyCode shootKey= KeyCode.Space;
    private PlayerControls _controls;
    private PlayerBullet _activeBullet;
    
    private void Awake() => _controls = new PlayerControls();

    private void OnEnable()
    {
        _controls.Player.Enable();
        _controls.Player.Fire.performed += HandleFireInput;
    }

    private void OnDisable()
    {
        _controls.Player.Fire.performed -= HandleFireInput;
        _controls.Player.Disable();
    }

    private void HandleFireInput(InputAction.CallbackContext context)
    {
        if (state != null && state.CurrentPlayerState == PlayerState.Dead)
        {
            return;
        }

        if (visual != null && !visual.gameObject.activeInHierarchy)
        {
            return;
        }
        TryShoot();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        if (visual != null)
        {
            visual.SetLoaded(true);
        }
    }

    private void TryShoot()
    {
        if (!CanShoot())
        {
            return;
        }

        if (visual != null)
        {
            visual.SetLoaded(false); 
        }
        
        _activeBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        SoundManager.Instance.PlayPlayerShoot();
        _activeBullet.OnBulletDestroyed += HandleBulletDestroyed;
    }

    private bool CanShoot()
    {
        if (_activeBullet != null)
        {
            return false;
        }

        if (state != null && state.CurrentPlayerState == PlayerState.Dead)
        {
            return false;
        }

        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("PlayerShooter is missing Bullet Prefab or FirePoint reference.");
            return false;
        }

        return true;
    }


    private void HandleBulletDestroyed(PlayerBullet bullet)
    {
        if (bullet != null)
        {
            bullet.OnBulletDestroyed -= HandleBulletDestroyed;
        }
        _activeBullet = null;
        if (visual != null)
        {
            visual.SetLoaded(true);
        }
    }

    public void ResetShooting()
    {
        if (_activeBullet != null)
        {
            _activeBullet.OnBulletDestroyed -= HandleBulletDestroyed;
            _activeBullet = null;
        }

        if (visual != null)
        {
            visual.SetLoaded(true);
        }
    }
}
