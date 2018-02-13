using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//TODO: AI Code <- Use enemy as parent / controller for behaviour (use a motor component and ability component - this class is the brain)
//TODO: Create movement component
//TODO: Create NPC aim component (e.g. track towards player with varying speed/accuracy depending on difficult)

/// <summary>
/// Inheritance is equivalent to interactableNPC. e.g.:  Targetable <- InteractableNPC <- (StoryNPCS) || Targetable <- Enemy <- (EnemyNPCs)
/// An enemy is going to be ANYTHING you can attack. As well as enemy NPCs, it may be animals, etc...
/// </summary>
public class Enemy : Targetable {

    protected static GameObject player;
    protected static Difficulty gameDifficulty;

    public EmotionChip emotionChip;
    public NavMeshAgent navAgent;
    public AbilityComponent abilityComponent;
    public Rigidbody rBody;
    public EntityStats stats;

    protected EnemySpawnPoint home;
    protected Transform destination;

    public Transform target;

    public EnemySpawnPoint Spawn
    {
        get { return home; }
        set { home = value; transform.SetParent(value.transform); }
    }
    public Vector3 Position { get { return transform.position; } }
    public Vector3 Velocity { get { return rBody.velocity; } set { rBody.velocity = value; } }

    protected void OnEnable()
    {
        //Need to spawn enemy at runtime for this to not throw a null ref exception
        //GameManager.Instance.OnDifficultyChanged += UpdateDifficulty;
    }

    private void OnDisable()
    {
        //GameManager.Instance.OnDifficultyChanged -= UpdateDifficulty;
    }

    protected void UpdateDifficulty()
    {
        //stats.maxTargetDistance = (difficulty) * var;
    }

    protected void Awake()  //Object initialised
    {
        emotionChip = GetComponent<EmotionChip>();
        navAgent = GetComponent<NavMeshAgent>();
        abilityComponent = GetComponent<AbilityComponent>();
        rBody = GetComponent<Rigidbody>();
        stats = GetComponent<EntityStats>();
    }

    protected void Start () //Scripts initialised
    {
        if (player == null) player = PlayerManager.Instance.player;
        if (gameDifficulty != GameMetaInfo._GAME_DIFFICULTY) gameDifficulty = GameMetaInfo._GAME_DIFFICULTY;

        emotionChip.ScaleEmotionWeights(gameDifficulty);
    }

    protected void FixedUpdate()
    {
        emotionChip.Execute(this);
    }
}
