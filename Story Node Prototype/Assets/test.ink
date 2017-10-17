EXTERNAL PushOff()

"Hey, look, a floating sphere!"

//We DIVERT to this knot via script
=== sphere ===  //Sphere "Knot" - a branch of the story
= interact  //A subdivision of a knot is a "Stitch"
"Hey!"
+   ["Sorry."]    //Bracketed choices aren't printed once you choose the response
    "Yeah, well, watch where you're going
+   ["Screw you."]    //Choices denoted by a '+' are re-usable (sticky choice)
    "What the hell, man? Back off! Jesus Christ."
    {PushOff()} //External function defined in Unity (NPC.cs) - pushes player away
-   -> DONE

== function PushOff ==
~ return 