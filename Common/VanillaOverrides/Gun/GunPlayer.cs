using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class HeldGunInfo
    {
        public int shotsFired;
        public int bonusBullets;
        public int bonusDamage;
        public int bonusAmmo;

        public HeldGunInfo(int shotsFired, int bonusBullets, int bonusDamage, int bonusAmmo)
        {
            this.shotsFired = shotsFired;
            this.bonusBullets = bonusBullets;
            this.bonusDamage = bonusDamage;
            this.bonusAmmo = bonusAmmo;
        }
    }

    public class GunPlayer : ModPlayer
    {
        public bool GraniteLauncher;

        /// <summary>
        /// Used to preserve data between guns whenever swapped to prevent reload skipping
        /// </summary>
        public Dictionary<int, HeldGunInfo> playerGunInfo = new Dictionary<int, HeldGunInfo>();

        public override void ResetEffects()
        {
            GraniteLauncher = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (crit && proj.DamageType == DamageClass.Ranged)
            {
                //for (int i = 0; i < 3; i++)
                Projectile.NewProjectile(null, target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 3, ModContent.ProjectileType<GraniteEnergy>(), 0, 0f, Player.whoAmI);
            }

            base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
        }
    }
}
