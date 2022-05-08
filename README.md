To play, open:

./Wizziot Prototype/Builds/wizziot.exe

What's good?:
--
Architecture is designed in a way that a game designer can create a multiple-choice story with multiple enemy types, each with customisable emotional dispositions and behaviours, solely through the Unity inspector with maximum flexibility.

# Story and Exciting AI Design
*How the story is handled*

Using Ink markup language and plugin with Unity to create a choice-based story RPG. Similar to games such as KOTOR and Mass Effect, you can select story dialogue choices which affect the subsequent story direction. Each section of the story is "pointed to" by Story NPCs in the game world. Conversing with them accesses the relevant part of the Story script.

*Interesting AI Architecture*

I designed an "EMOTIONAL STATE MACHINE", based upon a Finite State Machine and Goal Oriented Action Planning. The EmotionChip (EmotionChip.cs) provides whatever it is attached to with an emotional disposition and three primary emotions (anger, fear, calm). ANY external factor can INFLUENCE (through a public interface method) emotions. The INTENT of the external actor is INTERPRETED depending on the NPC's EMOTIONAL DISPOSITION. 

Once the NPC's emotion has been influenced enough (determined by their emotional stability), then they will switch State/Goal. Their States/Goals (e.g. angryState) carry out actions determined by ScriptableObjects which can be created by a game designer. This entire framework allows EXTREME variation in the creation of enemy types. You can make an enemy prefab and modify their emotion weights, disposition, reluctance, and goals! 

For example: 

1.) Attach EmotionChip to an enemy. 

2.) Set its emotional disposition and reluctance/emotional stability. 

3.) Create a State Object (ScriptableObject), e.g.: Attack State, and modify its target variable, and whether it is hostile. 

4.) Drag this state into either the Angry, the Calm, or the Fear State. 

5.) Add a spawner to the world, and hit Play.

**Features**
- Story System (Interactive, e.g., KOTOR)
- Emotion System - Each agent has an "EmotionChip" which stores their emotional disposition and calm state, angry state, etc..
- Mission System - Multi-stage missions & rewards, granted by Story NPCs (linked to Ink script). Mission journal & log
- Stat System - Ability modifiers determined by the component base modifier value, equipped items, and game difficulty
- Item System (Using Stat System)
- Ability/Combat System
- Inventory System
- Save/Load System - custom generic class (SaveData.cs) for storing Key Value pair data. Loader.cs/Saver.cs determine what to save.
- Keybinding System
- Spawn points for enemy prefabs
- Day/night cycle that scares enemies using the emotion system

**Most interesting scripts:**

- EmotionChip.cs
- State.cs (& HideState.cs, AttackState.cs, etc..)
- StoryManager.cs
- MissionUI.cs (Compass code)
- AbilityComponent.cs

**Feature Flows**

*Story*

![Story granting flowchart](https://raw.githubusercontent.com/Benjy96/Wizziot/master/story_flow%20(1).png)

*Emotion-State AI Architecture*

![Emotion-State change flowchart](https://raw.githubusercontent.com/Benjy96/Wizziot/master/emotion%20state%20process(1).png)

*Missions (Using Scriptable Object System)*

![Scriptable Object mission granting process](https://raw.githubusercontent.com/Benjy96/Wizziot/master/Mission_Creation_Flow_UML.png)

**Architecture UML Diagram**

![Architecture Image](https://raw.githubusercontent.com/Benjy96/Wizziot/master/Wiz_Game_Management_UML%20(9)%20(8).png)
