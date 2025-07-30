using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Daggers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract class HeldDagger : ModProjectile, IProjectileClassification
    {
        public WeaponType WeaponType => WeaponType.Dagger;

        public abstract int ParentItem { get; }
        public abstract int ThrownProjectile { get; }
        public virtual DaggerStats GetBaseDaggerStats() => new DaggerStats();

        // Core state tracking
        protected Player player => Main.player[Projectile.owner];
        protected DaggerStats currentStats;
        protected DaggerPlayer daggerPlayer;

        // AI references
        public ref float ComboIndex => ref Projectile.ai[0];
        public ref float DualWieldFlag => ref Projectile.ai[1];
        public ref float AICounter => ref Projectile.ai[2];

        // State variables
        public float HoldCounter = 0;
        public float ChargeProgress => Math.Min(HoldCounter / currentStats.ThrowMaxChargeTime, 1f);
        public bool IsChargedThrow => HoldCounter >= currentStats.ThrowChargeTime;

        private bool justReleasedWeapon = false;
        private bool inSwingState = false;
        private Vector2 lastMousePosition;
        private int lockedDirection = 1;
        private int flashCounter = 0;
        private Vector2 positionOffset;
        private float swingAngle = 0;

        public bool isDualWielding => player.HeldItem.type == ParentItem &&
                                     player.HeldItem.stack == 2 &&
                                     player.ownedProjectileCounts[ThrownProjectile] < 1;

        public override bool? CanHitNPC(NPC target) => !target.friendly && inSwingState;

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public sealed override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem || !player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            // Update stats and player reference
            currentStats = DaggerModifierHandler.GetModifiedStats(GetBaseDaggerStats(), player);
            daggerPlayer = player.GetModPlayer<DaggerPlayer>();

            if (player.active && player.HeldItem.type == ParentItem)
                Projectile.timeLeft = 10;

            Projectile.width = Projectile.height = 40;

            if (DualWieldFlag != 1)
                player.heldProj = Projectile.whoAmI;

            // Set the combo index based on the current combo sequence and player's combo progress
            if (ComboIndex != (int)DaggerAttack.Throw && currentStats.ComboSequence.Count > 0)
            {
                int comboIndex = Math.Min(daggerPlayer.CurrentComboIndex, currentStats.ComboSequence.Count - 1);
                ComboIndex = (float)currentStats.ComboSequence[comboIndex];
            }

            HandleArmDrawing();
            HandleWeaponUse();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // Apply base charged throw bonus
            if (IsChargedThrow)
            {
                modifiers.SourceDamage *= 1.5f;
                modifiers.Knockback *= 2f;
            }

            // Apply stat modifiers
            modifiers.SourceDamage *= currentStats.DamageMultiplier;
            modifiers.Knockback *= currentStats.KnockbackMultiplier;

            // Apply dual wield modifier
            if (isDualWielding)
                modifiers.SourceDamage *= currentStats.DualWieldDamageMultiplier;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (DualWieldFlag == 1)
            {
                Projectile.hide = true;
                behindProjectiles.Add(index);
            }
        }

        private void HandleWeaponHold()
        {
            if (HoldCounter < currentStats.ThrowMaxChargeTime)
                HoldCounter++;

            if (HoldCounter > currentStats.ThrowChargeTime && flashCounter <= 15)
                flashCounter++;

            if (flashCounter == 5)
                SoundEngine.PlaySound(currentStats.ChargeSound);

            if (AICounter <= GetBackTime())
            {
                AICounter++;
                switch (ComboIndex)
                {
                    case (int)DaggerAttack.Throw:
                        swingAngle = MathHelper.Lerp(0, 105, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, GetBackTime()) / GetBackTime()));
                        break;
                    default:
                        Vector2 baseOffset = player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6);
                        positionOffset = (baseOffset + currentStats.SlashPositionOffset).RotatedBy(Projectile.rotation);
                        swingAngle = MathHelper.Lerp(0, -135, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, GetBackTime()) / GetBackTime()));
                        break;
                }
            }
        }

        private void HandleWeaponRelease()
        {
            if (!justReleasedWeapon)
            {
                flashCounter = 0;
                justReleasedWeapon = true;
                lastMousePosition = Main.MouseWorld;
            }

            HandlePositioning();
            HandleAttackAngles();
            AICounter++;

            // Check for attack completion
            if (AICounter >= GetBackTime() + GetForwardTime() + GetHoldTime())
            {
                CompleteAttack();
            }
        }

        private void HandlePositioning()
        {
            switch (ComboIndex)
            {
                case (int)DaggerAttack.Stab:
                    HandleStabPositioning();
                    break;
                default:
                    Vector2 baseOffset = player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6);
                    positionOffset = (baseOffset + currentStats.SlashPositionOffset).RotatedBy(Projectile.rotation);
                    break;
            }
        }

        private void HandleStabPositioning()
        {
            if (AICounter == 0)
            {
                lockedDirection = Main.MouseWorld.X > player.Center.X ? 1 : -1;
            }

            player.direction = lockedDirection;

            float xOffset = 0f;
            float backTime = GetBackTime();
            float forwardTime = GetForwardTime();
            float holdTime = GetHoldTime();

            if (AICounter <= backTime)
                xOffset = MathHelper.Lerp(8, 4, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

            if (AICounter > backTime && AICounter <= backTime + forwardTime)
            {
                if (!inSwingState)
                {
                    SoundEngine.PlaySound(currentStats.StabSound, player.Center);
                    float stabCounter = AICounter - backTime - 1;
                    if (stabCounter == 0)
                        Projectile.rotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.ToRadians(swingAngle) * -player.direction;
                    OnDaggerStab(stabCounter);
                }

                inSwingState = true;
                xOffset = MathHelper.Lerp(4, 24, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
            }

            if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                xOffset = MathHelper.Lerp(24, 6, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

            positionOffset = new Vector2(xOffset, 0) + currentStats.StabPositionOffset;
        }

        private void HandleAttackAngles()
        {
            float backTime = GetBackTime();
            float forwardTime = GetForwardTime();
            float holdTime = GetHoldTime();

            switch (ComboIndex)
            {
                case (int)DaggerAttack.Throw:
                    HandleThrowAngles(backTime, forwardTime, holdTime);
                    break;
                case (int)DaggerAttack.Slash:
                    HandleSlashAngles(backTime, forwardTime, holdTime);
                    break;
                case (int)DaggerAttack.Stab:
                    // Stab angles are handled in positioning
                    break;
                default:
                    HandleDefaultAngles(backTime, forwardTime, holdTime);
                    break;
            }
        }

        private void HandleThrowAngles(float backTime, float forwardTime, float holdTime)
        {
            if (AICounter <= backTime)
                swingAngle = MathHelper.Lerp(0, 105, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

            if (AICounter > backTime && AICounter <= backTime + forwardTime)
            {
                if (!inSwingState)
                    SoundEngine.PlaySound(currentStats.ThrowSound, player.Center);

                inSwingState = true;
                swingAngle = MathHelper.Lerp(105, 15, EasingUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
            }

            if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
            {
                inSwingState = false;
                swingAngle = MathHelper.Lerp(15, -45, EasingUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
            }

            if (AICounter >= Math.Floor(backTime + forwardTime + holdTime))
            {
                ExecuteThrow();
            }
        }

        private void HandleSlashAngles(float backTime, float forwardTime, float holdTime)
        {
            if (AICounter <= backTime)
                swingAngle = MathHelper.Lerp(-45, 95, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

            if (AICounter > backTime && AICounter <= backTime + forwardTime)
            {
                if (!inSwingState)
                    SoundEngine.PlaySound(currentStats.SlashSound, player.Center);

                inSwingState = true;
                swingAngle = MathHelper.Lerp(95, -75, EasingUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
            }

            if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
            {
                inSwingState = false;
                swingAngle = MathHelper.Lerp(-75, -25, EasingUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
            }
        }

        private void HandleDefaultAngles(float backTime, float forwardTime, float holdTime)
        {
            if (AICounter <= backTime)
                swingAngle = MathHelper.Lerp(-45, -105, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

            if (AICounter > backTime && AICounter <= backTime + forwardTime)
            {
                if (!inSwingState)
                    SoundEngine.PlaySound(currentStats.SlashSound, player.Center);

                inSwingState = true;
                swingAngle = MathHelper.Lerp(-105, 35, EasingUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
            }

            if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
            {
                inSwingState = false;
                swingAngle = MathHelper.Lerp(35, -45, EasingUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
            }
        }

        private void ExecuteThrow()
        {
            Vector2 throwVelocity = Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld)) * currentStats.ThrowVelocity;

            // Allow modifiers to adjust throw velocity
            DaggerModifierHandler.ModifyThrowVelocity(this, player, ref throwVelocity, ChargeProgress);

            Projectile proj = Projectile.NewProjectileDirect(null, Projectile.Center, throwVelocity, ThrownProjectile,
                (int)(Projectile.damage * currentStats.ThrowDamageMultiplier), Projectile.knockBack, Projectile.owner, 0, Projectile.rotation);

            proj.CritChance = Projectile.CritChance;

            // Check for special throw behaviors
            if (!DaggerModifierHandler.HandleSpecialThrow(this, player, throwVelocity, ref proj))
            {
                // Default throw behavior
                ThrownDagger thrownDagger = proj.ModProjectile as ThrownDagger;
                if (thrownDagger != null)
                {
                    thrownDagger.isFocusShot = IsChargedThrow;
                    if (IsChargedThrow)
                    {
                        proj.CritChance += (int)currentStats.ThrowCritBonus;
                        DaggerModifierHandler.TriggerChargedThrow(this, player, thrownDagger);
                    }
                }
            }

            daggerPlayer.RecordThrow();
            Projectile.Kill();
        }

        private void CompleteAttack()
        {
            // Record hit and advance combo if this was a successful attack
            //if (inSwingState && ComboIndex != (int)DaggerAttack.Throw)
            //{
            //    daggerPlayer.AdvanceCombo(currentStats.ComboSequence);
            //}
            // For combo attacks, check if player wants to continue
            if (ComboIndex != (int)DaggerAttack.Throw)
            {
                // If player is still trying to attack, continue seamlessly
                if (player.controlUseItem)
                {
                    // Reset animation state for next attack
                    float dualWieldOffset = DualWieldFlag == 1 ? -5f : 0f;
                    AICounter = dualWieldOffset;
                    
                    HoldCounter = 0;
                    swingAngle = 0;
                    justReleasedWeapon = false;
                    inSwingState = false;

                    // Advance combo (handles single or multi-attack sequences)
                    daggerPlayer.AdvanceCombo(currentStats.ComboSequence);

                    // Reset item timing
                    player.itemTime = player.HeldItem.useTime;
                    player.itemAnimation = player.HeldItem.useAnimation;

                    return; // Continue attacking seamlessly
                }
            }

            // Kill projectile when player stops attacking or after throws
            Projectile.Kill();
        }

        // Timing calculation methods
        private float GetBackTime()
        {
            return ComboIndex switch
            {
                (int)DaggerAttack.Stab => currentStats.StabBackTime * (DualWieldFlag == 1 ? 0.8f : 1f),
                (int)DaggerAttack.Slash => currentStats.SlashBackTime * (DualWieldFlag == 1 ? 0.8f : 1f),
                _ => DualWieldFlag == 1 ? 5 : 15
            } / currentStats.SpeedMultiplier;
        }

        private float GetForwardTime()
        {
            return ComboIndex switch
            {
                (int)DaggerAttack.Throw => 4,
                (int)DaggerAttack.Stab => currentStats.StabForwardTime,
                (int)DaggerAttack.Slash => currentStats.SlashForwardTime,
                _ => DualWieldFlag == 1 ? 10 : 5
            } / currentStats.SpeedMultiplier;
        }

        private float GetHoldTime()
        {
            return ComboIndex switch
            {
                (int)DaggerAttack.Stab => currentStats.StabHoldTime,
                (int)DaggerAttack.Slash => currentStats.SlashHoldTime,
                (int)DaggerAttack.Throw => 4,
                _ => 10
            } / currentStats.SpeedMultiplier;
        }

        private void HandleArmDrawing()
        {
            Vector2 mousePosition = justReleasedWeapon ? lastMousePosition : Main.MouseWorld;
            Projectile.spriteDirection = mousePosition.X < player.Center.X ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            if (ComboIndex != (int)DaggerAttack.Stab || !inSwingState)
            {
                float weaponRotation = player.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -player.direction;
                Projectile.rotation = weaponRotation;
            }

            float backRotation = player.direction == -1 ? -150 : -30;

            switch (ComboIndex)
            {
                case (int)DaggerAttack.Stab:
                    HandleStabArmDrawing(backRotation);
                    break;
                default:
                    if (DualWieldFlag == 1)
                        player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                    else
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                    break;
            }
        }

        private void HandleStabArmDrawing(float backRotation)
        {
            float backTime = GetBackTime();
            float forwardTime = GetForwardTime();
            float holdTime = GetHoldTime();

            if (AICounter < backTime + 2)
            {
                float progress = MathHelper.Lerp(0, 100, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                var stretchAmount = progress < 50 ? Player.CompositeArmStretchAmount.ThreeQuarters : Player.CompositeArmStretchAmount.Quarter;

                player.SetCompositeArmBack(true, stretchAmount, Projectile.rotation + MathHelper.ToRadians(backRotation));
                player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation + MathHelper.ToRadians(-90));
            }

            if (justReleasedWeapon)
            {
                if (AICounter > backTime && AICounter <= backTime + forwardTime)
                {
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(backRotation));
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                }

                if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                {
                    float progress = MathHelper.Lerp(0, 100, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                    var stretchAmount = progress < 50 ? Player.CompositeArmStretchAmount.Quarter : Player.CompositeArmStretchAmount.ThreeQuarters;

                    player.SetCompositeArmBack(true, stretchAmount, Projectile.rotation + MathHelper.ToRadians(backRotation));
                    player.SetCompositeArmFront(true, stretchAmount, Projectile.rotation + MathHelper.ToRadians(-90));
                }
            }
        }

        private void HandleWeaponUse()
        {
            if (ComboIndex == (int)DaggerAttack.Throw)
            {
                if (Main.mouseRight && !justReleasedWeapon && currentStats.CanThrow)
                    HandleWeaponHold();
                else if (HoldCounter > 0)
                    HandleWeaponRelease();
            }
            else
                HandleWeaponRelease();

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
        }

        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var modifiers = new NPC.HitModifiers();

            switch (ComboIndex)
            {
                case (int)DaggerAttack.Stab:
                    if (AICounter > GetBackTime() && AICounter <= GetBackTime() + GetForwardTime())
                    {
                        DaggerModifierHandler.TriggerStabHit(this, player, target, ref modifiers);
                        OnDaggerStabHit();
                    }
                    break;
                case (int)DaggerAttack.Slash:
                    DaggerModifierHandler.TriggerSlashHit(this, player, target, ref modifiers);
                    OnDaggerSlashHit();
                    break;
            }

            daggerPlayer.RecordHit((DaggerAttack)ComboIndex, damageDone);
        }

        // Virtual methods for derived classes to override
        public virtual void OnDaggerStab(float stabCounter) { }
        public virtual void OnDaggerStabHit() { }
        public virtual void OnDaggerSlashHit() { }

        public sealed override bool PreDraw(ref Color lightColor)
        {
            HandleWeaponDrawing(lightColor);

            // Allow accessories to add additional draw effects
            foreach (var drawEffect in daggerPlayer.ActiveDrawEffects)
            {
                drawEffect.DrawDaggerEffects(this, player, Main.spriteBatch, Projectile.Center, (DaggerAttack)ComboIndex);

                if (IsChargedThrow)
                    drawEffect.DrawChargingEffects(this, player, Main.spriteBatch, ChargeProgress);
            }

            return false;
        }

        private void HandleWeaponDrawing(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 spritePositionOffset = Vector2.Zero;
            Vector2 dualWieldOffset = Vector2.Zero;
            float rotationOffset = 0f;
            float scaleFactor = DualWieldFlag == 1 ? 0.9f * currentStats.DualWieldDamageMultiplier : 1f;

            SetWeaponDrawing(ref spritePositionOffset, ref dualWieldOffset, ref rotationOffset, ref scaleFactor);
            if (ComboIndex == (int)DaggerAttack.Throw && HoldCounter > 0)
            {
                DrawThrowTrajectory();
            }

            if (IsChargedThrow && currentStats.ShowChargeEffect)
            {
                float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 5f), 0, 1);
                Color flashColor = Color.Lerp(lightColor, currentStats.FlashColor, flashProgress);

                // Could apply shader effect here if available
                Main.spriteBatch.Draw(texture, Projectile.Center + spritePositionOffset - Main.screenPosition, null,
                    flashColor, Projectile.rotation + rotationOffset, texture.Size() / 2f,
                    Projectile.scale * scaleFactor * currentStats.ScaleMultiplier, spriteEffects, 1);
            }
            else
            {
                Main.spriteBatch.Draw(texture, Projectile.Center + spritePositionOffset - Main.screenPosition, null,
                    lightColor, Projectile.rotation + rotationOffset, texture.Size() / 2f,
                    Projectile.scale * scaleFactor * currentStats.ScaleMultiplier, spriteEffects, 1);
            }
        }

        private void DrawThrowTrajectory()
        {
            Vector2 startPos = Projectile.Center;
            Vector2 throwVelocity = Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld)) * currentStats.ThrowVelocity;

            // Allow modifiers to adjust trajectory prediction
            DaggerModifierHandler.ModifyThrowVelocity(this, player, ref throwVelocity, ChargeProgress);

            Vector2 currentPos = startPos;
            Vector2 currentVel = throwVelocity;

            float alpha = IsChargedThrow ? 1f : 0.5f;
            Color trajectoryColor = Color.Lerp(Color.Gray, currentStats.ThrowTrailColor, ChargeProgress);

            // Draw trajectory points
            for (int i = 0; i < 60; i++)
            {
                Vector2 nextPos = currentPos + currentVel;

                // Apply physics simulation
                currentVel.X *= 0.99f; // Air resistance
                if (i > 6) currentVel.Y += 0.25f; // Gravity after initial throw

                // Draw trajectory dot
                float dotAlpha = alpha * (1f - i / 30f); // Fade out over distance
                Vector2 screenPos = currentPos - Main.screenPosition;

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, screenPos, new Rectangle(0, 0, 2, 2),
                    trajectoryColor * dotAlpha, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

                currentPos = nextPos;

                //// Stop if trajectory goes off screen or hits ground level
                //if (currentPos.Y > Main.worldSurface * 16 || !Main.screenPosition.WithinRange(currentPos, Main.screenWidth + 100))
                //    break;
            }
        }

        public virtual void SetWeaponDrawing(ref Vector2 spritePositionOffset, ref Vector2 dualWieldOffset, ref float rotationOffset, ref float scaleFactor) { }

        public sealed override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            Vector2 hitboxOffset = Vector2.Zero;
            SetDamageHitbox(positionOffset, ref hitboxOffset, ref hitbox);
        }

        public virtual void SetDamageHitbox(Vector2 positionOffset, ref Vector2 hitboxOffset, ref Rectangle hitbox) { }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HoldCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HoldCounter = reader.ReadSingle();
        }
    }
}