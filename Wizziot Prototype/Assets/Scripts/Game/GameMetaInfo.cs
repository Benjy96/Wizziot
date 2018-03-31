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
        "GameDifficulty",
        "PlayerPosition",
        "PlayerHealth",
        "Equipped",
        "Inventory",
        "Coins",
        "MissionsActive"
    };

    // ----- Actions ----- //
    public static void SetAbilityKeybindAction(Abilities abil, KeyCode key, Action action)
    {
        keybindActions[key] = action;
        abilityKeybinds[abil] = key;
    }

    //Default key bindings ( <Key, User's key binding, i.e.: ability used> )
    public static Dictionary<KeyCode, Action> keybindActions = new Dictionary<KeyCode, Action>();

    //Ability key bindings - Displayed in UI
    public static Dictionary<Abilities, KeyCode> abilityKeybinds = new Dictionary<Abilities, KeyCode>();

    // ----- Gameplay ----- //
    public static Difficulty _GAME_DIFFICULTY = Difficulty.Normal;

    public static int _MAX_COINS = 100000000;   //to fit UI

    // ----- Game World ----- //
    //Layer Contracts
    public static string _LAYER_AFFECTABLE_OBJECT { get { return "Object"; } }
    public static string _LAYER_IMMOVABLE_OBJECT { get { return "Environment"; } }
    public static string _LAYER_GROUND_WALKABLE { get { return "Ground"; } }
    public static string _LAYER_UI { get { return "UI"; } }

    //Tag Contracts
    public static string _TAG_SHOOTABLE_BY_PLAYER { get { return "Enemy"; } }
    public static string _TAG_SHOOTABLE_BY_NPC { get { return "Player"; } }

    // ----- Abilities ----- //
    //Codes
    public static int _INSTANT_ABILITY_CODE { get { return Enum.GetValues(typeof(Abilities)).Cast<int>().First(); } }
    public static int _AREA_ABILITY_CODE { get { return Enum.GetValues(typeof(Abilities)).Cast<int>().First() + 50;  } }
    public static int _DEFENSE_ABILITY_CODE { get { return Enum.GetValues(typeof(Abilities)).Cast<int>().First() + 100; } }

    //Ability Type Checkers
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

    //Ability Icons (Sprites)
    public static Dictionary<Abilities, Sprite> abilityIcons = new Dictionary<Abilities, Sprite>()
    {
        { Abilities.Zap, (Sprite)Resources.Load("zap", typeof(Sprite)) },
        { Abilities.Confuse, (Sprite)Resources.Load("confuse", typeof(Sprite)) },
        { Abilities.Vortex, (Sprite)Resources.Load("vortex", typeof(Sprite)) },
        { Abilities.Singularity, (Sprite)Resources.Load("singularity", typeof(Sprite)) },
        { Abilities.Heal, (Sprite)Resources.Load("heal", typeof(Sprite)) },
    };
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
    GameDifficulty,
    PlayerPosition,
    PlayerHealth,
    Equipped,
    Inventory,
    Coins,
    MissionsActive
}