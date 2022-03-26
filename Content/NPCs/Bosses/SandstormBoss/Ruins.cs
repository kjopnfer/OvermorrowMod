using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public abstract class Ruin : ModNPC
    {
        protected Vector2 InitialPosition;
        protected float InitialRotation;

        public bool CanFall = false;
        private bool Collided = false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => npc.ai[0] > 120;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Buried Ruin");
        }

        public override void SetDefaults()
        {
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            npc.friendly = false;
            npc.behindTiles = true;
            npc.alpha = 255;
        }

        public override void AI()
        {
            if (npc.ai[0] == 0)
            {
                InitialRotation = MathHelper.ToRadians(Main.rand.Next(0, 9) * 20);

                Tile tile = Framing.GetTileSafely((int)npc.Center.X / 16, (int)npc.Center.Y / 16);

                while (!tile.active() || tile.collisionType != 1)
                {
                    npc.position.Y += 1;
                    tile = Framing.GetTileSafely((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                }

                npc.position.Y += Main.rand.Next(4, 8) * 20;
            }

            /*if (!Collision.CanHit(npc.Center, npc.width, npc.height, npc.Center + Vector2.UnitY, 2, 2))
            {
                if (Main.rand.NextBool(3))
                {
                    Tile tile = Framing.GetTileSafely((int)(SpawnPosition.X + (Main.rand.Next(-2, 2) * 15)) / 16, (int)SpawnPosition.Y / 16);

                    while (!tile.active() || tile.collisionType != 1)
                    {
                        SpawnPosition.Y += 1;
                        tile = Framing.GetTileSafely((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                    }

                    Particle.CreateParticle(Particle.ParticleType<Smoke2>(), SpawnPosition * 16, Vector2.One.RotatedByRandom(MathHelper.TwoPi), new Color(182, 128, 70));
                }
            }*/

            if (!CanFall)
            {
                if (npc.ai[0]++ < 320)
                {
                    npc.alpha = 0;

                    npc.Center -= Vector2.UnitY * 2;
                    InitialPosition = npc.Center;
                }
                else
                {
                    npc.Center = Vector2.Lerp(InitialPosition, InitialPosition + Vector2.UnitY * 50, (float)Math.Sin(npc.localAI[0]++ / 120f));
                }

                npc.rotation = MathHelper.Lerp(InitialRotation, InitialRotation + MathHelper.PiOver4, (float)Math.Sin(npc.localAI[0] / 240));
            }
            else
            {
                // Boss sets the velocity of the NPC
                // The velocity is then stopped here if it detects a tile
                Tile tile = Framing.GetTileSafely(npc.Center + Vector2.UnitY * 25);
                if (tile.active() && tile.collisionType == 1)
                {
                    if (!Collided)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            Player player = Main.player[i];
                            if (player.active && npc.Distance(player.Center) < 2000)
                            {
                                player.GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                                player.GetModPlayer<OvermorrowModPlayer>().ShakeOffset = 5;
                            }
                        }

                        InitialPosition = npc.Center;
                    }

                    npc.noGravity = true;
                    npc.velocity.Y = 0;

                    Collided = true;
                }

                if (npc.ai[1] >= 240)
                {
                    npc.active = false;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (CanFall && !Collided)
            {
                float mult = (0.55f + (float)Math.Sin(Main.GlobalTime) * 0.1f);
                float scale = npc.scale * 2 * mult;
                Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "circle_05");
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0f);
            }

            // I tried to do this because I didn't want vanilla's stupid drawing shenanigans that forces the NPC to become black upon entering inside a tile
            // Unfortunately that didn't work out so this is just doing it manually I guess
            if (Collided)
            {
                Texture2D texture = Main.npcTexture[npc.type];
                Color color = Lighting.GetColor((int)InitialPosition.X / 16, (int)(InitialPosition.Y / 16f));


                Main.spriteBatch.Draw(texture, InitialPosition - Main.screenPosition, null, color, npc.rotation, new Vector2(texture.Width, texture.Height) / 2, npc.scale, SpriteEffects.None, 0f);

                if (npc.ai[1]++ > 120)
                {
                    npc.rotation += 0.004f;

                    InitialPosition.Y += 1;

                    if (Main.rand.NextBool(30))
                    {
                        Particle.CreateParticle(Particle.ParticleType<Smoke2>(), npc.Center + new Vector2(Main.rand.Next(-10, 10), 5), Vector2.One.RotatedByRandom(MathHelper.TwoPi), new Color(182, 128, 70), Main.rand.NextFloat(0.25f, 0.75f));
                    }
                }

                return false;
            }

            return base.PreDraw(spriteBatch, drawColor);
        }
    }

    public class Ruin1 : Ruin
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = 94;
            npc.height = 102;
            npc.timeLeft = 1200;
            npc.lifeMax = 200;
        }
    }

    public class Ruin2 : Ruin
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = 142;
            npc.height = 132;
            npc.timeLeft = 1200;
            npc.lifeMax = 400;
        }
    }

    public class Ruin3 : Ruin
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            npc.width = 60;
            npc.height = 60;
            npc.timeLeft = 1200;
            npc.lifeMax = 100;
        }
    }
}