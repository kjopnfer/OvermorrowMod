using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Guns;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Guns
{
    public abstract partial class HeldGun : ModProjectile, IProjectileClassification
    {
        private GunStats _baseStats;
        private GunStats _currentStats;

        /// <summary>
        /// Override this to define the gun's base statistics.
        /// </summary>
        public abstract GunStats BaseStats { get; }

        /// <summary>
        /// The weapon type classification for this gun.
        /// </summary>
        public abstract WeaponType WeaponType { get; }

        /// <summary>
        /// Gets the current stats after applying all modifiers.
        /// </summary>
        public GunStats CurrentStats
        {
            get
            {
                if (_currentStats == null || _baseStats == null)
                    RefreshStats();
                return _currentStats;
            }
        }

        /// <summary>
        /// Refreshes the current stats by applying all active modifiers to the base stats.
        /// </summary>
        public void RefreshStats()
        {
            // Store the current reload zone states before refreshing
            var preservedZoneStates = new Dictionary<int, bool>();
            if (_currentStats?.ClickZones != null)
            {
                for (int i = 0; i < _currentStats.ClickZones.Count; i++)
                {
                    preservedZoneStates[i] = _currentStats.ClickZones[i].HasClicked;
                }
            }

            _baseStats = BaseStats.Clone();
            _currentStats = GunModifierHandler.GetModifiedStats(_baseStats, player);

            // Restore the reload zone states after refreshing
            if (_currentStats?.ClickZones != null && preservedZoneStates.Count > 0)
            {
                for (int i = 0; i < _currentStats.ClickZones.Count && i < preservedZoneStates.Count; i++)
                {
                    _currentStats.ClickZones[i].HasClicked = preservedZoneStates[i];
                }
            }
        }

        // Properties derived from stats
        public (Vector2, Vector2) PositionOffset => CurrentStats.PositionOffset;
        public (Vector2, Vector2) BulletShootPosition => CurrentStats.BulletShootPosition;
        public float ProjectileScale => CurrentStats.ProjectileScale;
        public bool TwoHanded => CurrentStats.TwoHanded;
        public bool CanRightClick => CurrentStats.CanRightClick;
        public List<ReloadZone> ClickZones => CurrentStats.ClickZones;

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public virtual bool CanConsumeAmmo(Player player) => CurrentStats.CanConsumeAmmo;
        public virtual bool CanUseGun(Player player) => true;
        public virtual bool CanReload() => CurrentStats.CanReload;
        public virtual void Update(Player player) { }

        // Stats-based properties
        public int MaxShots => CurrentStats.MaxShots + CurrentStats.MaxShotsBonus;
        public int MaxReloadTime => (int)(CurrentStats.MaxReloadTime * CurrentStats.ReloadSpeedMultiplier);
        public int RecoilAmount => (int)(CurrentStats.RecoilAmount * CurrentStats.RecoilMultiplier);
        public SoundStyle ShootSound => CurrentStats.ShootSound;
        public SoundStyle ReloadFinishSound => CurrentStats.ReloadFinishSound;
        public int BulletType => CurrentStats.BulletType;
        public int ConvertBullet => CurrentStats.ConvertBullet;
        public bool ConsumePerShot => CurrentStats.ConsumePerShot;
        public bool UsesRightClickDelay => CurrentStats.UsesRightClickDelay;
        public int ShootTime => (int)(CurrentStats.ShootTime * CurrentStats.FireRateMultiplier);
        public int ShootAnimation => (int)(CurrentStats.ShootAnimation * CurrentStats.FireRateMultiplier);
        public int MaxChargeTime => (int)(CurrentStats.MaxChargeTime / CurrentStats.ChargeSpeedMultiplier);

        public abstract int ParentItem { get; }

        public virtual void SafeSetDefaults() { }

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;

            SafeSetDefaults();
        }

        public override void OnSpawn(IEntitySource source)
        {
            RefreshStats();

            // Apply damage modifiers from stats
            float finalDamage = (Projectile.damage * CurrentStats.DamageMultiplier) + CurrentStats.DamageFlat;
            Projectile.damage = (int)finalDamage;
            Projectile.knockBack *= CurrentStats.KnockbackMultiplier;

            if (UsesMagazine)
            {
                bool restored = LoadGunInfo();
                if (!restored) PreloadMagazine();
                SyncMagazineCounter();
            }

            ReloadBulletDisplay();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(inReloadState);
            writer.Write(rightClickDelay);
            writer.Write(shootCounter);
            writer.Write(chargeCounter);
            writer.Write(ShotsFired);
            writer.Write(reloadTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            inReloadState = reader.ReadBoolean();
            rightClickDelay = reader.ReadInt16();
            shootCounter = reader.ReadInt16();
            chargeCounter = reader.ReadInt16();
            ShotsFired = reader.ReadInt16();
            reloadTime = reader.ReadInt16();
        }

        private bool inReloadState = false;
        private Player player => Main.player[Projectile.owner];
        public ref float PrimaryCounter => ref Projectile.ai[0];
        public ref float SecondaryCounter => ref Projectile.ai[1];

        private int rightClickDelay = 0;

        private bool triggerHeldLastFrame = false;
        private int emptyClickTimer = 0;

        public int ShotsFired { get; private set; } = 0;
        private int shootCounter = 0;
        public int chargeCounter { private set; get; } = 0;
        private bool hasReleased = false;

        private int recoilTimer = 0;
        private const int RECOIL_TIME = 15;

        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem || !player.active || player.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 5;

            player.heldProj = Projectile.whoAmI;

            // Refresh stats every frame to account for dynamic modifiers
            RefreshStats();

            if (UsesMagazine) SyncMagazineCounter();

            HandleGunDrawing();
            ForceCorrectBulletDisplay();
            Update(player);

            if (rightClickDelay > 0) rightClickDelay--;
            if (emptyClickTimer > 0) emptyClickTimer--;

            if (!inReloadState)
            {
                if (reloadDelay > 0) reloadDelay--;

                var gunPlayer = player.GetModPlayer<GunPlayer>();
                if (gunPlayer.ReloadRequested)
                {
                    gunPlayer.ReloadRequested = false;
                    if (CanReload() && ShotsFired > 0 && reloadDelay == 0)
                    {
                        EnterReload();
                        triggerHeldLastFrame = player.controlUseItem;
                        return;
                    }
                }

                if (CanRightClick && rightClickDelay == 0 && shootCounter == 0 && Main.mouseRight)
                {
                    if (UsesRightClickDelay) rightClickDelay = 10;

                    RightClickEvent(player);
                    Projectile.netUpdate = true;
                }

                if (reloadDelay == 0)
                {
                    reloadSuccess = false;
                    ModUtils.AutofillAmmoSlots(player, AmmoID.Bullet);
                    if (rightClickDelay == 0) HandleGunUse();
                }
            }
            else
            {
                HandleReloadAction();
            }

            triggerHeldLastFrame = player.controlUseItem;
        }

        public virtual void RightClickEvent(Player player) { }

        public override void Kill(int timeLeft)
        {
            SaveGunInfo();
        }

        private void HandleGunUse()
        {
            if (CurrentStats.FireMode == GunFireMode.Automatic)
            {
                HandleMinigunUse();
                return;
            }

            HandleAmmoAction();
            HandleShootAction();
        }

        private void HandleMinigunUse()
        {
            if (player.controlUseItem && !hasReleased && CanUseGun(player))
            {
                OnChargeUpEffects(player, chargeCounter);

                if (chargeCounter < MaxChargeTime) chargeCounter++;

                if (chargeCounter == MaxChargeTime)
                {
                    OnChargeShootEffects(player);

                    if (player.controlUseItem && shootCounter == 0 && FindAmmo())
                    {
                        shootCounter = ShootTime + CurrentStats.UseTimeModifier;

                        if (CanReload()) PopBulletDisplay();

                        if (ShotsFired == MaxShots)
                        {
                            EnterReload();
                            return;
                        }
                        else
                        {
                            if (CanReload()) ShotsFired++;

                            // For machine guns, always consume but respect save chance
                            if (!ShouldSaveAmmo())
                            {
                                ConsumeAmmo();
                            }
                        }

                        Projectile.netUpdate = true;
                    }

                    HandleShootAction();
                }
            }
            else
            {
                OnChargeReleaseEffects(player, chargeCounter);
                shootCounter = 0;

                if (chargeCounter > 0)
                {
                    hasReleased = true;
                    chargeCounter -= 2;
                }

                if (chargeCounter < 0) chargeCounter = 0;
                if (chargeCounter == 0) hasReleased = false;
            }
        }

        private void HandleAmmoAction()
        {
            if (ShotsFired >= MaxShots)
            {
                if (player.controlUseItem && CanUseGun(player))
                {
                    // A fresh trigger pull starts the reload. Firing the last round never does.
                    if (CanReload() && !triggerHeldLastFrame)
                    {
                        EnterReload();
                        return;
                    }

                    // Holding the trigger on an empty gun dry-fires at the gun's firing cadence,
                    // only after the last real shot's cooldown has elapsed.
                    if (shootCounter == 0 && emptyClickTimer == 0)
                    {
                        SoundEngine.PlaySound(CurrentStats.EmptyClickSound with
                        {
                            Volume = 0.8f,
                            Pitch = Main.rand.NextFloat(-0.1f, 0.1f)
                        });
                        emptyClickTimer = ShootTime + CurrentStats.UseTimeModifier;
                    }
                }

                return;
            }

            if (player.controlUseItem && shootCounter == 0 && CanUseGun(player) && loadedRounds.Count > 0)
            {
                shootCounter = ShootTime + CurrentStats.UseTimeModifier;

                // Fire the round at the front of the chamber.
                LoadedBulletType = loadedRounds[0].ProjectileType;

                if (!ShouldSaveAmmo())
                {
                    PopBulletDisplay();
                    loadedRounds.RemoveAt(0);
                    SyncMagazineCounter();
                }

                Projectile.netUpdate = true;
            }
        }

        private void HandleShootAction()
        {
            if (shootCounter > 0)
            {
                if (shootCounter == (ShootTime + CurrentStats.UseTimeModifier))
                {
                    if (ConsumePerShot)
                    {
                        bool ammoSaved = ShouldSaveAmmo();

                        if (!ammoSaved)
                        {
                            PopBulletDisplay();
                            ConsumeAmmo();

                            if (CanReload()) ShotsFired++;

                            if (ShotsFired >= MaxShots)
                            {
                                shootCounter = 0;
                                return; // Only return here if we need to enter reload immediately
                            }
                        }
                    }

                    recoilTimer = RECOIL_TIME;

                    Vector2 shootOffset = player.direction == 1 ? BulletShootPosition.Item2 : BulletShootPosition.Item1;
                    Vector2 shootPosition = Projectile.Center + shootOffset.RotatedBy(Projectile.rotation);

                    SoundEngine.PlaySound(ShootSound);
                    Vector2 direction = Main.MouseWorld - shootPosition;
                    if (direction != Vector2.Zero)
                        direction.Normalize();

                    Vector2 velocity = direction * 16f;

                    OnShootEffects(player, Main.spriteBatch, velocity, shootPosition, CurrentStats.BonusBullets);

                    float damage = Projectile.damage + CurrentStats.BonusDamage;
                    OnGunShoot(player, velocity, shootPosition, (int)damage, LoadedBulletType, Projectile.knockBack, CurrentStats.BonusBullets);
                }

                if (shootCounter > 0) shootCounter--;
            }
        }

        public virtual void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets) { }

        /// <summary>
        /// Handles gun shooting with automatic modifier triggering.
        /// Override OnGunShootCore for gun-specific logic.
        /// </summary>
        public void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            // Call the overridable method for gun-specific shooting logic
            var bullets = OnGunShootCore(player, velocity, shootPosition, damage, bulletType, knockBack, BonusBullets);

            // Always trigger modifier events for each bullet created
            foreach (var bullet in bullets)
            {
                var bulletProjectile = Main.projectile[bullet];
                GunModifierHandler.TriggerGunShoot(this, player, bulletProjectile);
            }
        }

        /// <summary>
        /// Override this method to implement gun-specific shooting behavior.
        /// Return a list of bullet projectile IDs that were created.
        /// </summary>
        protected virtual List<int> OnGunShootCore(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            // Default implementation: fire a single bullet
            var bullet = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"),
                shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);

            return new List<int> { bullet };
        }

        public virtual void OnChargeUpEffects(Player player, int chargeCounter) { }
        public virtual void OnChargeReleaseEffects(Player player, int chargeCounter) { }
        public virtual void OnChargeShootEffects(Player player) { }

        protected void SpawnBulletCasing(Projectile projectile, Player player, Vector2 position, Vector2 offset = default, float scale = 0.75f, bool sticky = true)
        {
            Vector2 velocity = new Vector2(player.direction * -0.03f, 0.01f);
            int gore = Gore.NewGore(null, position + offset, velocity, Mod.Find<ModGore>("BulletCasing").Type, scale);

            Main.gore[gore].sticky = sticky;
        }

        protected void DropMultipleCasings(Projectile projectile, Player player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnBulletCasing(projectile, player, projectile.Center);
            }
        }
    }
}
