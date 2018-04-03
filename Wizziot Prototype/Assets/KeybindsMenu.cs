using UnityEngine;
using TMPro;

public class KeybindsMenu : MonoBehaviour {

    TMP_InputField[] inputFields;

	// Use this for initialization
	void Start () {
        inputFields = GetComponentsInChildren<TMP_InputField>(true);
	}
	
	public void SetKeybinds()
    {
        //Get abil slot values - take only first char
        Debug.Log("Keybinds updated");
    }

    //Open file, modify difficulty, save
    private void RewriteKeybindsEncryptedSave()
    {
        SaveData data = Loader.GetEncryptedSaveFile();
        //Save (modify)
        //Write
    }

    //Open file, modify difficulty, save
    private void RewriteKeybindsJsonSave()
    {
        //Load
        //Save (modify)
        //Write
    }
}
