using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls input options for altering the story's state
/// </summary>
public class StoryInterfaceManager : MonoBehaviour {

    // ----- 2D UI ----- //
    [Header("Set in Inspector")]
    [SerializeField] private RectTransform playerChoiceBox;   //The background "underlay" for the buttons
    [SerializeField] private List<Button> playerChoiceButtons;    //Resizable button array - may change how many choices the player can make

    private int numChoices; //The number of choices displayed

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
        playerChoiceButtons[choice].GetComponentInChildren<Text>().text = "Choice: " + (choice+1) + ": " +choiceText;
    }

    public void DisableChoiceWindow()
    {
        if (numChoices > 0)
        {
            for (int i = 0; i < numChoices; i++)
            {
                playerChoiceButtons[i].gameObject.SetActive(false); //Disable the no longer needed buttons
            }
            playerChoiceBox.gameObject.SetActive(false);
            numChoices = 0;
        }
    }
}
