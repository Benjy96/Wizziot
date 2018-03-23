using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Saver : MonoBehaviour
{
    // ----- Save ----- //
    public static void SaveGame(bool encrypt)
    {
        SaveData saveData = CreateSaveGame();
        //Dictionary<string, object> saveData = new Dictionary<string, object>
        //{
        //    { "playerPos", PlayerManager.Instance.player.transform.position },
        //    { "inventory", Inventory.Instance.items }
        //};


        string file = GameManager.Instance.GameSaveFile;

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
    private static SaveData CreateSaveGame()
    {
        SaveData save = new SaveData();

        //Player
        save.Save("playerPos", PlayerManager.Instance.player.transform.position);
        save.Save("playerHealth", PlayerManager.Instance.player.GetComponent<EntityStats>().CurrentHealth);
        save.Save("playerEquipped", PlayerManager.Instance.equipped);

        //Manager Data
        //Inventory
        save.Save("inventory", Inventory.Instance.items);
        save.Save("coins", Inventory.Instance.coins);

        //Missions
        save.Save("missions", MissionManager.Instance.activeMissions);

        return save;
    }
}
