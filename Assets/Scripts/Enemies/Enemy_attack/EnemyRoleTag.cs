using UnityEngine;

public enum EnemyRole
{
    Normal,
    Red,
    Flagship
}

public class EnemyRoleTag : MonoBehaviour
{
    public EnemyRole role = EnemyRole.Normal;
}
