using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveService : ISaveService
{
    private const int MAX_SAVE_SLOTS = 4;
    private const string SAVE_FOLDER = "Saves";
    private string savePath;

    public SaveService()
    {
        savePath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        Debug.Log($"[SaveService] Initialized. Save path: {savePath}");
    }

    public void SaveGame(int slotIndex, string saveName)
    {
        if (!IsValidSlot(slotIndex)) return;

        GameSaveData saveData = CollectGameData(saveName);
        SerializableGameData serializableData = ConvertToSerializable(saveData);

        string json = JsonUtility.ToJson(serializableData, true);
        string filePath = GetSaveFilePath(slotIndex);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"[SaveService] Game saved to slot {slotIndex}: {saveName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveService] Failed to save game: {e.Message}");
        }
    }

    public bool LoadGame(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return false;
        if (!HasSave(slotIndex)) return false;

        string filePath = GetSaveFilePath(slotIndex);

        try
        {
            string json = File.ReadAllText(filePath);
            SerializableGameData serializableData = JsonUtility.FromJson<SerializableGameData>(json);
            GameSaveData saveData = ConvertFromSerializable(serializableData);

            ApplyGameData(saveData);
            Debug.Log($"[SaveService] Game loaded from slot {slotIndex}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveService] Failed to load game: {e.Message}");
            return false;
        }
    }

    public void DeleteSave(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return;

        string filePath = GetSaveFilePath(slotIndex);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"[SaveService] Save slot {slotIndex} deleted");
        }
    }

    public bool HasSave(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return false;
        return File.Exists(GetSaveFilePath(slotIndex));
    }

    public SaveSlotData GetSaveSlotInfo(int slotIndex)
    {
        SaveSlotData slotData = new SaveSlotData(slotIndex);

        if (!HasSave(slotIndex))
        {
            return slotData;
        }

        try
        {
            string json = File.ReadAllText(GetSaveFilePath(slotIndex));
            SerializableGameData data = JsonUtility.FromJson<SerializableGameData>(json);

            slotData.isEmpty = false;
            slotData.saveName = data.saveName;
            slotData.saveDate = data.saveDate;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveService] Failed to read slot info: {e.Message}");
        }

        return slotData;
    }

    public SaveSlotData[] GetAllSaveSlots()
    {
        SaveSlotData[] slots = new SaveSlotData[MAX_SAVE_SLOTS];
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            slots[i] = GetSaveSlotInfo(i);
        }
        return slots;
    }

    // Helper Methods
    private bool IsValidSlot(int slotIndex)
    {
        bool valid = slotIndex >= 0 && slotIndex < MAX_SAVE_SLOTS;
        if (!valid)
        {
            Debug.LogWarning($"[SaveService] Invalid slot index: {slotIndex}");
        }
        return valid;
    }

    private string GetSaveFilePath(int slotIndex)
    {
        return Path.Combine(savePath, $"save_slot_{slotIndex}.json");
    }

    private GameSaveData CollectGameData(string saveName)
    {
        GameSaveData data = new GameSaveData();
        data.saveName = saveName;
        data.saveDate = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // Get player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            data.playerPosition = player.transform.position;
            data.playerRotation = player.transform.rotation;
        }
        else
        {
            Debug.LogWarning("[SaveService] Player not found. Using default position.");
        }

        // Get inventory data using new method
        if (ServiceLocator.TryGet<IInventoryService>(out IInventoryService inventory))
        {
            data.inventory = inventory.GetAllItems();
            Debug.Log($"[SaveService] Saved {data.inventory.Count} inventory items");
        }

        // Get minigame states using new method
        if (ServiceLocator.TryGet<IMinigameService>(out IMinigameService minigame))
        {
            data.minigameStates = minigame.GetAllMinigameStates();
            Debug.Log($"[SaveService] Saved {data.minigameStates.Count} minigame states");
        }

        return data;
    }

    private void ApplyGameData(GameSaveData data)
    {
        // Apply player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Disable CharacterController if exists to allow position change
            var charController = player.GetComponent<CharacterController>();
            if (charController != null)
            {
                charController.enabled = false;
                player.transform.position = data.playerPosition;
                player.transform.rotation = data.playerRotation;
                charController.enabled = true;
            }
            else
            {
                player.transform.position = data.playerPosition;
                player.transform.rotation = data.playerRotation;
            }
            Debug.Log($"[SaveService] Player position loaded: {data.playerPosition}");
        }
        else
        {
            Debug.LogWarning("[SaveService] Player not found. Cannot restore position.");
        }

        // Apply inventory using new method
        if (ServiceLocator.TryGet<IInventoryService>(out IInventoryService inventory))
        {
            inventory.SetInventory(data.inventory);
            Debug.Log($"[SaveService] Loaded {data.inventory.Count} inventory items");
        }

        // Apply minigame states using new method
        if (ServiceLocator.TryGet<IMinigameService>(out IMinigameService minigame))
        {
            minigame.SetMinigameStates(data.minigameStates);
            Debug.Log($"[SaveService] Loaded {data.minigameStates.Count} minigame states");
        }
    }

    private SerializableGameData ConvertToSerializable(GameSaveData data)
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

        serializable.inventoryItems = new List<SerializableGameData.InventoryItem>();
        foreach (var item in data.inventory)
        {
            serializable.inventoryItems.Add(new SerializableGameData.InventoryItem
            {
                itemID = item.Key,
                quantity = item.Value
            });
        }

        serializable.minigameStates = new List<SerializableGameData.MinigameState>();
        foreach (var state in data.minigameStates)
        {
            serializable.minigameStates.Add(new SerializableGameData.MinigameState
            {
                minigameID = state.Key,
                isCompleted = state.Value
            });
        }

        return serializable;
    }

    private GameSaveData ConvertFromSerializable(SerializableGameData serializable)
    {
        GameSaveData data = new GameSaveData();
        data.saveName = serializable.saveName;
        data.saveDate = serializable.saveDate;
        data.playerPosition = serializable.playerPosition;
        data.playerRotation = new Quaternion(
            serializable.playerRotation.x,
            serializable.playerRotation.y,
            serializable.playerRotation.z,
            serializable.playerRotation.w
        );

        data.inventory = new Dictionary<string, int>();
        if (serializable.inventoryItems != null)
        {
            foreach (var item in serializable.inventoryItems)
            {
                data.inventory[item.itemID] = item.quantity;
            }
        }

        data.minigameStates = new Dictionary<string, bool>();
        if (serializable.minigameStates != null)
        {
            foreach (var state in serializable.minigameStates)
            {
                data.minigameStates[state.minigameID] = state.isCompleted;
            }
        }

        return data;
    }
}