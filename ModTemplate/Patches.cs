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

        /**
         * Do certain actions when the player body wakes, like getting the list of owlks
         */
        /*public static void OnEnterDreamWorld()
        {
            //Generate all of the ghost containers
            ArtifactLaser.DebugPrint("Trying to find ghosts.");

            //Find all of the ghost brains
            GhostBrain[] brains = Resources.FindObjectsOfTypeAll<GhostBrain>();

            //Generate the list of containers
            ArtifactLaser.ghosts = new GhostContainer[brains.Length];
            for (int i = 0; i < brains.Length; i++)
            {
                ArtifactLaser.ghosts[i] = new GhostContainer(brains[i]);
            }

            ArtifactLaser.DebugPrint($"Found {ArtifactLaser.ghosts.Length} ghosts!");

             //Reset the kill count for this loop
             ArtifactLaser.ResetLoopKills();
        }*/

        /**
         * Add ghosts to the list as they appear
         */
        public static void OnGhostStart(GhostBrain __instance)
        {
            ArtifactLaser.ghosts.Add(new GhostContainer(__instance));
        }

        /**
         * Remove ghosts from the list as they are destroyed
         */
        public static void OnGhostDestroyed(GhostBrain __instance)
        {
            for(int i = 0; i < ArtifactLaser.ghosts.Count; i++)
            {
                if(ArtifactLaser.ghosts[i].ContainsGhost(__instance))
                {
                    ArtifactLaser.ghosts.RemoveAt(i);
                    return;
                }
            }
        }

        /**
         * Clears the loop kills when the player wakes up
         */
        public static void OnPlayerAwake()
        {
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
