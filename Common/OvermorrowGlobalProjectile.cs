using OvermorrowMod.Content.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalProjectile : GlobalProjectile
    {
        // Life is pain
        public override bool InstancePerEntity => true;

        private bool spawnedBlood = false;
        public bool slowedTime = false;

        public bool RetractSlow = false;

        public override bool PreAI(Projectile projectile)
        {
            if (slowedTime && !projectile.friendly)
            {
                projectile.position -= projectile.velocity * 0.95f;
            }

            return base.PreAI(projectile);
        }

        public override void SetDefaults(Projectile projectile)
        {
            if (projectile.type == 590)
            {
                projectile.timeLeft = 100;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.type == 130 || projectile.type == 131)
            {
                target.AddBuff(ModContent.BuffType<FungalInfection>(), 500);
            }
            if (projectile.type == 30)
            {
                target.immune[projectile.owner] = 0;
            }
        }

        public override void GrappleRetreatSpeed(Projectile projectile, Player player, ref float speed)
        {
            if (RetractSlow)
            {
                speed = 4;
            }

            base.GrappleRetreatSpeed(projectile, player, ref speed);
        }
    }
}