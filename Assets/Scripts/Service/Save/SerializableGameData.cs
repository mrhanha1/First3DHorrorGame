using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableGameData
{
    public string saveName;
    public string saveDate;
    public Vector3 playerPosition;
    public Vector4 playerRotation;
    public List<InventoryItem> inventoryItems;
    public List<MinigameState> minigameStates;

    [Serializable]
    public class InventoryItem
    {
        public string itemID;
        public int quantity;
    }

    [Serializable]
    public class MinigameState
    {
        public string minigameID;
        public bool isCompleted;
    }

    public static SerializableGameData FromGameData(GameSaveData data)
    {
        SerializableGameData serializable = new SerializableGameData();
        serializable.saveName = data.saveName;
        serializable.saveDate = data.saveDate;
        serializable.playerPosition = data.playerPosition;
        serializable.playerRotation = new Vector4(
            data.playerRotation.x,
            data.playerRotation.y,
            data.playerRotation.z,
            data.playerRotation.w
        );

        serializable.inventoryItems = new List<InventoryItem>();
        foreach (var item in data.inventory)
        {
            serializable.inventoryItems.Add(new InventoryItem
            {
                itemID = item.Key,
                quantity = item.Value
            });
        }

        serializable.minigameStates = new List<MinigameState>();
        foreach (var state in data.minigameStates)
        {
            serializable.minigameStates.Add(new MinigameState
            {
                minigameID = state.Key,
                isCompleted = state.Value
            });
        }

        return serializable;
    }

    public GameSaveData ToGameData()
    {
        GameSaveData data = new GameSaveData();
        data.saveName = this.saveName;
        data.saveDate = this.saveDate;
        data.playerPosition = this.playerPosition;
        data.playerRotation = new Quaternion(
            this.playerRotation.x,
            this.playerRotation.y,
            this.playerRotation.z,
            this.playerRotation.w
        );

        data.inventory = new Dictionary<string, int>();
        if (this.inventoryItems != null)
        {
            foreach (var item in this.inventoryItems)
            {
                data.inventory[item.itemID] = item.quantity;
            }
        }

        data.minigameStates = new Dictionary<string, bool>();
        if (this.minigameStates != null)
        {
            foreach (var state in this.minigameStates)
            {
                data.minigameStates[state.minigameID] = state.isCompleted;
            }
        }

        return data;
    }
}