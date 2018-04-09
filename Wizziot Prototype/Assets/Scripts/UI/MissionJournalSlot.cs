using UnityEngine;
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

            for (int i = 0; i < mission.missionRewards.Capacity; i++)
            {
                if (mission.missionRewards[i] == null) continue;
                Item item = Instantiate(mission.missionRewards[i]).GetComponent<Item>();
                if (item != null)
                {
                    rewards[i].enabled = true;
                    rewards[i].AddItem(item);
                }
            }
        }
    }

    public void RemoveMission()
    {
        if (rewards != null)
        {
            foreach (RewardSlot slot in rewards)
            {
                if (slot == null) continue;
                slot.ClearSlot();
            }
        }

        if (missionTitle != null && missionDescription != null)
        {
            missionDescription.text = "";
            missionTitle.text = "";
            enabled = false;
        }
    }
}
