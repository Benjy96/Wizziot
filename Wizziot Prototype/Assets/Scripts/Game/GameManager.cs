using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Action OnGameLoaded;
    public Action OnKeybindsChanged;

    //Singleton & accessor
    private static GameManager _GameManager = null;
    public static GameManager Instance { get { return _GameManager; } }

    public bool encryptGameSave = false;

    public string StorySaveFileName { get { return "ink_wizziot.json"; } }
    public string GameSaveFile
    {
        get
        {
            if (encryptGameSave)
            {
                return GameMetaInfo._SAVE_FILE_ENCRYPTED;
            }
            else
            {
                return GameMetaInfo._SAVE_FILE_JSON;
            }
        }
    }

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
    }

    //TODO: OnGameLoaded event in Update (i.e., if(notLoaded) LoadGame()) ?
    private void Start()
    {
        if (!SceneManager.GetActiveScene().name.Equals("Tutorial")) LoadGame();
        else AudioManager.Instance.Play("Forest");

        if (OnGameLoaded != null)
        {
            Debug.Log("Invoke OnGameLoaded");
            OnGameLoaded.Invoke();
        }
    }

    public void LoadGame()
    {
        Debug.Log("Loading story script...");
        StoryManager.Instance.LoadStory();
        Debug.Log("Loading game data...");
        Loader.LoadGame(encryptGameSave);
    }

    public void SaveGame()
    {
        Debug.Log("Saving story script...");
        StoryManager.Instance.SaveStory();

        Debug.Log("Saving game data...");
        Saver saver = new Saver();
        saver.SaveGame(encryptGameSave);        
    }
}
