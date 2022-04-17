using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.GameContent;
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
            Projectile.width = 20;
            Projectile.friendly = false;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
            Color1 = Color.LightYellow;
            Color2 = Color.Gold;
        }

        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 2000f);
            float sway = 40f;
            float divider = 16f;
            Positions = CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width, sway, divider);

            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
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
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Energy");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.timeLeft = 270;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                StartPosition = Projectile.Center;
                EndPosition = new Vector2(Projectile.ai[0], Projectile.ai[1]);

                MidPoint1 = StartPosition + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                MidPoint2 = MidPoint1 + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));

                Projectile.ai[0] = 0;
                Projectile.ai[1] = 0;

                RandomStart = Main.rand.Next(8, 11) * 10;
                RandomDelay = Main.rand.Next(6, 13) * 5;
                RunOnce = false;
            }

            Vector2 RandomDirection = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 2;
            Particle.CreateParticle(Particle.ParticleType<Orb>(), Projectile.Center, RandomDirection, Color.Orange, 1, Main.rand.NextFloat(0.25f, 0.4f), 0, 25);


            // Moves to the position
            if (Projectile.timeLeft > 110)
            {
                Projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint2, MidPoint1, Utils.Clamp(Projectile.ai[0]++, 0, 160) / 160f);
            }

            // Bounces up and down
            if (Projectile.timeLeft <= RandomStart && Projectile.timeLeft > RandomDelay)
            {
                float time = RandomStart - RandomDelay;

                if (Projectile.timeLeft == RandomStart)
                {
                    Projectile.ai[0] = 0;

                    StartPosition = Projectile.Center;
                    EndPosition = Projectile.Center;

                    MidPoint1 = StartPosition + new Vector2(0, -100);
                }

                Projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint1, MidPoint1, Utils.Clamp(Projectile.ai[0]++, 0, time) / time);
            }

            if (Projectile.timeLeft == RandomDelay)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<AncientElectricitiy>(), 20, 5f, Main.myPlayer);

                Vector2 end = Projectile.Center + (Vector2.UnitY * TRay.CastLength(Projectile.Center, Vector2.UnitY, 5000));
                NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)(end.X), (int)(end.Y), ModContent.NPCType<Pillar>());
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner").Value;
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = projectile.scale * 2 * mult;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    public class PillarShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Bolt");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 420;
            Projectile.scale = 0;
        }

        public override void AI()
        {
            if (Projectile.scale < 1f)
            {
                Projectile.scale += 0.01f;
            }

            // When the thing launches into the air
            if (Projectile.ai[0]++ == 270)
            {
                for (int i = 0; i < 18; i++)
                {
                    Vector2 Velocity = Vector2.One.RotatedBy(MathHelper.ToRadians(360 / 18 * i)) * 4;
                    Particle.CreateParticle(Particle.ParticleType<Spark>(), Projectile.Center, Velocity, Color.Yellow, 0.5f, 1.25f, 0, 1);
                }

                Projectile.velocity = -Vector2.UnitY * 6;
            }

            if (Main.rand.NextBool(3))
            {
                for (int _ = 0; _ < 3; _++)
                {
                    Vector2 RandomPosition = Projectile.Center + new Vector2(Main.rand.Next(125, 200), 0).RotatedByRandom(MathHelper.TwoPi);
                    Vector2 Direction = Vector2.Normalize(Projectile.Center - RandomPosition);

                    int DustSpeed = 8;

                    Particle.CreateParticle(Particle.ParticleType<Orb>(), RandomPosition, Direction * DustSpeed, Color.Orange, 1, Main.rand.NextFloat(0.1f, 0.22f), 0, 25);
                }
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (Projectile.ai[0] <= 270)
            {
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/PillarSpawner").Value;
                float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
                float scale = Projectile.scale * 2 * mult;

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0);
            }
            else
            {
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/NPCs/Bosses/SandstormBoss/Fragment").Value;

                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), new Vector2(texture.Width, texture.Height) / 2, 1f, SpriteEffects.None, 0);
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
            Main.npcFrameCount[NPC.type] = 19;
        }

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 100;
            NPC.lifeMax = 360;
            NPC.defense = 20;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.chaseable = false;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            base.AI();

            if (NPC.ai[0]++ == 0)
            {
                NPC.spriteDirection = Main.rand.NextBool() ? 1 : -1;
            }

            if (NPC.ai[0] == 60)
            {
                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PillarShot>(), 40, 0f, Main.myPlayer);
            }
        }

        private bool PillarLoop = false;
        public override void FindFrame(int frameHeight)
        {
            // Loop through the first 10 animation frames, spending 5 ticks on each.
            if (!PillarLoop)
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y >= frameHeight * 10)
                    {
                        PillarLoop = true;
                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y >= frameHeight * 19)
                    {
                        PillarLoop = true;
                        NPC.frame.Y = frameHeight * 10;
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            int frameHeight = TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type];
            int frame = frameHeight * (NPC.frame.Y / frameHeight);

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/SandstormBoss/Pillar_Glow").Value;
            Rectangle drawRectangle = new Rectangle(0, frame, TextureAssets.Npc[NPC.type].Value.Width, frameHeight);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    NPC.position.X - screenPos.X + NPC.width * 0.5f,
                    NPC.position.Y - screenPos.Y + NPC.height - drawRectangle.Height * 0.5f + 4
                ),
                drawRectangle,
                Color.White,
                NPC.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                NPC.scale,
                NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0f
            );
        }
    }
}