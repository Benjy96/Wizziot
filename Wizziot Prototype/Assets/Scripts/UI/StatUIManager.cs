using UnityEngine;
using UnityEngine.UI;

public class StatUIManager : MonoBehaviour {

    //Player
    public float health;
    public float stamina;
    public Image healthImage;
    public Image staminaImage;
    EntityStats playerStats;

    //Target
    public GameObject targetHealthUI;
    EntityStats targetStats;
    public float targetHealth;
    public float targetStamina;
    public Image targetHealthImage;
    public Image targetStaminaImage;
    bool setTargetBars;

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

        if (setTargetBars)
        {
            targetHealth = targetStats.CurrentHealth;
            targetStamina = targetStats.CurrentStamina;

            targetHealth /= targetStats.maxHealth;
            targetStamina /= targetStats.maxStamina;

            targetHealthImage.fillAmount = Mathf.Lerp(targetHealthImage.fillAmount, targetHealth, Time.deltaTime * 5f);
            targetStaminaImage.fillAmount = Mathf.Lerp(targetStaminaImage.fillAmount, targetStamina, Time.deltaTime * 5f);
        }
    }

    public void SetTarget(EntityStats eS)
    {
        targetHealthUI.SetActive(true);
        targetStats = eS;
        setTargetBars = true;
        eS.onDeath += ClearTarget;
    }

    private void ClearTarget()
    {
        targetStats = null;
        setTargetBars = false;
        targetHealthUI.SetActive(false);
    }
}
