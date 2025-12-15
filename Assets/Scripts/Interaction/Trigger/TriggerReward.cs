using UnityEngine;
using UnityEngine.Events;

public class TriggerReward : MonoBehaviour
{
    [Header("Reward Type")]
    [SerializeField] private RewardType rewardType = RewardType.None;

    [Header("Item Reward")]
    [SerializeField] private string rewardItemID;
    [SerializeField] private Sprite rewardIcon;

    [Header("Environment Reward")]
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    [Header("Stone Door Reward")]
    [SerializeField] private StoneDoorInteractable stoneDoor;

    [Header("GameObject Control")]
    [SerializeField] private GameObject[] objectsToActivate;
    [SerializeField] private GameObject[] objectsToDeactivate;

    [Header("Messages")]
    [SerializeField] private string successMessage = "";

    [Header("Events")]
    [SerializeField] private UnityEvent onTriggerActivated;

    private IInventoryService inventoryService;
    private IUIService uiService;

    public enum RewardType
    {
        None,
        Item,
        Environment,
        StoneDoor,
        ToggleObjects,
        CustomEvent
    }

    private void Awake()
    {
        inventoryService = ServiceLocator.Get<IInventoryService>();
        uiService = ServiceLocator.Get<IUIService>();

        ValidateRewardSettings();
    }

    private void ValidateRewardSettings()
    {
        switch (rewardType)
        {
            case RewardType.Item:
                if (string.IsNullOrEmpty(rewardItemID))
                    Debug.LogWarning($"[TriggerReward] {gameObject.name}: Reward type is Item but rewardItemID is empty");
                break;

            case RewardType.Environment:
                if (rewardPrefab == null)
                    Debug.LogWarning($"[TriggerReward] {gameObject.name}: Reward type is Environment but rewardPrefab is null");
                break;

            case RewardType.StoneDoor:
                if (stoneDoor == null)
                    Debug.LogWarning($"[TriggerReward] {gameObject.name}: Reward type is StoneDoor but stoneDoor is null");
                break;

            case RewardType.ToggleObjects:
                if ((objectsToActivate == null || objectsToActivate.Length == 0) &&
                    (objectsToDeactivate == null || objectsToDeactivate.Length == 0))
                    Debug.LogWarning($"[TriggerReward] {gameObject.name}: Reward type is ToggleObjects but no objects assigned");
                break;
        }
    }

    public void GiveReward()
    {
        switch (rewardType)
        {
            case RewardType.Item:
                GiveItemReward();
                break;

            case RewardType.Environment:
                GiveEnvironmentReward();
                break;

            case RewardType.StoneDoor:
                OpenStoneDoor();
                break;

            case RewardType.ToggleObjects:
                ToggleGameObjects();
                break;

            case RewardType.CustomEvent:
                InvokeCustomEvent();
                break;

            case RewardType.None:
                break;
        }

        ShowSuccessMessage();
    }

    private void GiveItemReward()
    {
        if (inventoryService != null && !string.IsNullOrEmpty(rewardItemID))
        {
            inventoryService.AddItem(rewardItemID);
            uiService?.ShowItemPickup(rewardItemID, rewardIcon);
            Debug.Log($"[TriggerReward] Item reward given: {rewardItemID}");
        }
        else
        {
            Debug.LogWarning("[TriggerReward] Cannot give item reward - inventory service or rewardItemID is null");
        }
    }

    private void GiveEnvironmentReward()
    {
        if (rewardPrefab != null)
        {
            Vector3 spawnPos = rewardSpawnPoint != null
                ? rewardSpawnPoint.position
                : transform.position + Vector3.up;

            GameObject spawnedObject = Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[TriggerReward] Environment reward spawned: {rewardPrefab.name}");
        }
        else
        {
            Debug.LogWarning("[TriggerReward] Cannot spawn environment reward - rewardPrefab is null");
        }
    }

    private void OpenStoneDoor()
    {
        if (stoneDoor != null)
        {
            var player = FindObjectOfType<PlayerInteractionController>();
            if (player != null)
            {
                stoneDoor.OnInteract(player);
                Debug.Log($"[TriggerReward] Stone door opened automatically");
                uiService?.ShowMessage("", 2f);
            }
            else
            {
                Debug.LogWarning("[TriggerReward] Cannot open stone door - player not found");
            }
        }
        else
        {
            Debug.LogWarning("[TriggerReward] Cannot open stone door - stoneDoor reference is null");
        }
    }

    private void ToggleGameObjects()
    {
        if (objectsToActivate != null)
        {
            foreach (var obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"[TriggerReward] Activated: {obj.name}");
                }
            }
        }

        if (objectsToDeactivate != null)
        {
            foreach (var obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log($"[TriggerReward] Deactivated: {obj.name}");
                }
            }
        }
    }

    private void InvokeCustomEvent()
    {
        onTriggerActivated?.Invoke();
        Debug.Log($"[TriggerReward] Custom event invoked");
    }

    private void ShowSuccessMessage()
    {
        if (!string.IsNullOrEmpty(successMessage))
        {
            uiService?.ShowMessage(successMessage, 2f);
        }
    }

    private void OnValidate()
    {
        ValidateRewardSettings();
    }
}