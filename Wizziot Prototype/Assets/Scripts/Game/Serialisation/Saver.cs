using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    ///// <summary>
    ///// Where the what is saved is stored (where the magic happens)
    ///// </summary>
    ///// <returns>A data structure containing key value (dictionary) pairs with all decided upon info to be tracked for game objects in the scene</returns>
    private SaveData CreateSaveGame()
    {
        SaveData save = new SaveData();

        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.PlayerPosition], PlayerManager.Instance.player.transform.position);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.PlayerHealth], PlayerManager.Instance.player.GetComponent<EntityStats>().CurrentHealth);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Inventory], Inventory.Instance.items);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Coins], Inventory.Instance.coins);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.Equipped], PlayerManager.Instance.equipped);
        save.Save(GameMetaInfo._STATE_DATA[(int)StateData.MissionsActive], MissionManager.Instance.activeMissions);

        //Ensure all state data has been added to the save list
        if (save.savedItems != GameMetaInfo._STATE_DATA.Count) throw new System.Exception("Not all required data has been saved");

        return save;
    }
}
