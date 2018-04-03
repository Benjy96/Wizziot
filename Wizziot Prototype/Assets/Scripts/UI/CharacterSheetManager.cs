using System.Collections.Generic;
using UnityEngine;

public class CharacterSheetManager : MonoBehaviour {

    public GameObject characterSheet;
    CharacterSheetStat[] sheetStats;

	// Use this for initialization
	void Start () {
        sheetStats = characterSheet.GetComponentsInChildren<CharacterSheetStat>(true);
        if (sheetStats.Length != System.Enum.GetValues(typeof(Stats)).Length) throw new System.Exception("Not all stats displayed in Char Sheet UI");

        PlayerManager.Instance.playerControls.OnEscapeKey += Close;
	}
	
	void UpdateStatUI ()
    {
        int i = 0;
        foreach(KeyValuePair<Stats, Stat> x in PlayerManager.Instance.PlayerStatModifiers)
        {
            sheetStats[i].SetStatUI(x.Value);
            i++;
        }
	}

    public void OpenOrClose()
    {
        if (characterSheet.activeSelf)
        {
            characterSheet.SetActive(false);
        }
        else
        {
            characterSheet.SetActive(true);
            UpdateStatUI();
        }
    }

    public void Close()
    {
        characterSheet.SetActive(false);
    }
}
