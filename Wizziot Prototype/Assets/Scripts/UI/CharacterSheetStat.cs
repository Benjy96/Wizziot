using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSheetStat : MonoBehaviour {

    private Stat stat;

    public TextMeshProUGUI statTitle;
    public TextMeshProUGUI statValue;
    public TextMeshProUGUI statDescription;

    public void SetStatUI(Stat stat)
    {
        this.stat = stat;

        statTitle.text = stat.StatType.ToString();
        statValue.text = stat.StatValue.ToString();
        statDescription.text = GameMetaInfo._STAT_DESCRIPTIONS[stat.StatType];
    }
}
