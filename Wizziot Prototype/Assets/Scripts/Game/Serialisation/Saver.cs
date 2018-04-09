using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class Saver {

    // ----- Save ----- //
    public void SaveGame(bool encrypt)
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
    /// Where the what is saved is stored (where the magic happens)
    /// </summary>
    /// <returns>A data structure containing key value (dictionary) pairs with all decided upon info to be tracked for game objects in the scene</returns>
    private SaveData CreateSaveGame()
    {
        SaveData save = new SaveData();

        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Scene], SceneManager.GetActiveScene().name);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], (int)GameMetaInfo._GAME_DIFFICULTY);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.PlayerPosition], PlayerManager.Instance.player.transform.position);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.PlayerHealth], PlayerManager.Instance.player.GetComponent<EntityStats>().CurrentHealth);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Coins], Inventory.Instance.coins);

        #region Custom Data Types (Keybinds, Equipment, items, missions)
        //Keybind Combos
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], GameMetaInfo.abilityKeybinds);
        
        //Inventory Items
        List<Equipment> equipment = new List<Equipment>();
        foreach (Item item in Inventory.Instance.items)
        {
            equipment.Add(item.equipment);
        }
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Inventory], equipment);

        //Equipped Items
        List<Equipment> equipped = new List<Equipment>();
        foreach (Item item in PlayerManager.Instance.equipped)
        {
            if (item == null) continue;
            equipped.Add(item.equipment);
        }
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Equipped], equipped);

        //Missions
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.MissionsActive], MissionManager.Instance.activeMissions);
        #endregion

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
        if (File.Exists(GameMetaInfo._SAVE_FILE_JSON))
        {
            File.WriteAllText(GameMetaInfo._SAVE_FILE_JSON, JsonConvert.SerializeObject(data, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                ));
        }
    }
}
