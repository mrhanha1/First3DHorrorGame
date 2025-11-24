using UnityEngine;

public class PlayerInventory :MonoBehaviour
{
    private IInventoryService inventory;

    private void Awake()
    {
        inventory = GetComponent<IInventoryService>();
    }
    public bool AddItem (string itemID) => inventory.AddItem(itemID);
    public bool RemoveItem(string itemID) => inventory.RemoveItem(itemID);
    public bool HasItem(string itemID) => inventory.HasItem(itemID);
}