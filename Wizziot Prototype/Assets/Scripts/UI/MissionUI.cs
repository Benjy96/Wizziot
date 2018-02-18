using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour {

    public Text missionTitle;
    public Text missionBody;

    public void SetMissionText(Mission mission)
    {
        missionTitle.text += " " + mission.name;
        missionBody.text += " " + mission.location;
    }
}
