EXTERNAL SpherePushOff()
EXTERNAL CubePushOff()

//LIST can be used for FLAGS or STATE MACHINES
//Flags: Events e.g. have met Gordon
    //+= mark event occurred
    //? test event
//State Machine: States e.g. Annoyed, standing
    //= set state
    //== test state

//State Machine for when the state can change, flags for one offs, e.g. 
//multiple flags can be set to true by subscribing (+=) - don't mix the syntax
//or it'll mix behaviours! i.e. not strongly typed - = on flag makes it equal
//the state

//Flag example:
// LIST Events = (unmolested), haveAnnoyedSphere   

//State Machine example:
LIST SphereState = (unmolested), annoyed

//We DIVERT to this knot via script
=== sphere ===  //Sphere "Knot" - a branch of the story
= interact  //A subdivision of a knot is a "Stitch"
"Hey!" the sphere shouts. <>
{ SphereState == unmolested:
+   ["Sorry."]    
    "Yeah, well, watch where you're going."
+   ["Screw you."]    
    "What the hell, man? Back off! Jesus Christ."
    ~ SphereState = annoyed
    {SpherePushOff()} //External function defined in Unity (NPC.cs) - pushes player
- else:
    <>"Get lost!"
    {SpherePushOff()}
    -> DONE
}
-   -> DONE

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