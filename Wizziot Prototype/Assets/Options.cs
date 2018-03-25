using UnityEngine;

public class Options : MonoBehaviour {

    public TMPro.TMP_Dropdown dropdown;

	public void SetDifficulty()
    {
        Debug.Log("Game Difficulty Set: " + (Difficulty)dropdown.value);
        GameMetaInfo._GAME_DIFFICULTY = (Difficulty)dropdown.value;
    }
}
