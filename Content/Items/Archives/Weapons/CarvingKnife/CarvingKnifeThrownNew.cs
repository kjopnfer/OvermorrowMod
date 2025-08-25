//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using OvermorrowMod.Common;
//using OvermorrowMod.Common.Utilities;
//using OvermorrowMod.Content.Particles;
//using OvermorrowMod.Core.Particles;
//using ReLogic.Content;
//using System;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace OvermorrowMod.Content.Items.Archives
//{
//    public class CarvingKnifeThrownNew : ModProjectile
//    {
//        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";
//        protected Player Owner => Main.player[Projectile.owner];
//        public virtual Color IdleColor => Color.White;

//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
//        }

//        public sealed override void SetDefaults()
//        {
//            Projectile.width = Projectile.height = 2;
//            Projectile.DamageType = DamageClass.Melee;
//            Projectile.aiStyle = -1;
//            Projectile.penetrate = -1;
//            Projectile.friendly = true;
//            Projectile.tileCollide = true;
//            Projectile.ignoreWater = true;
//            Projectile.timeLeft = ModUtils.SecondsToTicks(6);
//            Projectile.usesLocalNPCImmunity = true;
//            Projectile.localNPCHitCooldown = 15;
//        }

//        public enum AIStates
//        {
//            ThrowAnimation,
//            Thrown,
//            Idle,
//            Impaled
//        }

//        public NPC ImpaledNPC { get; private set; } = null;
//        private Vector2 impaledOffset;
//        private float impaledRotation;

//        public ref float AICounter => ref Projectile.ai[0];
//        public ref float AIState => ref Projectile.ai[1];

//        private float swingAngle = 0;
//        private Vector2 storedPosition;
//        private int initialDirection = 0;
//        private bool groundCollided = false;
//        private Vector2 oldPosition;

//        public override bool? CanDamage()
//        {
//            return AIState == (int)AIStates.Thrown && !groundCollided;
//        }

//        public override bool? CanHitNPC(NPC target)
//        {
//            // If already impaled, can't hit other NPCs
//            if (AIState == (int)AIStates.Impaled)
//                return false;

//            return base.CanHitNPC(target);
//        }

//        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
//        {
//            behindNPCs.Add(index);
//        }

//        private float GetBackTime() => 15f;
//        private float GetForwardTime() => 4f;
//        private float GetHoldTime() => 4f;

//        public override void OnSpawn(IEntitySource source)
//        {
//            initialDirection = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
//            rotationRate = Main.rand.NextFloat(0.4f, 0.52f);
//        }

//        public override void AI()
//        {
//            Projectile.hide = false;
//            switch ((AIStates)AIState)
//            {
//                case AIStates.ThrowAnimation:
//                    AICounter++;

//                    Owner.heldProj = Projectile.whoAmI;
//                    Owner.itemTime = Owner.itemAnimation = 2;
//                    HandleThrowAnimation();
//                    break;
//                case AIStates.Thrown:
//                    AICounter++;
//                    if (AICounter == 1)
//                    {
//                        Projectile.Center = storedPosition;
//                    }

//                    //Dust.NewDust(storedPosition, 1, 1, DustID.Torch);

//                    if (AICounter > 30)
//                    {
//                        HandleFlightPhase();
//                        Projectile.extraUpdates = 0;
//                    }
//                    else
//                    {
//                        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(50);
//                        Projectile.extraUpdates = 1;
//                    }
//                    Projectile.width = Projectile.height = 32;
//                    break;
//                case AIStates.Idle:
//                    AICounter++;
//                    HandleGroundPhase();
//                    break;
//                case AIStates.Impaled:
//                    Projectile.hide = true;

//                    AICounter++;
//                    HandleImpaledState();
//                    break;
//            }
//        }

//        private void HandleThrowAnimation()
//        {
//            float backTime = GetBackTime();
//            float forwardTime = GetForwardTime();
//            float holdTime = GetHoldTime();
//            float totalAnimTime = backTime + forwardTime + holdTime;

//            float releaseTime = backTime + (forwardTime * 0.9f);

//            Vector2 mousePosition = Main.MouseWorld;
//            Projectile.spriteDirection = initialDirection;
//            Owner.direction = initialDirection;

//            if (AICounter <= backTime)
//            {
//                swingAngle = MathHelper.Lerp(0, 105, EasingUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
//            }
//            else if (AICounter > backTime && AICounter <= backTime + forwardTime)
//            {
//                swingAngle = MathHelper.Lerp(105, 15, EasingUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
//            }
//            else if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
//            {
//                swingAngle = MathHelper.Lerp(15, -45, EasingUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
//            }

//            float weaponRotation = Owner.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -initialDirection;
//            Projectile.rotation = weaponRotation;

