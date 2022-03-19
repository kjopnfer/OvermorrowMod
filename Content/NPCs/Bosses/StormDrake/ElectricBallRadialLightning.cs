using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.StormDrake
{
    public class ElectricBallRadialLightning : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamer Ball");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            int num434 = Dust.NewDust(projectile.Center, 0, 0, DustID.Vortex, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;

            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            switch (projectile.ai[0])
            {
                case 0:
                    {
                        if (projectile.ai[1] > 60 && projectile.velocity != Vector2.Zero)
                        {
                            projectile.velocity = Vector2.SmoothStep(projectile.velocity, Vector2.Zero, 0.1f);//0.2f);//0.1f);//0.075f);
                            if (projectile.velocity.X < 0.05 || projectile.velocity.Y < 0.05)
                            {
                                projectile.velocity = Vector2.Zero;
                            }
                        }
                        if (projectile.velocity == Vector2.Zero)
                        {
                            projectile.ai[1] = 0;
                            projectile.ai[0]++;
                        }
                    }
                    break;
                case 1:
                    {
                        if (projectile.ai[1] % /*5*/ 4 == 0 && projectile.ai[1] <= 144/*180*/)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int proj = Projectile.NewProjectile(projectile.Center, new Vector2(0.01f, 0.01f).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<LaserWarning>(), projectile.damage, projectile.knockBack, projectile.owner);
                                ((LaserWarning)Main.projectile[proj].modProjectile).killearly = true;
                                ((LaserWarning)Main.projectile[proj].modProjectile).waittime = 100;
                            }
                        }

                        if (projectile.ai[1] > /*180*/ 144 && projectile.timeLeft > 90)
                        {
                            projectile.timeLeft = 90;
                        }
                        if (projectile.ai[1] > 200)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == mod.ProjectileType("LaserWarning") && ((LaserWarning)Main.projectile[i].modProjectile).killearly == true)
                                {
                                    ((LaserWarning)Main.projectile[i].modProjectile).killnow = true;
                                    Main.projectile[i].Kill();
                                }
                            }
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                                if (distance <= 1600)
                                {
                                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                                }
                            }
                        }
                    }
                    break;
            }
            projectile.ai[1]++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft >= 60)
            {
                return Color.White;
            }
            else
            {
                return null;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, Main.expertMode ? 180 : 90);//360 : 180);
        }
    }
}