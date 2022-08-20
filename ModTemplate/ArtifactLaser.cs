using OWML.ModHelper;
using UnityEngine;
using OWML.Common;

namespace ArtifactLaser
{
    public class ArtifactLaser : ModBehaviour
    {
        //Setting Variables
        private float timeToDie;
        private bool isLaserEnabled;

        //Functional Variables
        private GhostBrain[] ghosts;
        private float[] ghostLaserTimers;

        /**
         * Do some very basic start actions, like printing that we're in the mod, initializing things, and waiting for the player to wake up
         */
        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"In {nameof(ArtifactLaser)}!", MessageType.Success);

            //Set it so we wait for the player to wake up
            ModHelper.Events.Player.OnPlayerAwake += (player) => OnAwake();

            //Initialize our arrays to null for the time being
            this.ghosts = null;
            this.ghostLaserTimers = null;
        }

        /**
         * Once the player wakes up, get the list of ghosts and give them all death timers
         */
        private void OnAwake()
        {
            ModHelper.Console.WriteLine("Trying to find ghosts.");

            //Find all of the ghost brains
            this.ghosts = Resources.FindObjectsOfTypeAll<GhostBrain>();

            //Initialize the array of ghost timers
            this.ghostLaserTimers = new float[this.ghosts.Length];
            for (int i = 0; i < this.ghostLaserTimers.Length; i++)
            {
                this.ghostLaserTimers[i] = timeToDie;
            }

            ModHelper.Console.WriteLine($"Found {this.ghosts.Length} ghosts!");
        }

        /**
         * Every frame, run the method that checks if ghosts are being lasered and if they need to die
         */
        public void Update()
        {
            //Check to see if any ghosts are being lasered
            if (isLaserEnabled)
            {
                KillGhosts();
            }
        }

        /**
         * Check whether the player is illuminating any ghosts, and if they need to die
         */
        private void KillGhosts()
        {
            //Auto-return if we haven't initialized our lists
            if (this.ghosts == null || this.ghostLaserTimers == null)
                return;

            //Iterate through the array of ghost brains
            for (int i = 0; i < this.ghosts.Length; i++)
            {
                //Go to the next one if this brain is disabled, something is null, or the ghost is already dead
                if (this.ghosts[i] == null || !this.ghosts[i].enabled || this.ghosts[i]._sensors == null || this.ghosts[i]._sensors._lightSensor == null || !this.ghosts[i]._data.isAlive)
                    continue;

                //Check if their light sensor is lit by the player lantern
                if (this.ghosts[i]._sensors._lightSensor.IsIlluminatedByLantern(Locator.GetDreamWorldController().GetPlayerLantern().GetLanternController()))
                {
                    //Rumble the controller for style
                    RumbleManager.Pulse(0.05f, 0.05f, 0.05f);

                    //If it is, lower their timer
                    this.ghostLaserTimers[i] -= Time.deltaTime;

                    //If their timer is now over, kill them
                    if (this.ghostLaserTimers[i] <= 0)
                    {
                        this.ghosts[i].Die();
                        RumbleManager.Pulse(0.5f, 0.5f, 1.5f);
                    }
                }

                else
                {
                    //If it isn't, reset their timer
                    this.ghostLaserTimers[i] = timeToDie;
                }
            }
        }

        /**
         * Simple configuration behaviour so the settings actually work
         */
        public override void Configure(IModConfig config)
        {
            this.isLaserEnabled = config.GetSettingsValue<bool>("enableLaser");
            this.timeToDie = config.GetSettingsValue<float>("timeToKill");
        }
    }
 }


