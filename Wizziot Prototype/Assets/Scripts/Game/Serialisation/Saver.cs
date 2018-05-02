using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public static class Saver {

    // ----- Save ----- //
    public static void SaveGame(bool encrypt)
    {
        string file = GameManager.Instance.GameSaveFile;

        //Create a save data object
        SaveData saveData = CreateSaveGame();
        
        //Serialization process
        if (encrypt)
        {
            using (var fs = File.Open(file, FileMode.Create))
            {
                using (var writer = new BsonWriter(fs))
                {
                    var serializer = new JsonSerializer()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    serializer.Serialize(writer, saveData);
                }
            }
        }
        else
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(saveData, Formatting.Indented,
                    new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }
                    ));
        }
    }

    /// <summary>
    /// Creates a save object
    /// </summary>
    /// <returns>A data structure containing key-value pairs with all data to be maintained between game sessions</returns>
    private static SaveData CreateSaveGame()
    {
        SaveData save = new SaveData();

        //Primitive data types
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Scene], SceneManager.GetActiveScene().name);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], (int)GameMetaInfo._GAME_DIFFICULTY);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.PlayerPosition], PlayerManager.Instance.player.transform.position);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.PlayerHealth], PlayerManager.Instance.playerStats.CurrentHealth);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Coins], Inventory.Instance.coins);

        //Custom Data Types (Keybinds, Equipment, items, missions)
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], GameMetaInfo.abilityKeybinds);
        
        List<Equipment> equipment = new List<Equipment>();
        foreach (Item item in Inventory.Instance.items)
        {
            equipment.Add(item.equipment);
        }
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Inventory], equipment);

        List<Equipment> equipped = new List<Equipment>();
        foreach (Item item in PlayerManager.Instance.equipped)
        {
            if (item == null) continue;
            equipped.Add(item.equipment);
        }
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Equipped], equipped);

        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.MissionsActive], MissionManager.Instance.activeMissions);

        //Ensure all state data has been saved
        if (save.savedItems != GameMetaInfo._STATE_DATA.Count) throw new System.Exception("Not all state data has been saved");

        return save;
    }

    public static void WriteToEncryptedSaveFile(SaveData data)
    {
        using (var fs = File.Open(GameMetaInfo._SAVE_FILE_ENCRYPTED, FileMode.Create))
        {
            using (var writer = new BsonWriter(fs))
            {
                var serializer = new JsonSerializer()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                serializer.Serialize(writer, data);
            }
        }
    }

    public static void WriteToJsonSaveFile(SaveData data)
    {
        File.WriteAllText(GameMetaInfo._SAVE_FILE_JSON, JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            ));
    }
}
