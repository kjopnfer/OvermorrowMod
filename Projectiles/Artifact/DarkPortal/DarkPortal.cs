using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.WardenClass;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Artifact.DarkPortal
{
    public class DarkPortal : ArtifactProjectile
    {
        Vector2 spawnPosition;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Portal");
            Main.projFrames[projectile.type] = 14;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 100;
            projectile.height = 146;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft = 3600;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                spawnPosition = projectile.Center;
            }

            projectile.Center = spawnPosition - new Vector2(0, MathHelper.Lerp(0, 25, (float)Math.Sin(projectile.ai[0] / 100f)));

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }

            projectile.ai[0]++;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].friendly)
                {
                    float distance = Vector2.Distance(projectile.Center, Main.npc[i].Center);
                    if (distance <= 900 && projectile.ai[0] % 300 == 0)
                    {
                        Main.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, projectile.Center);
                        int proj = Projectile.NewProjectile(projectile.Center, Vector2.One.RotatedByRandom(Math.PI) * 2, ModContent.ProjectileType<DarkSerpent>(), 30, 2f, Main.myPlayer);
                        ((ArtifactProjectile)Main.projectile[proj].modProjectile).RuneID = RuneID;

                        return;
                    }
                }
            }


            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;

            Texture2D texture = mod.GetTexture("Projectiles/Artifact/DarkPortal/DarkPortal_Glow");
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - drawRectangle.Height * 0.5f
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
    }

    public class DarkSerpent : ArtifactProjectile
    {
        // TODO: Make the head draw on top of the body

        private bool spawnedSegment = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Serpent");
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!spawnedSegment)
            {
                // AI[1] keeps track of the segment number
                int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<DarkSerpent2>(), projectile.damage, 0f, Main.myPlayer, projectile.whoAmI, 1);
                ((ArtifactProjectile)Main.projectile[proj].modProjectile).RuneID = RuneID;
                spawnedSegment = true;
            }
            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }
            else
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 15;
                }
            }

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 900f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }

            if (target)
            {
                AdjustMagnitude(ref move);
                projectile.velocity += (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }

            if (projectile.velocity.X > 11)
            {
                projectile.velocity.X = 11;
            }
            if (projectile.velocity.X < -11)
            {
                projectile.velocity.X = -11;
            }

            if (projectile.velocity.Y > 11)
            {
                projectile.velocity.Y = 11;
            }
            if (projectile.velocity.Y < -11)
            {
                projectile.velocity.Y = -11;
            }

            projectile.velocity.Y = projectile.velocity.Y + 0.06f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Projectiles/Artifact/DarkPortal/DarkSerpent_Eyes");
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Main.spriteBatch.Draw(texture,
                //new Vector2(projectile.Center.X - Main.screenPosition.X, projectile.Center.Y - Main.screenPosition.Y - 42),
                //new Vector2(projectile.position.X - Main.screenPosition.X + (float)(projectile.width / 2) - (float)texture.Width * projectile.scale / 2f + halfSize.X * projectile.scale, projectile.position.Y - Main.screenPosition.Y + (float)projectile.height - (float)texture.Height * projectile.scale / (float)Main.projFrames[projectile.type] + 4f + halfSize.Y * projectile.scale + num37), 
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + 42 * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + 90 - drawRectangle.Height * 0.5f - 32
                ),
                null,
                Color.White,
                projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                //halfSize, 
                projectile.scale,
                SpriteEffects.None,
                0f);

            for (int num131 = 1; num131 < 10; num131++)
            {
                Main.spriteBatch.Draw(texture,
                    //new Vector2(projectile.position.X - Main.screenPosition.X + (float)(projectile.width / 2) - (float)texture.Width * projectile.scale / 2f + halfSize.X * projectile.scale, projectile.position.Y - Main.screenPosition.Y + (float)projectile.height - (float)texture.Height * projectile.scale / (float)Main.projFrames[projectile.type] + 4f + halfSize.Y * projectile.scale + num37) - projectile.velocity * num131 * 0.5f,
                    new Vector2
                    (
                        projectile.position.X - Main.screenPosition.X + 42 * 0.5f,
                        projectile.position.Y - Main.screenPosition.Y + 90 - drawRectangle.Height * 0.5f - 32
                    ) - projectile.velocity * num131 * 0.5f,
                    null,
                    new Color(110 - num131 * 10, 110 - num131 * 10, 110 - num131 * 10, 110 - num131 * 10),
                    projectile.rotation,
                                    new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                    projectile.scale,
                    SpriteEffects.None,
                    0f);
            }
        }
    }

    public class DarkSerpent2 : ArtifactProjectile
    {
        private bool spawnedSegment = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Serpent");
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.hide = true;
        }

        public override void AI()
        {
            if (!spawnedSegment)
            {
                // If we have 14 segments, spawn the tail
                if (projectile.ai[1] == 14)
                {
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<DarkSerpent3>(), projectile.damage, 0f, Main.myPlayer, projectile.whoAmI, projectile.ai[1]++);
                    ((ArtifactProjectile)Main.projectile[proj].modProjectile).RuneID = RuneID;
                }
                else
                {
                    // Increment segment
                    projectile.ai[1]++;

                    // AI[1] keeps track of the segment number
                    int proj = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<DarkSerpent2>(), projectile.damage, 0f, Main.myPlayer, projectile.whoAmI, projectile.ai[1]);
                    ((ArtifactProjectile)Main.projectile[proj].modProjectile).RuneID = RuneID;
                }

                spawnedSegment = true;
            }

            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && (projectile2.type == ModContent.ProjectileType<DarkSerpent>() || projectile2.type == ModContent.ProjectileType<DarkSerpent2>()))
            {
                // set rotation to the parent segment
                projectile.rotation = projectile.DirectionTo(projectile2.Center).ToRotation() + MathHelper.ToRadians(90f);
                // check if distance is over segment size (ps: adjust height to right value)
                // direction from parent to me
                Vector2 dir = projectile2.DirectionTo(projectile.Center);
                // position where the distance between parent and me is exactly the segment length
                projectile.Center = projectile2.Center + new Vector2(dir.X * projectile2.height, dir.Y * projectile2.width);

                // Fade in once the parent has faded in
                if (projectile2.timeLeft <= 60)
                {
                    projectile.timeLeft = projectile2.timeLeft;
                    projectile.alpha = projectile2.alpha;
                }
                else
                {
                    if (projectile2.alpha <= 0)
                    {
                        if (projectile.alpha > 0)
                        {
                            projectile.alpha -= 35;
                        }
                    }
                }
            }
            else
            {
                // kil
                projectile.Kill();
            }
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }
    }

    public class DarkSerpent3 : ArtifactProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Serpent");
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile projectile2 = Main.projectile[(int)projectile.ai[0]];
            if (projectile2.active && projectile2.type == ModContent.ProjectileType<DarkSerpent2>())
            {
                // set rotation to the parent segment
                projectile.rotation = projectile.DirectionTo(projectile2.Center).ToRotation() + MathHelper.ToRadians(90f);
                // check if distance is over segment size (ps: adjust height to right value)
                // direction from parent to me
                Vector2 dir = projectile2.DirectionTo(projectile.Center);
                // position where the distance between parent and me is exactly the segment length
                projectile.Center = projectile2.Center + new Vector2(dir.X * projectile2.height, dir.Y * projectile2.width);

                // Fade out when the parent starts fading out
                if (projectile2.timeLeft <= 60)
                {
                    projectile.timeLeft = projectile2.timeLeft;
                    projectile.alpha = projectile2.alpha;
                }
                else
                {
                    // Fade in once the parent has faded in
                    if (projectile2.alpha <= 0)
                    {
                        if (projectile.alpha > 0)
                        {
                            projectile.alpha -= 35;
                        }
                    }
                }

            }
            else
            {
                // kil
                projectile.Kill();
            }
        }
    }
}