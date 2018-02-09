using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {

    //TODO: Use animations for the model's ambient movements, not a script (e.g. bobcomponent)
    //TODO: For flocking behaviour, calculate a vector location and then set that as the desired position in the nav mesh agent

    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
