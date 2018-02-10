using System;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
                return Path.Combine(Application.streamingAssetsPath, "wizziot.dat");
            }
            else
            {
                return Path.Combine(Application.streamingAssetsPath, "wizziot.json");
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

    public void ChangeDifficulty()
    {
        OnDifficultyChanged.Invoke();
    }
}
