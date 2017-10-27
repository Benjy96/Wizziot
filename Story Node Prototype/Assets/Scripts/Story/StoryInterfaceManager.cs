using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls input options for altering the story's state
/// </summary>
public class StoryInterfaceManager : MonoBehaviour {

    #region Singleton  
    private static StoryInterfaceManager _StoryInterface = null;

    private void Awake()
    {
        if (_StoryInterface == null)
        {
            _StoryInterface = this;
        }
        else if (_StoryInterface != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // ----- SINGLETON ----- //
    public static StoryInterfaceManager Instance { get { return _StoryInterface; } }

    // ----- 2D UI ----- //
    [Header("Set in Inspector")]
    public RectTransform playerChoiceBox;   //The background "underlay" for the buttons
    public List<Button> playerChoiceButtons;    //Resizable button array - may change how many choices the player can make

    private int numChoices;

    public void EnableChoiceWindow(int numButtons)
    {
        numChoices = numButtons;
        playerChoiceBox.gameObject.SetActive(true);
        for (int i = 0; i < numChoices; i++)
        {
            playerChoiceButtons[i].gameObject.SetActive(true);  //Only activate the amount of buttons needed
        }
    }

    public void PresentChoice(string choiceText, int choice)
    {
        playerChoiceButtons[choice].GetComponentInChildren<Text>().text = choiceText;
    }

    public void DisableChoiceWindow()
    {
        for (int i = 0; i < numChoices; i++)
        {
            playerChoiceButtons[i].gameObject.SetActive(false); //Disable the no longer needed buttons
        }
        playerChoiceBox.gameObject.SetActive(false);
    }
}
