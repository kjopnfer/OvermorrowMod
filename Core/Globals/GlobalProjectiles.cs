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

        /// <summary>
        /// Tracks the number of times NPCs have been hit by this projectile.
        /// Does not count unique NPC instances (i.e., the same NPC hit multiple times will count each hit).
        /// </summary>
        public int NumberHits = 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];

            AccessoryKeywords.TriggerProjectileSpawn(player, projectile, source);

            Main.NewText(projectile.Name + ", " + projectile.penetrate);
            //ArtOfBallistics.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            NumberHits++;

            Player player = Main.player[projectile.owner];
            AccessoryKeywords.TriggerProjectileStrike(player, projectile, target, hit, damageDone);
            
            if (target.life <= 0)
            {
                AccessoryKeywords.TriggerExecute(player, target);
            }

            //ArtOfBallistics.OnHitNPC(projectile, target, hit, damageDone);

            //BlackPage.TryApplyShadowBrand(projectile, target);
            //WhitePage.TryApplyStellarCorona(projectile, target);
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];

            if (IsPowerShot)
            {
                modifiers.SourceDamage *= 1.25f;
            }

            AccessoryKeywords.TriggerProjectileModifyHit(player, projectile, target, modifiers);
        }
    }
}