using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Items.Daggers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract class ThrownDagger : ModProjectile
    {
        protected Player Owner => Main.player[Projectile.owner];

        public virtual Color IdleColor => Color.White;
        public virtual Color TrailColor => Color.Orange;
        public virtual bool CanImpale => true;
        public virtual SoundStyle? HitSound => null;
        public virtual SoundStyle? ThrowSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/DaggerThrow");

        public int FlightDuration => GetModifiedFlightDuration();
        public int IdleDuration => GetModifiedIdleDuration();
        public int ImpaleDuration => GetModifiedImpaleDuration();
        public float GravityStrength => GetModifiedGravityStrength();
        public float AirResistance => GetModifiedAirResistance();
        public float GroundFriction => GetModifiedGroundFriction();

        protected virtual int BaseFlightDuration => 30;
        protected virtual int BaseIdleDuration => 600;
        protected virtual int BaseImpaleDuration => 300;
        protected virtual float BaseGravityStrength => 0.25f;
        protected virtual float BaseAirResistance => 0.99f;
        protected virtual float BaseGroundFriction => 0.97f;

        protected float GetBackTime() => GetModifiedBackTime();
        protected float GetForwardTime() => GetModifiedForwardTime();
        protected float GetHoldTime() => GetModifiedHoldTime();

        protected virtual float BaseBackTime => 15f;
        protected virtual float BaseForwardTime => 4f;
        protected virtual float BaseHoldTime => 4f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = ModUtils.SecondsToTicks(6);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }

        public enum AIStates
        {
            ThrowAnimation,
            Thrown,
            Idle,
            Impaled
        }

        public NPC ImpaledNPC { get; private set; } = null;
        private Vector2 impaledOffset;
        private float impaledRotation;

        public ref float AICounter => ref Projectile.ai[0];
        public ref float AIState => ref Projectile.ai[1];

        private float swingAngle = 0;
        private Vector2 storedPosition;
        private int initialDirection = 0;
        private bool groundCollided = false;
        private Vector2 oldPosition;
        private float rotationRate = 0.48f;

        public override bool? CanDamage()
        {
            return AIState == (int)AIStates.Thrown && !groundCollided;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (AIState == (int)AIStates.Impaled)
                return false;
            return base.CanHitNPC(target);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void OnSpawn(IEntitySource source)
        {
            initialDirection = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
            rotationRate = Main.rand.NextFloat(0.38f, 0.58f);
            OnDaggerSpawn(source);
        }

        protected virtual void OnDaggerSpawn(IEntitySource source) { }

        public override void AI()
        {
            Projectile.hide = false;

            switch ((AIStates)AIState)
            {
                case AIStates.ThrowAnimation:
                    AICounter++;
                    Owner.heldProj = Projectile.whoAmI;
                    Owner.itemTime = Owner.itemAnimation = 2;
                    HandleThrowAnimation();
                    break;
                case AIStates.Thrown:
                    AICounter++;
                    HandleThrownState();
                    break;
                case AIStates.Idle:
                    AICounter++;
                    HandleIdleState();
                    break;
                case AIStates.Impaled:
                    Projectile.hide = true;
                    AICounter++;
                    HandleImpaledState();
                    break;
            }
        }

        private void HandleThrowAnimation()
        {
            float backTime = GetBackTime();
            float forwardTime = GetForwardTime();
            float holdTime = GetHoldTime();
            float releaseTime = backTime + (forwardTime * 0.9f);

            Vector2 mousePosition = Main.MouseWorld;
            Projectile.spriteDirection = initialDirection;
            Owner.direction = initialDirection;

            // Calculate swing angle based on animation phase
            if (AICounter <= backTime)
            {
                swingAngle = MathHelper.Lerp(0, 105, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
            }
            else if (AICounter <= backTime + forwardTime)
            {
                swingAngle = MathHelper.Lerp(105, 15, EasingUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
            }
            else
            {
                swingAngle = MathHelper.Lerp(15, -45, EasingUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
            }

            // Position dagger during throw animation
            float weaponRotation = Owner.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -initialDirection;
            Projectile.rotation = weaponRotation;

            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            armPosition.Y += Owner.gfxOffY;

            Vector2 knifeOffset = GetThrowOffset();
            Vector2 rotatedOffset = knifeOffset.RotatedBy(Projectile.rotation);
            Projectile.Center = armPosition + rotatedOffset;

            if (AICounter >= releaseTime)
            {
                storedPosition = Projectile.Center;
                if (initialDirection == -1)
                    storedPosition.X += -14;

                AIState = (int)AIStates.Thrown;
                AICounter = 0;
                Owner.heldProj = -1;
                OnThrowRelease();

                if (ThrowSound.HasValue)
                    SoundEngine.PlaySound(ThrowSound.Value, Projectile.Center);
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
        }

        protected virtual Vector2 GetThrowOffset()
        {
            return new Vector2(16, -8 * initialDirection);
        }

        protected virtual void OnThrowRelease() { }

        private void HandleThrownState()
        {
            if (AICounter == 1)
            {
                Projectile.Center = storedPosition;
            }

            if (AICounter > FlightDuration)
            {
                HandleFlightPhase();
                Projectile.extraUpdates = 0;
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(50);
                Projectile.extraUpdates = 1;
            }
            Projectile.width = Projectile.height = 32;
        }

        private void HandleFlightPhase()
        {
            Projectile.velocity.X *= AirResistance;
            rotationRate *= 0.98f; // Gradually slow rotation
            Projectile.rotation += rotationRate * (Projectile.velocity.X > 0 ? 1 : -1);

            if (AICounter > 10)
                Projectile.velocity.Y += GravityStrength;
        }

        private void HandleIdleState()
        {
            Projectile.velocity.X *= GroundFriction;

            if (AICounter == 60f)
            {
                Projectile.velocity.X *= 0.01f;
                oldPosition = Projectile.Center;
            }

            float rotationFactor = MathHelper.Lerp(0.48f, 0f, Utils.Clamp(AICounter, 0, 60f) / 60f);
            Projectile.rotation += rotationFactor * (Projectile.velocity.X > 0 ? 1 : -1);
            Projectile.velocity.Y *= 0.96f;

            if (AICounter > 60f)
            {
                Projectile.tileCollide = false;
                float floatProgress = (AICounter - 60f) / 40f;
                Projectile.Center = Vector2.Lerp(oldPosition, oldPosition + Vector2.UnitY * 24, (float)Math.Sin(floatProgress));
            }

            if (Owner.Hitbox.Intersects(Projectile.Hitbox))
            {
                SoundEngine.PlaySound(SoundID.Grab, Owner.Center);

                OnDaggerPickup();
                Projectile.Kill();
            }
        }

        private void HandleImpaledState()
        {
            if (ImpaledNPC == null || !ImpaledNPC.active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = ImpaledNPC.Center + impaledOffset;
            Projectile.rotation = impaledRotation;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;

            // Wobble effect
            if (AICounter < 30)
            {
                float wobbleAmount = (30 - AICounter) * 0.02f;
                float wobble = (float)Math.Sin(AICounter * 0.3f) * wobbleAmount;
                Projectile.rotation = impaledRotation + wobble;
            }

            // Damage over time
            if (AICounter % 60 == 0 && AICounter > 60)
            {
                int damage = Projectile.damage / 3;
                ImpaledNPC.SimpleStrikeNPC(damage, 0, false, 0f, null, false, 0f, true);
                OnImpaleDamage(ImpaledNPC, damage);
            }

            if (AICounter > ImpaleDuration)
            {
                Projectile.Kill();
            }
        }

        protected virtual void OnDaggerPickup() { }

        /// <summary>
        /// Called every time the impaled dagger deals damage over time
        /// Example: Heal player, create particles, etc.
        /// </summary>
        protected virtual void OnImpaleDamage(NPC target, int damage) { }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CreateThrownHitEffects(Projectile.Center);

            if (HitSound.HasValue)
                SoundEngine.PlaySound(HitSound.Value, Projectile.Center);

            bool canImpale = CanImpale && AIState == (int)AIStates.Thrown && AICounter < FlightDuration;
            if (canImpale)
            {
                ImpaledNPC = target;
                if (ImpaledNPC != null)
                {
                    impaledOffset = Projectile.Center - ImpaledNPC.Center;
                    impaledRotation = Projectile.rotation;

                    AIState = (int)AIStates.Impaled;
                    AICounter = 0;
                    Projectile.timeLeft = ImpaleDuration + 60;
                    OnDaggerImpale(target, hit, damageDone);
                }
            }

            OnDaggerHit(target, hit, damageDone);
        }

        /// <summary>
        /// Used for custom hit behavior. Different from impaling.
        /// Example: Special effects, damage over time, etc.
        /// </summary>
        protected virtual void OnDaggerHit(NPC target, NPC.HitInfo hit, int damageDone) { }

        /// <summary>
        /// Used for custom impaling behavior.
        /// Example: Apply bleeding debuff, special visual effects
        /// </summary>
        protected virtual void OnDaggerImpale(NPC target, NPC.HitInfo hit, int damageDone) { }

        protected virtual void CreateThrownHitEffects(Vector2 strikePoint)
        {
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!groundCollided && Projectile.velocity.Y > 0)
            {
                groundCollided = true;
                Projectile.velocity.X *= 0.5f;
                Projectile.velocity.Y = Main.rand.NextFloat(-2.2f, -1f);
                Projectile.timeLeft = IdleDuration;
                AIState = (int)AIStates.Idle;
                AICounter = 0;
                OnGroundHit(oldVelocity);
            }
            else
            {
                Projectile.velocity *= -0.5f;
            }
            return false;
        }

        protected virtual void OnGroundHit(Vector2 oldVelocity) { }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            float drawRotation = GetDrawRotation();
            float fadeAlpha = GetFadeAlpha();
            SpriteEffects spriteEffects = GetSpriteEffects();
            Color fadeColor = lightColor * fadeAlpha;

            DrawIdleEffects();
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, fadeColor, drawRotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);
            DrawSlashShine();

            return false;
        }

        protected virtual float GetDrawRotation()
        {
            if (AIState == (int)AIStates.ThrowAnimation)
            {
                float rotationOffset = initialDirection == 1 ? 45 : 135;
                return Projectile.rotation + MathHelper.ToRadians(rotationOffset);
            }
            else
            {
                if (initialDirection == -1)
                    return Projectile.rotation + MathHelper.ToRadians(90);
                return Projectile.rotation;
            }
        }

        protected virtual float GetFadeAlpha()
        {
            if (AIState == (int)AIStates.Impaled)
            {
                float fadeTime = ModUtils.SecondsToTicks(2);
                if (AICounter > ImpaleDuration - fadeTime)
                {
                    float timeRemaining = ImpaleDuration - AICounter;
                    return MathHelper.Clamp(timeRemaining / fadeTime, 0f, 1f);
                }
            }
            else if (Projectile.timeLeft < ModUtils.SecondsToTicks(1))
            {
                return Projectile.timeLeft / (float)ModUtils.SecondsToTicks(1);
            }
            return 1f;
        }

        protected virtual SpriteEffects GetSpriteEffects()
        {
            return initialDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }

        private void DrawIdleEffects()
        {
            if (!groundCollided) return;

            float activeAlpha = MathHelper.Lerp(0f, 1f, Utils.Clamp(Projectile.timeLeft, 0, 60f) / 60f);
            float glowAlpha = 0.55f * activeAlpha;

            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D ringTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_01").Value;
            Main.spriteBatch.Draw(ringTexture, Projectile.Center - Main.screenPosition, null,
                IdleColor * glowAlpha, Projectile.rotation, ringTexture.Size() / 2f, 0.1f, SpriteEffects.None, 1);

            Texture2D starTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            Main.spriteBatch.Draw(starTexture, Projectile.Center - Main.screenPosition, null,
                IdleColor * glowAlpha, Projectile.rotation, starTexture.Size() / 2f, 0.5f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }

        private void DrawSlashShine()
        {
            if (AIState != (int)AIStates.Thrown || AICounter > 40) return;

            float progress = AICounter / 40f;
            Vector2 knifeDirection = new Vector2(15, -15).RotatedBy(Projectile.rotation);
            Vector2 center = Projectile.Center + knifeDirection;

            Texture2D slashTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;

            float alpha = 0f;
            float scaleX = 1f;

            if (progress <= 0.2f)
            {
                alpha = (progress / 0.2f) * 0.8f;
                scaleX = progress / 0.2f;
            }
            else if (progress <= 0.4f)
            {
                alpha = 1f;
                scaleX = 1f;
            }
            else
            {
                float fadeProgress = (progress - 0.4f) / 0.6f;
                alpha = (1f - fadeProgress) * 0.8f;
                scaleX = 1f - fadeProgress;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, TrailColor * alpha, 0f + MathHelper.PiOver2, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.35f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, TrailColor * alpha, 0f, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.25f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        // Methods to get modified values from accessories
        private int GetModifiedFlightDuration()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyFlightDuration(BaseFlightDuration);
        }

        private int GetModifiedIdleDuration()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyIdleDuration(BaseIdleDuration);
        }

        private int GetModifiedImpaleDuration()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyImpaleDuration(BaseImpaleDuration);
        }

        private float GetModifiedGravityStrength()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyGravity(BaseGravityStrength);
        }

        private float GetModifiedAirResistance()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyAirResistance(BaseAirResistance);
        }

        private float GetModifiedGroundFriction()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyGroundFriction(BaseGroundFriction);
        }

        private float GetModifiedBackTime()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyTiming(BaseBackTime);
        }

        private float GetModifiedForwardTime()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyTiming(BaseForwardTime);
        }

        private float GetModifiedHoldTime()
        {
            var stats = DaggerModifierHandler.GetModifiedStats(new DaggerStats(), Owner);
            return stats.ModifyTiming(BaseHoldTime);
        }
    }
}