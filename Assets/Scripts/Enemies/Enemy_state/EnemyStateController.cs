using UnityEngine;

public enum EnemyState
{
    InFormation,
    Turning,
    Diving,
    Returning,
    Dead
}


public class EnemyStateController : MonoBehaviour
{
    public EnemyState CurrentState { get; private set; } = EnemyState.InFormation;

    public EnemyState PreviousState { get; private set; } = EnemyState.InFormation;
    public void SetState(EnemyState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }
        PreviousState = CurrentState;
        CurrentState = newState;
    }
}