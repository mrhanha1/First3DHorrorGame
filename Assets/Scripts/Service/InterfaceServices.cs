using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInputService
{
    // new input system
    Vector2 MoveInput { get; }
    Vector2 LookInput { get; }
    bool IsJumpPressed { get;}
    bool IsSprintHeld { get; }

    bool IsInteractPressed { get; }
    bool IsInteractReleased { get; }
    bool IsInteractHeld { get; }
    bool IsCancelPressed { get; }

    void Enable();
    void Disable();
    void SwitchActionMap(string actionMapName);
    void SetCursorState(bool locked);
    PlayerInputActions GetInputActions();
}


public interface ICameraProvider
{
    Camera GetCamera();
    Transform GetCameraTransform();
    Vector3 GetCameraPosition();
    Vector3 GetCameraForward();
    Vector3 GetCameraUp();
    Vector3 GetCameraRight();
    bool IsValid();
}

public interface IAudioService
{
    void PlaySound(AudioClip clip, Vector3 position, float volume = 1f);
    void PlaySound2D(AudioClip clip, float volume = 1f);
    void PlaySoundAtTransform(AudioClip clip, Transform targetTransform, float volume = 1f);
    void StopAllSounds();
}

public interface IHighlighter
{
    void Highlight(GameObject target, Color color);
    void RemoveHighlight(GameObject target);
}

public interface ICameraService
{
    void Shake(float intensity, float duration);
    void ForceLookAt(Transform target, float duration);
    void Zoom(float fov, float duration);
    void ResetZoom(float duration);
}

public interface IUIService
{
    void ShowPrompt(string text);
    void HidePrompt();
    void ShowHoldProgress(float progress);
    void HideHoldProgress();
    void ShowMessage(string message, float duration = 3f);
    void ShowItemPickup(string itemName, Sprite icon);
    void ShowDocument(string title, Sprite image, string content, System.Action onClose);
    void CloseDocument();
}
