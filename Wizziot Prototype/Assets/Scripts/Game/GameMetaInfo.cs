using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMetaInfo {
    // ----- Actions ----- //
    //public delegate void KeybindAction();   //Using instead of action because Action takes no params, custom delegate allows lambda expression with either no params or params

    //Default key bindings ( <Key, User's key binding i.e. ability used> )
    public static Dictionary<KeyCode, Action> allKeybinds = new Dictionary<KeyCode, Action>();

    public static string _AFFECTABLE_OBJECT_LAYER_NAME { get { return "Object"; } }
    public static string _IMMOVABLE_OBJECT_LAYER_NAME { get { return "Environment"; } }

    public static Difficulty _GAME_DIFFICULTY = Difficulty.Normal;
}

public enum Difficulty
{
    Easy, Normal, Hard, Suicidal
}

//Separate abilities between a large gap to act as a "code" difference
public enum Abilities
{
    Zap = 50, Confuse,
    Vortex = 100, Singularity,
    Heal = 150
}

