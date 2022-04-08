using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow
{
    public class IorichBowEnergy : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Natural Energy");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 900;
        }

        public override void AI()
        {
            // Get the ID of the Parent NPC that was passed in via ai[0]
            Player parent = Main.player[(int)projectile.ai[0]];

            projectile.position = Vector2.Lerp(projectile.position, parent.Center, projectile.ai[1]++ / 60f);

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.TerraBlade, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
                switch (num1103)
                {
                    case 0:
                        Main.dust[num1106].position = (Main.dust[num1106].position + projectile.Center * 5f) / 6f;
                        break;
                    case 1:
                        Main.dust[num1106].position = (Main.dust[num1106].position + (projectile.Center + projectile.velocity / 2f) * 5f) / 6f;
                        break;
                }
                Dust dust81 = Main.dust[num1106];
                dust81.velocity *= 0.1f;
                Main.dust[num1106].noGravity = true;
                Main.dust[num1106].fadeIn = 1f;
            }

            if (projectile.getRect().Intersects(parent.getRect()))
            {
                Main.PlaySound(SoundID.DD2_DarkMageHealImpact);
                Particle.CreateParticle(Particle.ParticleType<Pulse>(), projectile.Center, Vector2.Zero, new Color(195, 255, 154), 0.5f, 0.25f);

                parent.GetModPlayer<OvermorrowModPlayer>().BowEnergyCount++;

                projectile.Kill();
            }
        }
    }
}