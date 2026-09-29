using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemyGroupOrder groupOrder;
    [SerializeField] private AttackDirector attackDirector;

    [SerializeField] private float timeBetweenWaves = 3.0f;
    [SerializeField] private float firstWaveDuration = 2.0f;
    
    private float _victoryCheckTimer = 1.0f; 
    private bool _levelRunning;

    private void OnEnable()
    {
        _levelRunning = false; 
        if (attackDirector != null)
        {
            attackDirector.enabled = false;
        }
        CancelInvoke();
        if (groupOrder != null)
        {
            groupOrder.Rebuild();
        }
        Invoke(nameof(StartAttack), firstWaveDuration);
    }
    
    private void OnDisable()
    {
        CancelInvoke();
    }
    

    private void StartAttack()
    {
        _levelRunning = true;
        if (attackDirector != null)
        {
            attackDirector.enabled = true;
        }
    }
    
    private void Update()
    {
        if (_levelRunning)
        {
            _victoryCheckTimer -= Time.deltaTime;
            if (_victoryCheckTimer <= 0)
            {
                _victoryCheckTimer = 0.5f; 
                if (CheckVictory())
                {
                    _levelRunning = false;
                    HandleVictory();
                }
            }
        }
    }

    private bool CheckVictory()
    {
        if (EnemyIdentity.AllEnemies == null) return false;
        
        return EnemyIdentity.AllEnemies.Count == 0;
    }

    private void HandleVictory()
    {
        if (attackDirector != null)
        {
            attackDirector.enabled = false;
        }
        SessionManager.Instance?.PlayerVictory();
    }

    public void GameOver()
    {
        _levelRunning = false;
        CancelInvoke();
        if (attackDirector != null)
        {
            attackDirector.enabled = false;
        }
    }
}