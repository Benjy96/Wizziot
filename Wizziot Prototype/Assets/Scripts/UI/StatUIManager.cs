using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUIManager : MonoBehaviour {

    public float health;
    public float stamina;

    public Image healthImage;
    public Image staminaImage;

    EntityStats playerStats;

	// Use this for initialization
	void Start () {
		playerStats = PlayerManager.Instance.player.GetComponent<EntityStats>();
    }
	
	// Update is called once per frame
	void Update () {
        health = playerStats.CurrentHealth;
        stamina = playerStats.CurrentStamina;

        health /= playerStats.maxHealth;
        stamina /= playerStats.maxStamina;

        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, health, Time.deltaTime * 5f);
        staminaImage.fillAmount = Mathf.Lerp(staminaImage.fillAmount, stamina, Time.deltaTime * 5f);
    }
}
