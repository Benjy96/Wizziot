using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour {

    Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void AddItem(Item item)
    {
        icon.enabled = true;
        //icon.sprite = item.icon;
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
    }
}
