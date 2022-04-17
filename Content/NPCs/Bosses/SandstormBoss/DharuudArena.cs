using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class DharuudArena : ModProjectile
    {
        int BarrierCounter = 0;

        public override bool? CanDamage() => false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SandnadoHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dhood Arena");
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.alpha = 255;
        }
        public ref float AICounter => ref Projectile.ai[0];

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            if (AICounter++ == 0)
            {
                if (!Sandstorm.Happening)
                {
                    Sandstorm.Happening = true;
                    Sandstorm.TimeLeft = (int)(3600.0 * (8.0 + (double)Main.rand.NextFloat() * 16.0));
                    ModUtils.SandstormStuff();
                }

                for (int i = 0; i < 3; i++)
                {
                    int RADIUS = 60;
                    Vector2 SpawnRotation = Projectile.Center + new Vector2(RADIUS, 0).RotatedBy(120 * i);

                    int NPCType = -1;
                    switch (i)
                    {
                        case 0:
                            NPCType = ModContent.NPCType<LaserMinion>();
                            break;
                        case 1:
                            NPCType = ModContent.NPCType<BeamMinion>();
                            break;
                        case 2:
                            NPCType = ModContent.NPCType<BlasterMinion>();
                            break;
                    }

                    NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)SpawnRotation.X, (int)SpawnRotation.Y, NPCType, 0, Projectile.whoAmI, 0f, 120 * i);
                }

                //int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.SandnadoFriendly, 0, 0f, projectile.owner);
                //projectile.position = Main.projectile[proj].Center;
                //projectile.position -= new Vector2(Main.projectile[proj].width, Main.projectile[proj].height);
                //Main.projectile[proj].timeLeft = 600;
                //Main.projectile[proj].friendly = false;
                //Main.projectile[proj].hostile = false;
            }

            if (AICounter > 120)
            {
                if (Projectile.ai[1]++ % 10 == 0 && BarrierCounter < 8)
                {
                    int RADIUS = 400;
                    float Rotation = BarrierCounter * MathHelper.PiOver4;

                    Vector2 SpawnPosition = Projectile.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation);
                    int BarrierNPC = NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Barrier>(), 0, Projectile.Center.X, Projectile.Center.Y, Rotation, RADIUS);

                    // Goes from 1 - 8, add 1 to offset the counter that starts at 0
                    ((Barrier)Main.npc[BarrierNPC].ModNPC).BarrierID = BarrierCounter + 1;

                    if (BarrierCounter == 4)
                    {
                        RADIUS = 275;
                        SpawnPosition = Projectile.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation);
                        NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Spin>(), 0, Projectile.Center.X, Projectile.Center.Y, Rotation, RADIUS);
                        NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Spin>(), 0, Projectile.Center.X, Projectile.Center.Y, 2 * Rotation, RADIUS);
                    }

                    BarrierCounter++;
                }
            }

            if (AICounter == 360)
            {
                //Player player = Main.player[projectile.owner];
                //player.GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(projectile.Center, 90, 120f, 60f);
                //player.GetModPlayer<OvermorrowModPlayer>().TitleID = 1;
                //player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && npc.type == ModContent.NPCType<Barrier>())
                    {
                        ((Barrier)npc.ModNPC).Rotate = true;
                    }

                    if (npc.active && npc.type == ModContent.NPCType<Spin>())
                    {
                        ((Spin)npc.ModNPC).Rotate = true;
                    }
                }

                NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<SandstormBoss>(), 0, -1f, 0f, 0f, 0f, 255);

                Vector2 origin = new Vector2((int)Projectile.Center.X, (int)(Projectile.Center.Y));
                float radius = 100;
                int numLocations = 200;
                for (int i = 0; i < 200; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.Sand, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                    Main.dust[dust].noGravity = true;
                }
            }

            if (AICounter > 360)
            {
                if (AICounter == 460)
                {
                    //DisablePlatforms();

                    /*for (int i = 0; i < 4; i++)
                    {
                        int RADIUS = 300;
                        float Rotation = i * MathHelper.PiOver2;

                        Vector2 SpawnPosition = projectile.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation);
                        NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<SandPlatform>(), 0, projectile.Center.X, projectile.Center.Y, Rotation, RADIUS);
                    }

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && npc.type == ModContent.NPCType<SandPlatform>())
                        {
                            ((SandPlatform)npc.modNPC).Rotate = true;
                        }
                    }*/
                }

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && Projectile.Distance(player.Center) > 600 && Projectile.Distance(player.Center) < 1050 && player.immuneTime == 0)
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was torn apart by the desert winds."), 50, 0);
                    }
                }
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == ModContent.NPCType<SandstormBoss>())
                {
                    Projectile.timeLeft = 5;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "MagicCircle_2").Value;
            Color color = new Color(244, 188, 91);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, color * 0.65f, Utils.Clamp(Projectile.localAI[0], 0, 240) / 240f), MathHelper.ToRadians(Projectile.localAI[0]), texture.Size() / 2, 0.65f, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "magic_02").Value;
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            float scale = MathHelper.Lerp(0, 1.25f, Utils.Clamp(Projectile.localAI[0], 0, 240) / 240f);
            color = new Color(186, 99, 45);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * 0.65f, MathHelper.ToRadians(Projectile.localAI[0]), origin, scale, SpriteEffects.None, 0);


            if (AICounter > 460)
            {
                texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
                color = Color.Orange;
                float circleScale = MathHelper.Lerp(0, 2.5f, Utils.Clamp(AICounter - 360, 0, 100) / 100f);

                if (!Main.gamePaused) Projectile.localAI[0]++;

                float alpha = MathHelper.Lerp(0, 0.65f, (float)Math.Sin(Projectile.localAI[0] / 15f));
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, circleScale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(ref lightColor);
        }

        public override void Kill(int timeLeft)
        {
            //DisablePlatforms(false);
        }

        public void DisablePlatforms(bool inActive = true)
        {
            int RADIUS = 300;
            int xCoord = (int)(Projectile.Center.X / 16);
            int yCoord = (int)(Projectile.Center.Y / 16);

            int xAxis = xCoord;
            int yAxis = yCoord;


            for (int j = 0; j < RADIUS; j++)
            {
                yAxis++;
                xAxis = xCoord;

                // Draws bottom right quadrant
                for (int i = 0; i < RADIUS - j; i++)
                {
                    xAxis++;

                    Tile tile = Framing.GetTileSafely(xAxis, yAxis);
                    if (tile.HasTile && TileID.Sets.Platforms[tile.TileType])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);

                        // inActive is actuated + active
                        // nActive is not actuated + active
                        tile.IsActuated = inActive;
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws bottom left quadrant
                for (int i = 0; i < RADIUS - j; i++)
                {
                    xAxis--;

                    Tile tile = Framing.GetTileSafely(xAxis, yAxis);
                    if (tile.HasTile && TileID.Sets.Platforms[tile.TileType])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);
                        tile.IsActuated = inActive;
                    }
                }
            }


            // Reset the y axis, offset by 1 to fill in the gap
            yAxis = yCoord + 1;

            for (int j = 0; j < RADIUS; j++)
            {
                yAxis--;
                xAxis = xCoord;

                // Draws top right quadrant
                for (int i = 0; i < RADIUS - j; i++)
                {
                    xAxis++;

                    Tile tile = Framing.GetTileSafely(xAxis, yAxis);
                    if (tile.HasTile && TileID.Sets.Platforms[tile.TileType])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);
                        tile.IsActuated = inActive;
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws top left quadrant
                for (int i = 0; i < RADIUS - j; i++)
                {
                    xAxis--;

                    Tile tile = Framing.GetTileSafely(xAxis, yAxis);
                    if (tile.HasTile && TileID.Sets.Platforms[tile.TileType])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);
                        tile.IsActuated = inActive;
                    }
                }
            }
        }
    }
}
