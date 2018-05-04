using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        //Ensure correct scene loaded
        string sceneName = "";
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Scene], ref sceneName);
        if (!sceneName.Equals(""))
        {
            if (SceneManager.GetActiveScene().name != sceneName)
            {
                SceneManager.LoadScene(sceneName);
            }
        }

        LoadAbilityKeybinds(data);
        LoadScriptableObjects(data);

        int difficulty = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], ref difficulty);
        GameMetaInfo._GAME_DIFFICULTY = (Difficulty)difficulty;

        Vector3 newPlayerPos = new Vector3();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.PlayerPosition], ref newPlayerPos);
        if(newPlayerPos != Vector3.zero) PlayerManager.Instance.player.transform.position = newPlayerPos;

        int playerHealth = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.PlayerHealth], ref playerHealth);
        if(playerHealth != 0) PlayerManager.Instance.player.GetComponent<AgentStats>().CurrentHealth = playerHealth;

        int coins = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Coins], ref Inventory.Instance.coins);
        if (coins != 0) Inventory.Instance.coins = coins;

        return true;
    }

    public static SaveData GetEncryptedSaveFile()
    {
        //Set difficulty in json file
        if (File.Exists(GameMetaInfo._SAVE_FILE_ENCRYPTED))
        {
            //Get data from file
            SaveData savedData = new SaveData();
            using (var fs = File.OpenRead(GameMetaInfo._SAVE_FILE_ENCRYPTED))
            {
                using (var reader = new BsonReader(fs))
                {
                    var serializer = new JsonSerializer();
                    savedData = serializer.Deserialize<SaveData>(reader);
                }
            }
            return savedData;
        }
        return null;
    }

    public static SaveData GetJSONSaveFile()
    {
        if (File.Exists(GameMetaInfo._SAVE_FILE_JSON))
        {
            //Get save game file
            SaveData savedData = new SaveData();
            string dataAsJSON = File.ReadAllText(GameMetaInfo._SAVE_FILE_JSON);
            savedData = JsonConvert.DeserializeObject<SaveData>(dataAsJSON);
            return savedData;
        }
        return null;
    }

    #region Load Implementations
    private static void LoadAbilityKeybinds(SaveData data)
    {
        //Set Ability KeyCodes using ability keybinds (Abil/KeyCode)
        Dictionary<Abilities, KeyCode> savedAbilKeybinds = new Dictionary<Abilities, KeyCode>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], ref savedAbilKeybinds);
        if (savedAbilKeybinds.Count == 0) return;

        //Use Ability KeyCodes to set Action KeyCodes
        //Temp so can iterate and modify at same time (iterate temp)
        Dictionary<Abilities, KeyCode> abilKeybindsIteratable = new Dictionary<Abilities, KeyCode>(GameMetaInfo.abilityKeybinds);

        //Change all ABILITY keybinds
        foreach (KeyValuePair<Abilities, KeyCode> item in abilKeybindsIteratable)
        {
            Abilities currentAbil = item.Key;
            KeyCode currentAbilKey = item.Value;
            System.Action currentKeyAction = GameMetaInfo.keybindActions[currentAbilKey];

            //Update Dictionaries
            GameMetaInfo.keybindActions.Remove(currentAbilKey);
            GameMetaInfo.keybindActions.Add(savedAbilKeybinds[currentAbil], currentKeyAction);
            GameMetaInfo.abilityKeybinds[currentAbil] = savedAbilKeybinds[currentAbil];
        }
    }

    private static void LoadScriptableObjects(SaveData data)
    {
        //Equipment
        List<Equipment> equipment = new List<Equipment>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Equipped], ref equipment);
        if (equipment.Count > 0)
        {
            for (int i = 0; i < equipment.Count; i++)
            {
                GameObject newItem = Instantiate((GameObject)Resources.Load("Items/" + equipment[i].prefabName), PlayerManager.Instance.player.transform.position, Quaternion.identity);
                Debug.Assert(newItem != null);  //If triggered, items are likely stored in the wrong location
                PlayerManager.Instance.EquipItem(newItem.GetComponent<Item>());
            }
        }

        //Inventory
        List<Equipment> items = new List<Equipment>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Inventory], ref items);
        if (items.Count > 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                GameObject inventoryItem = Instantiate((GameObject)Resources.Load("Items/" + items[i].prefabName), PlayerManager.Instance.player.transform.position, Quaternion.identity);
                Debug.Assert(inventoryItem != null);    //If triggered, items are likely stored in the wrong location
                inventoryItem.GetComponent<Item>().AddToInventoryFromSaveFile();
            }
        }

        //Missions
        List<Mission> missions = new List<Mission>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.MissionsActive], ref missions);
        if (missions.Count > 0)
        {
            foreach (Mission m in missions)
            {
                if (m.missionStage > -1)
                {
                    MissionManager.Instance.GrantChildMission(m);
                }
                else
                {
                    MissionManager.Instance.GrantMission(m, m.inkName);
                }
            }
        }
    }
    #endregion
}