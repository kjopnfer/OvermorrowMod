using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class DharuudArena : ModProjectile
    {
        int BarrierCounter = 0;

        public override bool CanDamage() => false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.SandnadoHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoning Circles");
        }
        public override void SetDefaults()
        {
            projectile.width = projectile.height = 18;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            projectile.hide = true;
            projectile.alpha = 255;
        }

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override void AI()
        {
            if (projectile.ai[0]++ == 0)
            {
                if (!Sandstorm.Happening)
                {
                    Sandstorm.Happening = true;
                    Sandstorm.TimeLeft = (int)(3600.0 * (8.0 + (double)Main.rand.NextFloat() * 16.0));
                    ModUtils.SandstormStuff();
                }

                int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.SandnadoFriendly, 0, 0f, projectile.owner);
                projectile.position = Main.projectile[proj].Center;
                projectile.position -= new Vector2(Main.projectile[proj].width, Main.projectile[proj].height);
                Main.projectile[proj].timeLeft = 600;
                Main.projectile[proj].friendly = false;
                Main.projectile[proj].hostile = false;
            }

            if (projectile.ai[0] > 120)
            {
                if (projectile.ai[1]++ % 10 == 0 && BarrierCounter < 8)
                {
                    float Rotation = BarrierCounter * MathHelper.PiOver4;
                    int RADIUS = 400;

                    Vector2 SpawnPosition = projectile.Center + new Vector2(RADIUS, 0).RotatedBy(Rotation);
                    NPC.NewNPC((int)SpawnPosition.X, (int)SpawnPosition.Y, ModContent.NPCType<Barrier>(), 0, projectile.Center.X, projectile.Center.Y, Rotation, RADIUS);

                    BarrierCounter++;
                }
            }

            if (projectile.ai[0] == 360)
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

            if (projectile.ai[0] > 360)
            {
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
            if (projectile.ai[0] > 360)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D texture = ModContent.GetTexture(AssetDirectory.Textures + "PulseCircle");
                Color color = Color.Yellow;
                float scale = MathHelper.Lerp(0, 2.5f, Utils.Clamp(projectile.ai[0] - 360, 0, 100) / 100f);
                spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, color * 0.5f, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale, SpriteEffects.None, 0f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            }

            return base.PreDraw(spriteBatch, lightColor);
        }
    }
}
