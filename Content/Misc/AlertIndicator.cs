using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
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
        /// </summary>
        private float GetEffectiveAlertBuffer(OvermorrowNPC overmorrowNPC, Entity target)
        {
            var config = overmorrowNPC.TargetingConfig();

            if (!config.AlertRange.HasValue)
                return 0f;

            // Use the targeting module's GetAlertBuffer method which accounts for player bonuses
            float? alertBuffer = overmorrowNPC.TargetingModule.GetAlertBuffer(target);

            if (alertBuffer.HasValue)
                return alertBuffer.Value;

            // Fallback to base calculation if targeting module method fails
            return config.AlertRange.Value - config.MaxTargetRange;
        }

        /// <summary>
        /// Gets the effective aggro range for the current target, accounting for AlertBonus.
        /// </summary>
        private float GetEffectiveAggroRange(OvermorrowNPC overmorrowNPC, Entity target)
        {
            var config = overmorrowNPC.TargetingConfig();
            float baseAggroRange = config.MaxTargetRange;

            if (target is Player player)
            {
                int alertBonus = player.GetModPlayer<AccessoryPlayer>().AlertBonus;
                return Math.Max(0, baseAggroRange - alertBonus);
            }

            return baseAggroRange;
        }

        /// <summary>
        /// Gets the effective alert range for the current target, accounting for AlertBonus.
        /// </summary>
        private float GetEffectiveAlertRange(OvermorrowNPC overmorrowNPC, Entity target)
        {
            var config = overmorrowNPC.TargetingConfig();

            if (!config.AlertRange.HasValue)
                return config.MaxTargetRange;

            float baseAlertRange = config.AlertRange.Value;

            if (target is Player player)
            {
                int alertBonus = player.GetModPlayer<AccessoryPlayer>().AlertBonus;
                return baseAlertRange + alertBonus;
            }

            return baseAlertRange;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (!npc.active) Projectile.Kill();

            OvermorrowNPC overmorrowNPC = npc.ModNPC as OvermorrowNPC;
            if (overmorrowNPC.TargetingModule.IsInAlertState())
            {
                Projectile.timeLeft = 10;

                // Get the target entity (should already exist in alert state)
                Entity target = overmorrowNPC.TargetingModule.GetAlertTarget();
                if (target != null)
                {
                    float distance = Vector2.Distance(npc.Center, target.Center);

                    // Get effective ranges accounting for player AlertBonus
                    float effectiveAggroRange = GetEffectiveAggroRange(overmorrowNPC, target);
                    float effectiveAlertRange = GetEffectiveAlertRange(overmorrowNPC, target);
                    float effectiveAlertBuffer = effectiveAlertRange - effectiveAggroRange;

                    if (effectiveAlertBuffer > 0)
                    {
                        // Compute progress: 0 at effectiveAggroRange, 1 at effectiveAlertRange
                        float flashProgress = MathHelper.Clamp((distance - effectiveAggroRange) / effectiveAlertBuffer, 0f, 1f);

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