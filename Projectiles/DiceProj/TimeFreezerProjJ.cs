using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.DiceProj
{
    public class TimeFreezerProjJ : ModProjectile
    {
        //private int timer = 0;
        readonly int Random = Main.rand.Next(0, 111);
        readonly int defence = Main.rand.Next(0, 30);
        readonly int zoppler = Main.rand.Next(10, 200);
        readonly int attckter = Main.rand.Next(1, 15);
        readonly int scaleddd = Main.rand.Next(1, 5);
        public override void SetDefaults()
        {
            projectile.width = 3000;
            projectile.height = 3000;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.alpha = 0;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 150;
        }
        public override void AI()
        {
            projectile.scale = projectile.scale + 0.05f;
            projectile.width = projectile.width + 2;
            projectile.height = projectile.height + 2;
            projectile.rotation = projectile.rotation + 1;
            projectile.alpha = projectile.alpha + 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.boss == false)
            {
                target.aiStyle = Random;
                target.defense = defence;
                target.damage = zoppler;
                target.lifeMax += attckter;
                target.dontTakeDamage = false;
                if (scaleddd == 3)
                {
                    target.scale = 2f;
                }
                if (scaleddd == 5)
                {
                    target.scale = 0.5f;
                }
                if (scaleddd == 4)
                {
                    target.scale = 0.7f;
                }
                if (scaleddd == 2)
                {
                    target.scale = 1.7f;
                }
            }
        }
    }
}