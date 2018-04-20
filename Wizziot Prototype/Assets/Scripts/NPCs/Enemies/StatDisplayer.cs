using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDisplayer : MonoBehaviour {

    Renderer rend;
    AgentStats stats;

	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        stats = GetComponent<AgentStats>();
	}
	
	// Update is called once per frame
	void Update () {
        //Set enemy color from Red -> Green based upon health
        float r = (stats.maxHealth - stats.CurrentHealth) / (float)stats.maxHealth;    //red is 0 when health is full
        float g = stats.CurrentHealth / (float)stats.maxHealth;    //green is 1 when health is full 
        Color newColor = new Color(r, g, 0f, 1f);
        rend.material.color = Color.Lerp(rend.material.color, newColor, Time.deltaTime);
	}
}
