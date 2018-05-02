using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public class Options : MonoBehaviour {

    public TMPro.TMP_Dropdown dropdown;

	public void SetDifficulty()
    {
        //Debug.Log("Game Difficulty Set: " + (Difficulty)dropdown.value);
        Difficulty diff = (Difficulty)dropdown.value;

        RewriteEncryptedFileDifficulty(diff);
        RewriteJSONFileDifficulty(diff);

        Debug.Log("Game Difficulty Updated");
    }

    //Open file, modify difficulty, save
    private void RewriteEncryptedFileDifficulty(Difficulty diff)
    {
        SaveData saveData = Loader.GetEncryptedSaveFile();
        if(saveData != null)
        {
            //Modify difficulty
            saveData.Save(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], (int)diff);
            Saver.WriteToEncryptedSaveFile(saveData);
        }
    }

    //Open file, modify difficulty, save
    private void RewriteJSONFileDifficulty(Difficulty diff)
    {
        SaveData saveData = Loader.GetJSONSaveFile();
        if(saveData != null)
        {
            //Modify difficulty
            saveData.Save(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], (int)diff);
            Saver.WriteToJsonSaveFile(saveData);
        }
    }
}
