using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class AncientElectricitiy : GoldLightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Energy");
        }
        public override void SafeSetDefaults()
        {
            projectile.width = 10;
            projectile.friendly = false;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }

        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2000f);
            Positions = Lightning.CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
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

                RunOnce = false;
            }

            if (projectile.timeLeft > 90)
            {
                projectile.Center = ModUtils.Bezier(StartPosition, EndPosition, MidPoint2, MidPoint1, Utils.Clamp(projectile.ai[0]++, 0, 180) / 180f);
            }

            if (projectile.timeLeft == 60)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY, ModContent.ProjectileType<AncientElectricitiy>(), 20, 5f, Main.myPlayer);

                Vector2 end = projectile.Center + (Vector2.UnitY * TRay.CastLength(projectile.Center, Vector2.UnitY, 5000));
                NPC.NewNPC((int)(end.X), (int)(end.Y), ModContent.NPCType<Pillar>());

                /*Vector2 ProjectilePosition = new Vector2(projectile.Center.X / 16, projectile.Center.Y / 16);
                Tile tile = Framing.GetTileSafely((int)ProjectilePosition.X, (int)ProjectilePosition.Y);

                // Get the ground beneath the target
                while (!tile.active() || tile.type == TileID.Trees || tile.collisionType != 1)
                {
                    ProjectilePosition.Y += 1;
                    tile = Framing.GetTileSafely((int)ProjectilePosition.X, (int)ProjectilePosition.Y);
                }

                NPC.NewNPC((int)(ProjectilePosition.X * 16), (int)(ProjectilePosition.Y * 16), ModContent.NPCType<Pillar>());*/
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/SandstormBoss/PillarSpawner");
            //float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
            //float scale = projectile.scale * 2 * mult;

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Gold, 0, new Vector2(texture.Width, texture.Height) / 2, 1, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, 0, new Vector2(texture.Width, texture.Height) / 2, 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class Pillar : ModNPC
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
            if (npc.ai[0]++ == 0)
            {
                npc.spriteDirection = Main.rand.NextBool() ? 1 : -1;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    Rectangle PlayerBottom = new Rectangle((int)player.position.X, (int)player.position.Y + player.height, player.width, 1);
                    Rectangle PlayerLeft = new Rectangle((int)player.position.X, (int)player.position.Y, 1, player.height);
                    Rectangle PlayerRight = new Rectangle((int)player.position.X + player.width, (int)player.position.Y, 1, player.height);

                    Rectangle NPCTop = new Rectangle((int)npc.position.X, (int)npc.position.Y - (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(player.velocity.Y, 0));
                    Rectangle NPCRight = new Rectangle((int)npc.position.X + npc.width, (int)npc.position.Y, 8 + (int)Math.Max(player.velocity.X, 0), npc.height);
                    Rectangle NPCLeft = new Rectangle((int)npc.position.X, (int)npc.position.Y, 8 - (int)Math.Max(player.velocity.X, 0), npc.height);

                    if (PlayerBottom.Intersects(NPCTop))
                    {
                        if (player.position.Y <= npc.position.Y && !player.justJumped && player.velocity.Y >= 0)
                        {
                            player.gfxOffY = npc.gfxOffY;
                            player.position.Y = npc.position.Y - player.height + 4;
                            player.position += npc.velocity;
                            player.velocity.Y = 0;
                            player.fallStart = (int)(player.position.Y / 16f);
                        }
                    }

                    if (PlayerLeft.Intersects(NPCRight))
                    {
                        if (player.position.X >= npc.position.X + npc.width && player.velocity.X <= 0)
                        {
                            player.velocity.X = 0;
                            player.position.X = (npc.position.X + npc.width) + 4;
                        }
                    }

                    if (PlayerRight.Intersects(NPCLeft))
                    {
                        if (player.position.X <= npc.position.X && player.velocity.X >= 0)
                        {
                            player.velocity.X = 0;
                            player.position.X = npc.position.X - player.width;
                        }
                    }               
                }
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

            Texture2D texture = mod.GetTexture("NPCs/Bosses/SandstormBoss/Pillar_Glow");
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