using OvermorrowMod.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class FungiSpore2 : ModProjectile
    {
        public override string Texture => "Terraria/NPC_" + NPCID.FungiSpore;
        private int storeDamage = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungi Spore");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 0)
            {
                storeDamage = projectile.damage;
                projectile.damage = 0;
            }

            if (projectile.ai[1] == 120)
            {
                projectile.damage = storeDamage;
            }

            // Projectile gravity
            projectile.velocity.Y += 0.07f;

            if (projectile.velocity.Y > 2.5f) // Terminal velocity
            {
                projectile.velocity.Y = 2.5f;
            }

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }

            projectile.rotation = projectile.velocity.X * 0.05f;

            projectile.ai[1]++;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<FungalInfection>(), 400);
        }
    }
}