using UnityEngine;
using System;

public enum PlayerState
{
    Idle,
    Walking,
    Dead
}



public class PlayerStateController : MonoBehaviour
{
    public PlayerState CurrentPlayerState { get; private set; } = PlayerState.Idle;
    public event Action<PlayerState> OnPlayerStateChange;

    public void SetPlayerState(PlayerState newPlayerState)
    {
        CurrentPlayerState = newPlayerState;
        OnPlayerStateChange?.Invoke(CurrentPlayerState);
    }
}
