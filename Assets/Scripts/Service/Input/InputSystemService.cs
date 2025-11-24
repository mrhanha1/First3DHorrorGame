using System;
using UnityEngine;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM
public class InputSystemService : IInputService, IDisposable
{
    private PlayerInputActions inputActions;

    private bool interactPressed = false;
    private bool interactHeld = false;
    private bool interactReleased = false;
    private bool jumpPressed = false;
    private bool cancelPressed = false;

    public InputSystemService()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Interact.performed += OnInteractPerformed;
        inputActions.Player.Interact.canceled += OnInteractCanceled;
        inputActions.Player.Jump.performed += OnJumpPerfomred;
        inputActions.Player.Cancel.performed += OnCancelPerformed;
        inputActions.Player.Enable();
        //SetCursorState(true);
    }
    public PlayerInputActions GetInputActions() => inputActions;

    public bool IsInteractPressed => interactPressed;
    public bool IsInteractReleased => interactReleased;
    public bool IsInteractHeld => inputActions.Player.Interact.IsPressed();
    public bool IsJumpPressed => jumpPressed;
    public bool IsCancelPressed => cancelPressed;
    public bool IsSprintHeld => inputActions.Player.Sprint.IsPressed();
    public Vector2 MoveInput => inputActions.Player.Move.ReadValue<Vector2>();
    public Vector2 LookInput => inputActions.Player.Look.ReadValue<Vector2>();

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        interactPressed = true;
    }
    private void OnInteractCanceled(InputAction.CallbackContext ctx)
    {
        interactReleased = true;
    }
    private void OnJumpPerfomred(InputAction.CallbackContext ctx)
    {
        jumpPressed = true;
    }
    private void OnCancelPerformed(InputAction.CallbackContext ctx)
    {
        cancelPressed = true;
    }
    public void LateUpdate()
    {
        interactPressed = false;
        interactReleased = false;
        jumpPressed = false;
        cancelPressed = false;
    }
    public void Dispose()
    {
        inputActions.Player.Interact.performed -= OnInteractPerformed;
        inputActions.Player.Interact.canceled -= OnInteractCanceled;
        inputActions.Player.Jump.performed -= OnJumpPerfomred;
        inputActions.Player.Cancel.performed -= OnCancelPerformed;
        inputActions.Dispose();
    }
    public void Enable()
    {
        inputActions.Player.Enable();
    }
    public void Disable()
    {
        inputActions.Player.Disable();
    }
    public void SwitchActionMap(string actionMapName)
    {
        inputActions.asset.FindActionMap(actionMapName)?.Enable();
        foreach (var map in inputActions.asset.actionMaps)
        {
            if (map.name != actionMapName)
            {
                map.Disable();
            }
        }
    }
    public void SetCursorState(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        //Cursor.visible = !locked;
    }
}
#endif