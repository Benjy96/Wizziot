using UnityEngine;

[CreateAssetMenu(fileName = "New item data", menuName = "Items/ItemAttributes")]
public class ItemAttributes : ScriptableObject {

    new public string name = "New item";
    public Sprite icon = null;
}
