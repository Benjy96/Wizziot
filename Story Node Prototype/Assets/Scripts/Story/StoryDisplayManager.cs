using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Outputs the story in the game world
/// </summary>
public class StoryDisplayManager : MonoBehaviour {

    #region Singleton  
    private static StoryDisplayManager _StoryDisplay = null;
    
    private void Awake()
    {
        if (_StoryDisplay == null)
        {
            _StoryDisplay = this;
        }
        else if (_StoryDisplay != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // ----- SINGLETON ----- //
    public static StoryDisplayManager Instance { get { return _StoryDisplay; } }

    // ----- Worldspace UI ----- //
    [Header("Set in Inspector")]
    public GameObject storyDisplayCanvas;
    public Text storyText;

    public void EnableStoryDisplay()
    {
        storyDisplayCanvas.SetActive(true);
    }

    public void DisableStoryDisplay()
    {
        storyDisplayCanvas.SetActive(false);
    }

    public string DisplayedStoryText
    {
        set { storyText.text = value; }
    }

    public Transform SetWorldPosition
    {
        set
        {
            float npcHeight = value.localScale.y;
            Vector3 newTextPosition = new Vector3(value.position.x, value.position.y + npcHeight, value.position.z);

            storyDisplayCanvas.transform.position = newTextPosition;
        }
    }
}
