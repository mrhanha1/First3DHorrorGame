
using System.Collections;
using UnityEngine;

public class DoorInteractable : InteractableBase, ILockable
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPanel;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private string requiredKeyID = "";
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openDuration = 2f;

    [Header("Audio")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorLockedSound;

    private bool isOpen = false;
    private bool isAnimating = false;
    private Vector3 initialRotation;

    public bool IsLocked => isLocked;
    public string RequiredKeyID => requiredKeyID;

    protected override void Awake()
    {
        base.Awake();
        if (doorPanel != null) doorPanel = this.transform;
        initialRotation = doorPanel.localEulerAngles;
        UpdatePrompt();
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
            PlaySound(doorLockedSound);
            return;
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
        float elapsedTime = 0f;
        Vector3 startRotation = doorPanel.localEulerAngles;
        Vector3 targetRotation = initialRotation + new Vector3(0, openAngle, 0);
        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            doorPanel.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, elapsedTime / openDuration);
            yield return null;
        }
        doorPanel.localEulerAngles = targetRotation;
        isOpen = true;
        isAnimating = false;
    }
    private IEnumerator CloseDoor()
    {
        isAnimating = true;
        float elapsedTime = 0f;
        Vector3 startRotation = doorPanel.localEulerAngles;
        Vector3 targetRotation = initialRotation - new Vector3(0, openAngle, 0);
        while (elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            doorPanel.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, elapsedTime / openDuration);
            yield return null;
        }
        PlaySound(doorLockedSound);
        doorPanel.localEulerAngles = targetRotation;
        isOpen = false;
        isAnimating = false;
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
            promptText = "Locked";
        }
        else
        {
            promptText = isOpen ? "Close Door" : "Open Door";
        }
    }

}