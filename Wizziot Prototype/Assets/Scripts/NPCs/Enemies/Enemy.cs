using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//TODO: AI Code <- Use enemy as parent / controller for behaviour (use a motor component and ability component - this class is the brain)
//TODO: Create movement component
//TODO: Create NPC aim component (e.g. track towards player with varying speed/accuracy depending on difficult)

/// <summary>
/// Inheritance is equivalent to interactableNPC. e.g.:  Targetable <- InteractableNPC <- (StoryNPCS) || Targetable <- Enemy <- (EnemyNPCs)
/// </summary>
public class Enemy : Targetable {

    protected static GameObject player;

    protected Difficulty gameDifficulty = GameMetaInfo._GAME_DIFFICULTY;

    protected NavMeshAgent navAgent;
    protected AbilityComponent abilityComponent;

    protected State state;
    protected Transform destination;
    protected Transform target;

    protected void OnEnable()
    {
        GameManager.Instance.OnDifficultyChanged += UpdateDifficulty;
    }

    protected void UpdateDifficulty()
    {
        //stats.maxTargetDistance = (difficulty) * var;
    }

    protected void Awake()  //Object initialised
    {
        navAgent = GetComponent<NavMeshAgent>();
        abilityComponent = GetComponent<AbilityComponent>();
    }

    protected void Start () //Scripts initialised
    {
        if (player == null) player = PlayerManager.Instance.player;
	}

    protected void Update()
    {
        state.Execute();
    }

    protected void ChangeState(State newState)
    {
        state = newState;
    }
}
