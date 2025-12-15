using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private float triggerDelay = 0f;

    [Header("Item Requirement")]
    [SerializeField] private bool requiresItem = false;
    [SerializeField] private string requiredItemID;
    [SerializeField] private bool consumeItemOnUse = false;

    private TriggerReward[] triggerReward;
    private IInventoryService inventoryService;
    private bool isTriggered = false;

    private void Awake()
    {
        triggerReward = GetComponents<TriggerReward>();
        inventoryService = ServiceLocator.Get<IInventoryService>();

        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || isTriggered) return;

        if (requiresItem && !inventoryService.HasItem(requiredItemID))
        {
            return;
        }
        if (triggerDelay > 0)
            Invoke(nameof(ExecuteTrigger), triggerDelay);
        else
            ExecuteTrigger();
    }

    private void ExecuteTrigger()
    {
        if (oneTimeUse) isTriggered = true;
        foreach (var reward in triggerReward)
            reward?.GiveReward();
    }
}