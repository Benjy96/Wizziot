﻿using UnityEngine;

public class InventoryUIManager : MonoBehaviour {

    public Transform itemsParent;
    public TMPro.TextMeshProUGUI coinCounterText;

    public GameObject inventoryUI;
    public bool InventoryUIActive { get { return inventoryUI.activeSelf; } }

    Inventory inventory;
    InventorySlot[] slots;

    // Use this for initialization
    void Start () {
        inventory = Inventory.Instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        UpdateCoins();

        //Subscribe to inventory change events
        inventory.onItemChanged += UpdateUI;
        inventory.onCoinPickup += UpdateCoins;

        //Update UI on game file load
        GameManager.Instance.OnGameLoaded += UpdateUI;
        GameManager.Instance.OnGameLoaded += UpdateCoins; 

        PlayerManager.Instance.playerControls.OnEscapeKey += Close; //Close UI on escape press
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if(i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    void UpdateCoins()
    {
        coinCounterText.text = inventory.coins.ToString();
    }

    public void Close()
    {
        if(inventoryUI.activeSelf) inventoryUI.SetActive(false);
    }
}
