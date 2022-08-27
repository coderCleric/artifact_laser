using OWML.ModHelper;
using UnityEngine;
using OWML.Common;

namespace ArtifactLaser
{
    public class ArtifactLaser : ModBehaviour
    {
        //Setting Variables
        private static float timeToDie;

        //Functional Variables
        public static GhostContainer[] ghosts;
        private static int killsThisLoop = 0;
        private static int totalKills = 0;
        private static bool displayCount = false;
        private ScreenPrompt killDisplay = new ScreenPrompt("");

        //Needed just for inter-file communication
        /**
         * Gets the value of timeToDie
         */
        public static float GetTimeToDie()
        {
            return timeToDie;
        }
        /**
         * Reset this loop's kill count
         */
        public static void ResetLoopKills()
        {
            killsThisLoop = 0;
        }
        /**
         * Resets the whole mod
         */
        public static void Reset()
        {
            killsThisLoop = 0;
            totalKills = 0;
            displayCount = false;

        }
        /**
         * Tells whether or not the kill count should be displayed
         */
        public static bool ShouldDisplayKillCount()
        {
            return displayCount;
        }

        //More complicated & important functions
        /**
         * Do some very basic start actions, like printing that we're in the mod, initializing things, and waiting for the player to wake up
         */
        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"In {nameof(ArtifactLaser)}!", MessageType.Success);

            //Set it so we wait for the player to wake up
            ModHelper.HarmonyHelper.AddPostfix<PlayerBody>(
                "Awake",
                typeof(Patches),
                nameof(Patches.OnPlayerAwake));

            //Set it so we reset the mod if the player exits
            ModHelper.HarmonyHelper.AddPostfix<PauseMenuManager>(
                "OnExitToMainMenu",
                typeof(Patches),
                nameof(Patches.OnMainMenuExit));

            //Initialize our ghost array to null for the time being
            ghosts = null;

            Patches.mainThing = this;
        }

        /**
         * Check whether the player is illuminating any ghosts, and if they need to die
         */
        private void Update()
        {
            //Manage the kill count display
            this.ManageKillDisplay();

            //Manage the ghosts
            this.ManageGhosts();
        }

        /**
         * Manages the kill count display
         */
        private void ManageKillDisplay()
        {
            //Hide the kill count if the bool says not to display it
            if (!displayCount)
                this.killDisplay.SetVisibility(false);

            //Display the kill count if the bool says so and the game is not paused
            if (displayCount && !OWTime.IsPaused(OWTime.PauseType.Menu))
            {
                //Add the prompt if it's not already in the list
                if (!Locator.GetPromptManager().GetScreenPromptList(PromptPosition.LowerLeft).Contains(killDisplay))
                {
                    Locator.GetPromptManager().AddScreenPrompt(this.killDisplay, PromptPosition.LowerLeft);
                }

                //Update the text and make the prompt visible
                this.killDisplay.SetText($"Kills This Loop: {killsThisLoop}\nTotal Kills: {totalKills}");
                this.killDisplay.SetVisibility(true);
            }

            //Hide the count if the game is paused
            if (OWTime.IsPaused(OWTime.PauseType.Menu))
            {
                this.killDisplay.SetVisibility(false);
            }
        }

        /**
         * Manages the ghosts
         */
        private void ManageGhosts()
        {
            //Only do stuff if we've initialized our list
            if (ghosts != null)
            {
                //Iterate through the array of ghosts
                for (int i = 0; i < ghosts.Length; i++)
                {
                    //Check if they're lit up by the player
                    if (ghosts[i].IsLitByPlayer())
                    {
                        //Rumble the controller for style
                        RumbleManager.Pulse(0.05f, 0.05f, 0.05f);

                        //Damage them, do some stuff if they die
                        if (ghosts[i].TakeBurnDamage())
                        {
                            RumbleManager.Pulse(0.5f, 0.5f, 1.5f);
                            killsThisLoop++;
                            totalKills++;
                            displayCount = true;
                        }
                    }

                    //If they're not lit up, reset their timer
                    else
                    {
                        ghosts[i].ResetTimer();
                    }
                }
            }
        }

        /**
         * Simple configuration behaviour so the settings actually work
         */
        public override void Configure(IModConfig config)
        {
            timeToDie = config.GetSettingsValue<float>("timeToKill");
        }

        public void DebugPrint(string s)
        {
            ModHelper.Console.WriteLine(s);
        }
    }
 }


