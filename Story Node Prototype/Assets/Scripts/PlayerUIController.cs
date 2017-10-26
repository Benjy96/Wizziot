using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

    public Button buttonPrefab;
    public RectTransform playerChoiceBox;
    public Button[] playerChoiceButtons;

    private void Awake()
    {
        foreach(Button x in playerChoiceButtons)
        {

        }
    }

    //public Button PresentChoices()
    //{

    //}

    public void RemoveChoices()
    {

    }
}
