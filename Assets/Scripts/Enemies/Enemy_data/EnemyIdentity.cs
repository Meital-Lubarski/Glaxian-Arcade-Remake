using UnityEngine;
using System.Collections.Generic;

public class EnemyIdentity : MonoBehaviour
{
    private static List<EnemyIdentity> _allEnemies = new List<EnemyIdentity>();
    public static List<EnemyIdentity> AllEnemies => _allEnemies;

    private void OnEnable()
    {
        _allEnemies.Add(this);
    }

    private void OnDisable()
    {
        _allEnemies.Remove(this);
    }
    
    public EnemyData enemyData;
}

