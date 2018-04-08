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
        //Ensure pause menu pausing doesn't carry over between scenes
        Time.timeScale = 1f;

        //Singleton setup
        if (_GameManager == null)
        {
            _GameManager = this;
        }
        else if (_GameManager != this)
        {
            Destroy(gameObject);
        }
        
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //AudioManager.Instance.Play("Forest");
    }

    private void Update()
    {
        if (!gameLoaded && OnGameLoaded != null)
        {
            gameLoaded = true;
            LoadGame();
        }
    }

    /// <summary>
    /// To reload scenes (use the saved scene variable in file) must go from a level without the gamemanager so we don't get into a repeating load loop, loading the current level
    /// Could also do a blank "loader" level that checks last save scene and then goes to it - no manager will mean no looping loads, so choose scene to load outside of the current scene
    /// </summary>
    public void ExitToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadGame()
    {
        Debug.Log("Loading story script...");
        StoryManager.Instance.LoadStory();
        Debug.Log("Loading game data...");
        Loader.LoadGame(encryptGameSave);

        if (OnGameLoaded != null) OnGameLoaded.Invoke();
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
