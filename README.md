# Story and Exciting AI Design
*How the story is handled*

Using Ink markup language and plugin with Unity to create a choice-based story RPG. Similar to games such as KOTOR and Mass Effect, you can select story dialogue choices which affect the subsequent story direction. Each section of the story is "pointed to" by Story NPCs in the game world. Conversing with them accesses the relevant part of the Story script.

*Interesting AI Architecture*

Since it is an RPG, I have implemented some AI. The most interesting aspect is that I designed an "EMOTIONAL STATE MACHINE", based upon a Finite State Machine and Goal Oriented Action Planning. Every Enemy has an EmotionChip (EmotionChip.cs) which provides whatever it is plugged into with an emotional disposition and three primary emotions. ANY external factor can INFLUENCE (through a public interface method) emotions. The INTENT of the external actor is received DIFFERENTLY depending on the NPC's emotional DISPOSITION. As well as the fact any number of conditions can affect the emotional "state" of the NPC - meaning we don't need to know all the checks in advance (like with a FSM with limited pre-conditions) - the fact there is an emotional disposition further varies HOW an external factor's influence actually influences the NPC's emotions.

Once the NPC's emotion has been influenced enough (determined by their reluctance), then they will switch State/Goal. Their States/Goals (e.g. angryGoal) are determined by ScriptableObjects which can be created rapidly by the game designer. This entire framework allows EXTREME variation in the creation of enemy types. You can make an enemy prefab and modify their emotion weights, disposition, reluctance, and goals! 

**Most interesting scripts:**

***Architectural:***

EmotionChip.cs
StoryManager.cs

***Practical:***

FlockState.cs (simulates flocking behaviour like "Boids"), complimented by NeighbourhoodTracker.cs
