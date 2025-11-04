using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IInputService
{
    private readonly KeyCode interactKey = KeyCode.E;
    private readonly KeyCode cancelKey = KeyCode.Escape;

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
}

