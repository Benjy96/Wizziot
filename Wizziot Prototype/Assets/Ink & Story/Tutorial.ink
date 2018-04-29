VAR DEBUG = true
{DEBUG:
-> exile
}

VAR KillNuisance_Completed = false
VAR KillWizardsStock_Completed = false

// ----- External Functions ----- //
//Wizard: Kill Nuisance - Kill Enemies Destroying His Zoo
EXTERNAL KillNuisance()
== function KillNuisance() ==
~ return

//Exile
EXTERNAL KillWizardsStock()
== function KillWizardsStock() ==
~ return

EXTERNAL PushOff()
== function PushOff()
~ return
// -----

// ----- Dialogue ----- //
=== wizard ===
The wizard wavers from side to side.
+   {KillWizardsStock_Completed} What?
    "You're gonna end up face down in a pile of dirty robes."
+   {not KillNuisance_Completed && not KillWizardsStock_Completed} What's wrong?
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
                {KillNuisance()}
            ---- -> DONE
        ***  Wait, demons?
            "Don't worry, they're perfectly harmless. Unless, of course, you happen to be within a distance of, say, the average-sized continent."
            -> DONE
            
*   {KillNuisance_Completed} I killed it.
    "Thank you. You saved those creatures." The wizard strokes his beard. "Now, where was I? Ah, yes, by grinding those little buggers into dust I can mix them into a solution of..." He continues wavering.

+   {KillNuisance_Completed} How's things?
    "Sorry! No time to chat. I'm in the middle of outlining my reports. Thanks again for your help."
+   {deny} Still keeping those animals locked up, you prick?
    "Piss off, or I'll zap you." The wizard grumbles to himself. "God damn low-poly tree huggers."
- -> DONE

=== exile ===
{KillNuisance_Completed:
The sphere leers.
-> DONE
}
{insult:
"Get lost!"
-> DONE
}
{KillWizardsStock_Completed:
"Thanks for the help, I hear the Wizard is going to lose his tenure. He has no work to show for his funding."
-> DONE
}
{not KillWizardsStock_Completed: 
"Hey, got a minute?"
    +   (insult) You look a bit creepy, so no.
        "Watch your backs."
        -> DONE
    +   Sure, what's wrong?
        "See that Wizard? He's doing illegal experiments on one of these floating islands."
        **  Aren't Wizards exempt from the law?
            The sphere narrows its eyes. "You're not a lawyer, are you? Get lost."
            -> DONE
        ++  {not help_exile} Yeah, so what?
            The sphere's eyes dart up, down, and back up again. "You're a tough looking block, and you have the right attitude. Want to help me get some justice?"
            *** (help_exile) Yeah, why not.
                "Great, I'll give you something when you get back. If you didn't do it, I'll take something from you. Probably your third dimension. So don't mess around."
                    {KillWizardsStock()}
                    -> DONE
            *** Not right now.
                "If you change your mind, let me know. But don't take too long, or I'll change it for you."
                -> DONE
}

        

    
