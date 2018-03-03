using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;
using UnityEngine;

public class Saver : MonoBehaviour
{
    // ----- Save ----- //
    public static void SaveGame(bool encrypt)
    {
        string file = GameManager.Instance.GameSaveFile;
        SaveData dataToSave = CreateSaveGame();

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
                    serializer.Serialize(writer, dataToSave);
                }
            }
        }
        else
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(dataToSave));
        }
    }

    /// <summary>
    /// Where the what is saved is stored (where the magic happens)
    /// </summary>
    /// <returns>A data structure containing key value (dictionary) pairs with all decided upon info to be tracked for game objects in the scene</returns>
    private static SaveData CreateSaveGame()
    {
        SaveData save = new SaveData();
        GameObject[] scene = FindObjectsOfType<GameObject>();

        Debug.Log("Saver.CreateSaveGame(): Create unique keys for the save key-value pairs");
        Debug.Log("Quick and dirty example save: Name and Vector3 of all objects, no unique IDs");

        //TODO: Save objects in game that needed to be saved here:
        foreach (GameObject x in scene)
        {
            save.Save(x.name, x.transform.position);
        }

        //Implement: loop through each gameobject and use the SaveData object to save whatever attributes we wish.
        //Need a unique key for each object - current method is to get scene name, then use name of game object & position.

        return save;
    }
}
