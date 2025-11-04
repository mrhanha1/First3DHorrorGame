using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        HideAll();
    }
    private void HideAll()
    {
        HidePrompt();
        HideHoldProgress();
        if (messagePanel) messagePanel.SetActive(false);
        if (itemPickupPanel) itemPickupPanel.SetActive(false);
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
        if (itemPickupPanel == null || pickupIcon == null) return;
        pickupText.text = itemName;
        if (pickupIcon && icon) pickupIcon.sprite = icon;

        itemPickupPanel.SetActive(true);

        DOVirtual.DelayedCall(3f, () =>
        {
            if (itemPickupPanel) itemPickupPanel.SetActive(false);
        });
    }
    public void HidePrompt() => promptPanel?.SetActive(false);
    public void HideHoldProgress() => holdProgressPanel?.SetActive(false);
    public void HideMessagePanel() => messagePanel?.SetActive(false);
    public void HideItemPickupPanel() => itemPickupPanel?.SetActive(false);
}
