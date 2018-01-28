using System.Collections.Generic;
using UnityEngine;

public class GameControls {
    // ----- Actions ----- //
    public delegate void KeybindAction();

    //Default key bindings ( <Key, User's key binding i.e. ability used> )
    public static Dictionary<KeyCode, KeybindAction> allKeybinds = new Dictionary<KeyCode, KeybindAction>();
}

//General Keybindings
public enum General
{
    CameraMode = 0, Converse, Escape, Inventory, Map, Menu, Skills
}

//Separate abilities between a large gap to act as a "code" difference
public enum Abilities
{
    Zap = 50, Confuse,
    Vortex = 100, Singularity,
    Heal = 150
}

