using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
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
        public override void AI()
        {
            NPC npc = Main.npc[(int)ParentID];
            if (!npc.active) Projectile.Kill();

            OvermorrowNPC overmorrowNPC = npc.ModNPC as OvermorrowNPC;
            if (overmorrowNPC.TargetingModule.IsInAlertState())
            {
                Projectile.timeLeft = 10;


                var config = overmorrowNPC.TargetingConfig();
                float aggroThreshold = config.MaxTargetRange;

                if (config.AlertRange is float alertThreshold)
                {
                    float alertBuffer = alertThreshold - aggroThreshold;

                    // Get the target entity (should already exist in alert state)
                    Entity target = overmorrowNPC.TargetingModule.GetAlertTarget();
                    float distance = Vector2.Distance(npc.Center, target.Center);

                    // Compute progress: 0 at aggroThreshold, 1 at alertThreshold
                    float flashProgress = MathHelper.Clamp((distance - aggroThreshold) / alertBuffer, 0f, 1f);
                    flashSpeed = MathHelper.Lerp(5f, 30f, flashProgress);
                }
                else
                {
                    flashSpeed = 30f; // Fallback if alert is undefined (shouldn't happen in IsInAlertState)
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