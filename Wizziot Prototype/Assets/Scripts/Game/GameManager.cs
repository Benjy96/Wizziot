using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Action OnGameLoaded;
    private bool gameLoaded = false;

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
        AudioManager.Instance.Play("Forest");
    }

    private void Update()
    {
        if (!gameLoaded && OnGameLoaded != null)
        {
            gameLoaded = true;
            LoadGame();
            OnGameLoaded.Invoke();
            Debug.Log("Invoked OnGameLoaded");
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
