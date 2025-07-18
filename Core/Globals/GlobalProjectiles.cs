using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Core.Items.Accessories;
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
            Player player = Main.player[projectile.owner];
            AccessoryKeywords.TriggerProjectileStrike(player, projectile, target, hit, damageDone);
            
            if (target.life <= 0)
            {
                AccessoryKeywords.TriggerExecute(player, target);
            }

            ArtOfBallistics.OnHitNPC(projectile, target, hit, damageDone);

            BlackPage.TryApplyShadowBrand(projectile, target);
            //WhitePage.TryApplyStellarCorona(projectile, target);
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