﻿using Microsoft.Xna.Framework;
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

        public override bool CanDamage() => false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SandnadoHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dhood Arena");
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 16;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.alpha = 255;
        }
        public ref float AICounter => ref projectile.ai[0];
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
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
                    Vector2 SpawnRotation = projectile.Center + new Vector2(RADIUS, 0).RotatedBy(120 * i);

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

                    NPC.NewNPC((int)SpawnRotation.X, (int)SpawnRotation.Y, NPCType, 0, projectile.whoAmI, 0f, 120 * i);
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
                if (projectile.ai[1]++ % 10 == 0 && BarrierCounter < 8)
                {
                    int RADIUS = 400;
                    float Rotation = BarrierCounter * MathHelper.PiOver4;

                    Vector2 SpawnPosition = projectile.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation);
                    int BarrierNPC = NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Barrier>(), 0, projectile.Center.X, projectile.Center.Y, Rotation, RADIUS);

                    // Goes from 1 - 8, add 1 to offset the counter that starts at 0
                    ((Barrier)Main.npc[BarrierNPC].modNPC).BarrierID = BarrierCounter + 1;

                    if (BarrierCounter == 4)
                    {
                        RADIUS = 275;
                        SpawnPosition = projectile.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation);
                        NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Spin>(), 0, projectile.Center.X, projectile.Center.Y, Rotation, RADIUS);
                        NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Spin>(), 0, projectile.Center.X, projectile.Center.Y, 2 * Rotation, RADIUS);
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
                        ((Barrier)npc.modNPC).Rotate = true;
                    }

                    if (npc.active && npc.type == ModContent.NPCType<Spin>())
                    {
                        ((Spin)npc.modNPC).Rotate = true;
                    }
                }

                NPC.NewNPC((int)projectile.Center.X, (int)projectile.Center.Y, ModContent.NPCType<SandstormBoss>(), 0, -1f, 0f, 0f, 0f, 255);

                Vector2 origin = new Vector2((int)projectile.Center.X, (int)(projectile.Center.Y));
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
                    if (player.active && projectile.Distance(player.Center) > 600 && projectile.Distance(player.Center) < 1050 && player.immuneTime == 0)
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
                    projectile.timeLeft = 5;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.Reload(BlendState.Additive);
            Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "MagicCircle_2");
            Color color = new Color(244, 188, 91);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Lerp(Color.Transparent, color, Utils.Clamp(projectile.localAI[0], 0, 240) / 240f), MathHelper.ToRadians(projectile.localAI[0]), texture.Size() / 2, 0.65f, SpriteEffects.None, 0f);

            texture = ModContent.GetTexture(AssetDirectory.Textures + "magic_02");
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            float scale = MathHelper.Lerp(0, 1.25f, Utils.Clamp(projectile.localAI[0], 0, 240) / 240f);
            color = new Color(186, 99, 45);
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color, MathHelper.ToRadians(projectile.localAI[0]), origin, scale, SpriteEffects.None, 0f);


            if (AICounter > 460)
            {
                texture = ModContent.GetTexture(AssetDirectory.Textures + "PulseCircle");
                color = Color.Orange;
                float circleScale = MathHelper.Lerp(0, 2.5f, Utils.Clamp(AICounter - 360, 0, 100) / 100f);

                if (!Main.gamePaused) projectile.localAI[0]++;

                float alpha = MathHelper.Lerp(0, 0.65f, (float)Math.Sin(projectile.localAI[0] / 15f));
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color * alpha, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, circleScale, SpriteEffects.None, 0f);
            }

            spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(spriteBatch, lightColor);
        }

        public override void Kill(int timeLeft)
        {
            //DisablePlatforms(false);
        }

        public void DisablePlatforms(bool inActive = true)
        {
            int RADIUS = 300;
            int xCoord = (int)(projectile.Center.X / 16);
            int yCoord = (int)(projectile.Center.Y / 16);

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
                    if (tile.active() && TileID.Sets.Platforms[tile.type])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);

                        // inActive is actuated + active
                        // nActive is not actuated + active
                        tile.inActive(inActive);
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws bottom left quadrant
                for (int i = 0; i < RADIUS - j; i++)
                {
                    xAxis--;

                    Tile tile = Framing.GetTileSafely(xAxis, yAxis);
                    if (tile.active() && TileID.Sets.Platforms[tile.type])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);
                        tile.inActive(inActive);
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
                    if (tile.active() && TileID.Sets.Platforms[tile.type])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);
                        tile.inActive(inActive);
                    }
                }

                // Reset the xAxis, offset by 1 to fill in the gap
                xAxis = xCoord + 1;

                // Draws top left quadrant
                for (int i = 0; i < RADIUS - j; i++)
                {
                    xAxis--;

                    Tile tile = Framing.GetTileSafely(xAxis, yAxis);
                    if (tile.active() && TileID.Sets.Platforms[tile.type])
                    {
                        Wiring.ActuateForced(xAxis, yAxis);
                        tile.inActive(inActive);
                    }
                }
            }
        }
    }
}