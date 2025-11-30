using System.Collections.Generic;
using UnityEngine;


public class InventoryService : IInventoryService
{
    private Dictionary<string, int> inventory = new Dictionary<string, int>(); //dict chua <id item, so luong>
    public bool AddItem(string itemID, int quantity = 1)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            Debug.LogWarning("Invalid itemID");
            return false;
        }
        if (inventory.ContainsKey(itemID))
        {
            while (quantity-- > 0)
                inventory[itemID]++;
        }
        else
        {
            inventory[itemID] = quantity;
        }
        Debug.Log($"[InventoryService] Added: {itemID}. New count: {inventory[itemID]}");
        return true;
    }
    public bool RemoveItem(string itemID)
    {
        if (!inventory.ContainsKey(itemID)) return false;

        inventory[itemID]--;
        if (inventory[itemID]<=0) inventory.Remove(itemID);

        return true;
    }
    public bool HasItem(string itemID)
    {
        return inventory.ContainsKey(itemID) && inventory[itemID] > 0;
    }
    public int GetItemCount(string itemID)
    {
        return inventory.ContainsKey(itemID) ? inventory[itemID] : 0;
    }
    public void ClearInventory()
    {
        inventory.Clear();
    }
    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(inventory);
    }
    public void SetInventory(Dictionary<string, int> items)
    {
        inventory.Clear();
        if (items != null)
        {
            foreach (var item in items)
                inventory[item.Key] = item.Value;
        }
        Debug.Log($"[InventoryService] Inventory loaded. Total items: {inventory.Count}");
    }
}