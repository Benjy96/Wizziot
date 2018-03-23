using System;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // ----- Components ----- //
    public GameObject inventoryUI;

    private StoryManager storyManager;
    private AbilityComponent abilityComponent;
    private Projector targetIndicator;
    private Camera cam;
    private PlayerStats playerStats;

    //Interaction
    private Targetable target;

    public float Speed { get { return playerStats.speed; } }
    public float TurnSpeed { get { return playerStats.turnSpeed; } }

    private void Awake()    //References & initialisation - Start is once scripts are definitely enabled - Awake the GOs are all enabled
    {
        abilityComponent = GetComponent<AbilityComponent>();
        targetIndicator = GetComponentInChildren<Projector>();
        cam = Camera.main;
        playerStats = GetComponent<PlayerStats>();

        target = null;

        //TODO: Put all bindings in GameControls.cs
        //Abilities
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha1, new Action(() => abilityComponent.UseInstantAbility(Abilities.Zap, target.transform)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha2, new Action(() => abilityComponent.UseInstantAbility(Abilities.Confuse, target.transform)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha3, new Action(() => abilityComponent.UseAOEAbility(Abilities.Vortex)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha4, new Action(() => abilityComponent.UseAOEAbility(Abilities.Singularity)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha5, new Action(() => abilityComponent.UseInstantAbility(Abilities.Heal, target.transform)));
        //General
        GameMetaInfo.allKeybinds.Add(KeyCode.F, Interact);
        GameMetaInfo.allKeybinds.Add(KeyCode.Escape, new Action(() => ClearAndPauseGame())); //TODO: every ui subscribe to manager to close upon esc -> then menu
        //Camera
        GameMetaInfo.allKeybinds.Add(KeyCode.LeftAlt, cam.GetComponent<PlayerCamera>().SwitchCameraMode);
        GameMetaInfo.allKeybinds.Add(KeyCode.I, new Action(() => inventoryUI.gameObject.SetActive(!inventoryUI.activeSelf)));
    }

    private void Start()
    {
        storyManager = StoryManager.Instance;
    }

    private void Update () {
        HandleDirectionInput();
        HandleTargeting();
        HandleKeyboardInput();
        HandleConversationInput();
    }

    private void ClearAndPauseGame()
    {
        //Clear story input on first press
        if (StoryManager.Instance.StoryClosing == false && StoryManager.Instance.StoryInputEnabled)
        {
            StoryManager.Instance.CloseConversation();
            return;
        }
        if (StoryManager.Instance.StoryInputEnabled == false)
        {
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if(pauseMenu != null) pauseMenu.PauseGame();
        }
    }

    private void HandleDirectionInput()
    {
        float x = Input.GetAxis("Horizontal") * TurnSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * Speed * Time.deltaTime;
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

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
                    Debug.Log("here 1: " + pointHit);
                    if (pointHit.transform.GetComponent<Targetable>() != null)  
                    {
                        Targetable t = pointHit.transform.GetComponent<Targetable>();
                        if (t == null) t = pointHit.transform.GetComponentInParent<Targetable>();
                        if (t == null) return;
                        Debug.Log("name: " + t.name);

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
                    Debug.Log(e.Message);
                    Debug.Log(e.StackTrace);
                }
            }
        }
    }

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
                Debug.Log(target.targetType + " is not handled");
                break;
        }
    }
}