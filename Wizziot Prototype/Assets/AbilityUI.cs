using System.Collections.Generic;
using UnityEngine;

public class AbilityUI : MonoBehaviour {

    public GameObject abilityDisplay;
    public AbilitySlot selectedAbilityDisplay;

	// Use this for initialization
	void Start () {
        PlayerManager.Instance.playerControls.GetComponent<AbilityComponent>().OnPlayerAbilitySelected += ChangeSelectedDisplay;
	}
	
	void ChangeSelectedDisplay()
    {
        selectedAbilityDisplay.PlaceAbilityInSlot(PlayerManager.Instance.playerAbilityComponent.SelectedAbility);
    }
}
