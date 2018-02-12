using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A variant of a finite state machine... The EMOTIONAL State Machine (ESM)
/// Each agent with an emotion chip has an emotional disposition. 
/// External factors can influence this agent's emotions; the way in which each emotion is affected differs between emotional dispositions. 
/// The extent to which each emotion differs is then affected by weight values.
/// </summary>
public class EmotionChip : MonoBehaviour {

    //Disposition changes how influencing factors affect the agent
    public Emotion disposition = Emotion.Calm;

    //How likely an emotional agent is to change their behaviour
    [Range(0.1f, 0.9f)] public float reluctance = 0.5f;

    //The agent's emotional state(s)
    public Dictionary<Emotion, float> agentEmotions = new Dictionary<Emotion, float>();

    public float trust = 2f; //Calm weighting
    public float irascibility = 2f; //Anger weighting
    public float cowardice = 2f;    //Fear weighting

    //TODO: Verify whether single states or whether actual GOALS (FSM v GOAP)
    public State calmGoal;
    public State angryGoal;
    public State scaredGoal;

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

    private void Update()
    {
        //Step 1. Execute current emotional goal
        if (agentEmotions[Emotion.Calm] > reluctance)
        {
            //TODO: "List" of scared actions with costs and goal fulfillments? Like in GOAP
            calmGoal.Execute(this);
        }
        else if (agentEmotions[Emotion.Anger] > reluctance)
        {
            angryGoal.Execute(this);
        }
        else if (agentEmotions[Emotion.Fear] > reluctance)
        {
            scaredGoal.Execute(this);
        }

        //Step 2. Tend towards disposition
        foreach (KeyValuePair<Emotion, float> agentEmotion in agentEmotions)
        {
            //If disposition, tend towards max, else tend towards minimum
            if (agentEmotion.Key == disposition)
            {
                agentEmotions[agentEmotion.Key] = Mathf.Lerp(agentEmotions[disposition], 1f, Time.deltaTime);
            }
            else
            {
                agentEmotions[agentEmotion.Key] = Mathf.Lerp(agentEmotions[agentEmotion.Key], 0f, Time.deltaTime);
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
                    case Emotion.Calm:  //If actor intends to calm the enemy, calm them, but reduce it in relation to the agent's cowardice
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

    //Scale the emotions to be within the range [0:1]
    private void ScaleEmotions()
    {
        float totalValue = 0f;

        foreach (KeyValuePair<Emotion, float> enemyEmotion in agentEmotions)
        {
            totalValue += enemyEmotion.Value;
        }

        foreach (KeyValuePair<Emotion, float> enemyEmotion in agentEmotions)
        {
            agentEmotions[enemyEmotion.Key] = agentEmotions[enemyEmotion.Key] / totalValue;
        }
    }
}

public enum Emotion { Calm, Anger, Fear }
