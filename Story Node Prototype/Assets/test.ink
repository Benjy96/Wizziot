EXTERNAL SpherePushOff()
EXTERNAL CubePushOff()

//State tracking
LIST sphereState = (unmolested), pissed

"Hey, look, floating NPCs!"

//We DIVERT to this knot via script
=== sphere ===  //Sphere "Knot" - a branch of the story
= interact  //A subdivision of a knot is a "Stitch"
"Hey!" the sphere shouts. <>
{ sphereState == unmolested:
+   ["Sorry."]    
    "Yeah, well, watch where you're going."
+   ["Screw you."]    
    "What the hell, man? Back off! Jesus Christ."
    ~ sphereState = pissed
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