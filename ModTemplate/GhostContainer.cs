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
        private ParticleSystem particleSystem;
        //public static Material sparkMaterial = null;
        private bool isDoingParticles;

        /**
         * Make a new ghost container with the given ghost brain
         */
        public GhostContainer(GhostBrain ghost)
        {
            this.ghost = ghost;
            this.deathTimer = ArtifactLaser.GetTimeToDie();
            this.isDoingParticles = false;

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

            //Get the particle system
            this.particleSystem = this.ghost._effects._deathParticleSystem;
            
            /*Left over from when I wanted to add fire particles
            //Do stuff to the renderer
            ParticleSystemRenderer rend = this.particleSystem.GetComponent<ParticleSystemRenderer>();
            rend.material = sparkMaterial;
            rend.minParticleSize = 0;
            rend.maxParticleSize = 0.5f;

            //Do stuff to the main component
            ParticleSystem.MainModule main = this.particleSystem.main;
            main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.03f);
            main.loop = true;
            main.duration = 5;
            main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;*/
        }

        /**
         * Gets the name of the ghost
         */
        public string getName()
        {
            return this.ghost._name;
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

            //Start playing the burn noise if it isn't already
            if (!this.audioSource.isPlaying)
                this.audioSource.Play();

            //Start the particle system
            this.isDoingParticles = true;
            if (!this.particleSystem.isEmitting)
                this.particleSystem.Play();

            //If their timer is now over, kill them and return true
            if (deathTimer <= 0)
            {
                ghost.Die();
                this.isDoingParticles = false;
                return true;
            }

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

            //Only stop the particles if we're doing them
            if(this.isDoingParticles)
                this.particleSystem.Stop();
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
