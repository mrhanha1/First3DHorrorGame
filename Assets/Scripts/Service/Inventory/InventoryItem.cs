
[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public int quantity;
    public InventoryItem(string id, int quantity =1)
    {
        this.itemID = id;
        this.quantity = quantity;
    }
}