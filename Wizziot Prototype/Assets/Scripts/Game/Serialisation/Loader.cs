using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public static SaveData LoadGame(bool fileEncrypted)
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

            return savedData;
        }
        else
        {
            return null;
        }
    }
}


