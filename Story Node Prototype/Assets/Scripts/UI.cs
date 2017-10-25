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

    public Button AssignButton(RectTransform parent, string buttonText)
    {
        Button newButton = buttonPool.GetObject();
        newButton.gameObject.SetActive(true);
        newButton.transform.SetParent(parent);
        newButton.GetComponentInChildren<Text>().text = buttonText;
        return newButton;
    }

    public void RemoveButton(Button button)
    {
        button.gameObject.SetActive(false);
        buttonPool.ReturnObject(button);
    }
}