//            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
//            armPosition.Y += Owner.gfxOffY;

//            Vector2 knifeOffset = new Vector2(16, -8 * initialDirection);
//            Vector2 rotatedOffset = knifeOffset.RotatedBy(Projectile.rotation);
//            Projectile.Center = armPosition + rotatedOffset;

//            if (AICounter >= releaseTime)
//            {
//                storedPosition = Projectile.Center;
//                if (initialDirection == -1)
//                    storedPosition.X += -14;

//                AIState = (int)AIStates.Thrown;
//                AICounter = 0;
//                Owner.heldProj = -1;
//            }

//            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
//        }

//        float rotationRate = 0.48f;
//        private void HandleFlightPhase()
//        {
//            Projectile.velocity.X *= 0.99f;
//            Projectile.rotation += rotationRate * (Projectile.velocity.X > 0 ? 1 : -1);

//            if (AICounter > 10)
//                Projectile.velocity.Y += 0.25f;
//        }

//        private void HandleGroundPhase()
//        {
//            Projectile.velocity.X *= 0.97f;

//            if (AICounter == 60f)
//            {
//                Projectile.velocity.X *= 0.01f;
//                oldPosition = Projectile.Center;
//            }

//            float rotationFactor = MathHelper.Lerp(0.48f, 0f, Utils.Clamp(AICounter, 0, 60f) / 60f);
//            Projectile.rotation += rotationFactor * (Projectile.velocity.X > 0 ? 1 : -1);
//            Projectile.velocity.Y *= 0.96f;

//            if (AICounter > 60f)
//            {
//                Projectile.tileCollide = false;
//                float floatProgress = (AICounter - 60f) / 40f;
//                Projectile.Center = Vector2.Lerp(oldPosition, oldPosition + Vector2.UnitY * 24, (float)Math.Sin(floatProgress));
//            }

//            if (Owner.Hitbox.Intersects(Projectile.Hitbox))
//            {
//                Projectile.Kill();
//            }
//        }

//        private void HandleImpaledState()
//        {
//            // Check if the impaled NPC is still valid
//            if (ImpaledNPC == null || !ImpaledNPC.active)
//            {
//                Projectile.Kill();
//                return;
//            }

//            // Stick to the NPC at the impaled position
//            Projectile.Center = ImpaledNPC.Center + impaledOffset;
//            Projectile.rotation = impaledRotation;

//            Projectile.velocity = Vector2.Zero;
//            Projectile.tileCollide = false;

//            // Wobble effect
//            if (AICounter < 30)
//            {
//                float wobbleAmount = (30 - AICounter) * 0.02f;
//                float wobble = (float)Math.Sin(AICounter * 0.3f) * wobbleAmount;
//                Projectile.rotation = impaledRotation + wobble;
//            }

//            // Damage over time while impaled
//            if (AICounter % 60 == 0 && AICounter > 60)
//            {
//                int damage = Projectile.damage / 3; // 33% of original damage
//                ImpaledNPC.SimpleStrikeNPC(damage, 0, false, 0f, null, false, 0f, true);
//            }

//            // Auto-remove after some time
//            if (AICounter > 300)
//            {
//                Projectile.Kill();
//            }
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            Vector2 strikePoint = Projectile.Center;
//            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

//            int sparkCount = Main.rand.Next(6, 10);

//            Vector2 knifeDirection = Vector2.Normalize(Projectile.velocity);
//            Vector2 oppositeDirection = -knifeDirection;
//            float baseAngle = oppositeDirection.ToRotation();

//            float spreadAngle = MathHelper.ToRadians(135);

//            for (int i = 0; i < sparkCount; i++)
//            {
//                float randomScale = Main.rand.NextFloat(0.1f, 0.25f);
//                float randomAngleOffset = Main.rand.NextFloat(-spreadAngle * 0.5f, spreadAngle * 0.5f);
//                float finalAngle = baseAngle + randomAngleOffset;

//                Vector2 particleDirection = new Vector2((float)Math.Cos(finalAngle), (float)Math.Sin(finalAngle));
//                Vector2 particleVelocity = particleDirection * Main.rand.Next(3, 9);

//                var lightSpark = new Spark(sparkTexture, maxTime: 20, false, 0f)
//                {
//                    endColor = Color.White
//                };

//                ParticleManager.CreateParticleDirect(lightSpark, strikePoint, particleVelocity, Color.White, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
//            }

//            // Only impale if the knife is not rotating
//            bool canImpale = AIState == (int)AIStates.Thrown && AICounter < 30;
//            if (canImpale)
//            {
//                ImpaledNPC = target;
//                if (ImpaledNPC != null)
//                {
//                    impaledOffset = Projectile.Center - ImpaledNPC.Center;
//                    impaledRotation = Projectile.rotation;

