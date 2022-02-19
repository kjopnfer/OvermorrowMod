using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss.Sins
{
    public class Greed : ModProjectile
    {
        int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            timer++;
            projectile.rotation += 0.5f;
            if (timer == 20)
            {
                timer = 0;
            }

            float OutsideRing = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
            if (OutsideRing < 80f && timer == 10)
            {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 0.7f;
                int type = mod.ProjectileType("Xbolt");
                int damage = 11;
                Projectile.NewProjectile(position, direction * speed, type, damage, 1f, projectile.owner, 0f);
                timer = 0;
            }
        }



        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin of Greed");
        }
    }
}