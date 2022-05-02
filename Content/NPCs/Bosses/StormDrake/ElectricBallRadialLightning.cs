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
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0.5f);

            int num434 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Vortex, 0f, 0f, 100);
            Main.dust[num434].noLight = true;
            Main.dust[num434].noGravity = true;
            Main.dust[num434].velocity = Projectile.velocity;
            Main.dust[num434].position -= Vector2.One * 4f;
            Main.dust[num434].scale = 0.8f;

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            switch (Projectile.ai[0])
            {
                case 0:
                    {
                        if (Projectile.ai[1] > 60 && Projectile.velocity != Vector2.Zero)
                        {
                            Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, Vector2.Zero, 0.1f);//0.2f);//0.1f);//0.075f);
                            if (Projectile.velocity.X < 0.05 || Projectile.velocity.Y < 0.05)
                            {
                                Projectile.velocity = Vector2.Zero;
                            }
                        }
                        if (Projectile.velocity == Vector2.Zero)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0]++;
                        }
                    }
                    break;
                case 1:
                    {
                        if (Projectile.ai[1] % /*5*/ 4 == 0 && Projectile.ai[1] <= 144/*180*/)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0.01f, 0.01f).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<LaserWarning>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                ((LaserWarning)Main.projectile[proj].ModProjectile).killearly = true;
                                ((LaserWarning)Main.projectile[proj].ModProjectile).waittime = 100;
                            }
                        }

                        if (Projectile.ai[1] > /*180*/ 144 && Projectile.timeLeft > 90)
                        {
                            Projectile.timeLeft = 90;
                        }
                        if (Projectile.ai[1] > 200)
                        {
                            for (int i = 0; i < Main.maxProjectiles; i++)
                            {
                                if (Main.projectile[i].active && Main.projectile[i].type == Mod.Find<ModProjectile>("LaserWarning").Type && ((LaserWarning)Main.projectile[i].ModProjectile).killearly == true)
                                {
                                    ((LaserWarning)Main.projectile[i].ModProjectile).killnow = true;
                                    Main.projectile[i].Kill();
                                }
                            }
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(Projectile.Center, Main.player[i].Center);
                                if (distance <= 1600)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                                }
                            }
                        }
                    }
                    break;
            }
            Projectile.ai[1]++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft >= 60)
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