using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool _GamePaused = false;

    public GameObject pauseMenuUI;

    int count = 2;  //Used for determining if the story input was closed with the escape key, and whether the game should pause (or if player was only clearing UI)

    private void Start()
    {
        PlayerManager.Instance.playerControls.OnEscapeKey += PauseGame;
    }

    public void PauseGame()
    {
        if (_GamePaused) Resume();
        return;

        //Only use pausing logic if other UI elements are closed & escape wasn't used for closing UI
        if (!(Inventory.Instance.inventoryUI.InventoryUIActive && StoryManager.Instance.StoryInputEnabled))
        {
            if (StoryManager.Instance.StoryClosing) count++;
            if (count % 2 != 0) return; //if just cleared UI (one press - odd numbered) then return, else continue to pause logic

            Pause();
            count = 2;
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
        GameManager.Instance.SaveGame();
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}
