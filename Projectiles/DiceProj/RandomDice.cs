using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.DiceProj
{
    public class RandomDice : ModProjectile
    {
        //private int timer = 0;
        readonly int Ryc = Main.rand.Next(1, 7);
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.light = 0.5f;
            projectile.alpha = 0;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 3;
        }
        public override void Kill(int timeLeft)
        {
            if (Ryc == 6)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("HealDice"), projectile.damage, 1f, projectile.owner, 0f);
            }
            if (Ryc == 5)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("EffectDice"), projectile.damage, 1f, projectile.owner, 0f);
            }
            if (Ryc == 4)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("RocketDice"), projectile.damage, 1f, projectile.owner, 0f);
            }
            if (Ryc == 3)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("DartDice"), projectile.damage, 1f, projectile.owner, 0f);
            }
            if (Ryc == 2)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("NpcDice"), projectile.damage, 1f, projectile.owner, 0f);
            }
            if (Ryc == 1)
            {
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, projectile.velocity.X, projectile.velocity.Y, mod.ProjectileType("BombDice"), projectile.damage, 1f, projectile.owner, 0f);
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cone");
        }
    }
}
