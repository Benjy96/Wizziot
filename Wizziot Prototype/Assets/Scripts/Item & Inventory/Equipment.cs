﻿using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New equipment", menuName = "Items/Equipment")]
public class Equipment : ScriptableObject {

    public EquipmentSlot slot;
    public List<Stat> modifiers;
    [HideInInspector] public string prefabName;
}

public enum EquipmentSlot { Head, Body, Pendant, Weapon }