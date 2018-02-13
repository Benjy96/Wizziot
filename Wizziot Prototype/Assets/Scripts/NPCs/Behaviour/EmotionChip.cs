using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A variant of a finite state machine... The EMOTIONAL State Machine (ESM)
/// Each agent with an emotion chip has an emotional disposition. 
/// External factors can affect this agent's emotions; in addition, the way in which each emotion is influenced differs between emotional dispositions. 
/// The extent to which each emotion is influenced is then affected by weight values.
/// 
/// What is the difference between this and a Finite State Machine?
/// In an FSM, you enter into a State based upon a limited/defined set/number of conditions. With the Emotional State Machine, any number of external factors can influence the state you enter!
/// Instead of switching state based upon an internal check, any number of external factors (may not even be in game yet!) can influence the state of the agent!
/// Basically, the ESM responds to (unknown) external influences, versus a limited number of hard-coded true/false checks pre-runtime.
/// 
/// An example:
/// FSM: if(playerNear()) changeState(new RunState());
///     With the above architecture, to match the ESM, you would need hundreds of "if" checks, which you would need to create methods for, as well as design in advance
/// 
/// ESM: agent.Influence(Emotion.Anger, .4f);   //This shows how any external factor can affect state
///     With the ESM architecture, you don't need to write every possible check beforehand, but can add influences to the game as you go along throughout development
///     e.g. Add a boulder falling, and in its OnTriggerEnter method, access all nearby enemies and call Influence(Emotion.Fear, 1f);
///     If it were an FSM, you would add the boulder, then need to remember to add a check in code like: if(boulderFell() && closeTo(boulder)) changeState(new RunState());
///     
/// The essential difference between the two is knowledge - in an FSM, you need to know all the influencing factors. The ESM requires no such foreknowledge.
/// Instead of "If(any unlimited number of factors)", the ESM says: "If(emotion is high enough)". ESM does NOT need to track external factors! Has no knowledge outside its emotion variables!
///     
/// TL;DR - This class contains the logic/attributes that determine what behaviour an AI undertakes. If it's angry, then the "angry state" will execute.
/// </summary>
public class EmotionChip : MonoBehaviour {

    //Disposition changes how influencing factors affect the agent
    public Emotion disposition = Emotion.Calm;

    //How likely an emotional agent is to change their behaviour
    [Range(0.1f, 0.9f)] public float reluctance = 0.5f;

    //The agent's emotional state(s)
    public Dictionary<Emotion, float> agentEmotions = new Dictionary<Emotion, float>();

    public float trust = 1f; //Calm weighting
    public float irascibility = 1f; //Anger weighting
    public float cowardice = 1f;    //Fear weighting

    private bool inState;

    public State currentState;
    public State calmGoal;
    //public State angryGoal;
    //public State scaredGoal;

    /// <summary>
    /// As difficulty increases, enemy NPCs gain confidence.
    /// </summary>
    /// <param name="difficulty">The difficulty you wish to set this agent to</param>
    public void ScaleEmotionWeights(Difficulty difficulty)
    {
        int difficultyFactor = (int)difficulty + 1; //Easy == 1, Normal == 2

        trust /= difficultyFactor;
        irascibility *= difficultyFactor;
        cowardice /= difficultyFactor;
    }

    /// <summary>
    /// Like a Finite State Machine, this method executes state behaviours.
    /// </summary>
    /// <param name="agent">The owner of this emotion chip</param>
    public void Execute(Enemy agent)
    {
        //Step 1. Execute current emotional goal
        if (agentEmotions[Emotion.Calm] > reluctance)
        {
            if (!inState)
            {
                currentState = calmGoal.EnterState(agent);
                inState = true;
            }

            currentState.Execute();
        }
        else if (agentEmotions[Emotion.Anger] > reluctance)
        {
          //  angryGoal.Execute(agent);
        }
        else if (agentEmotions[Emotion.Fear] > reluctance)
        {
          //  scaredGoal.Execute(agent);
        }

        //Step 2. Tend towards disposition
        List<Emotion> emotions = new List<Emotion>(agentEmotions.Keys);
        foreach (Emotion key in emotions)
        {
            if (key == disposition)
            {
                agentEmotions[key] = Mathf.Lerp(agentEmotions[key], 1f, Time.deltaTime);
            }
            else
            {
                agentEmotions[key] = Mathf.Lerp(agentEmotions[key], 0f, Time.deltaTime);
            }
        }
    }

