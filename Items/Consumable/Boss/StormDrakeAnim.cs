﻿using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.NPCs.Bosses.Apollus;
using OvermorrowMod.NPCs.Bosses.GraniteMini;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss
{
    public class StormDrakeAnim : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summoning Circles");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            switch (projectile.ai[0])
            {
                case 0:
                    {
                        //for (int i = 0; i < 18; i++)
                        //{
                        //    Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * /*35*/ 20 + projectile.ai[0]));
                        //    Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 229, 0f, 0f, 0, default, 2.04f)];
                        //    dust.noGravity = true;
                        //    //Vector2 spawnpos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 35 + projectile.ai[0]));
                        //    //Projectile.NewProjectile(spawnpos, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(i * 20 + projectile.ai[0]))/*.RotatedByRandom(MathHelper.TwoPi)*/, ModContent.ProjectileType<TestLightning3>(), 0, 900, Main.myPlayer);
                        //}
                        //projectile.ai[1] -= 15;//10;//15;
                        //for (int i = 0; i < 18; i++)
                        //{
                        //    Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 20 + projectile.ai[0]));
                        //    Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 229, 0f, 0f, 0, default, 2.04f)];
                        //    dust.noGravity = true;
                        //}
                        projectile.ai[1] -= 15;
                        if (projectile.ai[1] <= 0)
                        {
                            projectile.ai[1] = 0;
                            projectile.ai[0] = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        if (projectile.ai[1] == 0)
                        {
                            Projectile.NewProjectile(projectile.Center /*+ new Vector2(25, 0).RotatedBy(MathHelper.ToRadians(-20))*/, (Vector2.UnitY * -1).RotatedBy(MathHelper.ToRadians(-20)), ModContent.ProjectileType<TestLightning4>(), projectile.damage, 2, Main.myPlayer, 0, projectile.whoAmI);
                            Projectile.NewProjectile(projectile.Center /*+ new Vector2(25, 0)*/, Vector2.UnitY * -1, ModContent.ProjectileType<TestLightning4>(), projectile.damage, 2, Main.myPlayer, 0, projectile.whoAmI);
                            Projectile.NewProjectile(projectile.Center /*+ new Vector2(25, 0).RotatedBy(MathHelper.ToRadians(20))*/, (Vector2.UnitY * -1).RotatedBy(MathHelper.ToRadians(20)), ModContent.ProjectileType<TestLightning4>(), projectile.damage, 2, Main.myPlayer, 0, projectile.whoAmI);
                        }
                        if (projectile.ai[1]++ > 300)
                        {
                            Player player = Main.player[projectile.owner];
                            player.GetModPlayer<OvermorrowModPlayer>().TitleID = 2;
                            player.GetModPlayer<OvermorrowModPlayer>().FocusBoss = true;
                            player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;
                            Main.PlaySound(SoundID.Roar, player.position, 0);
                            NPC.NewNPC((int)projectile.position.X, (int)(projectile.position.Y /*+ 33*/), ModContent.NPCType<StormDrake2>(), 0, -3f, -3f, 0f, 0f, 255);
                            Vector2 origin = new Vector2((int)projectile.position.X, (int)(projectile.position.Y /*+ 33*/));
                            float radius = 100;
                            int numLocations = 200;
                            for (int i = 0; i < 200; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                int dust = Dust.NewDust(position, 2, 2, 229, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                Main.dust[dust].noGravity = true;
                            }
                            projectile.Kill();
                        }
                    }
                    break;
            }
        }
    }
}