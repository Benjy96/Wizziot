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

    void Resume()
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
}
