using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private static PlayerManager _PlayerManager;
    public static PlayerManager Instance { get { return _PlayerManager; } }

    public GameObject player;

    private PlayerStats playerStats;

    private void Awake()
    {
        //Singleton setup
        if (_PlayerManager == null)
        {
            _PlayerManager = this;
        }
        else if (_PlayerManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        playerStats = player.GetComponent<PlayerStats>();
    }

    //TODO: Maybe make a separate EquipmentManager?
    /// <summary>
    /// Equip an item and apply its modifiers to the player's stats
    /// </summary>
    /// <param name="stats">The modifiers you wish to apply</param>
    public void EquipItem(List<Stat> stats)
    {
        //Get key
        foreach (Stat stat in stats)
        {
            //Set new stat value (apply modifier)
            playerStats.statModifiers[stat.StatType] = stat;
        }
    }
}
