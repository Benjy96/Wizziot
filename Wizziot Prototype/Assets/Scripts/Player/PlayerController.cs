using System;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public event Action OnEscapeKey;    //Used to signify when the "Escape" button has been pressed - clears UI, pauses the game, etc...

    // ----- References ----- //
    private PauseMenu pauseMenu;

    // ----- Components ----- //
    public GameObject inventoryUI;

    private StoryManager storyManager;
    private AbilityComponent abilityComponent;
    private Projector targetIndicator;
    private Camera cam;
    private EntityStats playerStats;

    //Interaction
    private Targetable target;
    public Targetable Target { get { return target; } }

    public float Speed { get { return playerStats.speed; } }
    public float TurnSpeed { get { return playerStats.turnSpeed; } }

    //References & initialisation - Start is once scripts are definitely enabled - Awake the GOs are all enabled
    private void Awake()    
    {
        abilityComponent = GetComponent<AbilityComponent>();
        targetIndicator = GetComponentInChildren<Projector>();
        cam = Camera.main;
        playerStats = GetComponent<EntityStats>();
        pauseMenu = FindObjectOfType<PauseMenu>();

        target = null;

        //TODO: Put all bindings in GameControls.cs
        //Abilities
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha1, new Action(() => InstantAttack(Abilities.Zap, target)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha2, new Action(() => InstantAttack(Abilities.Confuse, target)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha3, new Action(() => abilityComponent.PlayerUseAoE(Abilities.Vortex)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha4, new Action(() => abilityComponent.PlayerUseAoE(Abilities.Singularity)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha5, new Action(() => InstantAttack(Abilities.Heal, target)));
        //General
        GameMetaInfo.allKeybinds.Add(KeyCode.F, Interact);
        GameMetaInfo.allKeybinds.Add(KeyCode.Escape, new Action(() => TriggerEscapeAction()));
        //Camera
        GameMetaInfo.allKeybinds.Add(KeyCode.LeftAlt, cam.GetComponent<PlayerCamera>().SwitchCameraMode);
        GameMetaInfo.allKeybinds.Add(KeyCode.I, new Action(() => inventoryUI.gameObject.SetActive(!inventoryUI.activeSelf)));
    }

    //EVENT SUBSCRIPTIONS & Script property references
    private void Start()
    {
        storyManager = StoryManager.Instance;
        //EVENT: onTargetDestroyed - Invoked by target of player
        PlayerManager.Instance.onTargetDestroyed += ResetTarget;    //Delegate: Reset target indicator when target is destroyed
        PlayerManager.Instance.onTargetDestroyed += MissionManager.Instance.RegisterKill;    //Delegate: Reset target indicator when target is destroyed
    }

    //UNSUBSCRIBE EVENTS
    private void OnDisable()
    {
        PlayerManager.Instance.onTargetDestroyed -= ResetTarget;
        PlayerManager.Instance.onTargetDestroyed -= MissionManager.Instance.RegisterKill;
    }

    //Call Handle methods
    private void Update () {
        HandleDirectionInput();
        HandleTargeting();
        HandleKeyboardInput();
        HandleConversationInput();
    }

    //Used as a keybind action for the "Escape" key event
    private void TriggerEscapeAction()
    {
        if (OnEscapeKey != null) OnEscapeKey.Invoke();
    }

    //Simple axis movement
    private void HandleDirectionInput()
    {
        float x = Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

    //Sends a raycast to detect "Targetable" type objects on the "Object" layer, ignoring trigger colliders
    private void HandleTargeting()
    {
        //Must be within 10 metres - using sqr values since getting root is expensive
        if(target != null && 
            (target.transform.position - transform.position).sqrMagnitude > playerStats.sqrMaxTargetDistance)
        {
            targetIndicator.enabled = false;
            target = null;
            StoryManager.Instance.CloseConversation();
        }

        //TARGET OPERATION (LMB)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit pointHit;

            //Raycast
            if(Physics.Raycast(ray, out pointHit, 100f, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore))    //If the raycast hits something within 100
            {
                //If within range
                if ((pointHit.transform.position - transform.position).sqrMagnitude < playerStats.sqrMaxTargetDistance)
                {
                    if (pointHit.transform.GetComponent<Targetable>() != null)  
                    {
                        Targetable t = pointHit.transform.GetComponent<Targetable>();
                        Debug.Log(t.name);
                        if (t == null) t = pointHit.transform.GetComponentInParent<Targetable>();
                        if (t == null) return;

                        TargetType currentTargetType = t.targetType;

                        Debug.Log(currentTargetType);

                        switch (currentTargetType)
                        {
                            case TargetType.Item:
                                target = pointHit.transform.GetComponent<Item>();
                                SetTargetIndicatorPos(true);
                                break;

                            case TargetType.Enemy:
                                target = pointHit.transform.GetComponent<Enemy>();
                                SetTargetIndicatorPos(true);
                                break;

                            case TargetType.Story:
                                target = pointHit.transform.GetComponent<InteractableNPC>();
                                SetTargetIndicatorPos(true);
                                break;

                            default:
                                SetTargetIndicatorPos(false);
                                break;
                        }
                    }
                }
            }
            else
            {
                SetTargetIndicatorPos(false);
            }
        }
    }

    //Use the keybind dictionary to perform actions
    private void HandleKeyboardInput()
    {
        for (int i = 0; i < Enum.GetValues(typeof(KeyCode)).Cast<int>().Last(); i++)
        {
            KeyCode code = (KeyCode)i;
            if (Input.GetKeyDown(code) && GameMetaInfo.allKeybinds.ContainsKey(code))
            {
                try
                {
                    //todo: check target ain't null? or do in the methods / prob for abilcomp methods
                    GameMetaInfo.allKeybinds[code]();
                }
                catch (Exception e)
                {
                    Debug.Log("Keybind not able to trigger");
                }
            }
        }
    }

    //Make story choices depending on keyboard input
    private void HandleConversationInput()
    {
        if (target != null)
        {
            if (StoryManager.Instance.StoryInputEnabled)
            {
                int playerChoice;
                bool correctInput = int.TryParse(Input.inputString, out playerChoice);
                if (correctInput)
                {
                    StoryManager.Instance.Choice(playerChoice);
                }
            }
        }
    }

    //Resets the target indicator
    private void ResetTarget()
    {
        SetTargetIndicatorPos(false);
    }

    //Places a projector on the current target
    private void SetTargetIndicatorPos(bool aboveTarget)
    {
        float height;

        if (aboveTarget)
        {
            height = target.transform.localScale.y * 5;

            targetIndicator.transform.SetParent(target.transform);
            targetIndicator.transform.position = target.transform.position + new Vector3(0f, height);
            targetIndicator.enabled = true;
        }
        else
        {
            target = null;
            targetIndicator.transform.SetParent(transform);
            targetIndicator.transform.position = transform.position;
            targetIndicator.enabled = false;
        }
    }

    //Attempts to converse with story NPCs or pick up objects
    private void Interact()
    {
        switch (target.targetType)
        {
            case TargetType.Item:
                target.GetComponent<Item>().PickUp(transform);
                break;

            case TargetType.Story:
                Debug.Log(target.name);
                storyManager.AttemptToConverse((InteractableNPC) target);
                break;

            default:
                Debug.Log(target.targetType + " cannot be interacted with");
                break;
        }
    }

    //Checks target can be attacked & handles "self abilities"
    private void InstantAttack(Abilities abil, Targetable target = null)
    {
        if (target == null && GameMetaInfo._Is_Defense_Ability(abil))
        {
            abilityComponent.PlayerUseInstant(abil, transform);
        }
        else if((target.GetComponent<Targetable>().targetType == TargetType.Enemy || target.tag.Equals(GameMetaInfo._TAG_SHOOTABLE_BY_PLAYER)) 
            && !GameMetaInfo._Is_Defense_Ability(abil))
        {
            abilityComponent.PlayerUseInstant(abil, target.transform);
        }
    }
}