using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryDisplayer : MonoBehaviour {

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
