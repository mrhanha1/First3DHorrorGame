using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUIManager : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Text promptText;

    [Header("Hold Progress")]
    [SerializeField] private GameObject holdProgressPanel;
    [SerializeField] private Image holdProgressBar;

    [Header("Message")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Text messageText;

    [Header("Item Pickup")]
    [SerializeField] private GameObject itemPickupPanel;
    [SerializeField] private Text pickupText;
    [SerializeField] private Image pickupIcon;

    [Header("Document")]
    [SerializeField] private GameObject documentPanel;
    [SerializeField] private Image documentImage;
    [SerializeField] private Text documentTitle;
    [SerializeField] private Text documentContent;
    [SerializeField] private Button CloseButton;

    private Action onDocumentClose;
    private void Awake()
    {
        HideAll();
        if (CloseButton)
        {
            CloseButton.onClick.AddListener(() =>
            {
                CloseDocument();
                Cursor.lockState = CursorLockMode.Locked;
            });
        }
    }
    private void HideAll()
    {
        HidePrompt();
        HideHoldProgress();
        if (messagePanel) messagePanel.SetActive(false);
        if (itemPickupPanel) itemPickupPanel.SetActive(false);
        if (documentPanel) documentPanel.SetActive(false);
    }
    public void ShowPrompt(string text, bool instant = false)
    {
        if (promptPanel == null || promptText == null) return;
        promptText.text = text;
        promptPanel.SetActive(true);
    }
    public void UpdateHoldProgress (float progress)
    {
        if (holdProgressPanel == null || holdProgressBar == null) return;
        if (!holdProgressPanel.activeSelf) holdProgressPanel.SetActive(true);
        holdProgressBar.fillAmount = Mathf.Clamp01(progress);
    }
    public void ShowMessage(string message, float duration = 3f)
    {
        if (messagePanel == null || messageText == null) return;
        messageText.text = message;
        messagePanel.SetActive(true);

        DOVirtual.DelayedCall(duration, () =>
        {
            if (messagePanel) messagePanel.SetActive(false);
        });
    }
    public void ShowItemPickup(string itemName, Sprite icon)
    {
        if (itemPickupPanel == null)
        {
            Debug.LogWarning("[InteractionUIManager] Item Pickup Panel is missing.");
            return;
        }
        pickupText.text = $"Nhận được {itemName}";
        if (pickupIcon && icon) pickupIcon.sprite = icon;

        itemPickupPanel.SetActive(true);

        DOVirtual.DelayedCall(3f, () =>
        {
            if (itemPickupPanel) itemPickupPanel.SetActive(false);
        });
    }
    public void ShowDocument(Sprite image, string title, string content)
    {
        if (documentPanel == null) return;

        if (documentImage) documentImage.sprite = image;
        else Debug.LogWarning("[InteractionUIManager] Document Image is missing.");
        if (documentTitle) documentTitle.text = title;
        else Debug.LogWarning("[InteractionUIManager] Document Title is missing.");
        if (documentContent) documentContent.text = content;
        else Debug.LogWarning("[InteractionUIManager] Document Content is missing.");
        documentPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(WaitForDocumentClose());
    }
    private IEnumerator WaitForDocumentClose()
    {
        var inputService = ServiceLocator.Get<IInputService>();
        while (documentPanel.activeSelf)
        {
            if (inputService.IsCancelPressed)
            {
                CloseDocument();
                Cursor.lockState = CursorLockMode.Locked;
                yield break;
            }
            yield return null;
        }
    }
    public void CloseDocument()
    {
        if (!documentPanel.activeSelf) return;
        documentPanel?.SetActive(false);
        onDocumentClose?.Invoke();
        onDocumentClose = null;
    }
    public void SetDocumentCloseCallback(Action callback)
    {
        onDocumentClose = callback;
    }
    public void HidePrompt() => promptPanel?.SetActive(false);
    public void HideHoldProgress() => holdProgressPanel?.SetActive(false);
    public void HideMessagePanel() => messagePanel?.SetActive(false);
    public void HideItemPickupPanel() => itemPickupPanel?.SetActive(false);
}
