using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private PlayerStateController state;
    
    private PlayerControls _controls;

    private void Awake()
    {
        _controls = new PlayerControls(); 
    } 

    private void OnEnable() => _controls.Player.Enable();
    private void OnDisable() => _controls.Player.Disable();
    

    // Update is called once per frame
    void Update()
    {
        float input = _controls.Player.Move.ReadValue<Vector2>().x;
        Move(input);
        if (state != null)
            state.SetPlayerState(input == 0 ? PlayerState.Idle : PlayerState.Walking);
    }

    private void Move(float input)
    {
        Vector3 position = transform.position;
        position.x += input * speed * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        transform.position = position;
    }
}
