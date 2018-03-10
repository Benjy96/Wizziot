using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Inheritance is equivalent to interactableNPC. e.g.:  Targetable <- InteractableNPC <- (StoryNPCS) || Targetable <- Enemy <- (EnemyNPCs)
/// An enemy is going to be ANYTHING you can attack. As well as enemy NPCs, it may be animals, etc...
/// </summary>
public class Enemy : Targetable {

    protected static GameObject player;
    protected static Difficulty gameDifficulty;

    protected EnemySpawnPoint home;

    public Transform target;
    
    //Components
    public EmotionChip emotionChip;
    public NavMeshAgent navAgent;
    public AbilityComponent abilityComponent;
    public Rigidbody rBody;
    public EntityStats stats;
    public NeighbourhoodTracker neighbourhoodTracker;

    //Properties
    public EnemySpawnPoint Spawn
    {
        get { return home; }
        set { home = value; transform.SetParent(value.transform); }
    }
    public Vector3 Position { get { return transform.position; } }
    public Vector3 Velocity { get { return rBody.velocity; } set { rBody.velocity = value; } }
    public float SightRange { get { return neighbourhoodTracker.TrackingRadius; } }

    // -- Interface -- //
    public void Enrage()
    {
        emotionChip.enraged = true; 
    }

    public void Influence(GameObject influencer, Emotion intent, float amount)
    {
        emotionChip.Influence(influencer, intent, amount);
    }

    public void Influence(Emotion intent, float amount)
    {
        emotionChip.Influence(null, intent, amount);
    }

    protected void OnEnable()
    {
        //GameManager.Instance.OnDifficultyChanged += SetStats;
    }

    private void OnDisable()
    {
        //GameManager.Instance.OnDifficultyChanged -= SetStats;
        Spawn.RemoveEnemy(this);
    }

    protected void Awake()  //Object initialised
    {
        emotionChip = GetComponent<EmotionChip>();
        navAgent = GetComponent<NavMeshAgent>();
        abilityComponent = GetComponent<AbilityComponent>();
        rBody = GetComponent<Rigidbody>();
        stats = GetComponent<EntityStats>();
        neighbourhoodTracker = GetComponent<NeighbourhoodTracker>();
    }

    protected void Start () //Scripts initialised
    {
        if (player == null) player = PlayerManager.Instance.player;

        SetStats();
    }

    protected void FixedUpdate()
    {
        emotionChip.Execute(this);
    }

    // ----- COMPONENTs & SET-UP ----- //
    private void SetStats() //Use as a SetDifficulty() method as well - modify stats and components based upon gameDifficulty
    {
        gameDifficulty = GameMetaInfo._GAME_DIFFICULTY;

        neighbourhoodTracker.TrackingRadius = Mathf.Sqrt(stats.sqrMaxTargetDistance);

        //TODO: Change to access the entityStats dict
        abilityComponent.KnockbackForce = 0f;

        emotionChip.ScaleEmotionWeights(gameDifficulty);
    }

    // ----- NAVIGATION ----- //

    public void MoveTo(Vector3 target)
    {
        if (navAgent != null)
        {
            navAgent.destination = target;
        }
        else
        {
            Vector3 direction = target - Position;
            direction = direction.normalized;
            float ownerSpeed = stats.speed;

            Velocity = direction * ownerSpeed;
        }
        FaceTarget(target);
    }

    public void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - Position;
        direction = direction.normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10f);
    }

    public bool DestinationReached()
    {
        if(navAgent != null)
        {
            return navAgent.remainingDistance <= (navAgent.stoppingDistance);
        }
        else
        {
            return (target.position - Position).sqrMagnitude <= transform.localScale.sqrMagnitude;
        }
    }
}
