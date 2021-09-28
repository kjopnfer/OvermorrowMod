using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using OvermorrowMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace OvermorrowMod.Items.Consumable.Boss
{
    public class StormDrakeAnim : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Drake Spawner");
        }
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.tileCollide = false;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.timeLeft = 900;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            switch (projectile.ai[0])
            {
                case 0:
                    {
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
                            Projectile.NewProjectile(projectile.Center, (Vector2.UnitY * -1).RotatedBy(MathHelper.ToRadians(-20)), ModContent.ProjectileType<TestLightning4>(), projectile.damage, 2, Main.myPlayer, 0, projectile.whoAmI);
                            Projectile.NewProjectile(projectile.Center, Vector2.UnitY * -1, ModContent.ProjectileType<TestLightning4>(), projectile.damage, 2, Main.myPlayer, 0, projectile.whoAmI);
                            Projectile.NewProjectile(projectile.Center, (Vector2.UnitY * -1).RotatedBy(MathHelper.ToRadians(20)), ModContent.ProjectileType<TestLightning4>(), projectile.damage, 2, Main.myPlayer, 0, projectile.whoAmI);
                        }


                        if (projectile.ai[1] == 260)
                        {
                            Player player = Main.player[projectile.owner];
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/woow"), player.position);
                        }

                        if (projectile.ai[1]++ > 300)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Shockwave3>(), projectile.Center, Vector2.Zero, Color.DarkCyan, 1, 5, 0, 1f);
                            Player player = Main.player[projectile.owner];
                            player.GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(projectile.Center, 90, 120f, 60f);
                            player.GetModPlayer<OvermorrowModPlayer>().ScreenShake = 50;
                            player.GetModPlayer<OvermorrowModPlayer>().TitleID = 2;
                            player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;
                            Main.PlaySound(SoundID.Roar, player.position, 0);
                            NPC.NewNPC((int)projectile.Center.X, (int)(projectile.Center.Y + 146), ModContent.NPCType<StormDrake2>(), 0, -3f, -3f, 0f, 0f, 255);
                            /*Vector2 origin = new Vector2((int)projectile.Center.X, (int)(projectile.Center.Y));
                            float radius = 100;
                            int numLocations = 200;
                            for (int i = 0; i < 200; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                int dust = Dust.NewDust(position, 2, 2, 229, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                Main.dust[dust].noGravity = true;
                            }*/
                            projectile.Kill();
                        }
                    }
                    break;
            }
        }
    }
}
