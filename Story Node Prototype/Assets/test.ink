VAR DEBUG = true
{DEBUG:
-> cube.interact
}

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


//State Machine:
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
{SpherePushOff()} //External Function bound in Unity
-> DONE
}
-   -> DONE


=== cube ===
= interact
"They said be there or be square. Well, guess where I wasn't."
    +   ["Ha, nerd."]
        "Prick."
        {CubePushOff()}
    +   ["Who said that to you?"]
        That sphere guy over in the corner...
        + + ["Want me to do anything about it?"]
            "What do you mean?"
            +++ ["I need money, and I have a particular set of skills."]
                "Uh..."
                ++++ ["No, not that"]
                    "Oh, like, you'll kill him for me?"
                    +++++ ["Wow, I meant I'd just go talk to him."]
                        "Oh, uh... you should probably leave."
                    +++++ ["Of course."]
                        "Tell me when it's done, I have some milk money left over."
                ++++ ["Nevermind."]
        ++ ["What's his problem?]
            "I don't know, but I think I'm gonna tell my mum."
            ++++ ["Don't be a baby, man."]
                "Screw you."
-   -> DONE

=== shop_keeper ===
= interact
"I have some wares, if you'd like to take a look..."
*   [No, thanks.] // * choices can be used in a "state way" - ink tracks if seen
    "Alright, kid."
+   ->
    "You look familiar..."
- -> DONE


== function SpherePushOff ==
~ return 

== function CubePushOff ==
~ return