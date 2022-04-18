using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.NPCs.Bosses.StormDrake;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable.Boss
{
    public class StormDrakeAnim : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Drake Spawner");
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.tileCollide = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 900;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                case 0:
                    {
                        Projectile.ai[1] -= 15;
                        if (Projectile.ai[1] <= 0)
                        {
                            Projectile.ai[1] = 0;
                            Projectile.ai[0] = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        if (Projectile.ai[1] == 0)
                        {
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, (Vector2.UnitY * -1).RotatedBy(MathHelper.ToRadians(-20)), ModContent.ProjectileType<TestLightning4>(), Projectile.damage, 2, Main.myPlayer, 0, Projectile.whoAmI);
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, Vector2.UnitY * -1, ModContent.ProjectileType<TestLightning4>(), Projectile.damage, 2, Main.myPlayer, 0, Projectile.whoAmI);
                            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, (Vector2.UnitY * -1).RotatedBy(MathHelper.ToRadians(20)), ModContent.ProjectileType<TestLightning4>(), Projectile.damage, 2, Main.myPlayer, 0, Projectile.whoAmI);
                        }


                        if (Projectile.ai[1] == 260)
                        {
                            Player player = Main.player[Projectile.owner];
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("Sounds/NPC/woow"), player.position);
                        }

                        if (Projectile.ai[1]++ > 300)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), Projectile.Center, Vector2.Zero, Color.DarkCyan, 1, 5, 0, 1f);
                            Player player = Main.player[Projectile.owner];
                            //player.GetModPlayer<OvermorrowModPlayer>().PlayerFocusCamera(Projectile.Center, 90, 120f, 60f);
                            //player.GetModPlayer<OvermorrowModPlayer>().ScreenShake = 50;
                            //player.GetModPlayer<OvermorrowModPlayer>().TitleID = 2;
                            //player.GetModPlayer<OvermorrowModPlayer>().ShowText = true;
                            SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
                            NPC.NewNPC(Projectile.GetNPCSource_FromThis(), (int)Projectile.Center.X, (int)(Projectile.Center.Y + 146), ModContent.NPCType<StormDrake>(), 0, -3f, -3f, 0f, 0f, 255);
                            /*Vector2 origin = new Vector2((int)Projectile.Center.X, (int)(Projectile.Center.Y));
                            float radius = 100;
                            int numLocations = 200;
                            for (int i = 0; i < 200; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                int dust = Dust.NewDust(position, 2, 2, 229, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                Main.dust[dust].noGravity = true;
                            }*/
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }
    }
}
