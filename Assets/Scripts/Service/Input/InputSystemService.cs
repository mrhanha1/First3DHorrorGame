using System;
using UnityEngine;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM
public class InputSystemService : IInputService, IDisposable
{
    private PlayerInputActions inputActions;
    private InputAction interactAction;
    private InputAction cancelAction;
    private InputAction scrollAction;

    private bool interactPressed = false;
    private bool interactPressedThisFrame = false;
    private bool interactReleasedThisFrame = false;
    private bool cancelPressedThisFrame = false;

    public InputSystemService()
    {
        inputActions = new PlayerInputActions();
        interactAction = inputActions.Player.Interact;
        cancelAction = inputActions.Player.Cancel;
        scrollAction = inputActions.Player.Scroll;

        interactAction.started += _ => { interactPressed = true; interactPressedThisFrame = true; };
        interactAction.canceled += _ => { interactPressed = false; interactReleasedThisFrame = true; };
        cancelAction.performed += _ => { cancelPressedThisFrame = true; };

        inputActions.Enable();
    }

    public bool GetInteractKeyDown()
    {
        bool result = interactPressedThisFrame;
        interactPressedThisFrame = false;
        return result;
    }

    public bool GetInteractKey() => interactPressed;

    public bool GetInteractKeyUp()
    {
        bool result = interactReleasedThisFrame;
        interactReleasedThisFrame = false;
        return result;
    }

    public bool GetCancelKeyDown()
    {
        bool result = cancelPressedThisFrame;
        cancelPressedThisFrame = false;
        return result;
    }
    public float GetScrollDelta() => scrollAction.ReadValue<Vector2>().y;
    public void Dispose()
    {
        inputActions?.Disable();
        inputActions?.Dispose();
    }
}
#endif