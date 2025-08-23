using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public class CarvingKnifeThrownNew : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";
        protected Player Owner => Main.player[Projectile.owner];

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
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float AIState => ref Projectile.ai[1];

        private float swingAngle = 0;
        private Vector2 storedPosition;
        private int initialDirection = 0;
        private bool groundCollided = false;
        private Vector2 oldPosition;

        public override bool? CanDamage() => AIState == 1 && !groundCollided;

        private float GetBackTime() => 15f;
        private float GetForwardTime() => 4f;
        private float GetHoldTime() => 4f;

        public override void OnSpawn(IEntitySource source)
        {
            initialDirection = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
            rotationRate = Main.rand.NextFloat(0.4f, 0.52f);
        }

        public override void AI()
        {
            switch (AIState)
            {
                case 0:
                    AICounter++;

                    Owner.heldProj = Projectile.whoAmI;
                    Owner.itemTime = Owner.itemAnimation = 2;
                    HandleThrowAnimation();
                    break;
                case 1:
                    AICounter++;
                    if (AICounter == 1)
                    {
                        Projectile.Center = storedPosition;
                    }

                    //Dust.NewDust(storedPosition, 1, 1, DustID.Torch);

                    if (AICounter > 30)
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
                    break;
                case 2:
                    AICounter++;
                    HandleGroundPhase();
                    break;
            }
        }

        private void HandleThrowAnimation()
        {
            float backTime = GetBackTime();
            float forwardTime = GetForwardTime();
            float holdTime = GetHoldTime();
            float totalAnimTime = backTime + forwardTime + holdTime;

            float releaseTime = backTime + (forwardTime * 0.9f);

            Vector2 mousePosition = Main.MouseWorld;
            Projectile.spriteDirection = initialDirection;
            Owner.direction = initialDirection;

            if (AICounter <= backTime)
            {
                swingAngle = MathHelper.Lerp(0, 105, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
            }
            else if (AICounter > backTime && AICounter <= backTime + forwardTime)
            {
                swingAngle = MathHelper.Lerp(105, 15, EasingUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
            }
            else if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
            {
                swingAngle = MathHelper.Lerp(15, -45, EasingUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
            }

            float weaponRotation = Owner.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -initialDirection;
            Projectile.rotation = weaponRotation;

            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            armPosition.Y += Owner.gfxOffY;

            Vector2 knifeOffset = new Vector2(16, -8 * initialDirection);
            Vector2 rotatedOffset = knifeOffset.RotatedBy(Projectile.rotation);
            Projectile.Center = armPosition + rotatedOffset;

            if (AICounter >= releaseTime)
            {
                storedPosition = Projectile.Center;
                if (initialDirection == -1)
                    storedPosition.X += -14;

                AIState = 1;
                AICounter = 0;
                Owner.heldProj = -1;
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
        }

        float rotationRate = 0.48f;
        private void HandleFlightPhase()
        {
            Projectile.velocity.X *= 0.99f;
            Projectile.rotation += rotationRate * (Projectile.velocity.X > 0 ? 1 : -1);

            if (AICounter > 10)
                Projectile.velocity.Y += 0.25f;
        }

        private void HandleGroundPhase()
        {
            Projectile.velocity.X *= 0.97f;

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
                Projectile.Kill();
            }
        }

        private void DrawSlashShine()
        {
            if (AIState != 1 || AICounter > 40) return;

            float progress = AICounter / 40f;

            // Position at knife tip
            Vector2 knifeDirection = new Vector2(15, -15).RotatedBy(Projectile.rotation);
            Vector2 center = Projectile.Center + knifeDirection;

            Texture2D slashTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;

            float alpha = 0f;
            float scaleX = 1f;

            if (progress <= 0.2f)
            {
                // Fade in during first 20% of flight
                alpha = (progress / 0.2f) * 0.8f;
                scaleX = progress / 0.2f;
            }
            else if (progress <= 0.4f)
            {
                // Hold at full brightness
                alpha = 1f;
                scaleX = 1f;
            }
            else
            {
                // Fade out for remaining 60% of flight
                float fadeProgress = (progress - 0.4f) / 0.6f;
                alpha = (1f - fadeProgress) * 0.8f;
                scaleX = 1f - fadeProgress;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, Color.White * alpha, 0f + MathHelper.PiOver2, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.35f), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, Color.White * alpha, 0f, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.25f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!groundCollided && Projectile.velocity.Y > 0)
            {
                groundCollided = true;
                Projectile.velocity.X *= 0.5f;
                Projectile.velocity.Y = Main.rand.NextFloat(-2.2f, -1f);
                Projectile.timeLeft = 600;
                AIState = 2;
                AICounter = 0;
            }
            else
            {
                Projectile.velocity *= -0.5f;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "CarvingKnife").Value;

            float drawRotation = Projectile.rotation;

            if (AIState == 0)
            {
                float rotationOffset = initialDirection == 1 ? 45 : 135;
                drawRotation = Projectile.rotation + MathHelper.ToRadians(rotationOffset);
            }
            else
            {
                if (initialDirection == -1)
                    drawRotation = Projectile.rotation + MathHelper.ToRadians(90);
            }

            float fadeAlpha = 1f;
            if (Projectile.timeLeft < ModUtils.SecondsToTicks(1))
            {
                fadeAlpha = Projectile.timeLeft / (float)ModUtils.SecondsToTicks(1);
            }

            SpriteEffects spriteEffects = initialDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color fadeColor = lightColor * fadeAlpha;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, fadeColor, drawRotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);

            // add shine when throwing
            DrawSlashShine();
            return false;
        }
    }
}