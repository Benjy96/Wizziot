﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour {

    public float health;
    public float stamina;

    public Image healthImage;
    public Image staminaImage;

    PlayerStats playerStats;

	// Use this for initialization
	void Start () {
		playerStats = PlayerManager.Instance.player.GetComponent<PlayerStats>();
    }
	
	// Update is called once per frame
	void Update () {
        health = playerStats.CurrentHealth;
        stamina = playerStats.CurrentStamina;

        health /= playerStats.maxHealth;
        stamina /= playerStats.maxStamina;

        healthImage.fillAmount = health;
        staminaImage.fillAmount = stamina;
	}
}