    /// <summary>
    /// This method provides a way in which to influence this agent's emotional state
    /// </summary>
    /// <param name="intent">The way in which the actor intends to influence this agent</param>
    /// <param name="amount">How much to influence the agent's emotions in the range of 0 to 1</param>
    public void Influence(Emotion intent, float amount)
    {
        amount = Mathf.Clamp(amount, 0f, 1f);

        //Influence the NPC by the external factor's intent differently, based upon this agent's emotional disposition
        switch (disposition)
        {
            //If agent is predisposed to calm, handle the intended emotional influences as such:
            case Emotion.Calm:
                switch (intent)
                {
                    case Emotion.Calm:  //If actor intends to calm the agent, do it scaled by a factor of the agent's trust
                        agentEmotions[intent] += amount * trust;
                        break;

                    case Emotion.Anger: //If actor intends to scare or anger enemy, anger/scare them, but reduced by a factor of their trust (harder to anger/scare if calm)
                    case Emotion.Fear:
                        foreach (KeyValuePair<Emotion, float> agentEmotion in agentEmotions)
                        {
                            if (agentEmotion.Key != disposition) agentEmotions[agentEmotion.Key] += amount / trust;
                        }
                        break;
                }
                break;

            //If agent is predisposed to anger:
            case Emotion.Anger:
                switch (intent)
                {
                    case Emotion.Calm:  //If actor intends to calm the enemy, calm them, but reduce by a factor of their irascibility
                        agentEmotions[intent] += amount / irascibility;
                        break;

                    case Emotion.Anger: //If actor intends to anger the enemy, increase the intent by a factor of their irascibility
                        agentEmotions[intent] += amount * irascibility;
                        break;

                    case Emotion.Fear:  //If actor intends to scare enemy, simply anger the agent
                        agentEmotions[Emotion.Anger] += amount;
                        break;
                }
                break;

            //If agent is predisposed to fear:
            case Emotion.Fear:
                switch (intent)
                {
                    case Emotion.Calm:  //If actor intends to calm the agent, calm it, but reduce calming effect in relation to the agent's cowardice
                        agentEmotions[intent] += amount / cowardice;
                        break;

                    case Emotion.Anger: //If actor intends to anger the enemy, anger them, but anger them slowly in relation to cowardice
                        agentEmotions[intent] += amount / cowardice;
                        break;

                    case Emotion.Fear:  //If actor intends to scare enemy, scare them by a factor of the intent and cowardice
                        agentEmotions[intent] += amount * cowardice;
                        break;
                }
                break;
        }
        ScaleEmotions();    //This reduces the complexity of each assignment to emotion - we can go over 1 all the time without issue.
    }

    #region Implementation
    private void Awake()
    {
        //Store each emotion type in a state variable
        foreach (Emotion emotion in Enum.GetValues(typeof(Emotion)))
        {
            //Set the agent's most powerful emotion equal to their disposition
            if(emotion == disposition)
            {
                agentEmotions.Add(emotion, 1f);
            }
            else
            {
                agentEmotions.Add(emotion, 0f);
            }
        }
    }

    //Scale the emotions to be within the range [0:1]
    private void ScaleEmotions()
    {
        float totalValue = 0f;

        foreach (KeyValuePair<Emotion, float> agentEmotionPair in agentEmotions)
        {
            totalValue += agentEmotionPair.Value;
        }

        List<Emotion> emotions = new List<Emotion>(agentEmotions.Keys);
        foreach (Emotion key in emotions)
        {
            agentEmotions[key] = agentEmotions[key] / totalValue;
        }
    }
#endregion
}

public enum Emotion { Calm, Anger, Fear }
