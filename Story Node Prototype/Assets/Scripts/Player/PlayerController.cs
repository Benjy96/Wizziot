using System;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // ----- Components ----- //
    private AbilityComponent abilityComponent;
    private Projector targetIndicator;
    private Camera cam;

    // ----- State Variables ----- //
    public PlayerStateData playerState; //Player state manages the player's current state, and will be saved (if the game supports saving).

    [Serializable]
    public class PlayerStateData
    {
        [Range(0, 10)] public float speed;
        [Range(50, 150)] public float turnSpeed;
        [Range(0, 225)] public float sqrMaxTargetDistance;
    }

    // ---- Book-keeping Fields ----- //    //Convenience properties and variables, plus variables that do not need saved.
    //Player MODE
    //TODO: Use state machine when needed
    //private enum PlayerState { Normal, Combat, Conversing, Dead }
    //PlayerState State;  

    //Interaction
    private Targetable target;

    public float Speed { get { return playerState.speed; } }
    public float TurnSpeed { get { return playerState.turnSpeed; } }

    private void Awake()    //References & initialisation - Start is once scripts are definitely enabled - Awake the GOs are all enabled
    {
        abilityComponent = GetComponent<AbilityComponent>();
        targetIndicator = GetComponentInChildren<Projector>();
        cam = Camera.main;

        target = null;

        //TODO: Put all bindings in GameControls.cs
        //Abilities
        GameControls.allKeybinds.Add(KeyCode.Alpha1, new Action(() => UseInstantAbility(Abilities.Zap)));
        GameControls.allKeybinds.Add(KeyCode.Alpha2, new Action(() => UseInstantAbility(Abilities.Confuse)));
        GameControls.allKeybinds.Add(KeyCode.Alpha3, new Action(() => UseAOEAbility(Abilities.Vortex)));
        GameControls.allKeybinds.Add(KeyCode.Alpha4, new Action(() => UseAOEAbility(Abilities.Singularity)));
        GameControls.allKeybinds.Add(KeyCode.Alpha5, new Action(() => UseInstantAbility(Abilities.Heal)));
        //Story Manager
        GameControls.allKeybinds.Add(KeyCode.F, new Action(() => StoryManager.Instance.AttemptToConverse(target.GetComponent<InteractableNPC>())));
        GameControls.allKeybinds.Add(KeyCode.Escape, new Action(() => StoryManager.Instance.CloseConversation())); //TODO: event delegate with "close" methods subscribed
        //Camera
        GameControls.allKeybinds.Add(KeyCode.LeftAlt, cam.GetComponent<PlayerCamera>().SwitchCameraMode);
    }

    void Update () {
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
            (target.transform.position - transform.position).sqrMagnitude > playerState.sqrMaxTargetDistance)
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
                if ((pointHit.transform.position - transform.position).sqrMagnitude < playerState.sqrMaxTargetDistance)
                {
                    if (pointHit.transform.GetComponent<Targetable>() != null)  
                    {
                        TargetType currentTargetType = pointHit.transform.GetComponent<Targetable>().targetType;
                        switch (currentTargetType)
                        {
                            case TargetType.Item:
                                //TODO: Add item targeting code
                                break;

                            case TargetType.Enemy:
                                //TODO: Add enemy targeting code
                                break;

                            case TargetType.Story:
                                if (currentTargetType == TargetType.Story)
                                {
                                    targetIndicator.enabled = false;
                                }
                                target = pointHit.transform.GetComponent<InteractableNPC>();
                                targetIndicator.transform.SetParent(target.transform);
                                targetIndicator.transform.position = target.transform.position + new Vector3(0f, target.transform.localScale.y * 5);
                                targetIndicator.enabled = true;
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
            if (Input.GetKeyDown(code) && GameControls.allKeybinds.ContainsKey(code))
            {
                try
                {
                    GameControls.allKeybinds[code]();
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
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
}