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

    public void DisplayButtons(int buttonsNeeded)
    {
        numChoices = buttonsNeeded;
        if(playerChoiceBox.gameObject.activeSelf == false) playerChoiceBox.gameObject.SetActive(true);
        for (int i = 0; i < numChoices; i++)
        {
            playerChoiceButtons[i].gameObject.SetActive(true);  //Only activate the amount of buttons needed
        }

        for(int j = numChoices; j < playerChoiceButtons.Count; j++)
        {
            playerChoiceButtons[j].gameObject.SetActive(false);
        }
    }
    //TODO: we need an UPDATE choice window so that numChoices is kept updated and we can make choices
    //at a depth greater than one
    //Current Bug: input buttons stay up when cube convo ends
    //Need to make sure we track the state of numchoices and keep it updated - revolves around displayed
    //choices
    public void PresentChoice(string choiceText, int choice)
    {
        if(choice == 0)
        {
            numChoices = 1;
        }
        else
        {
            numChoices++;
        }
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
