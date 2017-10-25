using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public Button buttonPrefab;

    ObjectPool<Button> buttonPool = new ObjectPool<Button>();

    private void Awake()
    {
        if(buttonPool.prefab == null) buttonPool.prefab = buttonPrefab;
    }

    public Button PresentChoices(RectTransform parent, string buttonText)
    {
        if(parent.gameObject.activeInHierarchy == false) parent.gameObject.SetActive(true);
        Button newButton = buttonPool.GetObject();
        newButton.gameObject.SetActive(true);
        newButton.transform.SetParent(parent);
        newButton.GetComponentInChildren<Text>().text = buttonText;
        return newButton;
    }

    public void RemoveChoices(RectTransform parent, Button button)
    {
        if (parent.gameObject.activeInHierarchy == true) parent.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
        buttonPool.ReturnObject(button);
    }
}
