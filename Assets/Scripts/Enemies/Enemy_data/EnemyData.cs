using UnityEngine;



[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Galaxian/Enemy Data" )]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int scoreValue;
    public int divingScoreValue;
    public Sprite bonusScoreSprite;
}
