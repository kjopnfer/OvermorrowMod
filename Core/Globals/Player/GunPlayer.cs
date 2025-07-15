using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GunPlayer : ModPlayer
    {
        /// <summary>
        /// Used to preserve data between guns whenever swapped to prevent reload skipping
        /// </summary>
        public Dictionary<int, HeldGunInfo> playerGunInfo = new Dictionary<int, HeldGunInfo>();
        public List<IGunModifier> ActiveModifiers { get; private set; } = new List<IGunModifier>();

        public int BulletArmorPenetration;

        public bool CowBoySet;
        public bool GraniteLauncher;

        public bool ChicagoBonusShots = false;
        public bool FarlanderPierce = false;
        public bool WildEyeCrit = false;

        public int GraniteEnergyCount = 0;
        public float FarlanderCharge = 0;
        public int FarlanderSpeedBoost = 0;
        public int MusketInaccuracy = 0;

        //public Dictionary<int, GraniteShard> ShardList = new Dictionary<int, GraniteShard>();

        public override void ResetEffects()
        {
            BulletArmorPenetration = 0;

            CowBoySet = false;
            GraniteLauncher = false;

            ActiveModifiers.Clear();
        }

        public void AddGunModifier(IGunModifier modifier)
        {
            ActiveModifiers.Add(modifier);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            /*if (hit.Crit && proj.DamageType == DamageClass.Ranged)
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
            }*/
        }

        /// <summary>
        /// Gets all active modifiers (simplified for the new stats system).
        /// </summary>
        public List<IGunModifier> GetApplicableModifiers(WeaponType weaponType, int itemID)
        {
            return ActiveModifiers.ToList();
        }

        /// <summary>
        /// Triggers gun shoot events for all applicable modifiers.
        /// </summary>
        public void TriggerGunShoot(HeldGun gun, Player player, Projectile bullet)
        {
            foreach (var modifier in ActiveModifiers)
            {
                modifier.OnGunShoot(gun, player, bullet);
            }
        }

        /// <summary>
        /// Triggers reload success events for all applicable modifiers.
        /// </summary>
        public void TriggerReloadSuccess(HeldGun gun, Player player, List<BulletObject> bullets)
        {
            foreach (var modifier in ActiveModifiers)
            {
                modifier.OnReloadSuccess(gun, player, bullets);
            }
        }

        /// <summary>
        /// Triggers reload fail events for all applicable modifiers.
        /// </summary>
        public void TriggerReloadFail(HeldGun gun, Player player, List<BulletObject> bullets)
        {
            foreach (var modifier in ActiveModifiers)
            {
                modifier.OnReloadFail(gun, player, bullets);
            }
        }

        /// <summary>
        /// Triggers reload zone hit events for all applicable modifiers.
        /// </summary>
        public void TriggerReloadZoneHit(HeldGun gun, Player player, List<BulletObject> bullets, int zoneIndex, int zonesRemaining)
        {
            foreach (var modifier in ActiveModifiers)
            {
                modifier.OnReloadZoneHit(gun, player, bullets, zoneIndex, zonesRemaining);
            }
        }

        /// <summary>
        /// Triggers gun reload events for all applicable modifiers.
        /// </summary>
        public void TriggerGunReload(HeldGun gun, Player player, bool wasSuccessful)
        {
            foreach (var modifier in ActiveModifiers)
            {
                modifier.OnGunReload(gun, player, wasSuccessful);
            }
        }
    }
}