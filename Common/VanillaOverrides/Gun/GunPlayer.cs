using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
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
        public bool CowBoySet;
        public bool GraniteLauncher;

        public bool WildEyeCrit = false;

        public int MusketInaccuracy = 0;
        public int GraniteEnergyCount = 0;

        /// <summary>
        /// Used to preserve data between guns whenever swapped to prevent reload skipping
        /// </summary>
        public Dictionary<int, HeldGunInfo> playerGunInfo = new Dictionary<int, HeldGunInfo>();

        public override void ResetEffects()
        {
            CowBoySet = false;
            GraniteLauncher = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (crit && proj.DamageType == DamageClass.Ranged)
            {
                if (Player.GetModPlayer<GunPlayer>().GraniteEnergyCount < 8)
                {
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        float randomScale = Main.rand.NextFloat(0.1f, 0.15f);

                        Color color = Color.Cyan;
                        Particle.CreateParticle(Particle.ParticleType<Pulse>(), proj.Center, Vector2.Zero, color, 1, randomScale);
                    }

                    Projectile.NewProjectile(null, target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 3, ModContent.ProjectileType<GraniteEnergy>(), 0, 0f, Player.whoAmI);
                }
            }

            base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
        }
    }
}
