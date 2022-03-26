using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class AncientElectricitiy : Lightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Electricity");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 20;
            projectile.friendly = false;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
            Color1 = Color.LightYellow;
            Color2 = Color.Gold;
        }

        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2000f);
            float sway = 40f;
            float divider = 16f;
            Positions = CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width, sway, divider);

            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }

    public class PillarSpawner : ModProjectile
    {
        private bool RunOnce = true;

        private Vector2 StartPosition;
        private Vector2 EndPosition;
        private Vector2 MidPoint1;
        private Vector2 MidPoint2;

        private int RandomStart;
        private int RandomDelay;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.timeLeft = 270;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                StartPosition = projectile.Center;
                EndPosition = new Vector2(projectile.ai[0], projectile.ai[1]);

                MidPoint1 = StartPosition + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                MidPoint2 = MidPoint1 + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));

                projectile.ai[0] = 0;
                projectile.ai[1] = 0;

                RandomStart = Main.rand.Next(8, 11) * 10;
                RandomDelay = Main.rand.Next(6, 13) * 5;
                RunOnce = false;
            }

            Vector2 RandomDirection = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 2;
            Particle.CreateParticle(Particle.ParticleType<Orb>(), projectile.Center, RandomDirection, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);


            // Moves to the position
            if (projectile.timeLeft > 110)
            {
                projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint2, MidPoint1, Utils.Clamp(projectile.ai[0]++, 0, 160) / 160f);
            }

            // Bounces up and down
            if (projectile.timeLeft <= RandomStart && projectile.timeLeft > RandomDelay)
            {
                float time = RandomStart - RandomDelay;

                if (projectile.timeLeft == RandomStart)
                {
                    projectile.ai[0] = 0;

                    StartPosition = projectile.Center;
                    EndPosition = projectile.Center;

                    MidPoint1 = StartPosition + new Vector2(0, -100);
                }

                projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint1, MidPoint1, Utils.Clamp(projectile.ai[0]++, 0, time) / time);
            }

            if (projectile.timeLeft == RandomDelay)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY, ModContent.ProjectileType<AncientElectricitiy>(), 20, 5f, Main.myPlayer);

                Vector2 end = projectile.Center + (Vector2.UnitY * TRay.CastLength(projectile.Center, Vector2.UnitY, 5000));
                NPC.NewNPC((int)(end.X), (int)(end.Y), ModContent.NPCType<Pillar>());
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner");
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = projectile.scale * 2 * mult;

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class PillarShot : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 420;
            projectile.scale = 0;
        }

        public override void AI()
        {
            if (projectile.scale < 1f)
            {
                projectile.scale += 0.01f;
            }

            // When the thing launches into the air
            if (projectile.ai[0]++ == 270)
            {
                for (int i = 0; i < 18; i++)
                {
                    Vector2 Velocity = Vector2.One.RotatedBy(MathHelper.ToRadians(360 / 18 * i)) * 4;
                    Particle.CreateParticle(Particle.ParticleType<Spark>(), projectile.Center, Velocity, Color.Yellow, 0.5f, 1.25f, 0, 1);
                }

                projectile.velocity = -Vector2.UnitY * 6;
            }

            if (Main.rand.NextBool(3))
            {
                for (int _ = 0; _ < 3; _++)
                {
                    Vector2 RandomPosition = projectile.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                    Vector2 Direction = Vector2.Normalize(projectile.Center - RandomPosition);

                    int DustSpeed = 8;

                    Particle.CreateParticle(Particle.ParticleType<Orb>(), RandomPosition, Direction * DustSpeed, Color.Orange, 1, Main.rand.NextFloat(0.1f, 0.22f), 0, 25);
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (projectile.ai[0] <= 270)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner");
                float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
                float scale = projectile.scale * 2 * mult;

                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0f);
            }
            else
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment");

                Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, projectile.velocity.ToRotation(), new Vector2(texture.Width, texture.Height) / 2, 1f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class Pillar : CollideableNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Obelisk");
            Main.npcFrameCount[npc.type] = 19;
        }

        public override void SetDefaults()
        {
            npc.width = 42;
            npc.height = 100;
            npc.lifeMax = 360;
            npc.defense = 20;
            npc.aiStyle = -1;
            npc.knockBackResist = 0f;
            npc.chaseable = false;
            npc.noTileCollide = false;
        }

        public override void AI()
        {
            base.AI();

            if (npc.ai[0]++ == 0)
            {
                npc.spriteDirection = Main.rand.NextBool() ? 1 : -1;
            }

            if (npc.ai[0] == 60)
            {
                Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<PillarShot>(), 40, 0f, Main.myPlayer);
            }
        }

        private bool PillarLoop = false;
        public override void FindFrame(int frameHeight)
        {
            // Loop through the first 10 animation frames, spending 5 ticks on each.
            if (!PillarLoop)
            {
                if (++npc.frameCounter >= 5)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;

                    if (npc.frame.Y >= frameHeight * 10)
                    {
                        PillarLoop = true;
                        npc.netUpdate = true;
                    }
                }
            }
            else
            {
                if (++npc.frameCounter >= 5)
                {
                    npc.frameCounter = 0;
                    npc.frame.Y += frameHeight;

                    if (npc.frame.Y >= frameHeight * 19)
                    {
                        PillarLoop = true;
                        npc.frame.Y = frameHeight * 10;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int frameHeight = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int frame = frameHeight * (npc.frame.Y / frameHeight);

            Texture2D texture = mod.GetTexture("Content/NPCs/Bosses/SandstormBoss/Pillar_Glow");
            Rectangle drawRectangle = new Rectangle(0, frame, Main.npcTexture[npc.type].Width, frameHeight);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    npc.position.X - Main.screenPosition.X + npc.width * 0.5f,
                    npc.position.Y - Main.screenPosition.Y + npc.height - drawRectangle.Height * 0.5f + 4
                ),
                drawRectangle,
                Color.White,
                npc.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                npc.scale,
                npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }
    }
}