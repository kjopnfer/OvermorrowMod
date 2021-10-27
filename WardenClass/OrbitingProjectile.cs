using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass
{
    abstract public class OrbitingProjectile : ModProjectile
    {
        // The following code is an adaption of the Orbiting Projectile Code from Trinitarian
        // Credit to Marven, who gave permission to use this class

        private const int State_Initializing = 0;       // This is used for initilizing all values.
        private const int State_Spawning = 1;
        private const int State_Moving = 2;             // This is the state in which the projectiles moves on the circle with the fast period.
        private const int State_Cycling = 3;            // Unused but might be needed for better control over how the projectile moves when a new radius is set.
        private const int State_Attacking = 4;          // Abstract state used to give the projectile Attacking behaviour or any other things like getting fired at a enemy.
        private const int State_Empty = 5;

        // This player is not neccesarily the target that gets orbited. We do need a player instance to make sure we have a suitable positions and projectile array we can use.
        public Player player;

        // This ProjectileSlot refers to which array we use to store our projectiles every weapon/projectile that wants to be independent of other orbiting projectiles has to choose a new ProjectileSlot that is not yet used.
        public int ProjectileSlot;

        // The radius at which the projectile orbits. This is set per projectile this means that every projectile can theoretically have a different orbiting radius.
        public float OrbitingRadius;

        // This is how many frames have to pass before the projectiles completes a full rotation.
        public float Period;

        // This is the same as Period but for the rotation used to realign the projectiles. This should be significantly lower than Period.
        public float PeriodFast;

        // The speed that the projectile uses to get into position when it spawns or has to adjust the radius.
        public float ProjectileSpeed;

        // The position that the projectiles orbit around.
        public Vector2 OrbitCenter;

        // The velocity of the position the projectiles are orbiting around.
        public Vector2 RelativeVelocity;

        public int TimerStart = 0;
        public double angle = 0;
        public float CurrentOrbitingRadius = 0;

        public float ProjID
        {
            get => projectile.localAI[0];
            set => projectile.localAI[0] = value;
        }
        public float Proj_State
        {
            get => projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }

        public override void AI()
        {
            OrbitPlayer modplayer = player.GetModPlayer<OrbitPlayer>();
            Vector2 ProjectileVelocity = Vector2.Zero;
            // Update the position array.
            GenerateProjectilePositions();

            // This state can be used to keep the projectiles from updating.
            if (Proj_State == State_Empty)
            {
                return;
            }

            // This handles the spawning of the projectiles. 
            if (Proj_State == State_Initializing)
            {
                ProjID = modplayer.OrbitingProjectileCount[ProjectileSlot];
                modplayer.OrbitingProjectile[ProjectileSlot, modplayer.OrbitingProjectileCount[ProjectileSlot]] = projectile;
                modplayer.OrbitingProjectileCount[ProjectileSlot]++;
                // Make sure to update the array whenever the ProjectileSlot of projectiles changes.
                GenerateProjectilePositions();
                Proj_State = State_Spawning;
            }

            // Despawn distance since server despawned projectiles don't call the Kill hook.
            // This is important since Kill is where we do the calculations to reorder the projectiles once one dies.
            if (projectile.DistanceSQ(OrbitCenter) > 2000 * 2000)
            {
                projectile.Kill();
            }

            // Movement section
            if (Proj_State == State_Spawning || Proj_State == State_Moving)
            {
                // We make all the movement of the projectiles relative to the player to make sure the orbiting around the player works properly.
                projectile.velocity = RelativeVelocity;
                int ProjNumber = modplayer.OrbitingProjectileCount[ProjectileSlot];
                // Here we assign the projectile its position on the circle the positions are assigned from front to back to make sure that the projectiles rotate in the correct direction.
                // The positions are stored in the array that is stored on the modplayer instance. 

                Vector2 WantedPosition = modplayer.OrbitingProjectilePositions[ProjectileSlot, ProjNumber - (int)ProjID - 1];
                // Snapping distance of 20 meaning that once the projectile gets within 20 pixels of the ideal position it just snapps there and stays.
                // !!!!Make sure this ProjectileSlot is never smaller than the rotational velocity of the cirlce (the fast one)!!!!            
                if (projectile.DistanceSQ(WantedPosition) < 20 * 20)
                {
                    projectile.Center = WantedPosition;
                    // Once spawning is done aka we reached the correct OrbitingRadius. We can switch to using the radial movement.
                    Proj_State = State_Moving;
                    TimerStart = 0;
                    CurrentOrbitingRadius = OrbitingRadius;
                }
                else
                {
                    // This just moves straight to the wanted position. This does work for getting to the positions after just spawning it does look somewhat wierd tho since it doesn't move along the OrbitingRadius of the circle.
                    if (Proj_State == State_Spawning || CurrentOrbitingRadius != OrbitingRadius)
                    {
                        ProjectileVelocity = WantedPosition - projectile.Center;
                        if (ProjectileVelocity != Vector2.Zero)
                        {
                            ProjectileVelocity.Normalize();
                        }
                        ProjectileVelocity *= ProjectileSpeed;
                        projectile.velocity += ProjectileVelocity;
                    }

                    // This is our main way of movement. We initialze the TimerStart here and calculate the current angle towards the player. These 2 variables allow us to get a starting position for the movement.
                    // We use the timer that runns on the player to make sure everything is synced.
                    else
                    {
                        //if (CurrentOrbitingRadius != OrbitingRadius)
                        //{
                        //    float factor = OrbitingRadius - CurrentOrbitingRadius;
                        //    factor /= Math.Abs(factor);
                        //    ProjectileVelocity = WantedPosition - OrbitCenter;
                        //    ProjectileVelocity *= ProjectileSpeedRadial / OrbitingRadius * factor;
                        //    projectile.velocity += ProjectileVelocity;
                        //    CurrentOrbitingRadius += ProjectileSpeedRadial * factor;
                        //}
                        if (TimerStart == 0)
                        {
                            //Get the initial conditions and with that also the current starting location for the movement.
                            angle = Math.Atan2(projectile.Center.Y - player.Center.Y, projectile.Center.X - player.Center.X);
                            TimerStart = modplayer.RotationTimer;
                        }
                        //This period determines how fast the projectiles move along the radial path.
                        double period = 2 * Math.PI / PeriodFast;
                        projectile.Center = OrbitCenter + new Vector2(OrbitingRadius * (float)Math.Cos(period * (modplayer.RotationTimer - TimerStart) + angle), OrbitingRadius * (float)Math.Sin(period * (modplayer.RotationTimer - TimerStart) + angle));
                    }
                }
            }
            else if (Proj_State == State_Attacking) // Here the abstract attacking behaviour gets called. T oacces just override Attack().
            {
                // Attack();
            }
        }

        // This is where the reordering happens. Override this if there are any cases in which you don't want the projectile to count as killed.
        // For example when putting a shooting behaviour into the Attack hook you would probably want the projectile to instanty count as killed when it was shoot.
        // In that case you would manually call GeneratePositionsAfterKill() when Attack() is called and override Kill to exclude projectiles that are in attacking.
        public override void Kill(int timeLeft)
        {
            GeneratePositionsAfterKill();
        }

        // This algorithm handles what happens when 1 projetile is not considered to be in the circle anymore.
        // Its also important to uptade the positions array whenever the ProjectileSlot of projectiles changes.
        public void GeneratePositionsAfterKill()
        {
            Player player = Main.player[projectile.owner];
            OrbitPlayer modplayer = player.GetModPlayer<OrbitPlayer>();
            modplayer.OrbitingProjectileCount[ProjectileSlot]--;
            int ProjNumber = modplayer.OrbitingProjectileCount[ProjectileSlot];
            // This reorders the Projectile array that has all the currently active projectiles.
            if (ProjNumber > 0 && ProjID != 0)
            {
                for (int i = (int)ProjID; i < ProjNumber; i++)
                {
                    modplayer.OrbitingProjectile[ProjectileSlot, i] = modplayer.OrbitingProjectile[ProjectileSlot, i + 1];
                }
                modplayer.OrbitingProjectile[ProjectileSlot, ProjNumber] = modplayer.OrbitingProjectile[ProjectileSlot, 0];
            }


            for (int j = 0; j < ProjNumber; j++)
            {
                modplayer.OrbitingProjectile[ProjectileSlot, j] = modplayer.OrbitingProjectile[ProjectileSlot, j + 1];
            }

            // This assigns the projectile IDs
            for (int k = 0; k < ProjNumber; k++)
            {
                modplayer.OrbitingProjectile[ProjectileSlot, k].localAI[0] = k;
            }

            // Here we update the positions.
            GenerateProjectilePositions();
        }

        // This is the function that generates the positions.
        public void GenerateProjectilePositions()
        {
            OrbitPlayer modplayer = player.GetModPlayer<OrbitPlayer>();
            for (int i = 0; i < modplayer.OrbitingProjectileCount[ProjectileSlot]; i++)
            {
                modplayer.OrbitingProjectilePositions[ProjectileSlot, i] = OrbitCenter + new Vector2(OrbitingRadius * (float)Math.Cos(2 * Math.PI / Period * modplayer.RotationTimer + (2 * Math.PI / modplayer.OrbitingProjectileCount[ProjectileSlot] * i)), OrbitingRadius * (float)Math.Sin(2 * Math.PI / Period * modplayer.RotationTimer + (2 * Math.PI / modplayer.OrbitingProjectileCount[ProjectileSlot] * i)));
            }
        }
    }
}