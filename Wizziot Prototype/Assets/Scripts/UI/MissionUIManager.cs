using UnityEngine;
using UnityEngine.UI;

public class MissionUIManager : MonoBehaviour {

    MissionManager missionManager;

    public Transform missionLog;
    public MissionUI[] missions;

    //public Text missionTitle;
    //public Text missionBody;
    //public WaypointCompass compass; //TODO: Convert player pos - waypoint to 2D vector and rotate an arrow to direction

    private void Start()
    {
        missionManager = MissionManager.Instance;
        missions = GetComponentsInChildren<MissionUI>();

        missionManager.onActiveMissionsChanged += UpdateUI;
    }

    public void SetMissionText(Mission mission)
    {
        Debug.Log("Remove this method");
    }

    void UpdateUI()
    {
        for (int i = 0; i < missions.Length; i++)
        {
            //if (i < inventory.items.Count)
            //{
            //    //missions[i].AddItem(inventory.items[i]);
            //}
            //else
            //{
            //    //missions[i].ClearSlot();
            //}
        }
    }
}
