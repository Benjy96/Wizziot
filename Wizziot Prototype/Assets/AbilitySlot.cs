using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//To display available abilities & their keybind
public class AbilitySlot : MonoBehaviour {

    public TextMeshProUGUI abilKeyDisplay;
    public Image abilityImage;
    public Abilities slotAbility;

    KeyCode keybind;
    // Use this for initialization
    void Start () {
        PlaceAbilityInSlot(slotAbility);
	}
	
	public void PlaceAbilityInSlot(Abilities abil)
    {
        //Keybind & text
        keybind = GameMetaInfo.abilityKeybinds[abil];
        abilKeyDisplay.text = keybind.ToString();

        //Ability Image
        slotAbility = abil;
        abilityImage.enabled = true;
        abilityImage.sprite = GameMetaInfo.abilityIcons[abil];
    }

    public void RemoveAbilityFromSlot(Abilities abil)
    {
        //Keybind & text
        abilKeyDisplay.text = "";
        
        //Ability Image
        abilityImage.sprite = null;
        abilityImage.enabled = false;
    }

    //For clicking the ability (Don't do this, noob)
    public void Use()
    {
        try
        {
            GameMetaInfo.keybindActions[keybind]();
        }
        catch (Exception e)
        {
            Debug.Log("Keybind " + keybind + " not able to execute");
        }
    }
}
