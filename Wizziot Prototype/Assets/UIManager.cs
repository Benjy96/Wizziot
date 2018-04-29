using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    private static UIManager _UIManager;
    public static UIManager Instance { get { return _UIManager; } }

    //Singleton
    private void Awake()
    {
        if(_UIManager == null)
        {
            _UIManager = this;
        }
        else if(_UIManager != this)
        {
            Destroy(gameObject);
        }
    }

    public GameObject background;
    private event Action uiAction;

    //UI elements that need confirmation subscribe their action through here, which enables a confirmation dialogue
    public void GetPlayerPermission(Action x)
    {
        Debug.Log("Here");
        background.SetActive(true);
        uiAction += x;
    }

    public void PerformUIAction()
    {
        background.SetActive(false);
        if (uiAction != null) uiAction.Invoke();
    }

    public void CancelUIAction()
    {
        background.SetActive(false);
        uiAction = null;
    }
}
