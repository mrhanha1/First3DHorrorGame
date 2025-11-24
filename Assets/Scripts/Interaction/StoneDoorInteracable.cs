using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StoneDoorInteractable : InteractableBase, ILockable
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPanel;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyID = "";

    [Header("Animation Settings")]
    [SerializeField] private Vector3 retractOffset = new Vector3(0, 0, -0.5f);
    [SerializeField] private Vector3 slideOffset = new Vector3(2f, 0, 0);
    [SerializeField] private float retractDuration = 1f;
    [SerializeField] private float slideDuration = 1.5f;
    [SerializeField] private bool isOpen = false;

    [Header("Prompt")]
    [SerializeField] private string lockPromptText = "Không mở được";
    [SerializeField] private string OpenPromptText = "Mở cửa đá";
    [SerializeField] private string ClosePromptText = "Đóng cửa đá";

    [Header("Audio")]
    [SerializeField] private AudioClip stoneRetractSound;
    [SerializeField] private AudioClip stoneSlideSound;
    [SerializeField] private AudioClip doorLockedSound;

    private bool isAnimating = false;
    private Vector3 initialPosition;

    public bool IsLocked => isLocked;
    public string RequiredKeyID => requiredKeyID;

    protected override void Awake()
    {
        base.Awake();
        if (doorPanel == null) doorPanel = this.transform;

        if (isOpen)
        {
            // Nếu cửa đang mở, position ban đầu là vị trí đóng
            initialPosition = doorPanel.localPosition - retractOffset - slideOffset;
        }
        else
        {
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
            StartCoroutine(CloseStoneDoor());
        }
        else
        {
            StartCoroutine(OpenStoneDoor());
        }
    }

    private IEnumerator OpenStoneDoor()
    {
        isAnimating = true;

        // Bước 1: Thụt vào trong
        PlaySound(stoneRetractSound);
        Vector3 retractPosition = initialPosition + retractOffset;

        yield return doorPanel.DOLocalMove(retractPosition, retractDuration)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();

        // Bước 2: Trượt sang ngang
        PlaySound(stoneSlideSound);
        Vector3 finalPosition = retractPosition + slideOffset;

        yield return doorPanel.DOLocalMove(finalPosition, slideDuration)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        isOpen = true;
        isAnimating = false;
        UpdatePrompt();
    }

    private IEnumerator CloseStoneDoor()
    {
        isAnimating = true;

        // Bước 1: Trượt ngược lại
        PlaySound(stoneSlideSound);
        Vector3 retractPosition = initialPosition + retractOffset;

        yield return doorPanel.DOLocalMove(retractPosition, slideDuration)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        // Bước 2: Đẩy ra ngoài về vị trí ban đầu
        PlaySound(stoneRetractSound);

        yield return doorPanel.DOLocalMove(initialPosition, retractDuration)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();

        isOpen = false;
        isAnimating = false;
        UpdatePrompt();
    }

    public bool TryUnlock(string keyID)
    {
        if (!isLocked) return true;
        if (keyID != requiredKeyID) return false;

        isLocked = false;
        UpdatePrompt();
        return true;
    }

    public void Lock()
    {
        isLocked = true;
        UpdatePrompt();
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

    // Gizmos để visualize trong Editor
    private void OnDrawGizmosSelected()
    {
        if (doorPanel == null) return;

        Vector3 pos = Application.isPlaying ? initialPosition : doorPanel.localPosition;
        Vector3 worldPos = transform.TransformPoint(pos);

        // Vị trí thụt vào
        Gizmos.color = Color.yellow;
        Vector3 retractPos = transform.TransformPoint(pos + retractOffset);
        Gizmos.DrawWireCube(retractPos, doorPanel.lossyScale);
        Gizmos.DrawLine(worldPos, retractPos);

        // Vị trí trượt sang
        Gizmos.color = Color.green;
        Vector3 finalPos = transform.TransformPoint(pos + retractOffset + slideOffset);
        Gizmos.DrawWireCube(finalPos, doorPanel.lossyScale);
        Gizmos.DrawLine(retractPos, finalPos);
    }
}