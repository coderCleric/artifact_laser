using OWML.ModHelper;
using UnityEngine;
using OWML.Common;
using System.Collections.Generic;

namespace ArtifactLaser
{
    public class ArtifactLaser : ModBehaviour
    {
        //Setting Variables
        private static float timeToDie;
        private bool countEnabled;
        private bool feedEnabled;

        //For actual functionality
        public static List<GhostContainer> ghosts = new List<GhostContainer>();
        private static ArtifactLaser instance;

        //For the kill counter
        private static int killsThisLoop = 0;
        private static int totalKills = 0;
        private static bool displayCount = false;
        private ScreenPrompt killDisplay = new ScreenPrompt("");

        //For the killfeed
        private ScreenPrompt killfeed = new ScreenPrompt("");
        private List<string> recentlyKilled = new List<string>();
        private float killfeedResetTime = 3;
        private float timeToReset = 0;

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

            //These are needed to manage the ghost containers
            //Set it so we stick ghosts in containers as they appear
            ModHelper.HarmonyHelper.AddPostfix<GhostBrain>(
                "Start",
                typeof(Patches),
                nameof(Patches.OnGhostStart));

            //Set it so we remove containers as their ghosts are destroyed
            ModHelper.HarmonyHelper.AddPrefix<GhostBrain>(
                "OnDestroy",
                typeof(Patches),
                nameof(Patches.OnGhostDestroyed));

            //These are needed for the kill counter
            //Set it so we reset the kill count for this loop when the player wakes up
            ModHelper.HarmonyHelper.AddPostfix<PlayerBody>(
                "Awake",
                typeof(Patches),
                nameof(Patches.OnPlayerAwake));

            //Set it so we reset the mod if the player exits
            ModHelper.HarmonyHelper.AddPostfix<PauseMenuManager>(
                "OnExitToMainMenu",
                typeof(Patches),
                nameof(Patches.OnMainMenuExit));

            instance = this;
        }

        /**
         * Check whether the player is illuminating any ghosts, and if they need to die
         */
        private void Update()
        {
            //Manage the kill count display
            this.ManageKillDisplay();

            //Manage the kill feed
            this.ManageKillfeed();

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

            //Hide the count if it is disabled
            if (!countEnabled)
                this.killDisplay.SetVisibility(false);
        }

        /**
         * Manages the killfeed
         */
        private void ManageKillfeed()
        {
            //Decay the name list
            this.timeToReset -= Time.deltaTime;
            this.timeToReset = Mathf.Max(0, this.timeToReset);

            //Check if we need to clear it
            if(this.timeToReset <= 0)
            {
                this.recentlyKilled.Clear();
            }

            //Add the prompt if it's not already in the list
            if (displayCount && !Locator.GetPromptManager().GetScreenPromptList(PromptPosition.BottomCenter).Contains(killfeed))
            {
                Locator.GetPromptManager().AddScreenPrompt(this.killfeed, PromptPosition.BottomCenter);
            }

            //If there are names, display them and make the prompt visible
            if (this.recentlyKilled.Count > 0)
            {
                string display = "";
                foreach(string i in this.recentlyKilled)
                {
                    display = display + "\n" + i + " just died!";
                }
                this.killfeed.SetText(display);
                this.killfeed.SetVisibility(true);
            }

            //If there aren't, make the prompt invisible
            else
                this.killfeed.SetVisibility(false);

            //Hide the feed if it is disabled
            if (!feedEnabled)
                this.killfeed.SetVisibility(false);
        }

        /**
         * Manages the ghosts
         */
        private void ManageGhosts()
        {
            //Only do stuff if we have things in our list
            if (!(ghosts.Count == 0))
            {
                //Iterate through the array of ghosts
                for (int i = 0; i < ghosts.Count; i++)
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
                            this.recentlyKilled.Add(ghosts[i].getName());
                            this.timeToReset = this.killfeedResetTime;
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
            countEnabled = config.GetSettingsValue<bool>("countEnabled");
            feedEnabled = config.GetSettingsValue<bool>("feedEnabled");
        }

        public static void DebugPrint(string s)
        {
            instance.ModHelper.Console.WriteLine(s);
        }
    }
 }


