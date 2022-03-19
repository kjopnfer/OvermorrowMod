using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Common
{
    public class OrbitEntityContainer
    {
        // Update these when you add more 
        public const int DISTINCT_ORBITING_PROJECTILES = 1;
        public const int MAX_PROJECTILES_PER_TYPE = 50;
        // The following code is an adaption of the Orbiting Projectile Code from Trinitarian
        // Credit to Marven, who gave permission to use this class

        // These are all important for the Orbiting staff. They could be used for other uses though. The orbiting staff uses at most the first 15 of the OrbitingProjectile array so the rest are free to use for other weapons.
        public int RotationTimer = 0;
        public int[] OrbitingProjectileCount = new int[DISTINCT_ORBITING_PROJECTILES]; // Current upadted count of how many projectiles are active.
        public Vector2[,] OrbitingProjectilePositions = new Vector2[DISTINCT_ORBITING_PROJECTILES, MAX_PROJECTILES_PER_TYPE];  // Used to store the desired positions for the projectiles.
        public Projectile[,] OrbitingProjectile = new Projectile[DISTINCT_ORBITING_PROJECTILES, MAX_PROJECTILES_PER_TYPE]; // This stores all the projectiles that are currently beeing used. A projectiles ID is equal to the index in this array.

        // Call this in the PostUpdate method of the entity
        public void OnUpdate()
        {
            bool anyProjectile = false;
            for (int i = 0; i < DISTINCT_ORBITING_PROJECTILES; i++)
            {
                if (OrbitingProjectileCount[i] > 0)
                {
                    anyProjectile = true;
                    break;
                }
            }

            if (anyProjectile)
            {
                RotationTimer++;
            }
            else RotationTimer = 0;
        }

        public void ResetProjectiles()
        {
            for (int i = 0; i < DISTINCT_ORBITING_PROJECTILES; i++)
            {
                OrbitingProjectileCount[i] = 0;
            }
        }

        public int RegisterNewProjectile(int slot, Projectile projectile)
        {
            int index = OrbitingProjectileCount[slot];
            OrbitingProjectile[slot, index] = projectile;
            OrbitingProjectileCount[slot]++;
            return index;
        }
    }
}
