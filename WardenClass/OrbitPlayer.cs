using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace OvermorrowMod.WardenClass
{
    public class OrbitPlayer : ModPlayer
    {
        // The following code is an adaption of the Orbiting Projectile Code from Trinitarian
        // Credit to Marven, who gave permission to use this class

        // These are all important for the Orbiting staff. They could be used for other uses though. The orbiting staff uses at most the first 15 of the OrbitingProjectile array so the rest are free to use for other weapons.
        public int RotationTimer = 0;
        public int[] OrbitingProjectileCount = new int[5]; // Current upadted count of how many projectiles are active.
        public Vector2[,] OrbitingProjectilePositions = new Vector2[5, 50];  // Used to store the desired positions for the projectiles.
        public Projectile[,] OrbitingProjectile = new Projectile[5, 50]; // This stores all the projectiles that are currently beeing used. A projectiles ID is equal to the index in this array.

        public Vector2[] PreviousVelocity = new Vector2[30];

        public override void OnEnterWorld(Player player)
        {
            for (int i = 0; i < OrbitingProjectileCount.Length; i++)
            {
                OrbitingProjectileCount[i] = 0;
            }
        }
     
        public override void UpdateDead()
        {
            for (int i = 0; i < OrbitingProjectileCount.Length; i++)
            {
                OrbitingProjectileCount[i] = 0;
            }
        }

        // This is where we make our central timer that the orbiting projectile uses.
        public override void PostUpdate()
        {
            bool temp = false;
            for (int i = 0; i < 5; i++)
            {
                if (OrbitingProjectileCount[i] > 0) temp = true;
            }

            if (temp)
            {
                // The resetting the timer every 300 is not necessary but makes the period of the timer more apperant. 
                GenerateProjectilePositions();
                RotationTimer++;
            }
            else RotationTimer = 0;
        }
 
        public void GenerateProjectilePositions()
        {
            double period = 2f * Math.PI / 300f;
            for (int i = 0; i < OrbitingProjectileCount[0]; i++)
            {
                // Radius 200.
                OrbitingProjectilePositions[0, i] = player.Center + new Vector2(100 * (float)Math.Cos(period * (RotationTimer + (300 / OrbitingProjectileCount[0] * i))), 200 * (float)Math.Sin(period * (RotationTimer + (300 / OrbitingProjectileCount[0] * i))));
            }
        }
    }
}