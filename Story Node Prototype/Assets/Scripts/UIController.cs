using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [Header("Set in Inspector")]
    public RectTransform playerChoiceBox;   //The background "underlay" for the buttons
    public List<Button> playerChoiceButtons;    //Resizable button array - may change how many choices the player can make

    private int numChoices;

    public void EnableChoiceWindow(int numButtons)
    {
        numChoices = numButtons;
        playerChoiceBox.gameObject.SetActive(true);
        for(int i = 0; i < numChoices; i++)
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
        for(int i = 0; i < numChoices; i++)
        {
            playerChoiceButtons[i].gameObject.SetActive(false); //Disable the no longer needed buttons
        }
        playerChoiceBox.gameObject.SetActive(false);
    }
}