//                    AIState = (int)AIStates.Impaled;
//                    AICounter = 0;
//                    Projectile.timeLeft = 600;
//                }
//            }
//        }

//        private void DrawIdleEffects()
//        {
//            if (!groundCollided) return;

//            float activeAlpha = MathHelper.Lerp(0f, 1f, Utils.Clamp(Projectile.timeLeft, 0, 60f) / 60f);
//            float glowAlpha = 0.65f * activeAlpha;

//            Main.spriteBatch.Reload(BlendState.Additive);

//            // Draw glow ring
//            Texture2D ringTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_01").Value;
//            Main.spriteBatch.Draw(ringTexture, Projectile.Center - Main.screenPosition, null,
//                IdleColor * glowAlpha, Projectile.rotation, ringTexture.Size() / 2f, 0.1f, SpriteEffects.None, 1);

//            // Draw sparkle effect
//            Texture2D starTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
//            Main.spriteBatch.Draw(starTexture, Projectile.Center - Main.screenPosition, null,
//                IdleColor * glowAlpha, Projectile.rotation, starTexture.Size() / 2f, 0.5f, SpriteEffects.None, 1);


//            Main.spriteBatch.Reload(BlendState.AlphaBlend);
//        }

//        private void DrawSlashShine()
//        {
//            if (AIState != (int)AIStates.Thrown || AICounter > 40) return;

//            float progress = AICounter / 40f;

//            // Position at knife tip
//            Vector2 knifeDirection = new Vector2(15, -15).RotatedBy(Projectile.rotation);
//            Vector2 center = Projectile.Center + knifeDirection;

//            Texture2D slashTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06").Value;

//            float alpha = 0f;
//            float scaleX = 1f;

//            if (progress <= 0.2f)
//            {
//                // Fade in during first 20% of flight
//                alpha = (progress / 0.2f) * 0.8f;
//                scaleX = progress / 0.2f;
//            }
//            else if (progress <= 0.4f)
//            {
//                // Hold at full brightness
//                alpha = 1f;
//                scaleX = 1f;
//            }
//            else
//            {
//                // Fade out for remaining 60% of flight
//                float fadeProgress = (progress - 0.4f) / 0.6f;
//                alpha = (1f - fadeProgress) * 0.8f;
//                scaleX = 1f - fadeProgress;
//            }

//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

//            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, Color.White * alpha, 0f + MathHelper.PiOver2, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.35f), SpriteEffects.None, 0);
//            Main.spriteBatch.Draw(slashTexture, center - Main.screenPosition, null, Color.White * alpha, 0f, slashTexture.Size() * 0.5f, new Vector2(0.05f * scaleX, 0.25f), SpriteEffects.None, 0);

//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
//        }

//        public override bool OnTileCollide(Vector2 oldVelocity)
//        {
//            if (!groundCollided && Projectile.velocity.Y > 0)
//            {
//                groundCollided = true;
//                Projectile.velocity.X *= 0.5f;
//                Projectile.velocity.Y = Main.rand.NextFloat(-2.2f, -1f);
//                Projectile.timeLeft = 600;
//                AIState = (int)AIStates.Idle;
//                AICounter = 0;
//            }
//            else
//            {
//                Projectile.velocity *= -0.5f;
//            }
//            return false;
//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "CarvingKnife").Value;

//            float drawRotation = Projectile.rotation;

//            if (AIState == (int)AIStates.ThrowAnimation)
//            {
//                float rotationOffset = initialDirection == 1 ? 45 : 135;
//                drawRotation = Projectile.rotation + MathHelper.ToRadians(rotationOffset);
//            }
//            else
//            {
//                if (initialDirection == -1)
//                    drawRotation = Projectile.rotation + MathHelper.ToRadians(90);
//            }

//            float fadeAlpha = 1f;
//            if (AIState == (int)AIStates.Impaled)
//            {
//                float fadeTime = ModUtils.SecondsToTicks(2); // 2 seconds fade time
//                if (AICounter > 300 - fadeTime)
//                {
//                    float timeRemaining = 300 - AICounter;
//                    fadeAlpha = timeRemaining / fadeTime;
//                    fadeAlpha = MathHelper.Clamp(fadeAlpha, 0f, 1f);
//                }
//            }
//            else if (Projectile.timeLeft < ModUtils.SecondsToTicks(1))
//            {
//                // Normal fade for other states
//                fadeAlpha = Projectile.timeLeft / (float)ModUtils.SecondsToTicks(1);
//            }

//            SpriteEffects spriteEffects = initialDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
//            Color fadeColor = lightColor * fadeAlpha;

//            DrawIdleEffects();
//            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, fadeColor, drawRotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);

//            // add shine when throwing
//            DrawSlashShine();
//            return false;
//        }
//    }
//}