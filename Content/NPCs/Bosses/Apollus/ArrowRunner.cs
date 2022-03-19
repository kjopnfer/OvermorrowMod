using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ArrowRunner : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.BoneArrow;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ArrowRunner");
            Main.projFrames[projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = 30;
            projectile.timeLeft = 420;
            projectile.light = 0.75f;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
        }
        int timer = 0;
        public override void AI()
        {
            /*
            Player player = Main.player[projectile.owner];
            */
            Vector2 value1 = new Vector2(0f, 1f);
            value1.Normalize();
            value1 *= 1f;
            int projectiles = 8;
            if (++timer % 120 == 0)
            {
                for (int j = 0; j < projectiles; j++)
                {
                    Projectile.NewProjectile(projectile.Center, new Vector2(0f, -6f).RotatedBy(j * MathHelper.ToRadians(360f) / projectiles), ModContent.ProjectileType<HomingArrow>(), 2, 10f, Main.myPlayer);
                }
            }
        }
    }
}