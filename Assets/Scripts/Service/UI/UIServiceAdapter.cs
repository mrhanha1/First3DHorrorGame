using System;
using UnityEngine;

public class UIServiceAdapter : IUIService
{
    private readonly InteractionUIManager uiManager;
    private Action currentCloseCallback;
    private bool isDocumentActive = false;
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
    public void ShowDocument(string title, Sprite image, string content, Action onClose)
    {
        if (isDocumentActive)
        {
            Debug.LogWarning("Document already active");
            return;
        }
        isDocumentActive = true;
        currentCloseCallback = onClose;

        uiManager.SetDocumentCloseCallback(() =>
        {
            CloseDocument();
        });
        uiManager.ShowDocument(image, title, content);
    }
    public void CloseDocument()
    {
        if (!isDocumentActive)
        {
            Debug.LogWarning("[UIServiceAdapter] Document not active, cannot close");
            return;
        }
        Debug.Log("[UIServiceAdapter] Closing document");
        isDocumentActive = false;

        var callback = currentCloseCallback;
        currentCloseCallback = null;

        uiManager.CloseDocument();
        callback?.Invoke();
        Debug.Log("[UIServiceAdapter] Document closed, callback invoked");
    }
}