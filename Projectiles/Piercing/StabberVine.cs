using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class StabberVine : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Piercing/VinePiercerProjectile";
        private bool hasLaunched = false;
        private bool canRetract = false;
        private Vector2 retractPosition;
        private Vector2 idlePosition;
        private int launchCounter;
        private bool retractIdle = false;
        private float retractCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stabber Vine");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hasLaunched);
            writer.Write(canRetract);
            Utils.WriteVector2(writer, retractPosition);
            Utils.WriteVector2(writer, idlePosition);
            writer.Write(launchCounter);
            writer.Write(retractIdle);
            writer.Write(retractCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hasLaunched = reader.ReadBoolean();
            canRetract = reader.ReadBoolean();
            retractPosition = Utils.ReadVector2(reader);
            idlePosition = Utils.ReadVector2(reader);
            launchCounter = reader.ReadInt32();
            retractIdle = reader.ReadBoolean();
            retractCounter = reader.ReadInt32();
        }

        public override void AI()
        {
            // Fetch projectile owner
            var player = Main.player[projectile.owner];

            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<VineBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<VineBuff>()))
            {
                projectile.timeLeft = 2;
            }

            // Various position stuff
            if (projectile.ai[1] == -1)
            {
                idlePosition = player.Center - new Vector2(-45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
            }
            else if (projectile.ai[1] == 1)
            {
                idlePosition = player.Center - new Vector2(45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
            }
            else
            {
                idlePosition = player.Center - new Vector2(0, MathHelper.Lerp(75, 85, (float)Math.Sin(projectile.ai[0] / 60f)));
            }



            if (projectile.ai[0] < 80)
            {
                projectile.alpha -= 4;
            }

            projectile.ai[0]++;


            Vector2 targetPos = projectile.position;
            float targetDist = 320f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                NPC npc = Main.npc[k];
                if (npc.CanBeChasedBy(this))
                {
                    float distance = Vector2.Distance(npc.Center, projectile.Center);
                    if ((distance < targetDist && !target) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        targetDist = distance;
                        targetPos = npc.Center;
                        if (!retractIdle)
                        {
                            target = true;
                        }
                    }
                }
            }

            // Attack behavior
            if (target)
            {
                // Angle towards the target
                if (!hasLaunched)
                {
                    projectile.rotation = (projectile.Center - targetPos).ToRotation() - MathHelper.PiOver2;
                }

                // Hasn't launched, shoot at target
                if (!hasLaunched && projectile.ai[0] % 180 == 0)
                {
                    Vector2 shootVelocity = targetPos - projectile.Center;
                    shootVelocity.Normalize();
                    projectile.velocity = shootVelocity * Main.rand.Next(16, 22);
                    launchCounter = 10;
                    hasLaunched = true;
                }

                // Time spent moving
                if (launchCounter > 0)
                {
                    launchCounter--;
                }
                else
                {
                    // Not retracting and has been launched
                    if (!canRetract && hasLaunched)
                    {
                        projectile.velocity = Vector2.Zero;
                        retractPosition = projectile.Center;
                        canRetract = true;
                    }
                    else
                    {
                        // Not retracting and hasn't been launched
                        // Idle animation
                        if (projectile.ai[1] == -1)
                        {
                            projectile.Center = player.Center - new Vector2(-45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
                        }
                        else if (projectile.ai[1] == 1)
                        {
                            projectile.Center = player.Center - new Vector2(45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
                        }
                        else
                        {
                            projectile.Center = player.Center - new Vector2(0, MathHelper.Lerp(75, 85, (float)Math.Sin(projectile.ai[0] / 60f)));
                        }
                    }
                }

                if (canRetract)
                {
                    retractCounter += 0.025f;
                    projectile.position = Vector2.Lerp(retractPosition, idlePosition, retractCounter);

                    if (retractCounter >= 1)
                    {
                        retractCounter = 0;
                        canRetract = false;
                        hasLaunched = false;
                    }
                }
            }
            else
            {
                if (hasLaunched)
                {
                    if (!retractIdle)
                    {
                        canRetract = false;
                        retractIdle = true;
                        projectile.velocity = Vector2.Zero;
                        retractPosition = projectile.Center;
                    }

                    if (retractIdle)
                    {
                        retractCounter += 0.025f;
                        projectile.position = Vector2.Lerp(retractPosition, idlePosition, retractCounter);

                        if (retractCounter >= 1)
                        {
                            retractCounter = 0;
                            retractIdle = false;
                            hasLaunched = false;
                        }
                    }
                }
                else
                {
                    // No target, idle animation

                    Vector2 vector221 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                    float num494 = player.position.X + (player.width / 2) - vector221.X;
                    float num499 = player.position.Y + (player.height / 2) - vector221.Y;
                    projectile.rotation = (float)Math.Atan2(num499, num494) - 1.57f;

                    // Various position stuff
                    if (projectile.ai[1] == -1)
                    {
                        projectile.Center = player.Center - new Vector2(-45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
                    }
                    else if (projectile.ai[1] == 1)
                    {
                        projectile.Center = player.Center - new Vector2(45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
                    }
                    else
                    {
                        projectile.Center = player.Center - new Vector2(0, MathHelper.Lerp(75, 85, (float)Math.Sin(projectile.ai[0] / 60f)));
                    }

                    projectile.velocity = Vector2.Zero;
                }
            }
        }

        public override bool CanDamage()
        {
            if (hasLaunched)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = player.Center;
            Texture2D chainTexture = ModContent.GetTexture("OvermorrowMod/Projectiles/Piercing/VinePiercerChain");

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of the chain texture and the spacing between links
                drawPosition += remainingVectorToPlayer * 12 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, Color.Lerp(Color.Transparent, color, projectile.ai[0] < 64 ? projectile.ai[0] / 64 : 1), rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}