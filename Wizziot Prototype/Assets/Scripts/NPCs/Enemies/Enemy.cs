using System;
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

    public GameObject deathEffect;

    public Transform target;
    
    //Components
    [HideInInspector] public EmotionChip emotionChip;
    [HideInInspector] public NavMeshAgent navAgent;
    [HideInInspector] public AbilityComponent abilityComponent;
    [HideInInspector] public Rigidbody rBody;
    [HideInInspector] public EntityStats stats;
    [HideInInspector] public NeighbourhoodTracker neighbourhoodTracker;

    //Properties
    public EnemySpawnPoint Spawn
    {
        get { return home; }
        set { home = value; transform.SetParent(value.transform); }
    }
    public Vector3 Position { get { return transform.position; } }
    public Vector3 Velocity { get { return navAgent.velocity; } set { navAgent.velocity = value; } }
    public float SightRange { get { return neighbourhoodTracker.TrackingRadius; } }

    // -- Interface -- //
    public void Enrage()
    {
        emotionChip.enraged = true; 
    }

    /// <summary>
    /// Influence the enemy. The caller will be recorded as the influencer if the agent's emotion changes as a result of this method call.
    /// </summary>
    public void Influence(GameObject influencer, Emotion intent, float amount)
    {
        emotionChip.Influence(influencer, intent, amount);
    }

    /// <summary>
    /// Influences the enemy towards the specified emotion. 
    /// </summary>
    public void Influence(Emotion intent, float amount)
    {
        emotionChip.Influence(null, intent, amount);
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

    //References & Event subscriptions
    protected void Start () //Scripts initialised
    {
        if (player == null) player = PlayerManager.Instance.player;
        SetStats(); //Update stats at start

        //Events
        stats.onDeath += Die;
    }

    //Event unsubscriptions
    protected void OnDisable()
    {
        Spawn.RemoveEnemy(this);
        //Event Removals
        stats.onDeath -= Die;
    }

    protected void FixedUpdate()
    {
        emotionChip.Execute(this);
    }

    private void Die()
    {
        MissionManager.Instance.RegisterKill(this);

        abilityComponent.enabled = false;
        emotionChip.enabled = false;
        navAgent.velocity = new Vector3(0f, 0f, 0f);
        navAgent.destination = transform.position;
        GameObject deathFx = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathFx, 2f);
        Destroy(gameObject, .25f);
    }

    //Stats - Related to difficulty/load events
    public void SetStats()
    {
        //Apply stat modifiers with difficulty setting
        stats.ApplyStatModifiers();

        //Set Component Variables (may use stats)
        emotionChip.ScaleEmotionWeights();
        neighbourhoodTracker.TrackingRadius = Mathf.Sqrt(stats.sqrMaxTargetDistance);
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

    public void MoveToRandomWaypoint()
    {
        if(Spawn.spawnAreaWaypoints.Count <= 1)
        {
            MoveTo(RandomNavMeshPoint(SightRange));
        }

        //Get random point, based upon spawner waypoints
        Vector3 patrolPos = Spawn.spawnAreaWaypoints[UnityEngine.Random.Range(0, Spawn.spawnAreaWaypoints.Count)];

        //Prevent waypoint at current location being set as new target
        if (patrolPos == Position)
        {
            MoveToRandomWaypoint();
            return;
        }

        MoveTo(patrolPos);
    }

    public Vector3 RandomNavMeshPoint(float sightRange)
    {
        Vector3 random = UnityEngine.Random.insideUnitSphere * sightRange;
        random += Position;

        NavMeshHit hit;
        Vector3 finalPos = Vector3.zero;

        if (NavMesh.SamplePosition(random, out hit, sightRange, 1))
        {
            finalPos = hit.position;
        }
        return finalPos;
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - Position;
        direction = direction.normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * 10f);
        }
    }

    public bool DestinationReached()
    {
        if(navAgent != null)
        {
            return (navAgent.destination - transform.position).sqrMagnitude <= (navAgent.stoppingDistance * navAgent.stoppingDistance); //multiply better performance than sqrRoot
        }
        else
        {
            return (target.position - Position).sqrMagnitude <= transform.localScale.sqrMagnitude;
        }
    }

    public bool CanSeeTarget(Transform target)
    {
        Ray ray = new Ray(Position, (target.position - Position));
        RaycastHit hit;
        //Racyast everything except default & affectable objects

        //Player raycast - disregard layers
        if (target.tag.Equals("Player"))
        {
            if (Physics.Raycast(ray, out hit, SightRange, LayerMask.GetMask("Default", GameMetaInfo._LAYER_IMMOVABLE_OBJECT), QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag.Equals(target.tag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else //Target other objects - change layer mask
        {
            if (Physics.Raycast(ray, out hit, SightRange, LayerMask.GetMask(GameMetaInfo._LAYER_AFFECTABLE_OBJECT), QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.tag.Equals(target.tag))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }
}
