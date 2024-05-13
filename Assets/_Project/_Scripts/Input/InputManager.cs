using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Inputs inputs;

    public event EventHandler OnMove;
    public event EventHandler OnMovePressed;
    public bool IsMoveHeld;

    public event EventHandler OnRotatePressed;

    public event EventHandler OnDropPressed;

    public event EventHandler OnPausePressed;

    public event EventHandler OnHoldPressed;

    private void Awake()
    {
        inputs = new Inputs();
        inputs.Player.Enable();

        inputs.Player.Move.started += MovementPressed;
        inputs.Player.Move.performed += MovementPressed;
        // inputs.Player.Move.canceled += MovementPressed;

        inputs.Player.Rotate.performed += RotatePressed;

        inputs.Player.Drop.performed += DropPressed;

        inputs.Player.Pause.performed += PausePressed;

        inputs.Player.Hold.performed += HoldPressed;
    }

    private void HoldPressed(InputAction.CallbackContext context)
    {
        OnHoldPressed?.Invoke(this, EventArgs.Empty);
    }

    private void PausePressed(InputAction.CallbackContext context)
    {
        OnPausePressed?.Invoke(this, EventArgs.Empty);
    }

    private void DropPressed(InputAction.CallbackContext context)
    {
        OnDropPressed?.Invoke(this, EventArgs.Empty);
    }

    private void MovementPressed(InputAction.CallbackContext ctx)
    {
        if (ctx.started) OnMovePressed?.Invoke(this, EventArgs.Empty);
        OnMove?.Invoke(this, EventArgs.Empty);
        IsMoveHeld = ctx.performed;
    }

    public Vector3 GetMovement()
    {
        return inputs.Player.Move.ReadValue<Vector3>();
    }

    public void RotatePressed(InputAction.CallbackContext ctx)
    {
        OnRotatePressed?.Invoke(this, EventArgs.Empty);
    }
}
