using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool _GamePaused = false;

    public GameObject pauseMenuUI;

    public void PauseGame()
    {
        if(_GamePaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _GamePaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _GamePaused = true;
    }

    public void SaveGame()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SaveGame();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game - implement for build");
        Application.Quit();
    }
}
