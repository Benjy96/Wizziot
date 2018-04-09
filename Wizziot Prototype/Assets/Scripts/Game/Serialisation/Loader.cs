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

    private static bool LoadData(SaveData data)
    {
        int difficulty = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], ref difficulty);
        GameMetaInfo._GAME_DIFFICULTY = (Difficulty) difficulty;

        Vector3 newPlayerPos = new Vector3();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.PlayerPosition], ref newPlayerPos);
        PlayerManager.Instance.player.transform.position = newPlayerPos;

        int playerHealth = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.PlayerHealth], ref playerHealth);
        PlayerManager.Instance.player.GetComponent<EntityStats>().CurrentHealth = playerHealth;

        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Equipped], ref PlayerManager.Instance.equipped);
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Inventory], ref Inventory.Instance.items);
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Coins], ref Inventory.Instance.coins);
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.MissionsActive], ref MissionManager.Instance.activeMissions);

        if (data.loadedItems != GameMetaInfo._STATE_DATA.Count)
        {
            throw new System.Exception("Not all registered state data types have been loaded");
        }
        else
        {
            return true;
        }
    }
}


