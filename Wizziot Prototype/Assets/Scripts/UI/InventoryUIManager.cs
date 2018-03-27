using UnityEngine;

public class InventoryUIManager : MonoBehaviour {

    public Transform itemsParent;
    public TMPro.TextMeshProUGUI coinCounterText;

    public GameObject inventoryUI;

    Inventory inventory;
    InventorySlot[] slots;

	// Use this for initialization
	void Start () {
        inventory = Inventory.Instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        UpdateCoins();

        inventory.onItemChanged += UpdateUI;
        inventory.onCoinPickup += UpdateCoins;

        GameManager.Instance.onGameLoaded += UpdateUI;
        GameManager.Instance.onGameLoaded += UpdateCoins;

        PlayerManager.Instance.playerControls.OnEscapeKey += Close;
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
