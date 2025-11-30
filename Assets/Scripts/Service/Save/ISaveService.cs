using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveService
{
    void SaveGame(int slotIndex, string saveName);
    bool LoadGame(int slotIndex);
    void DeleteSave(int slotIndex);
    bool HasSave(int slotIndex);
    SaveSlotData GetSaveSlotInfo(int slotIndex);
    SaveSlotData[] GetAllSaveSlots();
}

[System.Serializable]
public class SaveSlotData
{
    public int slotIndex;
    public string saveName;
    public string saveDate;
    public bool isEmpty;

    public SaveSlotData(int index)
    {
        slotIndex = index;
        isEmpty = true;
        saveName = "";
        saveDate = "";
    }
}

[System.Serializable]
public class GameSaveData
{
    public string saveName;
    public string saveDate;

    // Player Position
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    // Inventory
    public Dictionary<string, int> inventory;

    // Minigame States
    public Dictionary<string, bool> minigameStates;

    public GameSaveData()
    {
        saveName = "New Save";
        saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        inventory = new Dictionary<string, int>();
        minigameStates = new Dictionary<string, bool>();
    }
}

// Serializable version for JSON
[System.Serializable]
public class SerializableGameData
{
    public string saveName;
    public string saveDate;
    public Vector3 playerPosition;
    public Vector4 playerRotation; // Quaternion as Vector4
    public List<InventoryItem> inventoryItems;
    public List<MinigameState> minigameStates;

    [System.Serializable]
    public class InventoryItem
    {
        public string itemID;
        public int quantity;
    }

    [System.Serializable]
    public class MinigameState
    {
        public string minigameID;
        public bool isCompleted;
    }
}