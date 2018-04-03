using UnityEngine;

public class AbilityUI : MonoBehaviour {

    #region Singleton
    private static AbilityUI _AbilityUI;
    public static AbilityUI Instance { get { return _AbilityUI;  } }

    AbilitySlot[] abilSlots;

    private void Awake()
    {
        if(_AbilityUI == null)
        {
            _AbilityUI = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        abilSlots = GetComponentsInChildren<AbilitySlot>(true);
    }
    #endregion

    private void Start()
    {
        UpdateAbilityDisplays();

        GameManager.Instance.OnKeybindsChanged += UpdateAbilityDisplays;
    }

    public GameObject abilityDisplay;
    public AbilitySlot selectedAbilitySlot;
	
    //Updates the selected ability slot
	public void ChangeSelectedDisplay(Abilities ability)
    {
        if (!StoryManager.Instance.StoryInputEnabled)
        {
            selectedAbilitySlot.PlaceAbilityInSlot(ability);
        }
    }

    //Check ability slots against number of player unlocked, and dynamically activate an appropriate number
    public void UpdateAbilityDisplays()
    {
        int abilEnumIndex = 0;
        foreach (var item in abilSlots)
        {
            if (item.selectedAbilitySlot) continue; //Keep selected active
            //If the ability is unlocked, place it in the slot, else set inactive
            if (!AbilityUnlocked(item.SlotAbility))
            {
                Debug.Log(item.SlotAbility);
                item.gameObject.SetActive(false);
            }
            else
            {
                item.PlaceAbilityInSlot((Abilities)abilEnumIndex);
                abilEnumIndex++;
            }
        }
    }

    private bool AbilityUnlocked(Abilities slotAbility)
    {
        for (int i = 0; i < PlayerManager.Instance.UnlockedAbilities.Count; i++)
        {
            if ((int)slotAbility == (int)PlayerManager.Instance.UnlockedAbilities[i]) return true;
        }
        return false;
    }
}
