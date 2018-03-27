using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameMetaInfo {

    // ----- Save & State Data ----- //
    public static string _SAVE_FILE_ENCRYPTED = Path.Combine(Application.streamingAssetsPath, "wizziot.dat");
    public static string _SAVE_FILE_JSON = Path.Combine(Application.streamingAssetsPath, "wizziot.json");

    public static List<string> _STATE_DATA = new List<string>()
    {
        "PlayerPosition",
        "PlayerHealth",
        "Equipped",
        "Inventory",
        "Coins",
        "MissionsActive"
    };

    // ----- Actions ----- //
    //Default key bindings ( <Key, User's key binding i.e. ability used> )
    public static Dictionary<KeyCode, Action> allKeybinds = new Dictionary<KeyCode, Action>();

    // ----- Gameplay ----- //
    public static Difficulty _GAME_DIFFICULTY = Difficulty.Normal;

    public static int _MAX_COINS = 100000000;   //to fit UI

    // ----- Game World ----- //
    //Layer Contracts
    public static string _LAYER_AFFECTABLE_OBJECT { get { return "Object"; } }
    public static string _LAYER_IMMOVABLE_OBJECT { get { return "Environment"; } }
    public static string _LAYER_GROUND_WALKABLE { get { return "Ground"; } }

    //Tag Contracts
    public static string _TAG_SHOOTABLE_BY_PLAYER { get { return "Enemy"; } }
    public static string _TAG_SHOOTABLE_BY_NPC { get { return "Player"; } }

    //Ability Contracts & Verification
    public static int _INSTANT_ABILITY_CODE { get { return Enum.GetValues(typeof(Abilities)).Cast<int>().First(); } }
    public static int _AREA_ABILITY_CODE { get { return Enum.GetValues(typeof(Abilities)).Cast<int>().First() + 50;  } }
    public static int _DEFENSE_ABILITY_CODE { get { return Enum.GetValues(typeof(Abilities)).Cast<int>().First() + 100; } }

    public static bool _Is_Instant_Ability(Abilities ability)
    {
        int abilityCode = (int)ability;
        if (abilityCode >= _INSTANT_ABILITY_CODE && abilityCode < _AREA_ABILITY_CODE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool _Is_AoE_Ability(Abilities ability)
    {
        int abilityCode = (int)ability;
        if (abilityCode >= _AREA_ABILITY_CODE && abilityCode < _DEFENSE_ABILITY_CODE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool _Is_Defense_Ability(Abilities ability)
    {
        int abilityCode = (int)ability;
        if (abilityCode >= _DEFENSE_ABILITY_CODE)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
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

public enum StateData
{
    PlayerPosition,
    PlayerHealth,
    Equipped,
    Inventory,
    Coins,
    MissionsActive
}