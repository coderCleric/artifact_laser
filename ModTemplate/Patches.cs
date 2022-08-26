using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ArtifactLaser
{
    public class Patches : MonoBehaviour
    {
        public static ArtifactLaser mainThing;

        /**
         * Do certain actions when the player body wakes, like getting the list of owlks
         */
        public static void OnPlayerAwake()
        {
            mainThing.debugPrint("Trying to find ghosts.");

            //Find all of the ghost brains
            ArtifactLaser.ghosts = Resources.FindObjectsOfTypeAll<GhostBrain>();

            //Initialize the array of ghost timers
            ArtifactLaser.ghostLaserTimers = new float[ArtifactLaser.ghosts.Length];
            for (int i = 0; i < ArtifactLaser.ghostLaserTimers.Length; i++)
            {
                ArtifactLaser.ghostLaserTimers[i] = ArtifactLaser.timeToDie;
            }

            mainThing.debugPrint($"Found {ArtifactLaser.ghosts.Length} ghosts!");
        }
    }
}
