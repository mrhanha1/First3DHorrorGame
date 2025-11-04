using UnityEngine;

public class UIServiceAdapter : IUIService
{
    private readonly InteractionUIManager uiManager;
    public UIServiceAdapter(InteractionUIManager manager)
    {
        this.uiManager = manager;
    }
    public void ShowPrompt(string text)
        => uiManager.ShowPrompt(text);
    public void HidePrompt()
        => uiManager.HidePrompt();
    public void ShowHoldProgress(float progress)
        => uiManager.UpdateHoldProgress(progress);
    public void HideHoldProgress()
        => uiManager.HideHoldProgress();
    public void ShowMessage(string message, float duration =3f)
        => uiManager.ShowMessage(message, duration);
    public void ShowItemPickup (string itemName, Sprite icon)
        =>uiManager.ShowItemPickup(itemName, icon);
}
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
}