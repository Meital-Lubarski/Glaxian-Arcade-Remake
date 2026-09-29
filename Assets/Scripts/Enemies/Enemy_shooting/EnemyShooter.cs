using UnityEngine;

public class EnemyShooter : MonoBehaviour, IEnemyShooter
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    public void ExecuteShoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            SoundManager.Instance.PlayEnemyShoot();
        }
    }
}
