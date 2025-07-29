using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Daggers;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract partial class ThrownDagger : ModProjectile, IProjectileClassification
    {
        public WeaponType WeaponType => WeaponType.Dagger;
        public override string Texture => AssetDirectory.Empty;

        // Required overrides for derived thrown daggers
        public abstract int ParentItem { get; }

        // Virtual properties that can be overridden
        public virtual Color IdleColor => Color.Orange;
        protected virtual bool canBePickedUp => true;
        protected virtual bool canShowDagger => true;

        // Enhanced properties using the stats system
        protected DaggerStats parentStats;
        protected Player owner => Main.player[Projectile.owner];

        public override bool? CanDamage() => !groundCollided;

        // Focus shot from the new system
        public bool isFocusShot = false;

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2; // Small hitbox to prevent early ground collision
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public override void AI()
        {
            // Get parent dagger stats for enhanced behavior
            if (parentStats == null)
            {
                // Try to get stats from a held dagger of the same type, or use defaults
                parentStats = GetParentDaggerStats();
            }

            // Handle slope collisions that don't trigger normal collision
            if (Projectile.CheckEntityBottomSlopeCollision())
                HandleCollisionBounce();

            // Initialize rotation from spawn
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Projectile.ai[1];
                Projectile.ai[1] = 0;
            }

            // Expand hitbox after initial throw phase
            if (Projectile.ai[0] > 10)
                Projectile.width = Projectile.height = 32;

            if (!groundCollided)
            {
                HandleFlightPhase();
            }
            else
            {
                HandleGroundPhase();
            }

            // Check for pickup by owner
            if (groundCollided && canBePickedUp)
            {
                CheckForPickup();
            }

            base.AI();
        }

        private void HandleFlightPhase()
        {
            // Air resistance
            Projectile.velocity.X *= 0.99f;

            // Spinning motion based on velocity direction
            Projectile.rotation += 0.48f * (Projectile.velocity.X > 0 ? 1 : -1);

            // Gravity after initial throw
            if (Projectile.ai[0]++ > 10)
                Projectile.velocity.Y += 0.25f;
        }

        private void HandleGroundPhase()
        {
            // Slow down horizontal movement
            Projectile.velocity.X *= 0.97f;

            // Landing stabilization
            if (Projectile.ai[1] == 60f)
            {
                Projectile.velocity.X *= 0.01f;
                oldPosition = Projectile.Center;
            }

            // Gradually reduce rotation
            float rotationFactor = MathHelper.Lerp(0.48f, 0f, Utils.Clamp(Projectile.ai[1]++, 0, 60f) / 60f);
            Projectile.rotation += rotationFactor * (Projectile.velocity.X > 0 ? 1 : -1);

            // Reduce vertical bouncing
            Projectile.velocity.Y *= 0.96f;

            // Floating animation after landing
            if (Projectile.ai[1] > 60f)
            {
                Projectile.tileCollide = false;
                float floatProgress = (Projectile.ai[1] - 60f) / 40f;
                Projectile.Center = Vector2.Lerp(oldPosition, oldPosition + Vector2.UnitY * 24, (float)Math.Sin(floatProgress));
            }
        }

        private void CheckForPickup()
        {
            if (owner.Hitbox.Intersects(Projectile.Hitbox))
            {
                OnPickup();
                Projectile.Kill();
            }
        }

        private DaggerStats GetParentDaggerStats()
        {
            // Try to find an active held dagger to get stats from
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                var proj = Main.projectile[i];
                if (proj.active && proj.owner == Projectile.owner && proj.ModProjectile is HeldDagger heldDagger)
                {
                    if (heldDagger.ParentItem == ParentItem)
                        return heldDagger.GetBaseDaggerStats();
                }
            }

            // Fallback to default stats
            return new DaggerStats();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!canShowDagger) return false;

            DrawIdleEffects(lightColor);
            DrawDaggerSprite(lightColor);

            // Allow accessories to add effects to thrown daggers
            var daggerPlayer = owner.GetModPlayer<DaggerPlayer>();
            foreach (var drawEffect in daggerPlayer.ActiveDrawEffects)
            {
                drawEffect.DrawThrownDaggerEffects(this, owner, Main.spriteBatch);
            }

            return false;
        }

        private void DrawIdleEffects(Color lightColor)
        {
            if (!groundCollided) return;

            float activeAlpha = MathHelper.Lerp(0f, 1f, Utils.Clamp(Projectile.timeLeft, 0, 60f) / 60f);
            float glowAlpha = 0.65f * activeAlpha;

            Main.spriteBatch.Reload(BlendState.Additive);

            // Draw glow ring
            Texture2D ringTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "RingSolid").Value;
            Main.spriteBatch.Draw(ringTexture, Projectile.Center - Main.screenPosition, null,
                IdleColor * glowAlpha, Projectile.rotation, ringTexture.Size() / 2f, 0.1f, SpriteEffects.None, 1);

            // Draw sparkle effect
            Texture2D starTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            Main.spriteBatch.Draw(starTexture, Projectile.Center - Main.screenPosition, null,
                IdleColor * glowAlpha, Projectile.rotation, starTexture.Size() / 2f, 0.5f, SpriteEffects.None, 1);

            // Enhanced effect for focus shots
            if (isFocusShot)
            {
                Color focusColor = Color.Lerp(IdleColor, Color.White, 0.5f);
                Main.spriteBatch.Draw(ringTexture, Projectile.Center - Main.screenPosition, null,
                    focusColor * glowAlpha * 0.8f, Projectile.rotation + MathHelper.PiOver4, ringTexture.Size() / 2f, 0.15f, SpriteEffects.None, 1);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }

        private void DrawDaggerSprite(Color lightColor)
        {
            Texture2D texture = TextureAssets.Item[ParentItem].Value;
            SpriteEffects spriteEffects = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            float activeAlpha = MathHelper.Lerp(0f, 1f, Utils.Clamp(Projectile.timeLeft, 0, 60f) / 60f);
            Color drawColor = groundCollided ? Color.White : lightColor;

            // Enhanced color for focus shots
            if (isFocusShot && !groundCollided)
            {
                drawColor = Color.Lerp(drawColor, parentStats?.FlashColor ?? Color.White, 0.3f);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null,
                drawColor * activeAlpha, Projectile.rotation, texture.Size() / 2f,
                Projectile.scale, spriteEffects, 1);
        }

        bool groundCollided = false;
        Vector2 oldPosition;

        public sealed override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!groundCollided && Projectile.velocity.Y > 0)
            {
                HandleCollisionBounce();
            }
            else
            {
                Projectile.velocity *= -0.5f;
            }

            return false;
        }

        /// <summary>
        /// Called before the default bounce behavior. Return false to prevent bouncing.
        /// </summary>
        public virtual bool PreHandleCollisionBounce() { return true; }

        private void HandleCollisionBounce()
        {
            if (groundCollided || !PreHandleCollisionBounce()) return;

            OnLanding();

            groundCollided = true;
            Projectile.velocity.X *= 0.5f;
            Projectile.velocity.Y = Main.rand.NextFloat(-2.2f, -1f);

            // Use stats system for recovery time
            if (parentStats != null)
                Projectile.timeLeft = parentStats.ThrowRecoveryTime;
            else
                Projectile.timeLeft = 600;
        }

        public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Apply modifiers from accessories
            var modifiers = new NPC.HitModifiers();
            var daggerPlayer = owner.GetModPlayer<DaggerPlayer>();
            DaggerModifierHandler.TriggerThrownDaggerHit(this, owner, target, ref modifiers);

            // Apply focus shot effects
            if (isFocusShot)
            {
                OnFocusShotHit(target, hit, damageDone);
            }

            OnThrownDaggerHit();
            base.OnHitNPC(target, hit, damageDone);
        }

        /// <summary>
        /// Called when the thrown dagger hits any enemy.
        /// </summary>
        public virtual void OnThrownDaggerHit() { }

        /// <summary>
        /// Called when a focus shot (charged throw) hits an enemy.
        /// </summary>
        public virtual void OnFocusShotHit(NPC target, NPC.HitInfo hit, int damageDone) { }

        /// <summary>
        /// Called when the dagger lands on the ground.
        /// </summary>
        public virtual void OnLanding() { }

        /// <summary>
        /// Called when the player picks up the dagger.
        /// </summary>
        public virtual void OnPickup() { }
    }
}