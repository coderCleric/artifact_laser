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
         * Add ghosts to the list as they appear
         */
        public static void OnGhostStart(GhostBrain __instance)
        {
            /*Leftover from when I wanted to add spark particles to burning ghosts
            //Steal the campfire spark material
            if (GhostContainer.sparkMaterial == null)
            {
                GameObject tmp1 = GameObject.Find("LaunchTower/Effects_HEA_Campfire/Effects/Sparks");
                ParticleSystem tmp2 = tmp1.GetComponent<ParticleSystem>();
                ParticleSystemRenderer tmpRend = tmp2.GetComponent<ParticleSystemRenderer>();
                GhostContainer.sparkMaterial = tmpRend.material;
            }*/

            //Make and add the container
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
