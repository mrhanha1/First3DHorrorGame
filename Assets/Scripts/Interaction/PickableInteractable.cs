using UnityEngine;

public class PickableInteractable : InteractableBase
{
    [Header("Item Settings")]
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private bool destroyOnPickup = true;

    public override void OnInteract(PlayerInteractionController player)
    {
        var inventory = ServiceLocator.Get<IInventoryService>();
        if (inventory.AddItem(itemID))
        {
            var uiService = ServiceLocator.Get<IUIService>();
            uiService?.ShowItemPickup(itemName, itemIcon);
            PlaySound(interactSound);
        }
        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
    }
}