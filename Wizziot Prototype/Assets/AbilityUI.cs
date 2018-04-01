using UnityEngine;

public class AbilityUI : MonoBehaviour {

    #region Singleton
    private static AbilityUI _AbilityUI;
    public static AbilityUI Instance { get { return _AbilityUI;  } }

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
    }
    #endregion

    public AbilitySlot selectedAbilityDisplay;
	
	public void ChangeSelectedDisplay(Abilities ability)
    {
        if (!StoryManager.Instance.StoryInputEnabled)
        {
            selectedAbilityDisplay.PlaceAbilityInSlot(ability);
        }
    }
}
