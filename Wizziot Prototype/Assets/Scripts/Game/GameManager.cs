using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Action onGameLoaded;

    //Singleton & accessor
    private static GameManager _GameManager = null;
    public static GameManager Instance { get { return _GameManager; } }

    public event Action OnDifficultyChanged;

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

    private void Start()
    {
        LoadGame();
        Debug.Log("Game Difficulty: " + GameMetaInfo._GAME_DIFFICULTY);
    }

    public void LoadGame()
    {
        //TODO: remove hard-coding of save/load keys
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

    //Assuming param is from a button (e.g. menu - buttons are ints)
    public void ChangeDifficulty(int difficulty)
    {
        //Adjust static difficulty tracker variable
        difficulty -= 1;
        GameMetaInfo._GAME_DIFFICULTY = (Difficulty)difficulty;
        //Inform interested parties (scripts that use difficulty to modify their stats, etc.)
        OnDifficultyChanged.Invoke();
    }
}
