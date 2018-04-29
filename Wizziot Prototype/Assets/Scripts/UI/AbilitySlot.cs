using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

//To display available abilities & their keybind
public class AbilitySlot : MonoBehaviour {

    public bool selectedAbilitySlot = false;

    public TextMeshProUGUI abilKeyDisplay;
    public Image abilityImage;

    [SerializeField] private Abilities slotAbility; //Use Setter method "PlaceAbilityInSlot" to handle all related info
    public Abilities SlotAbility { get { return slotAbility; } private set { slotAbility = value; } }

    public TextMeshProUGUI cooldownText;
    private AbilityComponent playerAbilComponent;
    private float currentCD;
    private IEnumerator coroutine;

    KeyCode keybind;
    // Use this for initialization
    void Start () {
        if(!selectedAbilitySlot) PlaceAbilityInSlot();

        GameManager.Instance.OnGameLoaded += PlaceAbilityInSlot;

        playerAbilComponent = PlayerManager.Instance.player.GetComponent<AbilityComponent>();
        if(selectedAbilitySlot) playerAbilComponent.OnPlayerAbilityUsed += DisplayCooldownText;
	}

    /// <summary>
    /// Only called after an ability has been selected - the selected ability will be in this selected slot
    /// </summary>
    private void DisplayCooldownText()
    {
        if(coroutine != null) StopCoroutine(coroutine);
        currentCD = playerAbilComponent.GetCurrentCooldown(slotAbility);
        if (currentCD > 0f)
        {
            coroutine = DecrementCooldownText(currentCD);
            StartCoroutine(coroutine);
        }
        else
        {
            cooldownText.SetText("");
        }
    }

    public void PlaceAbilityInSlot()
    {
        //Keybind & text
        keybind = GameMetaInfo.abilityKeybinds[slotAbility];
        abilKeyDisplay.text = keybind.ToString();

        //Ability Image
        abilityImage.enabled = true;
        abilityImage.sprite = GameMetaInfo.abilityIcons[slotAbility];
    }

    /// <summary>
    /// Called upon key pressed from player
    /// </summary>
    /// <param name="abil"></param>
    public void PlaceAbilityInSlot(Abilities abil)
    {
        //Set cooldown text on switch (if it has a cooldown)
        if (selectedAbilitySlot && abil != slotAbility)
        {
            //Keybind & text
            keybind = GameMetaInfo.abilityKeybinds[abil];
            abilKeyDisplay.text = keybind.ToString();

            //Ability Image
            slotAbility = abil;
            abilityImage.enabled = true;
            abilityImage.sprite = GameMetaInfo.abilityIcons[abil];

            DisplayCooldownText();
        }
        else
        {
            //Keybind & text
            keybind = GameMetaInfo.abilityKeybinds[abil];
            abilKeyDisplay.text = keybind.ToString();

            //Ability Image
            slotAbility = abil;
            abilityImage.enabled = true;
            abilityImage.sprite = GameMetaInfo.abilityIcons[abil];
        }
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

    private IEnumerator DecrementCooldownText(float cooldown)
    {
        cooldownText.SetText("");
        while (cooldown > 0)
        {
            cooldownText.SetText(cooldown.ToString("F1"));
            yield return new WaitForSeconds(.1f);
            cooldown -= .1f;
        }
        if (cooldown <= 0f)
        {
            cooldownText.SetText("");
        }
    }
}
