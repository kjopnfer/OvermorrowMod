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
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = 19;
            projectile.penetrate = -1;
            //projectile.scale = 1.3f;
            projectile.alpha = 0;

            projectile.hide = true;
            projectile.ownerHitCheck = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }

        // In here the AI uses this example, to make the code more organized and readable
        // Also showcased in ExampleJavelinProjectile.cs
        public float movementFactor // Change this value to alter how fast the spear moves
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player projOwner = Main.player[projectile.owner];

            // Here we set some of the projectile's owner properties, such as held item and itemtime, along with projectile direction and position projectiled on the player
            Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
            projectile.direction = projOwner.direction;
            projOwner.heldProj = projectile.whoAmI;
            projOwner.itemTime = projOwner.itemAnimation;
            projectile.position.X = ownerMountedCenter.X - (float)(projectile.width / 2);
            projectile.position.Y = ownerMountedCenter.Y - (float)(projectile.height / 2);

            // As long as the player isn't frozen, the spear can move
            if (!projOwner.frozen)
            {
                if (movementFactor == 0f) // When initially thrown out, the ai0 will be 0f
                {
                    int proj = Projectile.NewProjectile(projectile.Center, projectile.velocity * 4, ModContent.ProjectileType<GraniteSpike>(), projectile.damage, 2f, projectile.owner);
                    Main.projectile[proj].magic = false;
                    Main.projectile[proj].melee = true;

                    movementFactor = 3f; // Make sure the spear moves forward when initially thrown out
                    projectile.netUpdate = true; // Make sure to netUpdate this spear
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

            projectile.ai[1]++;

            // Change the spear position projectiled off of the velocity and the movementFactor
            projectile.position += projectile.velocity * movementFactor;

            // When we reach the end of the animation, we can kill the spear projectile
            if (projOwner.itemAnimation == 0)
            {
                projectile.Kill();
            }
            // Apply proper rotation, with an offset of 135 degrees due to the sprite's rotation, notice the usage of MathHelper, use this class!
            // MathHelper.ToRadians(xx degrees here)
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            // Offset by 90 degrees here
            if (projectile.spriteDirection == -1)
            {
                projectile.rotation -= MathHelper.ToRadians(90f);
            }

            // These dusts are added later, for the 'ExampleMod' effect
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.height, projectile.width, DustID.UnusedWhiteBluePurple,
                    projectile.velocity.X * .2f, projectile.velocity.Y * .2f, 200, Scale: 1.2f);
                dust.velocity += projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
            }

            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.height, projectile.width, DustID.UnusedWhiteBluePurple,
                    0, 0, 254, Scale: 0.3f);
                dust.velocity += projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
            }

            //spawns the circle of electricity 

            if (!spawnedProjectiles)
            {
                //if (Main.netMode != NetmodeID.MultiplayerClient)
                //{
                for (int i = 0; i < 3; i++)
                {
                    // AI[0] is the ID of the parent projectile, AI[1] is the degree of the initial position in a circle 
                    Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<GraniteSpearElectricity>(), projectile.damage / 2, 1, projectile.owner, projectile.whoAmI, 120f * i);
                }
                //}
                spawnedProjectiles = true;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].AddBuff(ModContent.BuffType<GraniteSpearBuff>(), 720);
        }

    }
}