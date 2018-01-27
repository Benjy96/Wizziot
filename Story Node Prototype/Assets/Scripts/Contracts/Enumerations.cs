using System;
using System.Collections.Generic;

public class Enumerations {

    class CONTROL_CODE
    {
        public int LowerBound;
        public int UpperBound;
    }

    private readonly CONTROL_CODE GENERAL = new CONTROL_CODE { LowerBound = 0, UpperBound = 50 };
    private readonly CONTROL_CODE ABILITY = new CONTROL_CODE { LowerBound = 50, UpperBound = 200 };

    public bool IsGeneral(int code)
    {
        if(code > GENERAL.LowerBound && code < ABILITY.LowerBound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsAbility(int code)
    {
        if(code < ABILITY.UpperBound && code > ABILITY.LowerBound)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

//General Keybindings
public enum General
{
    CameraMode = 0, Converse, Escape, Inventory, Map, Menu, Skills
}

//Separate abilities between a large gap to act as a "code" difference
public enum Abilities
{
    //Instant
    Zap = 100, Confuse,
    //AOE
    Vortex = 150, Singularity,
    //Defense
    Heal = 200
}

