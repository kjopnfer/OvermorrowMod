using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.DrakeStaff
{
    public class LightningBreathFriendly : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Breath");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.friendly = true;
            projectile.hostile = false;
        }

        public override void AI()
        {
            for (int num1101 = 0; num1101 < 3; num1101++)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Clentaminator_Cyan, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            for (int num1103 = 0; num1103 < 2; num1103++)
            {
                int num1106 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Frost, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 0.4f);
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
        }
    }
}