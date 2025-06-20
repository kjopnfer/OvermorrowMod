using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class AlertIndicator : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc + Name;
        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.timeLeft = 120;
        }

        public ref float ParentID => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];

        public override void OnSpawn(IEntitySource source)
        {
        }

        float flashSpeed = 20;

        /// <summary>
        /// Gets the effective alert buffer for the current target, accounting for AlertBonus.
        /// Now uses the new AggroRadius system.
        /// </summary>
        private float GetEffectiveAlertBuffer(OvermorrowNPC overmorrowNPC, Entity target)
        {
            // Use the targeting module's GetAlertBuffer method which now works with AggroRadius
            float? alertBuffer = overmorrowNPC.TargetingModule.GetAlertBuffer(target);

            if (alertBuffer.HasValue)
                return alertBuffer.Value;

            // Fallback calculation using the new system if targeting module method fails
            var config = overmorrowNPC.TargetingConfig();
            if (config.AlertRadius == null || config.TargetRadius == null)
                return 0f;

            // Get the effective radii for this specific target and NPC direction
            AggroRadius effectiveAlertRadius = GetEffectiveAlertRadius(overmorrowNPC, target);
            AggroRadius effectiveTargetRadius = GetEffectiveTargetRadius(overmorrowNPC, target);

            // Calculate the buffer in the direction of the target
            Vector2 directionToTarget = (target.Center - overmorrowNPC.NPC.Center);
            if (directionToTarget != Vector2.Zero)
            {
                directionToTarget.Normalize();

                float alertRadiusInDirection = effectiveAlertRadius.GetRadiusInDirection(directionToTarget, overmorrowNPC.NPC.direction);
                float targetRadiusInDirection = effectiveTargetRadius.GetRadiusInDirection(directionToTarget, overmorrowNPC.NPC.direction);

                return alertRadiusInDirection - targetRadiusInDirection;
            }

            // Fallback to max radius difference
            return effectiveAlertRadius.GetMaxRadius() - effectiveTargetRadius.GetMaxRadius();
        }

        /// <summary>
        /// Gets the effective aggro radius for the current target, accounting for AlertBonus.
        /// Updated to work with AggroRadius system.
        /// </summary>
        private AggroRadius GetEffectiveTargetRadius(OvermorrowNPC overmorrowNPC, Entity target)
        {
            var config = overmorrowNPC.TargetingConfig();
            AggroRadius baseRadius = config.TargetRadius?.Clone() ?? AggroRadius.Circle(160f);

            if (target is Player player)
            {
                int alertBonus = player.GetModPlayer<AccessoryPlayer>().AlertBonus;

                // AlertBonus reduces all radius values proportionally
                float reductionFactor = Math.Max(0.1f, 1f - (alertBonus / baseRadius.GetMaxRadius()));

                return new AggroRadius(
                    right: Math.Max(0, baseRadius.Right * reductionFactor),
                    left: Math.Max(0, baseRadius.Left * reductionFactor),
                    up: Math.Max(0, baseRadius.Up * reductionFactor),
                    down: Math.Max(0, baseRadius.Down * reductionFactor),
                    flipWithDirection: baseRadius.FlipWithDirection
                );
            }

            return baseRadius;
        }

        /// <summary>
        /// Gets the effective alert radius for the current target, accounting for AlertBonus.
        /// Updated to work with AggroRadius system.
        /// </summary>
        private AggroRadius GetEffectiveAlertRadius(OvermorrowNPC overmorrowNPC, Entity target)
        {
            var config = overmorrowNPC.TargetingConfig();

            if (config.AlertRadius == null)
                return config.TargetRadius ?? AggroRadius.Circle(160f);

            AggroRadius baseRadius = config.AlertRadius.Clone();

            if (target is Player player)
            {
                int alertBonus = player.GetModPlayer<AccessoryPlayer>().AlertBonus;

                // AlertBonus increases alert radius
                return new AggroRadius(
                    right: baseRadius.Right + alertBonus,
                    left: baseRadius.Left + alertBonus,
                    up: baseRadius.Up + alertBonus,
                    down: baseRadius.Down + alertBonus,
                    flipWithDirection: baseRadius.FlipWithDirection
                );
            }

            return baseRadius;
        }

        /// <summary>
        /// Gets the distance from NPC to target in the specific direction of the target.
        /// This accounts for the custom shape of the aggro radius.
        /// </summary>
        private float GetDirectionalDistance(OvermorrowNPC overmorrowNPC, Entity target, AggroRadius radius)
        {
            Vector2 directionToTarget = target.Center - overmorrowNPC.NPC.Center;
            float actualDistance = directionToTarget.Length();

            if (directionToTarget != Vector2.Zero)
            {
                directionToTarget.Normalize();
                float radiusInDirection = radius.GetRadiusInDirection(directionToTarget, overmorrowNPC.NPC.direction);

                // Return the "normalized" distance - how far along the radius the target is
                return actualDistance;
            }

            return actualDistance;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (!npc.active) Projectile.Kill();

            OvermorrowNPC overmorrowNPC = npc.ModNPC as OvermorrowNPC;
            if (overmorrowNPC.TargetingModule.IsInAlertState())
            {
                Projectile.timeLeft = 10;

                Entity target = overmorrowNPC.TargetingModule.GetAlertTarget();
                if (target != null)
                {
                    AggroRadius effectiveTargetRadius = GetEffectiveTargetRadius(overmorrowNPC, target);
                    AggroRadius effectiveAlertRadius = GetEffectiveAlertRadius(overmorrowNPC, target);

                    Vector2 directionToTarget = target.Center - npc.Center;
                    float actualDistance = directionToTarget.Length();

                    if (directionToTarget != Vector2.Zero)
                    {
                        directionToTarget.Normalize();

                        // Get the radius values in the specific direction of the target
                        float targetRadiusInDirection = effectiveTargetRadius.GetRadiusInDirection(directionToTarget, npc.direction);
                        float alertRadiusInDirection = effectiveAlertRadius.GetRadiusInDirection(directionToTarget, npc.direction);
                        float effectiveAlertBuffer = alertRadiusInDirection - targetRadiusInDirection;

                        if (effectiveAlertBuffer > 0)
                        {
                            float flashProgress = MathHelper.Clamp(
                                (actualDistance - targetRadiusInDirection) / effectiveAlertBuffer, 0f, 1f);

                            // Flash speed increases as the target gets closer to aggro range
                            // At max alert distance (flashProgress = 1): slow flash (30f)
                            // At aggro threshold (flashProgress = 0): fast flash (5f)
                            flashSpeed = MathHelper.Lerp(5f, 30f, flashProgress);
                        }
                        else
                        {
                            // If buffer is 0 or negative, use fast flashing
                            flashSpeed = 5f;
                        }
                    }
                    else
                    {
                        flashSpeed = 5f;
                    }
                }
                else
                {
                    // Fallback if no target found (shouldn't happen in IsInAlertState)
                    flashSpeed = 30f;
                }
            }

            AICounter++;
            float progress = MathHelper.Clamp(EasingUtils.EaseOutBounce(AICounter / 20f), 0, 1f);
            Projectile.Center = npc.Hitbox.Top() + Vector2.Lerp(Vector2.Zero, Vector2.UnitY * -25f, progress);
        }

        int textureFrame = 0;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Misc + Name).Value;
            Color textureColor = Color.White;
            float size = 1f;
            float alpha = 1f;

            if (AICounter % (int)flashSpeed == 0)
            {
                textureFrame = textureFrame == 1 ? 0 : 1;
            }

            //Projectile.rotation = MathHelper.Lerp(0, -MathHelper.PiOver4, EasingUtils.EaseOutBounce(Math.Clamp(MathHelper.Lerp(0, 1f, Projectile.timeLeft / 60f), 0, 1f)));
            alpha = Math.Clamp(MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 10f), 0, 1f);

            Rectangle drawRectangle = new Rectangle(0, (texture.Height / 2) * textureFrame, texture.Width, texture.Height / 2);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, textureColor * alpha, Projectile.rotation, texture.Size() / 2f, size, SpriteEffects.None, 0);

            return false;
        }
    }
}