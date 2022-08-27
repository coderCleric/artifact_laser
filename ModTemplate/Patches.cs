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
            //Generate all of the ghost containers
            mainThing.DebugPrint("Trying to find ghosts.");
            //Find all of the ghost brains
            GhostBrain[] brains = Resources.FindObjectsOfTypeAll<GhostBrain>();

            //Generate the list of containers
            ArtifactLaser.ghosts = new GhostContainer[brains.Length];
            for(int i = 0; i < brains.Length; i++)
            {
                ArtifactLaser.ghosts[i] = new GhostContainer(brains[i]);
            }
            mainThing.DebugPrint($"Found {ArtifactLaser.ghosts.Length} ghosts!");

             //Reset the kill count for this loop
             ArtifactLaser.ResetLoopKills();
        }

        /**
         * Clears the total kills and sets the counter to not display
         */
        public static void OnMainMenuExit()
        {
            ArtifactLaser.Reset();
        }
    }
}
