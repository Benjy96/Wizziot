using System;
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

        GameControls.allKeybinds.Add(KeyCode.Alpha1, new GameControls.KeybindAction(() => UseInstantAbility(Abilities.Zap)));
        GameControls.allKeybinds.Add(KeyCode.Alpha2, new GameControls.KeybindAction(() => UseInstantAbility(Abilities.Confuse)));
        GameControls.allKeybinds.Add(KeyCode.Alpha3, new GameControls.KeybindAction(() => UseInstantAbility(Abilities.Vortex)));
        GameControls.allKeybinds.Add(KeyCode.Alpha4, new GameControls.KeybindAction(() => UseInstantAbility(Abilities.Singularity)));
        GameControls.allKeybinds.Add(KeyCode.Alpha5, new GameControls.KeybindAction(() => UseInstantAbility(Abilities.Heal)));
        GameControls.allKeybinds.Add(KeyCode.F, new GameControls.KeybindAction(() => StoryManager.Instance.AttemptToConverse(target.GetComponent<InteractableNPC>())));
        GameControls.allKeybinds.Add(KeyCode.Escape, new GameControls.KeybindAction(() => StoryManager.Instance.CloseConversation()));
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
        for (KeyCode i = KeyCode.A; i < KeyCode.Z; i++)
        {
            if (Input.GetKeyDown(i) && GameControls.allKeybinds.ContainsKey(i))
            {
                Invoke(GameControls.allKeybinds[i].ToString(), 0f);
            }//GetKeyDown(i)
        }
    }

    private void HandleConversationInput()
    {
        if (target != null)
        {
            if (StoryManager.Instance.takeStoryInput)
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
            abilityComponent.UseAbility(target.transform);
        }
        else
        {
            abilityComponent.SelectAbility(ability);
        }
    }
}
