using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Tiles.DesertTemple;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public abstract class Ruin : ModNPC
    {
        protected Vector2 InitialPosition;
        protected float InitialRotation;

        public bool CanFall = false;
        private bool Collided = false;
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => CanFall;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }

        public override void SetDefaults()
        {
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            npc.friendly = false;
            //npc.behindTiles = true;
            npc.dontTakeDamage = true;
            npc.alpha = 255;
        }

        public ref float AICounter => ref npc.ai[0];
        public ref float Offset => ref npc.ai[1];

        public override void DrawBehind(int index)
        {
            if (!CanFall)
            {
                npc.hide = true;
                Main.instance.DrawCacheNPCsMoonMoon.Add(index);
            }
            else
            {
                npc.hide = false;
            }
        }

        public override void AI()
        {
            if (AICounter == 0)
            {
                InitialRotation = MathHelper.ToRadians(Main.rand.Next(0, 9) * 20);

                Tile tile = Framing.GetTileSafely((int)npc.Center.X / 16, (int)npc.Center.Y / 16);

                while (!tile.active() || tile.collisionType != 1 || tile.type == TileID.Gold || tile.type == ModContent.TileType<SandBrick>())
                {
                    npc.position.Y += 1;
                    tile = Framing.GetTileSafely((int)npc.Center.X / 16, (int)npc.Center.Y / 16);
                }

                npc.position.Y += Main.rand.Next(4, 8) * 20;
                InitialPosition = npc.Center;
            }

            if (!CanFall)
            {
                if (AICounter++ <= 1280)
                {
                    npc.alpha = 0;

                    //npc.Center -= Vector2.UnitY * 5;

                    npc.Center = Vector2.Lerp(InitialPosition, new Vector2(InitialPosition.X, Desert.DesertArenaCenter.Y - Offset - 750), Utils.Clamp((float)AICounter++, 0, 1280) / 1280);

                    if (AICounter == 1280) InitialPosition = npc.Center;
                }
                else
                {
                    npc.Center = Vector2.Lerp(InitialPosition, InitialPosition + Vector2.UnitY * 50, (float)Math.Sin(npc.localAI[0]++ / 120f));
                }

                npc.rotation = MathHelper.Lerp(InitialRotation, InitialRotation + MathHelper.PiOver4, (float)Math.Sin(npc.localAI[0] / 240f));
            }
            else
            {
                // Boss sets the velocity of the NPC
                // The velocity is then stopped here if it detects a tile
                Tile tile = Framing.GetTileSafely(npc.Center + Vector2.UnitY * 25);
                if (tile.active() && tile.collisionType == 1 && 
                    (tile.type == TileID.Gold || tile.type == ModContent.TileType<SandBrick>() || tile.type == TileID.Sand))
                {
                    if (!Collided)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            Player player = Main.player[i];
                            if (player.active && npc.Distance(player.Center) < 2000)
                            {
                                player.Overmorrow().AddScreenShake(60, 10);
                            }
                        }

                        InitialPosition = npc.Center;
                    }

                    npc.noGravity = true;
                    npc.velocity.Y = 0;

                    Collided = true;
                }

                if (Collided)
                {
                    for (int i = 0; i < Main.rand.Next(7, 10); i++)
                    {
                        Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-10, 10), 5);
                        Vector2 RandomVelocity = -Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.Next(1, 3);
                        Particle.CreateParticle(Particle.ParticleType<Smoke2>(), RandomPosition, RandomVelocity, new Color(182, 128, 70), Main.rand.NextFloat(0.15f, 0.35f), 1, 0, 0, Main.rand.Next(90, 120));
                    }

                    npc.life = 0;
                    npc.active = false;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            /*if (CanFall && !Collided)
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
            }*/

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
            npc.damage = 85;
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
            npc.damage = 160;
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
            npc.damage = 50;
        }
    }
}