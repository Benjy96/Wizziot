using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionJournalSlot : MonoBehaviour {

    //Mission Texts
    public TextMeshProUGUI missionDescription;
    TextMeshProUGUI missionTitle;

    RewardSlot[] rewards;

    private void Awake()
    {
        rewards = GetComponentsInChildren<RewardSlot>(true);
    }

    public void AddMission(Mission mission)
    {
        missionTitle = GetComponent<TextMeshProUGUI>();
        if (mission != null)
        {
            missionTitle.text = mission.title;
            missionDescription.text = mission.description;

            for (int i = 0; i < mission.missionRewards.Length; i++)
            {
                if (mission.missionRewards[i] != null)
                {
                    rewards[i].enabled = true;
                    rewards[i].AddItem(mission.missionRewards[i]);
                }
            }
        }
    }

    public void RemoveMission()
    {
        rewards = null;
        missionDescription = null;
        missionTitle = null;
        enabled = false;
    }
}
