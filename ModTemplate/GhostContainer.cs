/**
 * A simple class that packages the ghosts with some of their objects
 */

using OWML.ModHelper;
using UnityEngine;
using OWML.Common;

namespace ArtifactLaser
{
    public class GhostContainer
    {
        private GhostBrain ghost;
        private float deathTimer;
        private AudioSource audioSource;

        /**
         * Make a new ghost container with the given ghost brain
         */
        public GhostContainer(GhostBrain ghost)
        {
            this.ghost = ghost;
            this.deathTimer = ArtifactLaser.GetTimeToDie();

            //Make the audio source
            this.audioSource = this.ghost.gameObject.AddComponent<AudioSource>();
            if (Locator.GetAudioManager() == null)
                ArtifactLaser.DebugPrint("No audio manager");
            this.audioSource.clip = Locator.GetAudioManager().GetSingleAudioClip(AudioType.EnterVolumeDamageFire_LP, true);
            this.audioSource.loop = true;
            this.audioSource.volume = 1;
            this.audioSource.spatialBlend = 1f;
            this.audioSource.minDistance = 2;
            this.audioSource.Stop();
        }

        /**
         * Tells whether or not the ghost is lit by the player's lantern
         */
        public bool IsLitByPlayer()
        {
            //Auto return false if some important part of the ghost is disabled
            if (ghost == null || !ghost.enabled || ghost._sensors == null || ghost._sensors._lightSensor == null || !ghost._data.isAlive)
                return false;

            //Check if their light sensor is lit by the focused player lantern
            if (ghost._sensors._lightSensor.IsIlluminatedByLantern(Locator.GetDreamWorldController().GetPlayerLantern().GetLanternController()) && 
                Locator.GetDreamWorldController().GetPlayerLantern().GetLanternController().IsFocused())
                return true; //Return true if it is

            //Otherwise, return false
            return false;
        }

        /**
         * Lowers the timer by the amount of time that elapsed since the last frame
         * 
         * Returns true if they died from the damage, and false otherwise
         */
        public bool TakeBurnDamage()
        {
            //Reduce their timer
            this.deathTimer -= Time.deltaTime;

            //If their timer is now over, kill them and return true
            if (deathTimer <= 0)
            {
                ghost.Die();
                return true;
            }

            //Start playing the burn noise if it isn't already
            if(!this.audioSource.isPlaying)
                this.audioSource.Play();

            //Return false if they lived
            return false;
        }

        /**
         * Resets the timer
         */
        public void ResetTimer()
        {
            this.deathTimer = ArtifactLaser.GetTimeToDie();
            this.audioSource.Stop();
        }

        /**
         * Tells whether or not the container contains the given ghost brain
         */
        public bool ContainsGhost(GhostBrain g)
        {
            return this.ghost == g;
        }
    }
}
