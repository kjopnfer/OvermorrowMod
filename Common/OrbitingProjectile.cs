using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public enum OrbitingProjectileState
    {
        Initializing = 0, // This is used for initilizing all values.
        Spawning = 1,     
        Moving = 2,       // This is the state in which the projectiles moves on the circle with the fast period.
        Cycling = 3,      // Unused but might be needed for better control over how the projectile moves when a new radius is set.
        Attacking = 4,    // Abstract state used to give the projectile Attacking behaviour or any other things like getting fired at a enemy.
        Empty = 5
    }

    public abstract class OrbitingProjectile : ModProjectile
    {
        // The following code is an adaption of the Orbiting Projectile Code from Trinitarian
        // Credit to Marven, who gave permission to use this class


        public int ProjectileSlot { get; }

        protected OrbitingProjectile(int projectileSlot)
        {
            ProjectileSlot = projectileSlot;
        }

        // This entity is not neccesarily the target that gets orbited. We do need an entity instance to make sure we have a suitable positions and projectile array we can use.
        protected Entity entity;

        // The radius at which the projectile orbits. This is set per projectile this means that every projectile can theoretically have a different orbiting radius.
        protected float OrbitingRadius;

        // This is how many frames have to pass before the projectiles completes a full rotation.
        protected float Period;

        // This is the same as Period but for the rotation used to realign the projectiles. This should be significantly lower than Period.
        protected float PeriodFast;

        // The speed that the projectile uses to get into position when it spawns or has to adjust the radius.
        protected float ProjectileSpeed;

        // The position that the projectiles orbit around.
        protected Vector2 OrbitCenter;

        // The velocity of the position the projectiles are orbiting around.
        protected Vector2 RelativeVelocity;

        protected int TimerStart = 0;
        protected double angle = 0;
        protected float CurrentOrbitingRadius = 0;

        protected OrbitEntityContainer GetOrbitContainer()
        {
            if (entity is Player player)
            {
                return player.GetModPlayer<OrbitPlayer>().Container;
            }
            else if (entity is NPC npc)
            {
                return npc.GetGlobalNPC<OrbitNPC>().Container;
            }
            else
            {
                return null;
            }
        }

        public int ProjIndex
        {
            get => (int)Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }
        protected OrbitingProjectileState ProjState
        {
            get => (OrbitingProjectileState)Projectile.localAI[1];
            set => Projectile.localAI[1] = (float)value;
        }

        public override void AI()
        {
            // This state can be used to keep the projectiles from updating.
            if (ProjState == OrbitingProjectileState.Empty)
            {
                return;
            }

            var container = GetOrbitContainer();
            // If there is no target entity, enter attack mode
            if (container == null || ProjState == OrbitingProjectileState.Attacking)
            {
                ProjState = OrbitingProjectileState.Attacking;
                Attack();
                return;
            }

            Vector2 velocityChange;

            // This handles the spawning of the projectiles. 
            if (ProjState == OrbitingProjectileState.Initializing)
            {
                ProjIndex = container.RegisterNewProjectile(ProjectileSlot, Projectile);
                // Make sure to update the array whenever the ProjectileSlot of projectiles changes.
                ProjState = OrbitingProjectileState.Spawning;
            }

            GenerateProjectilePositions(container);

            // Despawn distance since server despawned projectiles don't call the Kill hook.
            // This is important since Kill is where we do the calculations to reorder the projectiles once one dies.
            if (Projectile.DistanceSQ(OrbitCenter) > 2000 * 2000)
            {
                Projectile.Kill();
                return;
            }

            // Movement section
            if (ProjState == OrbitingProjectileState.Spawning || ProjState == OrbitingProjectileState.Moving)
            {
                // We make all the movement of the projectiles relative to the player to make sure the orbiting around the player works properly.
                Projectile.velocity = RelativeVelocity;
                int numProjectilesOfType = container.OrbitingProjectileCount[ProjectileSlot];
                // Here we assign the projectile its position on the circle the positions are assigned from front to back to make sure that the projectiles rotate in the correct direction.
                // The positions are stored in the array that is stored on the modplayer instance. 

                Vector2 wantedPosition = container.OrbitingProjectilePositions[ProjectileSlot, numProjectilesOfType - ProjIndex - 1];
                // Snapping distance of 20 meaning that once the projectile gets within 20 pixels of the ideal position it just snapps there and stays.
                // !!!!Make sure this ProjectileSlot is never smaller than the rotational velocity of the cirlce (the fast one)!!!!            
                if (Projectile.DistanceSQ(wantedPosition) < 20 * 20)
                {
                    Projectile.Center = wantedPosition;
                    // Once spawning is done aka we reached the correct OrbitingRadius. We can switch to using the radial movement.
                    ProjState = OrbitingProjectileState.Moving;
                    TimerStart = 0;
                    CurrentOrbitingRadius = OrbitingRadius;
                }
                else
                {
                    // This just moves straight to the wanted position. This does work for getting to the positions after just spawning it does look somewhat wierd tho since it doesn't move along the OrbitingRadius of the circle.
                    if (ProjState == OrbitingProjectileState.Spawning || CurrentOrbitingRadius != OrbitingRadius)
                    {
                        velocityChange = wantedPosition - Projectile.Center;
                        if (velocityChange != Vector2.Zero)
                        {
                            velocityChange.Normalize();
                        }
                        velocityChange *= ProjectileSpeed;
                        Projectile.velocity += velocityChange;
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
                        //    Projectile.velocity += ProjectileVelocity;
                        //    CurrentOrbitingRadius += ProjectileSpeedRadial * factor;
                        //}
                        if (TimerStart == 0)
                        {
                            //Get the initial conditions and with that also the current starting location for the movement.
                            angle = Math.Atan2(Projectile.Center.Y - entity.Center.Y, Projectile.Center.X - entity.Center.X);
                            TimerStart = container.RotationTimer;
                        }
                        //This period determines how fast the projectiles move along the radial path.
                        double period = 2 * Math.PI / PeriodFast;
                        Projectile.Center = 
                            OrbitCenter 
                            + new Vector2(
                                OrbitingRadius * (float)Math.Cos(period * (container.RotationTimer - TimerStart) + angle),
                                OrbitingRadius * (float)Math.Sin(period * (container.RotationTimer - TimerStart) + angle)
                            );
                    }
                }
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
            var container = GetOrbitContainer();
            // If count is 0, then the entity owner has been killed, and we shouldn't do anything else.
            if (container.OrbitingProjectileCount[ProjectileSlot] <= 0) return;

            container.OrbitingProjectileCount[ProjectileSlot]--;
            int ProjNumber = container.OrbitingProjectileCount[ProjectileSlot];
            // This reorders the Projectile array that has all the currently active projectiles.
            if (ProjNumber > 0 && ProjIndex != 0)
            {
                for (int i = ProjIndex; i < ProjNumber; i++)
                {
                    container.OrbitingProjectile[ProjectileSlot, i] = container.OrbitingProjectile[ProjectileSlot, i + 1];
                }
                container.OrbitingProjectile[ProjectileSlot, ProjNumber] = container.OrbitingProjectile[ProjectileSlot, 0];
            }


            for (int j = 0; j < ProjNumber; j++)
            {
                container.OrbitingProjectile[ProjectileSlot, j] = container.OrbitingProjectile[ProjectileSlot, j + 1];
            }

            // This assigns the projectile IDs
            for (int k = 0; k < ProjNumber; k++)
            {
                container.OrbitingProjectile[ProjectileSlot, k].localAI[0] = k;
            }

            // Here we update the positions.
            GenerateProjectilePositions(container);
        }

        // This is the function that generates the positions.
        public void GenerateProjectilePositions(OrbitEntityContainer container)
        {
            int projectileCount = container.OrbitingProjectileCount[ProjectileSlot];
            for (int i = 0; i < projectileCount; i++)
            {
                container.OrbitingProjectilePositions[ProjectileSlot, i] =
                    OrbitCenter
                    + new Vector2(OrbitingRadius * (float)Math.Cos(2 * Math.PI / Period * container.RotationTimer + (2 * Math.PI / projectileCount * i)),
                    OrbitingRadius * (float)Math.Sin(2 * Math.PI / Period * container.RotationTimer + (2 * Math.PI / projectileCount * i)));
            }
        }

        abstract public void Attack();
    }
}