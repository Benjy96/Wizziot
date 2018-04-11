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
        LoadAbilityKeybinds(data);
        LoadScriptableObjects(data);

        int difficulty = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], ref difficulty);
        GameMetaInfo._GAME_DIFFICULTY = (Difficulty)difficulty;

        Vector3 newPlayerPos = new Vector3();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.PlayerPosition], ref newPlayerPos);
        PlayerManager.Instance.player.transform.position = newPlayerPos;

        int playerHealth = 0;
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.PlayerHealth], ref playerHealth);
        PlayerManager.Instance.player.GetComponent<EntityStats>().CurrentHealth = playerHealth;

        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Coins], ref Inventory.Instance.coins);

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

        //Set Ability KeyCodes using ability keybinds (Abil/KeyCode)
        Dictionary<Abilities, KeyCode> savedAbilKeybinds = new Dictionary<Abilities, KeyCode>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], ref savedAbilKeybinds);

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
        for (int i = 0; i < equipment.Count; i++)
        {
            GameObject newItem = Instantiate((GameObject)Resources.Load("Item Prefabs/" + equipment[i].prefabName), PlayerManager.Instance.player.transform.position, Quaternion.identity);
            PlayerManager.Instance.EquipItem(newItem.GetComponent<Item>());
        }

        //Inventory
        List<Equipment> items = new List<Equipment>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Inventory], ref items);
        for(int i = 0; i < items.Count; i++)
        {
            GameObject inventoryItem = Instantiate((GameObject)Resources.Load("Item Prefabs/" + items[i].prefabName), PlayerManager.Instance.player.transform.position, Quaternion.identity);
            Debug.Assert(inventoryItem != null);
            Debug.Log("Adding to inventory: " + inventoryItem.name);
            inventoryItem.GetComponent<Item>().AddToInventoryFromSaveFile();
        }

        //Missions
        List<Mission> missions = new List<Mission>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.MissionsActive], ref missions);
        foreach (Mission m in missions)
        {
            if(m.parentMission == null)
            {
                MissionManager.Instance.GrantMission(m, m.inkName);
            }
            else
            {
                MissionManager.Instance.GrantChildMission(m, m.inkName, ref m.missionRewards);
            }
            
        }
    }
    #endregion
}