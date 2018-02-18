using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New equipment", menuName = "Items/Equipment")]
public class Equipment : ScriptableObject {

    public EquipmentSlot slot;

    public List<Stat> modifiers;

    //Modifiers:
    //How to check we have all?
    //Need to relate to EntityStats
        //Enum of all stats? 
            //Reflection to make variables? e.g. foreach(var in Enum Stats) dict[x] = var;
	
}

public enum EquipmentSlot { Head, Body, Pendant, Weapon }