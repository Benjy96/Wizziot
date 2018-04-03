using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class KeybindsMenu : MonoBehaviour {

    TMP_InputField[] inputFields;

	// Use this for initialization
	void Start () {
        inputFields = GetComponentsInChildren<TMP_InputField>(true);
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
        foreach (TMP_InputField item in inputFields)
        {
            if (item.text.Equals(""))
            {
                count++;
                continue;
            }

            //Get current ability option
            Abilities abil = (Abilities)count;
            //Get user input for new keybind
            KeyCode newKeybindForAbility = (KeyCode)Enum.Parse(typeof(KeyCode), item.text[0].ToString());

            //Get old keybind for current ability
            KeyCode oldCodeForAbil = tempAbilityKeys[abil];

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
        foreach (TMP_InputField item in inputFields)
        {
            if (item.text.Equals("")) continue;

            //Get current ability option
            Abilities abil = (Abilities)count;
            //Get user input for new keybind
            KeyCode newKeybindForAbility = (KeyCode)Enum.Parse(typeof(KeyCode), item.text[0].ToString());

            //Get old keybind for current ability
            KeyCode oldCodeForAbil = tempAbilityKeys[abil];

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
