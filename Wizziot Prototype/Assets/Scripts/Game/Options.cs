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

            //Modify difficulty
            savedData.Save(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], (int)diff);

            //Re-write to file
            using (var fs = File.Open(GameMetaInfo._SAVE_FILE_ENCRYPTED, FileMode.Create))
            {
                using (var writer = new BsonWriter(fs))
                {
                    var serializer = new JsonSerializer()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };
                    serializer.Serialize(writer, savedData);
                }
            }
        }
    }

    //Open file, modify difficulty, save
    private void RewriteJSONFileDifficulty(Difficulty diff)
    {
        //Set difficulty in JSON file
        if (File.Exists(GameMetaInfo._SAVE_FILE_JSON))
        {
            //Get save game file
            SaveData savedData = new SaveData();
            string dataAsJSON = File.ReadAllText(GameMetaInfo._SAVE_FILE_JSON);
            savedData = JsonConvert.DeserializeObject<SaveData>(dataAsJSON);

            //Modify difficulty
            savedData.Save(GameMetaInfo._STATE_DATA[(int)StateData.GameDifficulty], (int)diff);

            //Save modified file
            File.WriteAllText(GameMetaInfo._SAVE_FILE_JSON, JsonConvert.SerializeObject(savedData, Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    }
                ));
        }
    }
}
