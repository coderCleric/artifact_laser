using OWML.ModHelper;
using UnityEngine;
using OWML.Common;

namespace ArtifactLaser
{
    public class ArtifactLaser : ModBehaviour
    {
        //Setting Variables
        public static float timeToDie;

        //Functional Variables
        public static GhostBrain[] ghosts;
        public static float[] ghostLaserTimers;

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

            //Initialize our arrays to null for the time being
            ghosts = null;
            ghostLaserTimers = null;

            Patches.mainThing = this;
        }

        /**
         * Every frame, run the method that checks if ghosts are being lasered and if they need to die
         */
        public void Update()
        {
            //Check to see if any ghosts are being lasered
            KillGhosts();
        }

        /**
         * Check whether the player is illuminating any ghosts, and if they need to die
         */
        private void KillGhosts()
        {
            //Auto-return if we haven't initialized our lists
            if (ghosts == null || ghostLaserTimers == null)
                return;

            //Iterate through the array of ghost brains
            for (int i = 0; i < ghosts.Length; i++)
            {
                //Go to the next one if this brain is disabled, something is null, or the ghost is already dead
                if (ghosts[i] == null || !ghosts[i].enabled || ghosts[i]._sensors == null || ghosts[i]._sensors._lightSensor == null || !ghosts[i]._data.isAlive)
                    continue;

                //Check if their light sensor is lit by the player lantern
                if (ghosts[i]._sensors._lightSensor.IsIlluminatedByLantern(Locator.GetDreamWorldController().GetPlayerLantern().GetLanternController()))
                {
                    //Rumble the controller for style
                    RumbleManager.Pulse(0.05f, 0.05f, 0.05f);

                    //If it is, lower their timer
                    ghostLaserTimers[i] -= Time.deltaTime;

                    //If their timer is now over, kill them
                    if (ghostLaserTimers[i] <= 0)
                    {
                        ghosts[i].Die();
                        RumbleManager.Pulse(0.5f, 0.5f, 1.5f);
                    }
                }

                else
                {
                    //If it isn't, reset their timer
                    ghostLaserTimers[i] = timeToDie;
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

        public void debugPrint(string s)
        {
            ModHelper.Console.WriteLine(s);
        }
    }
 }


