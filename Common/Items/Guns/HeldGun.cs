using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Accessories;
using OvermorrowMod.Core.Items.Guns;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
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

            LoadGunInfo();
            ReloadBulletDisplay();

            // Deactivate any bullets that were previously fired and stored
            for (int i = 0; i < ShotsFired; i++)
            {
                if (BulletDisplay.Count - 1 - i >= 0)
                {
                    BulletDisplay[BulletDisplay.Count - 1 - i].isActive = false;
                }
            }

            int activeBulletsAfter = 0;
            foreach (var bullet in BulletDisplay)
            {
                if (bullet.isActive) activeBulletsAfter++;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(inReloadState);
            writer.Write(rightClickDelay);
            writer.Write(shootCounter);
            writer.Write(chargeCounter);
            writer.Write(ShotsFired);
            writer.Write(reloadTime);
            writer.Write(isEmptyFiring);
            writer.Write(hasReleasedAfterEmpty);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            inReloadState = reader.ReadBoolean();
            rightClickDelay = reader.ReadInt16();
            shootCounter = reader.ReadInt16();
            chargeCounter = reader.ReadInt16();
            ShotsFired = reader.ReadInt16();
            reloadTime = reader.ReadInt16();
            isEmptyFiring = reader.ReadBoolean();
            hasReleasedAfterEmpty = reader.ReadBoolean();
        }

        private bool inReloadState = false;
        private Player player => Main.player[Projectile.owner];
        public ref float PrimaryCounter => ref Projectile.ai[0];
        public ref float SecondaryCounter => ref Projectile.ai[1];

        public int rightClickDelay = 0;

        private bool isEmptyFiring = false;  // Track if we're in empty firing state
        private bool hasReleasedAfterEmpty = false;  // Track if player released trigger after going empty
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

            HandleGunDrawing();
            ForceCorrectBulletDisplay();
            Update(player);

            if (rightClickDelay > 0) rightClickDelay--;

            if (!inReloadState)
            {
                if (reloadDelay > 0) reloadDelay--;

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
                    if (FindAmmo() && rightClickDelay == 0) HandleGunUse();
                }
            }
            else
            {
                HandleReloadAction();
            }
        }

        public virtual void RightClickEvent(Player player) { }
        public virtual bool PreDrawAmmo(Player player, SpriteBatch spriteBatch) { return true; }
        public override bool PreDraw(ref Color lightColor)
        {
            if (PreDrawGun(player, Main.spriteBatch, ShotsFired, shootCounter, lightColor))
                DrawGun(lightColor);

            if (!isEmptyFiring)
            {
                DrawGunOnShoot(player, Main.spriteBatch, lightColor, shootCounter, ShootTime + CurrentStats.UseTimeModifier);
            }

            if (reloadTime == 0 && PreDrawAmmo(player, Main.spriteBatch))
            {
                if (CanReload()) DrawAmmo();
            }
            else
                if (CanReload()) DrawReloadBar();

            // These need to be here otherwise the player arm gets drawn additively for some reason
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SaveGunInfo();
        }

        public Projectile LoadedBullet { private set; get; }
        public int LoadedBulletType { private set; get; }
        public int LoadedBulletItemType { private set; get; }

        private int AmmoSlotID;
        private int recoilTimer = 0;
        private const int RECOIL_TIME = 15;

        private void HandleGunDrawing()
        {
            if (recoilTimer > 0) recoilTimer--;

            float recoilRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-RecoilAmount * player.direction), Utils.Clamp(recoilTimer, 0, RECOIL_TIME) / (float)RECOIL_TIME);

            float gunRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation() + recoilRotation;
            Projectile.rotation = gunRotation;
            Projectile.spriteDirection = gunRotation > MathHelper.PiOver2 || gunRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = (player.direction == -1 ? PositionOffset.Item1 : PositionOffset.Item2).RotatedBy(gunRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            if (TwoHanded)
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2 + recoilRotation);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 + recoilRotation);
        }

        public int ShotsFired = 0;
        private int shootCounter = 0;
        public int chargeCounter { private set; get; } = 0;
        public bool hasReleased = false;

        private void HandleGunUse()
        {
            if (WeaponType == WeaponType.MachineGun || CurrentStats.MaxShots > 50) // Machine gun behavior
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

                    if (player.controlUseItem && shootCounter == 0)
                    {
                        shootCounter = ShootTime + CurrentStats.UseTimeModifier;

                        if (CanReload()) PopBulletDisplay();

                        if (ShotsFired == MaxShots)
                        {
                            shootCounter = 0;
                            inReloadState = true;
                            reloadTime = MaxReloadTime;
                            reloadBuffer = 10;
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
            // Check if player has released the trigger after emptying the gun
            if (isEmptyFiring && !player.controlUseItem)
            {
                hasReleasedAfterEmpty = true;
            }

            // If player clicks again after releasing when empty, enter reload
            if (isEmptyFiring && hasReleasedAfterEmpty && player.controlUseItem && shootCounter == 0)
            {
                isEmptyFiring = false;
                hasReleasedAfterEmpty = false;
                inReloadState = true;
                reloadTime = MaxReloadTime;
                reloadBuffer = 10;
                Projectile.netUpdate = true;
                return;
            }

            if (player.controlUseItem && shootCounter == 0 && CanUseGun(player))
            {
                if (ShotsFired >= MaxShots)
                {
                    // We're out of ammo, enter empty firing state
                    isEmptyFiring = true;
                    shootCounter = ShootTime + CurrentStats.UseTimeModifier;

                    // Play click sound instead of shoot sound
                    OnEmptyFire(player);
                    Projectile.netUpdate = true;
                    return;
                }

                shootCounter = ShootTime + CurrentStats.UseTimeModifier;

                if (!ConsumePerShot)
                {
                    // Check if ammo should be saved BEFORE doing anything
                    bool ammoSaved = ShouldSaveAmmo();

                    if (!ammoSaved)
                    {
                        // Only progress shots and consume ammo if NOT saved
                        PopBulletDisplay();
                        ConsumeAmmo();

                        if (CanReload()) ShotsFired++;

                        if (ShotsFired > MaxShots)
                        {
                            shootCounter = 0;
                            inReloadState = true;
                            reloadTime = MaxReloadTime;
                            reloadBuffer = 10;
                            return;
                        }
                    }
                    // If ammo was saved, we still shoot but don't progress anything
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
                    // Only do shooting effects if we're NOT empty firing
                    if (!isEmptyFiring)
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

        private bool reloadFail = false;
        public bool reloadSuccess { get; private set; } = false;
        public int reloadTime = 0;
        private int clickDelay = 0;
        public int reloadDelay { get; private set; } = 0;
        private int reloadBuffer = 10;

        /// <summary>
        /// Called when the gun attempts to fire but has no ammo.
        /// </summary>
        private void OnEmptyFire(Player player)
        {
            SoundEngine.PlaySound(CurrentStats.EmptyClickSound with
            {
                Volume = 0.8f,
                Pitch = Main.rand.NextFloat(-0.1f, 0.1f)
            });
        }


        private void HandleReloadAction()
        {
            if (reloadTime == MaxReloadTime)
            {
                OnReloadStart(player);
            }

            if (reloadTime > 0) reloadTime--;
            if (clickDelay > 0) clickDelay--;
            if (reloadBuffer > 0)
            {
                reloadBuffer--;
                return;
            }

            player.itemTime = 2;
            player.itemAnimation = 2;

            // Only process clicks if there's no click delay and reload hasn't failed
            if (player.controlUseItem && clickDelay == 0 && !reloadFail)
            {
                float clickPercentage = (1 - (float)reloadTime / MaxReloadTime) * 100f;
                clickDelay = 15;

                // Check if we clicked in any zone that hasn't been clicked yet
                bool hitValidZone = false;
                for (int i = 0; i < ClickZones.Count; i++)
                {
                    var zone = ClickZones[i];
                    bool inRange = clickPercentage >= zone.StartPercentage && clickPercentage <= zone.EndPercentage;
                    bool alreadyClicked = zone.HasClicked;

                    if (!alreadyClicked && inRange)
                    {
                        zone.HasClicked = true;
                        ReloadEventTrigger(player, i, GetClicksLeft());
                        hitValidZone = true;
                        break; // Only hit one zone per click
                    }
                }

                if (!hitValidZone)
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/youmissedthatone") with
                    {
                        Volume = 3f
                    }, player.Center);
                    reloadFail = true;
                }
            }

            if (reloadTime == 0)
            {
                bool wasSuccessful = CheckEventSuccess();
                reloadSuccess = wasSuccessful;

                // Reset state variables
                reloadFail = false;
                reloadDelay = 30;
                inReloadState = false;
                ShotsFired = 0;
                clickDelay = 0;

                if (wasSuccessful)
                {
                    OnReloadEventSuccess(player);
                }
                else
                {
                    OnReloadEventFail(player);
                }

                AccessoryKeywords.TriggerReload(player, wasSuccessful);

                // Refresh stats after success/fail events to pick up bonus changes
                RefreshStats();

                ReloadBulletDisplay();
                OnReloadEnd(player);

                // IMPORTANT: Reset zones LAST, after everything else is done
                ResetReloadZones();

                SoundEngine.PlaySound(ReloadFinishSound);
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// Handles reload completion with automatic modifier triggering.
        /// Override OnReloadComplete for gun-specific logic.
        /// </summary>
        public void OnReloadEnd(Player player)
        {
            // Reset empty firing state when reload completes
            isEmptyFiring = false;
            hasReleasedAfterEmpty = false;

            // Always trigger modifier events
            GunModifierHandler.TriggerGunReload(this, player, reloadSuccess);

            if (reloadSuccess)
                GunModifierHandler.TriggerReloadSuccess(this, player, BulletDisplay);
            else
                GunModifierHandler.TriggerReloadFail(this, player, BulletDisplay);

            // Call overridable method for gun-specific logic
            OnReloadComplete(player, reloadSuccess);
        }

        /// <summary>
        /// Override this method to add gun-specific reload completion effects.
        /// Modifier events are automatically triggered before this method.
        /// </summary>
        protected virtual void OnReloadComplete(Player player, bool wasSuccessful) { }


        public virtual void OnReloadStart(Player player) { }

        /// <summary>
        /// Handles reload zone hits with automatic modifier triggering.
        /// Override OnReloadZoneHit for gun-specific logic.
        /// </summary>
        public void ReloadEventTrigger(Player player, int zoneIndex, int clicksLeft)
        {
            // Always trigger modifier events first
            GunModifierHandler.TriggerReloadZoneHit(this, player, BulletDisplay, zoneIndex, clicksLeft);

            // Call the overridable method for gun-specific logic
            OnReloadZoneHit(player, zoneIndex, clicksLeft);
        }

        /// <summary>
        /// Override this method to add gun-specific reload zone hit effects.
        /// Modifier events are automatically triggered before this method.
        /// </summary>
        protected virtual void OnReloadZoneHit(Player player, int zoneIndex, int clicksLeft) { }

        /// <summary>
        /// Handles reload success with automatic modifier triggering.
        /// Override OnReloadSuccessCore for gun-specific logic.
        /// </summary>
        public void OnReloadEventSuccess(Player player)
        {
            // Call overridable method for gun-specific logic
            OnReloadSuccessCore(player);
        }

        /// <summary>
        /// Override this method to add gun-specific reload success effects.
        /// </summary>
        protected virtual void OnReloadSuccessCore(Player player) { }

        /// <summary>
        /// Handles reload failure with automatic modifier triggering.
        /// Override OnReloadFailCore for gun-specific logic.
        /// </summary>
        public void OnReloadEventFail(Player player)
        {
            // Call overridable method for gun-specific logic
            OnReloadFailCore(player);
        }

        /// <summary>
        /// Override this method to add gun-specific reload failure effects.
        /// </summary>
        protected virtual void OnReloadFailCore(Player player) { }

        public List<BulletObject> BulletDisplay = new();

        #region Ammo Drawing
        private void DrawAmmo()
        {
            if (Main.gamePaused || Main.LocalPlayer != Main.player[Projectile.owner]) return;

            string bulletTexture = GetBulletTexture();
            float textureWidth = ModContent.Request<Texture2D>(AssetDirectory.GunUI + bulletTexture).Value.Width;

            // Count active bullets for display purposes
            int activeBullets = 0;
            foreach (var bullet in BulletDisplay)
            {
                if (bullet.isActive) activeBullets++;
            }

            int bulletCounts = activeBullets % 10;
            if (bulletCounts == 0 && activeBullets > 0) bulletCounts = 10;

            float gapOffset = 6 * Utils.Clamp(bulletCounts - 1, 0, MaxShots);
            float total = textureWidth * bulletCounts + gapOffset;

            float startOffset = bulletTexture == "GunBullet_Shotgun" ? 12 : 8;
            float startPosition = (-total / 2) + startOffset;

            // Draw only the active bullets
            var offsetCounter = 0;
            foreach (var bullet in BulletDisplay)
            {
                if (!bullet.isActive) continue;

                bullet.Update();

                Vector2 offset = new Vector2(startPosition + 18 * offsetCounter, 42);
                bullet.Draw(Main.spriteBatch, player.Center + offset);

                offsetCounter++;
            }

            DrawAmmoCounter(startPosition, bulletCounts);
        }

        private void DrawAmmoCounter(float startPosition, int bulletCounts)
        {
            if (BulletDisplay.Count > 10)
            {
                Texture2D xTexture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "OverflowDisplay_X").Value;

                Vector2 counterOffset = new Vector2(startPosition + 18 * bulletCounts, 42);
                Main.spriteBatch.Draw(xTexture, player.Center + counterOffset - Main.screenPosition, null, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                Texture2D counterTexture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "OverflowDisplay_Numbers").Value;
                int counterTextureWidth = counterTexture.Width / 10;

                int initialCount = BulletDisplay.Count - 1;
                int firstPlace = GetPlace(initialCount, 100);

                counterOffset = new Vector2(startPosition + 18 * (bulletCounts + 1), 40);
                Rectangle drawRectangle = new Rectangle(counterTextureWidth * firstPlace, 0, 14, counterTexture.Height);
                Main.spriteBatch.Draw(counterTexture, player.Center + counterOffset - Main.screenPosition, drawRectangle, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                int secondPlace = GetPlace(initialCount, 10);

                counterOffset = new Vector2(startPosition + 18 * (bulletCounts + 2), 40);
                drawRectangle = new Rectangle(counterTextureWidth * secondPlace, 0, 14, counterTexture.Height);
                Main.spriteBatch.Draw(counterTexture, player.Center + counterOffset - Main.screenPosition, drawRectangle, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }
        #endregion

        public float reloadRotation = 0;

        public virtual bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor) { return true; }

        private void DrawGun(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            // Handle revolver spin effect
            if (WeaponType == WeaponType.Revolver)
            {
                if (reloadDelay > 0 && reloadSuccess)
                {
                    float spinRate = MathHelper.Lerp(0.09f, 0.99f, reloadDelay / 30f);
                    reloadRotation -= spinRate * player.direction;
                }
                else
                    reloadRotation = 0;
            }

            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, lightColor,
                Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);
        }

        public virtual void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime) { }

        private void DrawReloadBar()
        {
            float scale = 1;
            if (clickDelay > 0) scale = MathHelper.Lerp(1.25f, 1f, 1 - (clickDelay / 15f));

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "GunReloadFrame").Value;
            Vector2 offset = new Vector2(-2, 41);
            Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            foreach (ReloadZone clickZone in ClickZones)
            {
                // Calculate the actual pixel positions for this zone
                float zoneStartPixel = (clickZone.StartPercentage / 100f) * texture.Width;
                float zoneEndPixel = (clickZone.EndPercentage / 100f) * texture.Width;
                float zoneWidthPixels = zoneEndPixel - zoneStartPixel;

                Vector2 zoneOffset = new Vector2(-texture.Width / 2f + zoneStartPixel, 41f);
                Vector2 zonePosition = player.Center + zoneOffset;

                Texture2D clickTexture = clickZone.HasClicked ?
                    ModContent.Request<Texture2D>(AssetDirectory.GunUI + "ReloadZone_Clicked").Value :
                    ModContent.Request<Texture2D>(AssetDirectory.GunUI + "ReloadZone").Value;

                // Create a rectangle for the specific zone width
                Rectangle drawRectangle = new Rectangle(0, 0, (int)zoneWidthPixels, clickTexture.Height);

                Main.spriteBatch.Draw(clickTexture, zonePosition - Main.screenPosition, drawRectangle, Color.White, 0f,
                    new Vector2(0, clickTexture.Height / 2f), scale, SpriteEffects.None, 1);
            }

            // Draw reload cursor
            float cursorProgress = (1 - (float)reloadTime / MaxReloadTime) * texture.Width;
            Texture2D cursor = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "ReloadCursor").Value;
            Vector2 cursorOffset = new Vector2(-texture.Width / 2f + cursorProgress, 42.5f);
            Vector2 cursorPosition = player.Center + cursorOffset;
            Main.spriteBatch.Draw(cursor, cursorPosition - Main.screenPosition, null, Color.White, 0f, cursor.Size() / 2f, scale, SpriteEffects.None, 1);

            // Draw bullet icon
            string bulletTexture = GetBulletTexture();
            Texture2D bullet = ModContent.Request<Texture2D>(AssetDirectory.GunUI + bulletTexture).Value;
            Vector2 bulletPosition = player.Center + new Vector2(-texture.Width / 2f - 2, 40f);
            Main.spriteBatch.Draw(bullet, bulletPosition - Main.screenPosition, null, Color.White, 0f, bullet.Size() / 2f, scale, SpriteEffects.None, 1);
        }

        private string GetBulletTexture()
        {
            return WeaponType.GetDefaultBulletTexture();
        }

        #region Helper Methods
        public int GetPlace(int value, int place)
        {
            return ((value % (place * 10)) - (value % place)) / place;
        }

        private void LoadGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                ShotsFired = gunPlayer.playerGunInfo[ParentItem].shotsFired;
            }
        }

        private void SaveGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (!gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                gunPlayer.playerGunInfo.Add(ParentItem, new HeldGunInfo(ShotsFired, CurrentStats.BonusBullets, CurrentStats.BonusDamage, CurrentStats.BonusAmmo));
            }
            else
            {
                gunPlayer.playerGunInfo[ParentItem] = new HeldGunInfo(ShotsFired, CurrentStats.BonusBullets, CurrentStats.BonusDamage, CurrentStats.BonusAmmo);
            }
        }

        private bool FindAmmo()
        {
            LoadedBulletItemType = -1;
            if (ConvertBullet != ItemID.None)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    if (item.type == ConvertBullet)
                    {
                        LoadedBulletType = BulletType;
                        LoadedBulletItemType = item.type;
                        AmmoSlotID = 54 + i;
                        return true;
                    }
                }
            }

            if (LoadedBulletItemType == -1)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    LoadedBulletType = item.shoot;
                    LoadedBulletItemType = item.type;
                    AmmoSlotID = 54 + i;
                    return true;
                }
            }

            return false;
        }

        private bool ShouldSaveAmmo()
        {
            float totalSaveChance = Math.Min(CurrentStats.AmmoSaveChance, 100f);
            return Main.rand.NextFloat(0f, 100f) < totalSaveChance;
        }

        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessMusketPouch)
                player.inventory[AmmoSlotID].stack--;
        }

        private void ReloadBulletDisplay()
        {
            BulletDisplay.Clear();

            for (int i = 0; i < MaxShots; i++)
            {
                BulletDisplay.Add(new BulletObject(GetBulletTexture(), Main.rand.Next(0, 9) * 7));
            }
        }

        private void ForceCorrectBulletDisplay()
        {
            while (BulletDisplay.Count > MaxShots)
            {
                BulletDisplay.RemoveAt(BulletDisplay.Count - 1);
            }
        }

        public void UpdateBulletDisplay()
        {
            List<BulletObject> activeBullets = new List<BulletObject>();

            foreach (BulletObject bullet in BulletDisplay)
            {
                if (bullet.isActive)
                {
                    activeBullets.Add(bullet);
                }
            }

            BulletDisplay = activeBullets;
        }

        public void PopBulletDisplay()
        {
            for (int i = BulletDisplay.Count - 1; i >= 0; i--)
            {
                if (BulletDisplay[i].isActive && !BulletDisplay[i].startDeath)
                {
                    BulletDisplay[i].Deactivate();
                    return;
                }
            }
        }

        private bool CheckEventSuccess()
        {
            int clickedCount = 0;
            for (int i = 0; i < ClickZones.Count; i++)
            {
                bool clicked = ClickZones[i].HasClicked;
                if (clicked) clickedCount++;
            }

            bool success = clickedCount == ClickZones.Count;
            return success;
        }

        private int GetClicksLeft()
        {
            var numLeft = ClickZones.Count;
            foreach (ReloadZone clickZone in ClickZones)
            {
                if (clickZone.HasClicked) numLeft--;
            }

            return numLeft;
        }

        private void ResetReloadZones()
        {
            // Print the call stack to see where this is being called from
            var stackTrace = new System.Diagnostics.StackTrace(true);
            for (int i = 0; i < Math.Min(5, stackTrace.FrameCount); i++)
            {
                var frame = stackTrace.GetFrame(i);
            }

            foreach (ReloadZone clickZone in ClickZones)
            {
                clickZone.HasClicked = false;
            }
        }

        private bool CheckInZone(float clickPercentage, out int zoneIndex)
        {
            for (int i = 0; i < ClickZones.Count; i++)
            {
                var zone = ClickZones[i];
                if (!zone.HasClicked && clickPercentage >= zone.StartPercentage && clickPercentage <= zone.EndPercentage)
                {
                    zoneIndex = i;
                    return true;
                }
            }

            zoneIndex = -1;
            return false;
        }

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
        #endregion
    }
}