using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Bows;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Weapons.Bows
{
    public abstract class HeldBow : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        protected BowStats currentStats;
        protected BowStats baseStats;

        /// <summary>
        /// Defines the base statistics for this bow before any modifiers are applied.
        /// Override this method to set the bow's default behavior and appearance.
        /// </summary>
        /// <returns>A BowStats instance containing the bow's base properties.</returns>
        public virtual BowStats GetBaseBowStats() => new BowStats();
        public virtual SoundStyle DrawbackSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/BowCharge");
        public virtual SoundStyle ShootSound => SoundID.Item5;

        /// <summary>
        /// The item type that creates this bow projectile. Used to determine when to despawn the projectile.
        /// </summary>
        public abstract int ParentItem { get; }

        /// <summary>
        /// Override this method to set bow-specific defaults like damage type and size.
        /// This is called after the base bow setup is complete.
        /// </summary>
        public virtual void SafeSetDefaults() { }

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1;

            baseStats = GetBaseBowStats();
            SafeSetDefaults();
        }

        public Projectile LoadedArrow { private set; get; }
        public int LoadedArrowType { private set; get; }
        public int LoadedArrowItemType { private set; get; }

        private int AmmoSlotID;
        protected int flashCounter = 0;

        public Player player => Main.player[Projectile.owner];
        public ref float drawCounter => ref Projectile.ai[0];
        public ref float delayCounter => ref Projectile.ai[1];

        /// <summary>
        /// Override this method to provide a custom arrow texture for drawing while charging.
        /// Return null to use the default arrow texture.
        /// </summary>
        /// <param name="defaultTexture">The default arrow texture that would be used</param>
        /// <param name="isPowerShot">Whether this is currently a power shot</param>
        /// <returns>Custom texture to use, or null to use default</returns>
        protected virtual Texture2D GetCustomArrowTexture(Texture2D defaultTexture, bool isPowerShot)
        {
            return null;
        }

        /// <summary>
        /// Draws visual effects while the bow is being charged.
        /// Override this method to add bow-specific charging effects like particles or glows.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with.</param>
        /// <param name="chargeProgress">Charging progress from 0.0 (not charged) to 1.0 (fully charged).</param>
        public virtual void DrawChargingEffects(SpriteBatch spriteBatch, float chargeProgress) { }

        /// <summary>
        /// Draws visual effects on the loaded arrow.
        /// Override this method to add bow-specific arrow effects like trails or glows.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with.</param>
        /// <param name="arrowPosition">World position of the arrow.</param>
        /// <param name="chargeProgress">Charging progress from 0.0 (not charged) to 1.0 (fully charged).</param>
        public virtual void DrawArrowEffects(SpriteBatch spriteBatch, Vector2 arrowPosition, float chargeProgress) { }

        /// <summary>
        /// Draws visual effects before the actual bow drawing.
        /// Override this method to add bow-specific effects like auras or overlays.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with.</param>
        /// <param name="bowPosition">World position of the bow.</param>
        /// <param name="chargeProgress">Charging progress from 0.0 (not charged) to 1.0 (fully charged).</param>
        public virtual void PreDrawBowEffects(SpriteBatch spriteBatch, Vector2 bowPosition, float chargeProgress) { }

        /// <summary>
        /// Draws visual effects on the bow itself.
        /// Override this method to add bow-specific effects like auras or overlays.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to draw with.</param>
        /// <param name="bowPosition">World position of the bow.</param>
        /// <param name="chargeProgress">Charging progress from 0.0 (not charged) to 1.0 (fully charged).</param>
        public virtual void PostDrawBowEffects(SpriteBatch spriteBatch, Vector2 bowPosition, float chargeProgress) { }

        /// <summary>
        /// Called every frame during AI updates while the bow is being charged.
        /// Override this method to add particle effects, lighting, or other continuous charging effects.
        /// </summary>
        /// <param name="chargeProgress">Charging progress from 0.0 (not charged) to 1.0 (fully charged).</param>
        public virtual void UpdateChargingEffects(float chargeProgress) { }

        /// <summary>
        /// Called every frame during AI updates when the bow is fully charged and ready for a power shot.
        /// Override this method to add continuous effects that indicate a power shot is ready.
        /// </summary>
        public virtual void UpdatePowerShotReadyEffects() { }

        /// <summary>
        /// Determines which arrow type to fire based on the loaded arrow and shot conditions.
        /// Override this method to customize arrow selection behavior for specific bows.
        /// </summary>
        /// <param name="defaultArrowType">The arrow type that would normally be fired based on loaded ammo</param>
        /// <param name="isPowerShot">Whether this shot is a fully charged power shot</param>
        /// <returns>The projectile type ID of the arrow to actually fire</returns>
        protected virtual int GetArrowTypeForShot(int defaultArrowType, bool isPowerShot)
        {
            return defaultArrowType;
        }

        /// <summary>
        /// Called immediately after an arrow projectile is created and configured.
        /// Override this method to add custom behavior like firing additional arrows,
        /// applying special effects, or modifying the fired arrow's properties.
        /// </summary>
        /// <param name="arrow">The arrow projectile that was just created</param>
        /// <param name="isPowerShot">Whether this shot was a fully charged power shot</param>
        protected virtual void OnArrowFired(Projectile arrow, bool isPowerShot) { }

        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem || !player.active || player.dead)
                Projectile.Kill();
            else
                Projectile.timeLeft = 5;

            player.heldProj = Projectile.whoAmI;

            currentStats = BowModifierHandler.GetModifiedStats(baseStats, player);

            HandlePlayerDrawing();
            HandleBowUse();

            if (drawCounter > 0)
            {
                float chargeProgress = Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime;
                UpdateChargingEffects(chargeProgress);

                if (drawCounter >= ModifiedChargeTime)
                {
                    UpdatePowerShotReadyEffects();
                }
            }
        }

        //private float PracticeTargetModifier => currentStats.MaxChargeTime * (0.05f * player.GetModPlayer<BowPlayer>().PracticeTargetCounter);
        private int ModifiedChargeTime => (int)Math.Ceiling(currentStats.MaxChargeTime < 6 ? 6 : currentStats.MaxChargeTime);

        /// <summary>
        /// Handles player arm positioning and bow rotation based on mouse position.
        /// </summary>
        private void HandlePlayerDrawing()
        {
            float bowRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Projectile.rotation = bowRotation;
            Projectile.spriteDirection = bowRotation > MathHelper.PiOver2 || bowRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = currentStats.PositionOffset.RotatedBy(bowRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            Player.CompositeArmStretchAmount stretchAmount = drawCounter < Math.Round(ModifiedChargeTime * 0.33) ? Player.CompositeArmStretchAmount.Full : Player.CompositeArmStretchAmount.Quarter;
            if (drawCounter > Math.Round(ModifiedChargeTime * 0.66f)) stretchAmount = Player.CompositeArmStretchAmount.None;

            player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation - MathHelper.PiOver2);
        }

        /// <summary>
        /// Handles bow charging, firing, and delay mechanics.
        /// </summary>
        private void HandleBowUse()
        {
            if (delayCounter > 0) delayCounter--;

            if (player.controlUseItem && drawCounter >= 0)
            {
                Projectile.timeLeft = 120;

                if (delayCounter != 0) return;

                if (drawCounter == 0)
                {
                    if (FindAmmo()) SoundEngine.PlaySound(DrawbackSound);
                    ModUtils.AutofillAmmoSlots(player, AmmoID.Arrow);
                }

                if (FindAmmo())
                {
                    if (drawCounter > ModifiedChargeTime) drawCounter = ModifiedChargeTime;
                    drawCounter += currentStats.ChargeSpeed;

                    player.itemTime = 2;
                    player.itemAnimation = 2;
                }
            }
            else
            {
                if (drawCounter > 0)
                {
                    ShootArrow();
                    drawCounter = -6;

                    player.itemTime = 30;
                    player.itemAnimation = 30;

                    flashCounter = 0;
                }

                if (drawCounter < 0) drawCounter++;
            }
        }

        /// <summary>
        /// Handles arrow firing, damage calculation, and effect triggering.
        /// </summary>
        private void ShootArrow()
        {
            bool isPowerShot = IsPowerShot();

            float progress = Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime;
            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, progress).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.Center + arrowOffset;

            Vector2 velocity = Vector2.Normalize(player.Center.DirectionTo(Main.MouseWorld));
            float speed = MathHelper.Lerp(1, currentStats.MaxSpeed, progress);

            SoundEngine.PlaySound(ShootSound);

            float damage = MathHelper.Lerp(0.25f, 1, Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime) * Projectile.damage * currentStats.DamageMultiplier;

            float speedBonus = isPowerShot ? 1.5f : 1f;

            // Allow derived classes to override arrow type selection
            int finalArrowType = GetArrowTypeForShot(LoadedArrowType, isPowerShot);

            // Create the main arrow projectile
            int arrow = Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, finalArrowType, "HeldBow"), arrowPosition, velocity * speed * speedBonus, finalArrowType, (int)damage, Projectile.knockBack * currentStats.KnockbackMultiplier, player.whoAmI);

            Projectile arrowProjectile = Main.projectile[arrow];

            if (isPowerShot)
            {
                BowModifierHandler.TriggerPowerShot(this, player);

                for (int i = 0; i < 4; i++)
                {
                    float randomScale = Main.rand.NextFloat(0.05f, 0.15f);
                    float angleOffset = Main.rand.Next(-3, 3) * 5;
                    Vector2 particleVelocity = -(velocity * Main.rand.Next(4, 8)).RotatedBy(MathHelper.ToRadians(-90 / 4 * i + 30 + angleOffset));
                    //Particle.CreateParticle(Particle.ParticleType<WhiteSpark>(), arrowPosition, particleVelocity, Color.White, randomScale, 0.5f, 0f, randomScale, 0f, 20f);
                }

                Main.projectile[arrow].GetGlobalProjectile<GlobalProjectiles>().IsPowerShot = true;

                //if (player.GetModPlayer<OvermorrowModPlayer>().CapturedMirage)
                //{
                //    MirageDummyProjectile mirage = Projectile.NewProjectileDirect(null, arrowPosition, velocity * speed * speedBonus, ModContent.ProjectileType<MirageDummyProjectile>(), (int)damage, Projectile.knockBack, player.whoAmI).ModProjectile as MirageDummyProjectile;
                //    mirage.mirageArrow = GetRandomArrow();
                //}
            }

            BowModifierHandler.TriggerArrowFired(this, player, arrowProjectile);

            // Allow derived classes to add custom behavior after arrow creation
            OnArrowFired(arrowProjectile, isPowerShot);

            ConsumeAmmo();

            delayCounter = currentStats.ShootDelay;
            LoadedArrowItemType = -1;
        }

        /// <summary>
        /// Consumes ammo from the player's inventory if allowed by current stats.
        /// </summary>
        private void ConsumeAmmo()
        {
            if (!currentStats.CanConsumeAmmo) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessQuiver)
                player.inventory[AmmoSlotID].stack--;
        }

        /// <summary>
        /// Searches the player's ammo slots for compatible arrows and loads them into the bow.
        /// </summary>
        /// <returns>True if ammo was found and loaded, false otherwise.</returns>
        private bool FindAmmo()
        {
            LoadedArrowItemType = -1;

            if (currentStats.ConvertArrow != ItemID.None)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    if (item.type == currentStats.ConvertArrow)
                    {
                        LoadedArrowType = currentStats.ArrowType;
                        LoadedArrowItemType = item.type;
                        AmmoSlotID = 54 + i;
                        return true;
                    }
                }
            }

            if (LoadedArrowItemType == -1)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                    LoadedArrowType = item.shoot;
                    LoadedArrowItemType = item.type;
                    AmmoSlotID = 54 + i;
                    return true;
                }
            }

            return false;
        }

        private int GetRandomArrow()
        {
            for (int i = 0; i <= 3; i++)
            {
                Item item = player.inventory[54 + i];
                if (item.type == ItemID.None || item.ammo != AmmoID.Arrow) continue;

                if (item.shoot != LoadedArrowType)
                {
                    return item.shoot;
                }
            }

            return LoadedArrowType;
        }

        /// <summary>
        /// Determines if the current shot will be a power shot based on charge time and flash counter.
        /// </summary>
        /// <returns>True if this shot is a power shot, false otherwise.</returns>
        private bool IsPowerShot() => flashCounter >= 6 && flashCounter <= 36;

        public override bool PreDraw(ref Color lightColor)
        {
            float chargeProgress = Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime;

            PreDrawBowEffects(Main.spriteBatch, Projectile.Center, chargeProgress);

            DrawBow(lightColor);
            DrawArrow(lightColor);

            // Draw bow-specific effects first
            PostDrawBowEffects(Main.spriteBatch, Projectile.Center, chargeProgress);

            // Draw arrow-specific effects
            if (LoadedArrowItemType != -1)
            {
                Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
                Vector2 arrowPosition = player.MountedCenter + arrowOffset;

                DrawArrowEffects(Main.spriteBatch, arrowPosition, chargeProgress);
            }

            // Draw charging effects
            if (drawCounter > 0)
            {
                DrawChargingEffects(Main.spriteBatch, chargeProgress);
            }

            // Draw all global and accessory effects LAST (so they appear on top)
            BowDrawEffectsHandler.DrawAllBowEffects(this, player, Main.spriteBatch, Projectile.Center, chargeProgress);

            if (LoadedArrowItemType != -1)
            {
                Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
                Vector2 arrowPosition = player.MountedCenter + arrowOffset;
                BowDrawEffectsHandler.DrawAllArrowEffects(this, player, Main.spriteBatch, arrowPosition, chargeProgress);
            }

            if (drawCounter > 0)
            {
                BowDrawEffectsHandler.DrawAllChargingEffects(this, player, Main.spriteBatch, chargeProgress);
            }

            return false;
        }

        private void DrawBow(Color lightColor)
        {
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 topPosition = Projectile.Center + currentStats.StringPositions.Item1.RotatedBy(Projectile.rotation);
            Vector2 bottomPosition = Projectile.Center + currentStats.StringPositions.Item2.RotatedBy(Projectile.rotation);

            Vector2 restingPosition = Vector2.UnitX * (drawCounter < 0 ? 12 : 10);
            Vector2 armOffset = Vector2.Lerp(restingPosition, Vector2.UnitX * -1, Utils.Clamp(drawCounter, 0, ModifiedChargeTime) / (float)ModifiedChargeTime).RotatedBy(Projectile.rotation);
            Vector2 armPosition = player.MountedCenter + armOffset;

            Color color = currentStats.StringGlow ? currentStats.StringColor : currentStats.StringColor * Lighting.Brightness((int)player.Center.X / 16, (int)player.Center.Y / 16);

            Utils.DrawLine(Main.spriteBatch, topPosition, armPosition, color, currentStats.StringColor, 1.25f);
            Utils.DrawLine(Main.spriteBatch, bottomPosition, armPosition, color, currentStats.StringColor, 1.25f);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, 1f, spriteEffects, 1);
        }

        private void DrawArrow(Color lightColor)
        {
            if (drawCounter >= ModifiedChargeTime)
            {
                if (flashCounter == 0) SoundEngine.PlaySound(SoundID.MaxMana);
                if (flashCounter < 48 && !Main.gamePaused) flashCounter++;
            }

            Vector2 arrowOffset = Vector2.Lerp(Vector2.UnitX * 20, Vector2.UnitX * 16, Utils.Clamp(drawCounter, 0, 40f) / 40f).RotatedBy(Projectile.rotation);
            Vector2 arrowPosition = player.MountedCenter + arrowOffset;

            if (LoadedArrowItemType == -1) return;

            if (LoadedArrowType == ProjectileID.FireArrow) Lighting.AddLight(arrowPosition, 1f, 0.647f, 0);
            if (LoadedArrowType == ProjectileID.FrostburnArrow) Lighting.AddLight(arrowPosition, 0f, 0.75f, 0.75f);
            if (LoadedArrowType == ProjectileID.CursedArrow) Lighting.AddLight(arrowPosition, 0.647f, 1f, 0f);

            Color color = LoadedArrowType == ProjectileID.JestersArrow ? Color.White : lightColor;

            Main.spriteBatch.Reload(SpriteSortMode.Immediate);

            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 12f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
            effect.Parameters["ColorFillColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["ColorFillProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["ColorFill"].Apply();

            Color lerpColor = Color.Lerp(color, Color.White, flashProgress);

            Main.instance.LoadProjectile(LoadedArrowType);
            Texture2D defaultTexture = TextureAssets.Projectile[LoadedArrowType].Value;

            //Texture2D texture = TextureAssets.Projectile[LoadedArrowType].Value;
            bool isPowerShot = drawCounter >= ModifiedChargeTime;
            Texture2D finalTexture = GetCustomArrowTexture(defaultTexture, isPowerShot) ?? defaultTexture;

            Main.spriteBatch.Draw(finalTexture, arrowPosition + new Vector2(0, Projectile.gfxOffY) - Main.screenPosition, null, lerpColor, Projectile.rotation + MathHelper.PiOver2, finalTexture.Size() / 2f, 0.75f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);
        }
    }
}