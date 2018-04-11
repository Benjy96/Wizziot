VAR DEBUG = true
{DEBUG:
-> wizard
}

// ----- External Functions ----- //
//Wizard: Kill Nuisance - Kill Enemies Destroying His Zoo
EXTERNAL Tutorial_KillNuisance()
VAR Tutorial_KillNuisance_Completed = false

== function Tutorial_KillNuisance() ==
~ return

// ----- Dialogue ----- //
=== wizard ===
The wizard wavers from side to side.
+   {not Tutorial_KillNuisance_Completed} What's wrong?
    "I'll lose my tenure at the University if I don't get rid of that bloody ball!"
    **   What are you on about?
    **   It looks like a perfectly nice portal to me.
        "Not that, you moron!"
        ***  Oh.
        --   "I've been doing a thesis on tiny demons. I have a safe enclosure on one of the islands, but one of them must be eating the others, because there were more of them at last count. There's that bloody massive one, too..."
        ***  What do you want me to do about it?
            "Could you. . ." the wizard pauses and looks at you spherically. ". . . euthanize it?"
            **** (deny) No! Of Course not!
                "Piss off, then."
            **** Of course
                "Excellent! I'll give you a pancake when you get back."
                {Tutorial_KillNuisance()}
            ---- -> DONE
        ***  Wait, demons?
            "Don't worry, they're perfectly harmless. Unless, of course, you happen to be within a distance of, say, the average-sized continent."
*   {Tutorial_KillNuisance_Completed} I killed it.
    "Thank you. You saved those creatures." The wizard strokes his beard. "Now, where was I? Ah, yes, by grinding those little buggers into dust I can mix them into a solution of..." He continues wavering.
    -> DONE

+   {Tutorial_KillNuisance_Completed} How's things?
    "Sorry! No time to chat. I'm in the middle of outlining my reports. Thanks again for your help."
+   {deny} Still keeping those animals locked up, you prick?
    "Piss off, or I'll zap you." The wizard grumbles to himself. "God damn low-poly tree huggers."
- -> DONE