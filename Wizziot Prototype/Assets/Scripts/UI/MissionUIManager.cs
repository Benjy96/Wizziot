using UnityEngine;

public class MissionUIManager : MonoBehaviour {

    MissionManager missionManager;

    public Transform missionLog;
    public MissionUI[] missions;

    private void Start()
    {
        missionManager = MissionManager.Instance;
        missions = GetComponentsInChildren<MissionUI>(true);    //TODO: activate each slot

        missionManager.onActiveMissionsChanged += UpdateUI;

        GameManager.Instance.onGameLoaded += UpdateUI;
    }

    public void SetMissionText(Mission mission)
    {
        Debug.Log("Remove this method");
    }

    /// <summary>
    /// Go through every mission and add to the UI (re-arranging entire UI from scratch every time the event is called)
    /// </summary>
    void UpdateUI()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            //if num of missions not out of sync with mission UIs active
            if(i < missionManager.activeMissions.Count)
            {
                missions[i].gameObject.SetActive(true);
                missions[i].ActivateMission(missionManager.activeMissions[i]);
            }
            else
            {
                missions[i].gameObject.SetActive(false);
            }
        }
    }
}
