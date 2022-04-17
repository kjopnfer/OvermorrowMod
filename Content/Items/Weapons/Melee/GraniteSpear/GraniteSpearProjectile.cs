using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Items.Weapons.Magic.GraniteBook;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.GraniteSpear
{
    public class GraniteSpearProjectile : ModProjectile
    {
        private bool spawnedProjectiles = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Pike");
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 19;
            Projectile.penetrate = -1;
            //Projectile.scale = 1.3f;
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
                    int proj = Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Projectile.velocity * 4, ModContent.ProjectileType<GraniteSpike>(), Projectile.damage, 2f, Projectile.owner);
                    Main.projectile[proj].DamageType = DamageClass.Melee;

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

            // These dusts are added later, for the 'ExampleMod' effect
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.UnusedWhiteBluePurple,
                    Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
                dust.velocity += Projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
            }

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.UnusedWhiteBluePurple,
                    0, 0, 254, Scale: 0.3f);
                dust.velocity += Projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
            }

            //spawns the circle of electricity 

            if (!spawnedProjectiles)
            {
                //if (Main.netMode != NetmodeID.MultiplayerClient)
                //{
                for (int i = 0; i < 3; i++)
                {
                    // AI[0] is the ID of the parent Projectile, AI[1] is the degree of the initial position in a circle 
                    Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<GraniteSpearElectricity>(), Projectile.damage / 2, 1, Projectile.owner, Projectile.whoAmI, 120f * i);
                }
                //}
                spawnedProjectiles = true;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].AddBuff(ModContent.BuffType<GraniteSpearBuff>(), 720);
        }

    }
}