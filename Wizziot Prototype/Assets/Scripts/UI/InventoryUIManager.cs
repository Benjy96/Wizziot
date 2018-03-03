using UnityEngine;

public class InventoryUI : MonoBehaviour {

    public Transform itemsParent;

    Inventory inventory;
    InventorySlot[] slots;

	// Use this for initialization
	void Start () {
        inventory = Inventory.Instance;
        slots = itemsParent.GetComponentsInChildren<InventorySlot>();

        inventory.onItemChanged += UpdateUI;
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
}
