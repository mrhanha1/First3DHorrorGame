using UnityEngine;

public class PlayerInventory :MonoBehaviour
{
    private IInventoryService inventory;
    private IUIService uiService;

    private void Awake()
    {
        inventory = ServiceLocator.Get<IInventoryService>();
        uiService = ServiceLocator.Get<IUIService>();

    }
    public bool AddItem(string itemID)// => inventory.AddItem(itemID);
    {
        bool success = inventory.AddItem(itemID);
        if (success)
        {
            uiService?.ShowItemPickup(itemID, null);
            if (uiService == null)
            {
                Debug.LogWarning("[PlayerInventory] UI Service not found. Cannot show item pickup.");
            }
        }
        return success;
    }
    public bool RemoveItem(string itemID) => inventory.RemoveItem(itemID);
    public bool HasItem(string itemID) => inventory.HasItem(itemID);
}