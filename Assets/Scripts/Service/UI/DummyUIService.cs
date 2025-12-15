using System;
using UnityEngine;

public class DummyUIService : IUIService
{
    public void ShowPrompt(string text) => Debug.Log($"[UI] {text}");
    public void HidePrompt() { }
    public void ShowHoldProgress(float progress) { }
    public void HideHoldProgress() { }
    public void ShowMessage(string message, float duration = 3f)
        => Debug.Log($"[UI] {message}");
    public void ShowItemPickup(string itemName, Sprite icon)
        => Debug.Log($"[UI] Picked up: {itemName}");

    public void ShowDocument(string title, Sprite image, string content, Action onClose)
    {
        throw new NotImplementedException();
    }

    public void CloseDocument()
    {
        throw new NotImplementedException();
    }

}