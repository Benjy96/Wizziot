using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public static bool LoadGame(bool fileEncrypted)
    {
        string file = GameManager.Instance.GameSaveFile;

        if (File.Exists(file))
        {
            //Get data from file
            Debug.Log("Deserialising file");
            SaveData savedData = new SaveData();

            if (fileEncrypted)
            {
                using (var fs = File.OpenRead(file))
                {
                    using (var reader = new BsonReader(fs))
                    {
                        var serializer = new JsonSerializer();
                        savedData = serializer.Deserialize<SaveData>(reader);
                    }
                }
            }
            else
            {
                string dataAsJSON = File.ReadAllText(file);
                savedData = JsonConvert.DeserializeObject<SaveData>(dataAsJSON);
            }

            if (savedData == null) return false;

            LoadData(savedData);
            return true;
        }
        else
        {
            return false;
        }
    }

    private static void LoadData(SaveData data)
    {
        //Player
        Vector3 playerPos = new Vector3();
        data.Load("playerPos", ref playerPos);
        PlayerManager.Instance.player.transform.position = playerPos;

        int playerHealth = 0;
        data.Load("playerHealth", ref playerHealth);
        PlayerManager.Instance.player.GetComponent<EntityStats>().CurrentHealth = playerHealth;

        data.Load("playerEquipped", ref PlayerManager.Instance.equipped);

        //Inventory
        data.Load("inventory", ref Inventory.Instance.items);
        data.Load("coins", ref Inventory.Instance.coins);

        //Missions
        data.Load("missions", ref MissionManager.Instance.activeMissions);
    }
}


