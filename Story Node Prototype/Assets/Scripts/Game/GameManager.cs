using System.IO;
using UnityEngine;

//TODO: Use save / load prototype & Unity adventure game tutorial
public class GameManager : MonoBehaviour {

    //Singleton & accessor
    private static GameManager _GameManager = null;
    public static GameManager Instance { get { return _GameManager; } }

    private string gameDataFileName;
    private string filepath;

    private void Awake()
    {
        //Singleton setup
        if (_GameManager == null)
        {
            _GameManager = this;
        }
        else if (_GameManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        gameDataFileName = "wizziot.json";
        filepath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
    }

    void LoadGame()
    {
        StoryManager.Instance.LoadStory();
    }

    void SaveGame()
    {
        StoryManager.Instance.SaveStory();
        //Save game state data
    }

    void ExitGame()
    {

    }
}
