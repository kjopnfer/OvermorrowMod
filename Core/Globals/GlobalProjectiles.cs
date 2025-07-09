using OvermorrowMod.Content.Items.Archives.Accessories;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsPowerShot = false;

        public int ArtOfBallisticsHit = 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            ArtOfBallistics.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            ArtOfBallistics.OnHitNPC(projectile, target, hit, damageDone);

            BlackPage.TryApplyShadowBrand(projectile, target);
            WhitePage.TryApplyStellarCorona(projectile, target);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (IsPowerShot)
            {
                modifiers.SourceDamage *= 1.25f;
            }
        }
    }
}