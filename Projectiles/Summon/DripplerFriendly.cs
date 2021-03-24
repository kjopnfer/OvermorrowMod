using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs.Summon;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class DripplerFriendly : ModProjectile
    {
        private int randDelay;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Looming Drippler");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 30;
            //projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.ignoreWater = true;
        }



        public override void AI()
        {
            // Loop through the 8 animation frames, spending 5 ticks on each.
            if (++projectile.frameCounter >= 8)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            //Making player variable "p" set as the projectile's owner
            Player player = Main.player[projectile.owner];

            //Factors for calculations
            double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
            double rad = deg * (Math.PI / 180); //Convert degrees to radians
            double dist = 72; //Distance away from the player

            /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
			/distance for the desired distance away from the player minus the projectile's width   /
			/and height divided by two so the center of the projectile is at the right place.     */
            projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            //Increase the counter/angle in degrees by 2.5 point, you can change the rate here too, but the orbit may look choppy depending on the value
            projectile.ai[1] += 2.5f;

            if (player.HasBuff(ModContent.BuffType<DripplerBuff>()))
            {
                projectile.timeLeft = 2;
            }

            // Starting search distance
            float distanceFromTarget = 2000f; // range of 700f
            Vector2 targetCenter = projectile.position;
            NPC targetNPC = null;
            bool foundTarget = false;

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, projectile.Center); // distance between the npc and minion
                        bool closest = Vector2.Distance(projectile.Center, targetCenter) > between; // targetcenter = npc center
                        bool inRange = between < distanceFromTarget; // if the distance between npc and minion is less than 700
                        bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 2000f;
                        if (((closest && inRange) || !foundTarget) && ((lineOfSight) || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            targetNPC = npc;
                            foundTarget = true;
                        }
                    }
                }
            }

            if (foundTarget)
            {
                if ((targetCenter - projectile.Center).X > 0f)
                {
                    projectile.spriteDirection = projectile.direction = -1;
                }
                else if ((targetCenter - projectile.Center).X < 0f)
                {
                    projectile.spriteDirection = projectile.direction = 1;
                }

                Vector2 delta = targetCenter - projectile.Center;
                float magnitude = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
                if (magnitude > 0)
                {
                    delta *= 5f / magnitude;
                }
                else
                {
                    delta = new Vector2(0f, 5f);
                }

                if (projectile.ai[0] == 1)
                {
                    randDelay = Main.rand.Next(150, 225);
                    projectile.netUpdate = true;
                }

                bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetNPC.position, targetNPC.width, targetNPC.height);

                if (projectile.ai[0] % randDelay == 0 && lineOfSight) // prevent from instantly shooting when spawned
                {
                    Main.PlaySound(SoundID.NPCHit19, (int)projectile.position.X, (int)projectile.position.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(projectile.Center, delta * 2, ModContent.ProjectileType<BloodyBallFriendly>(), projectile.damage, 0f, projectile.owner);
                        projectile.netUpdate = true;
                    }
                }
                projectile.ai[0]++;
            }
        }


        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Texture2D texture = mod.GetTexture("Projectiles/Summon/StormWhelp_Glowmask");

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;

            Texture2D texture = mod.GetTexture("Projectiles/Summon/DripplerFriendly_Glowmask");
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f + 2f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - drawRectangle.Height * 0.5f + 14f
                ),
                drawRectangle,
                Color.White,
                projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}