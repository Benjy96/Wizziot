using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class KeybindsMenu : MonoBehaviour {

    Dictionary<int, KeyCode> keyCodeMappings;

    TMP_Dropdown[] dropdowns;

    private void Awake()
    {
        keyCodeMappings = new Dictionary<int, KeyCode>();

        //0 -> 9 converted to KeyCodes
        for(int i = 0; i < 10; i++)
        {
            keyCodeMappings[i] = (KeyCode)i + 48;
        }
    }

    // Use this for initialization
    void Start () {
        dropdowns = GetComponentsInChildren<TMP_Dropdown>(true);
	}
	
	public void SetKeybinds()
    {
        RewriteKeybindsEncryptedSave();
        RewriteKeybindsJsonSave();

        //Get abil slot values - take only first char
        Debug.Log("Keybinds updated");
    }

    //Open file, modify difficulty, save
    private void RewriteKeybindsEncryptedSave()
    {
        //Load save file & get temps
        SaveData data = Loader.GetEncryptedSaveFile();
        if (data == null) return;

        Dictionary<Abilities, KeyCode> tempAbilityKeys = new Dictionary<Abilities, KeyCode>();

        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], ref tempAbilityKeys);

        //Re-write binds of temp dictionaries
        int count = 0;  //Abils listed in menu in order
        foreach (TMP_Dropdown item in dropdowns)
        {
            if (item.value.Equals(0))
            {
                count++;
                continue;
            }

            //Get current ability option
            Abilities abil = (Abilities)count;
            //Get user input for new keybind
            KeyCode newKeybindForAbility = keyCodeMappings[item.value];

            //Set new keybind for current abil
            tempAbilityKeys[abil] = newKeybindForAbility;

            count++;
        }

        //Update save file
        data.Save(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], tempAbilityKeys);

        //Save
        Saver.WriteToEncryptedSaveFile(data);
    }

    //Open file, modify difficulty, save
    private void RewriteKeybindsJsonSave()
    {
        //Load save file & get temps
        SaveData data = Loader.GetJSONSaveFile();
        if (data == null) return;

        Dictionary<Abilities, KeyCode> tempAbilityKeys = new Dictionary<Abilities, KeyCode>();
        data.Load(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], ref tempAbilityKeys);

        //Re-write binds of temp dictionaries
        int count = 0;  //Abils listed in menu in order
        foreach (TMP_Dropdown item in dropdowns)
        {
            if (item.value.Equals(0))
            {
                count++;
                continue;
            }

            //Get current ability option
            Abilities abil = (Abilities)count;
            //Get user input for new keybind
            KeyCode newKeybindForAbility = keyCodeMappings[item.value];

            //Set new keybind for current abil
            tempAbilityKeys[abil] = newKeybindForAbility;

            count++;
        }

        //Update save file
        data.Save(GameMetaInfo._STATE_DATA[(int)StateData.Keybinds], tempAbilityKeys);

        //Save
        Saver.WriteToJsonSaveFile(data);
    }
}
