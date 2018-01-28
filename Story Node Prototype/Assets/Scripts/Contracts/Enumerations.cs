using System;
using System.Linq;

public class Enumerations {

    class CONTROL_CODE<E>
    {
        public int LowerBound { get; private set; }
        public int UpperBound { get; private set; }

        public CONTROL_CODE()
        {
            LowerBound = GetLowerBound();
            UpperBound = GetUpperBound();
        }

        public int GetLowerBound()
        {
            return Enum.GetValues(typeof(E)).Cast<int>().First();
        }

        public int GetUpperBound()
        {
            return (Enum.GetValues(typeof(E)).Cast<int>().Last() + 50); //+ 50 because last sub-type needs a range too
        }
    }

    private readonly CONTROL_CODE<General> GENERAL = new CONTROL_CODE<General>();
    private readonly CONTROL_CODE<Abilities> ABILITY = new CONTROL_CODE<Abilities>();

    //Can reduce redundancy here - e.g. iterate through all control_codes and return their <Type> if within range
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
    Zap = 50, Confuse,
    //AOE
    Vortex = 100, Singularity,
    //Defense
    Heal = 150
}

