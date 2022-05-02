using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.StormTalon
{
    public class StormTalonProjectile : ModProjectile
    {
        // Higher is slower
        private const int ProjectileSpawnRate = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Talon");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 19;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.alpha = 0;

            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        // In here the AI uses this example, to make the code more organized and readable
        // Also showcased in ExampleJavelinProjectile.cs
        public float movementFactor // Change this value to alter how fast the spear moves
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];

            // Here we set some of the Projectile's owner properties, such as held item and itemtime, along with Projectile direction and position Projectiled on the player
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            Projectile.direction = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            Projectile.position.X = ownerMountedCenter.X - (float)(Projectile.width / 2);
            Projectile.position.Y = ownerMountedCenter.Y - (float)(Projectile.height / 2);

            // As long as the player isn't frozen, the spear can move
            if (!projOwner.frozen)
            {
                if (movementFactor == 0f) // When initially thrown out, the ai0 will be 0f
                {
                    movementFactor = 3f; // Make sure the spear moves forward when initially thrown out
                    Projectile.netUpdate = true; // Make sure to netUpdate this spear
                }
                if (projOwner.itemAnimation < projOwner.itemAnimationMax / 3) // Somewhere along the item animation, make sure the spear moves back
                {
                    movementFactor -= 2.4f;
                }
                else // Otherwise, increase the movement factor
                {
                    movementFactor += 2.1f;
                }
            }



            Projectile.ai[1]++;

            // Change the spear position Projectiled off of the velocity and the movementFactor
            Projectile.position += Projectile.velocity * movementFactor;

            if (Projectile.ai[1] % ProjectileSpawnRate == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<StormTalonSparks>(), (Projectile.damage / 4) + 2, 1, Projectile.owner, 0, 0);
            }

            // When we reach the end of the animation, we can kill the spear Projectile
            if (projOwner.itemAnimation == 0)
            {
                Projectile.Kill();
            }
            // Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
            // MathHelper.ToRadians(xx degrees here)
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            // Offset by 90 degrees here
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation -= MathHelper.ToRadians(90f);
            }
        }
    }
}