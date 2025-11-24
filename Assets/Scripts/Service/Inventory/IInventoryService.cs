using UnityEngine;

public interface IInventoryService
{
    bool AddItem(string itemID, int quantity =1);
    bool RemoveItem(string itemID);
    bool HasItem(string itemID);
    int GetItemCount(string itemID);
    void ClearInventory();
}