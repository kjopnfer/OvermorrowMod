using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Misc;
using OvermorrowMod.Projectiles.Piercing;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.WardenClass
{
    public abstract class ArtifactProjectile : ModProjectile
    {
        public WardenRunePlayer.Runes RuneID;
        public virtual void SafeSetDefaults()
        {

        }

        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            projectile.melee = false;
            projectile.ranged = false;
            projectile.magic = false;
            projectile.thrown = false;
            projectile.minion = false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (RuneID == WardenRunePlayer.Runes.CorruptionRune)
            {
                target.AddBuff(BuffID.CursedInferno, 120);
            }

            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}