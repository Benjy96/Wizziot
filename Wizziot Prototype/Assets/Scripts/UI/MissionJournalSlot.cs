using UnityEngine;
using TMPro;

public class MissionJournalSlot : MonoBehaviour {

    //Mission Texts
    public TextMeshProUGUI missionDescription;
    TextMeshProUGUI missionTitle;

    Mission missionInSlot;
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
                Item item = null;
                if (mission != missionInSlot)
                {
                    missionInSlot = mission;
                    item = Instantiate(mission.missionRewards[i]).GetComponent<Item>();
                }

                Sprite icon = item.icon;

                if (icon != null)
                {
                    rewards[i].enabled = true;
                    rewards[i].SetImage(icon);
                }

                if (item != null)
                {
                    Destroy(item.gameObject);
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
