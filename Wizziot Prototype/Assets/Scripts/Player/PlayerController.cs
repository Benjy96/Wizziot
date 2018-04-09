using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public event Action OnEscapeKey;    //Used to signify when the "Escape" button has been pressed - clears UI, pauses the game, etc...

    // ----- References ----- //
    private PauseMenu pauseMenu;

    // ----- Components ----- //
    public GameObject inventoryUI;
    public StatUIManager statUI;
    public CharacterSheetManager characterSheet;

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

        //Refresh keybinds
        GameMetaInfo.keybindActions = new System.Collections.Generic.Dictionary<KeyCode, Action>();
        GameMetaInfo.abilityKeybinds = new System.Collections.Generic.Dictionary<Abilities, KeyCode>();
        //Abilities
        GameMetaInfo.SetAbilityKeybindAction(Abilities.Zap, KeyCode.Alpha1, new Action(() => UseAbility(Abilities.Zap, target)));
        GameMetaInfo.SetAbilityKeybindAction(Abilities.Confuse, KeyCode.Alpha2, new Action(() => UseAbility(Abilities.Confuse, target)));
        GameMetaInfo.SetAbilityKeybindAction(Abilities.Vortex, KeyCode.Alpha3, new Action(() => UseAbility(Abilities.Vortex, target)));
        GameMetaInfo.SetAbilityKeybindAction(Abilities.Singularity, KeyCode.Alpha4, new Action(() => UseAbility(Abilities.Singularity, target)));
        GameMetaInfo.SetAbilityKeybindAction(Abilities.Heal, KeyCode.Alpha5, new Action(() => UseAbility(Abilities.Heal, target)));

        //General
        GameMetaInfo.keybindActions.Add(KeyCode.F, Interact);
        GameMetaInfo.keybindActions.Add(KeyCode.Escape, TriggerEscapeAction);

        //Camera
        GameMetaInfo.keybindActions.Add(KeyCode.LeftAlt, cam.GetComponent<PlayerCamera>().SwitchCameraMode);
        GameMetaInfo.keybindActions.Add(KeyCode.I, new Action(() => inventoryUI.gameObject.SetActive(!inventoryUI.activeSelf)));
        GameMetaInfo.keybindActions.Add(KeyCode.J, TriggerJournalLoad);
        GameMetaInfo.keybindActions.Add(KeyCode.C, characterSheet.OpenOrClose);
    }

    //EVENT SUBSCRIPTIONS & Script property references
    private void Start()
    {
        storyManager = StoryManager.Instance;
        //EVENT: onTargetDestroyed - Invoked by target of player
        PlayerManager.Instance.onTargetDestroyed += RegisterKill;    //Delegate: Register kill to mission manager & Reset target indicator when target is destroyed 
    }

    //UNSUBSCRIBE EVENTS
    private void OnDisable()
    {
        PlayerManager.Instance.onTargetDestroyed -= RegisterKill;
    }

    //Call Handle methods
    private void Update () {
        HandleDirectionInput();
        HandleTargeting();
        HandleKeyboardInput();
        HandleConversationInput();
    }
    
    #region Event Triggers - Escape & Mission Journal
    //Used as a keybind action for the "Escape" key event
    public void TriggerEscapeAction()
    {
        if (OnEscapeKey != null) OnEscapeKey.Invoke();
    }

    //Open & Update the Mission Journal - event
    public void TriggerJournalLoad()
    {
        if (MissionManager.Instance.onJournalOpened != null) MissionManager.Instance.onJournalOpened.Invoke();
    }
    #endregion

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
        if (target == null) statUI.ClearTarget();
        //Must be within x metres - using sqr values since getting root is expensive
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
                        if (t == null) t = pointHit.transform.GetComponentInParent<Targetable>();
                        if (t == null) return;

                        TargetType currentTargetType = t.targetType;

                        switch (currentTargetType)
                        {
                            case TargetType.Item:
                                target = pointHit.transform.GetComponent<Item>();
                                SetTargetIndicatorPos(true);
                                break;

                            case TargetType.Enemy:
                                target = pointHit.transform.GetComponent<Enemy>();
                                SetTargetIndicatorPos(true);
                                statUI.SetTarget(target.GetComponent<EntityStats>());
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
            //Check if we have clicked UI
            else if(EventSystem.current.IsPointerOverGameObject())
            {
                //Debug.Log("Hit ui");
                return; //Don't lose target if mouse clicking UI
            }
            //If nothing clicked, reset the target
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
            if (Input.GetKeyDown(code) && GameMetaInfo.keybindActions.ContainsKey(code))
            {
                try
                {
                    //todo: check target ain't null? or do in the methods / prob for abilcomp methods
                    GameMetaInfo.keybindActions[code]();
                }
                catch (Exception e)
                {
                Debug.Log(e.StackTrace);
                Debug.Log("Either Keybind not able to trigger or an exception occurred");
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

    //Registers a kill and then resets the target
    private void RegisterKill()
    {
        MissionManager.Instance.RegisterKill();
        SetTargetIndicatorPos(false);
    }

    //Places a projector on the current target
    private void SetTargetIndicatorPos(bool dontResetTarget)
    {
        float height;

        if (dontResetTarget)
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
    public void Interact()
    {
        switch (target.targetType)
        {
            case TargetType.Item:
                target.GetComponent<Item>().AddToInventory();
                SetTargetIndicatorPos(false);
                break;

            case TargetType.Story:
                storyManager.AttemptToConverse((InteractableNPC) target);
                break;

            default:
                break;
        }
    }

    //Checks target can be attacked & handles "self abilities"
    public void UseAbility(Abilities abil, Targetable target)
    {
        AbilityUI.Instance.ChangeSelectedDisplay(abil);

        if (GameMetaInfo._Is_AoE_Ability(abil))
        {
            abilityComponent.PlayerUseAoE(abil);
        }
        else if (GameMetaInfo._Is_Defense_Ability(abil))
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