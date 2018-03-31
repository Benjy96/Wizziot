using UnityEngine;

public class MissionUIManager : MonoBehaviour {

    MissionManager missionManager;

    public GameObject missionJournal;   //Detailed info - mission quest pane
    private MissionJournalSlot[] journalSlots;

    public Transform missionLog;    //Onscreen UI (log/waypoints/title/objective)
    private MissionUI[] missions;

    private void Start()
    {
        missionManager = MissionManager.Instance;
        missions = GetComponentsInChildren<MissionUI>(true);    

        journalSlots = missionJournal.GetComponentsInChildren<MissionJournalSlot>(true);

        missionManager.onActiveMissionsChanged += UpdateUI;
        missionManager.onJournalOpened += LoadJournal;

        GameManager.Instance.onGameLoaded += UpdateUI;
        PlayerManager.Instance.playerControls.OnEscapeKey += Close;
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
                //Mission Log Updates
                missions[i].gameObject.SetActive(true);
                missions[i].SetMissionText(missionManager.activeMissions[i]);
            }
            else
            {
                //Log
                missions[i].gameObject.SetActive(false);
            }
        }
    }

    void LoadJournal()
    {
        //Open or close journal
        if(missionJournal.activeSelf)
        {
            missionJournal.SetActive(false);
            return;
        }
        else
        {
            missionJournal.SetActive(true);
        }

        for (int i = 0; i < missions.Length; i++)
        {
            //if num of missions not out of sync with mission UIs active
            if (i < missionManager.activeMissions.Count)
            {
                //Mission Journal Updates
                journalSlots[i].gameObject.SetActive(true);
                journalSlots[i].AddMission(missionManager.activeMissions[i]);
            }
            else
            {
                //Journal
                journalSlots[i].RemoveMission();
                journalSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void Close()
    {
        if (missionJournal.activeSelf) missionJournal.SetActive(false);
    }
}
