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
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha1, new Action(() => UseInstantAbility(Abilities.Zap)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha2, new Action(() => UseInstantAbility(Abilities.Confuse)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha3, new Action(() => UseAOEAbility(Abilities.Vortex)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha4, new Action(() => UseAOEAbility(Abilities.Singularity)));
        GameMetaInfo.allKeybinds.Add(KeyCode.Alpha5, new Action(() => UseInstantAbility(Abilities.Heal)));
        //Story Manager
        GameMetaInfo.allKeybinds.Add(KeyCode.F, Interact);
        GameMetaInfo.allKeybinds.Add(KeyCode.Escape, new Action(() => StoryManager.Instance.CloseConversation())); //TODO: event delegate with "close" methods subscribed
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
            if(Physics.Raycast(ray, out pointHit, 100f, LayerMask.GetMask("Object")))    //If the raycast hits something within 100
            {
                //If within range
                if ((pointHit.transform.position - transform.position).sqrMagnitude < playerStats.sqrMaxTargetDistance)
                {
                    if (pointHit.transform.GetComponent<Targetable>() != null)  
                    {
                        TargetType currentTargetType = pointHit.transform.GetComponent<Targetable>().targetType;
                        switch (currentTargetType)
                        {
                            case TargetType.Item:
                                target = pointHit.transform.GetComponent<Item>();
                                SetTargetIndicatorPos();
                                break;

                            case TargetType.Enemy:
                                //TODO: Add enemy targeting code
                                break;

                            case TargetType.Story:
                                target = pointHit.transform.GetComponent<InteractableNPC>();
                                SetTargetIndicatorPos();
                                break;

                            default:
                                targetIndicator.enabled = false;
                                break;
                        }
                    }
                }
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

    //TODO: Put these methods into abil component and make public (interface) for use in keybinds
    private void UseInstantAbility(Abilities ability)
    {
        if (abilityComponent.SelectedAbility == ability)
        {
            if (target != null && target.tag.Equals("Enemy"))
            {
                abilityComponent.UseAbility(target.transform);
            }
            else
            {
                Debug.Log("Invalid target");
            }
        }
        else
        {
            abilityComponent.SelectAbility(ability);
        }
    }

    private void UseAOEAbility(Abilities ability)
    {
        if (abilityComponent.SelectedAbility == ability)
        {
            abilityComponent.UseAbility(null);
        }
        else
        {
            abilityComponent.SelectAbility(ability);
        }
    }

    private void SetTargetIndicatorPos()
    {
        targetIndicator.transform.SetParent(target.transform);
        targetIndicator.transform.position = target.transform.position + new Vector3(0f, target.transform.localScale.y * 5);
        targetIndicator.enabled = true;
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