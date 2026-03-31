using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event EventHandler OnRunning;
    public event EventHandler UnRunning;

    private PlayerInputActions playerInputActions;

    private void Awake() {

        Instance = this;

        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Enable();
        playerInputActions.Player.Run.performed += Run_performed;
        playerInputActions.Player.Run.canceled += Run_canceled;
    }

    private void OnDestroy() {

        playerInputActions.Player.Run.performed -= Run_performed;
        playerInputActions.Player.Run.canceled -= Run_canceled;
    }

    private void Run_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {

        UnRunning?.Invoke(this, EventArgs.Empty);
    }

    private void Run_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {

        OnRunning?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetInputVectorNormalized() {

        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector.normalized;
    }
}
