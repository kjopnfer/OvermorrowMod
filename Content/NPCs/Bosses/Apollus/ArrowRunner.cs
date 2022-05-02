using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
{
    public class ArrowRunner : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoneArrow;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ArrowRunner");
            Main.projFrames[Projectile.type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 30;
            Projectile.timeLeft = 420;
            Projectile.light = 0.75f;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
        }
        int timer = 0;
        public override void AI()
        {
            /*
            Player player = Main.player[Projectile.owner];
            */
            Vector2 value1 = new Vector2(0f, 1f);
            value1.Normalize();
            value1 *= 1f;
            int Projectiles = 8;
            if (++timer % 120 == 0)
            {
                for (int j = 0; j < Projectiles; j++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0f, -6f).RotatedBy(j * MathHelper.ToRadians(360f) / Projectiles), ModContent.ProjectileType<HomingArrow>(), 2, 10f, Main.myPlayer);
                }
            }
        }
    }
}