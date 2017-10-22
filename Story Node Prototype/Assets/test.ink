EXTERNAL SpherePushOff()
EXTERNAL CubePushOff()

//LIST can be used for FLAGS or STATE MACHINES
//Flags: Events e.g. have met Gordon
    //+= mark event occurred
    //? test event
//State Machine: States e.g. Annoyed, standing
    //= set state
    //test event

//State Machine for when the state can change, flags for one offs 

//Flag example:
LIST Events = (unmolested), haveAnnoyedSphere   

//State Machine example:
// LIST sphereState = (unmolested), pissed

"Hey, look, floating NPCs!"

//We DIVERT to this knot via script
=== sphere ===  //Sphere "Knot" - a branch of the story
= interact  //A subdivision of a knot is a "Stitch"
"Hey!" the sphere shouts. <>
{ Events ? unmolested:
+   ["Sorry."]    
    "Yeah, well, watch where you're going."
+   ["Screw you."]    
    "What the hell, man? Back off! Jesus Christ."
    ~ Events += haveAnnoyedSphere
    {SpherePushOff()} //External function defined in Unity (NPC.cs) - pushes player aw
    -> DONE
- else:
    <>"Get lost!"
    {SpherePushOff()}
    ->DONE
}
-   ->DONE

=== cube ===
= interact
"They said be there or be square. Well, guess where I wasn't."
+   ["Ha, nerd."]
    "Prick."
    {CubePushOff()}
+   ["My Condolences."]
    The cube sulks.
-   -> DONE

== function SpherePushOff ==
~ return 

== function CubePushOff ==
~ return