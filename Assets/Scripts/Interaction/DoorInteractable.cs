
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DoorInteractable : InteractableBase, ILockable
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPanel;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyID = "";
    [SerializeField] private Vector3 openAngle = new Vector3(0, 90, 0);
    [SerializeField] private Vector3 translateOffset = Vector3.zero;
    [SerializeField] private float openDuration = 2f;
    [SerializeField] private bool isOpen = false;

    [Header("Prompt")]
    [SerializeField] private string lockPromptText = "Bị khoá rồi";
    [SerializeField] private string OpenPromptText = "Mở cửa";
    [SerializeField] private string ClosePromptText = "Đóng cửa";

    [Header("Audio")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorLockedSound;

    private bool isAnimating = false;
    private Vector3 initialRotation;
    private Vector3 initialPosition;
    public bool IsLocked => isLocked;
    public string RequiredKeyID => requiredKeyID;

    protected override void Awake()
    {
        base.Awake();
        if (doorPanel == null) doorPanel = this.transform;
        if (isOpen)
        {
            initialRotation = doorPanel.localEulerAngles - openAngle;
            initialPosition = doorPanel.localPosition - translateOffset;
        }
        else
        {
            initialRotation = doorPanel.localEulerAngles;
            initialPosition = doorPanel.localPosition;
        }
    }

    public override string getPromptText()
    {
        if (isLocked)
        {
            return lockPromptText;
        }
        return isOpen ? ClosePromptText : OpenPromptText;
    }
    public override bool CanInteract(PlayerInteractionController player)
    {
        return !isAnimating;
    }

    public override void OnInteract(PlayerInteractionController player)
    {
        if (isAnimating) return;
        if (isLocked)
        {
            var inventory = ServiceLocator.Get<IInventoryService>();
            if (inventory != null && inventory.HasItem(requiredKeyID))
            {
                TryUnlock(requiredKeyID);
            }
            else
            {
                PlaySound(doorLockedSound);
                var uiService = ServiceLocator.Get<IUIService>();
                uiService.ShowMessage(lockPromptText);
                return;
            }
        }
        if (isOpen)
        {
            StartCoroutine(CloseDoor());
        }
        else
        {
            StartCoroutine(OpenDoor());
        }
    }
    private IEnumerator OpenDoor()
    {
        isAnimating = true;
        PlaySound(doorOpenSound);

        Vector3 targetRotation = initialRotation + openAngle;
        Vector3 targetPosition = initialPosition + translateOffset;

        Sequence sequence = DOTween.Sequence();
        sequence.SetEase(Ease.InOutSine);
        sequence.Append(doorPanel.DOLocalRotate(targetRotation, openDuration));
        sequence.Join(doorPanel.DOLocalMove(targetPosition, openDuration));
        yield return sequence.WaitForCompletion();

        isOpen = true;
        isAnimating = false;
        UpdatePrompt();
    }
    private IEnumerator CloseDoor()
    {
        isAnimating = true;

        Sequence sequence = DOTween.Sequence();
        sequence.SetEase(Ease.InOutSine);
        sequence.Append(doorPanel.DOLocalRotate(initialRotation, openDuration));
        sequence.Join(doorPanel.DOLocalMove(initialPosition, openDuration));
        yield return sequence.WaitForCompletion();

        PlaySound(doorLockedSound);
        isOpen = false;
        isAnimating = false;
        UpdatePrompt();
    }
    public bool TryUnlock(string keyID)
    {
        if (!isLocked) return true;
        if (keyID != requiredKeyID) return false;

        isLocked = false;
        return true;
    }

    public void Lock()
    {
        isLocked = true;
    }

    private void UpdatePrompt()
    {
        if (isLocked)
        {
            promptText = lockPromptText;
        }
        else
        {
            promptText = isOpen ? ClosePromptText : OpenPromptText;
        }
    }

}