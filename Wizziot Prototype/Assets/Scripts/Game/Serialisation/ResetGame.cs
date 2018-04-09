using UnityEngine;
using System.IO;

public class ResetGame : MonoBehaviour {

	public void DeleteSaveFile()
    {
        File.Delete(GameMetaInfo._SAVE_FILE_ENCRYPTED);
        File.Delete(GameMetaInfo._SAVE_FILE_ENCRYPTED + ".meta");
        File.Delete(GameMetaInfo._SAVE_FILE_JSON);
        File.Delete(GameMetaInfo._SAVE_FILE_JSON + ".meta");

        File.Delete(StoryManager.Instance.StoryFilepath);
        File.Delete(StoryManager.Instance.StoryFilepath + ".meta");
    }
}
