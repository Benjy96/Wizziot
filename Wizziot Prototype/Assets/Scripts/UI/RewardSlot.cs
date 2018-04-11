using UnityEngine;
using UnityEngine.UI;

public class RewardSlot : MonoBehaviour {

    Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    public void SetImage(Sprite icon)
    {
        Debug.Log("Set iamge");
        this.icon.enabled = true;
        this.icon.sprite = icon;
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        icon.enabled = false;
    }
}
