using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IInputService
{
    private readonly KeyCode interactKey = KeyCode.E;
    private readonly KeyCode cancelKey = KeyCode.Escape;

    public Vector2 MoveInput => throw new NotImplementedException();

    public Vector2 LookInput => throw new NotImplementedException();

    public bool IsJumpPressed => throw new NotImplementedException();

    public bool IsSprintHeld => throw new NotImplementedException();

    public bool IsInteractPressed => throw new NotImplementedException();

    public bool IsInteractReleased => throw new NotImplementedException();

    public bool IsInteractHeld => throw new NotImplementedException();

    public bool IsCancelPressed => throw new NotImplementedException();

    public InputService(KeyCode interactKey, KeyCode cancelKey)
    {
        this.interactKey = interactKey;
        this.cancelKey = cancelKey;
    }

    public bool GetInteractKeyDown() => Input.GetKeyDown(interactKey);
    public bool GetInteractKeyUp() => Input.GetKeyUp(interactKey);
    public bool GetInteractKey() => Input.GetKey(interactKey);
    public bool GetCancelKeyDown() => Input.GetKeyDown(cancelKey);
    public float GetScrollDelta() => Input.mouseScrollDelta.y;

    public void Enable()
    {
        throw new NotImplementedException();
    }

    public void Disable()
    {
        throw new NotImplementedException();
    }

    public void SwitchActionMap(string actionMapName)
    {
        throw new NotImplementedException();
    }

    public void SetCursorState(bool locked)
    {
        throw new NotImplementedException();
    }
}

