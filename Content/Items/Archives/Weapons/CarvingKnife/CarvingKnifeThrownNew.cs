using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria;
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

        private float GetBackTime() => 15f;
        private float GetForwardTime() => 4f;
        private float GetHoldTime() => 4f;

        public override void AI()
        {
            switch (AIState)
            {
                case 0:
                    AICounter++;
                    if (AICounter == 1)
                    {
                        initialDirection = Main.MouseWorld.X < Owner.Center.X ? -1 : 1;
                    }

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
            }
        }

        private void HandleThrowAnimation()
        {
            float backTime = GetBackTime();
            float forwardTime = GetForwardTime();
            float holdTime = GetHoldTime();
            float totalAnimTime = backTime + forwardTime + holdTime;

            // Release point - middle of the forward swing (adjust this value)
            float releaseTime = backTime + (forwardTime * 0.9f); // 60% through the forward swing

            Vector2 mousePosition = Main.MouseWorld;
            Projectile.spriteDirection = initialDirection; // Use stored direction
            Owner.direction = initialDirection; // Use stored direction

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

            // Position projectile at the player's hand position with offset
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            armPosition.Y += Owner.gfxOffY;

            // Adjust knife offset based on initial direction
            Vector2 knifeOffset = new Vector2(16, -8 * initialDirection);
            Vector2 rotatedOffset = knifeOffset.RotatedBy(Projectile.rotation);
            Projectile.Center = armPosition + rotatedOffset;

            // Release the knife at the optimal point, not at the end
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

        private void HandleFlightPhase()
        {
            Projectile.velocity.X *= 0.99f;
            Projectile.rotation += 0.48f * (Projectile.velocity.X > 0 ? 1 : -1);

            if (AICounter > 10)
                Projectile.velocity.Y += 0.25f;
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

            SpriteEffects spriteEffects = initialDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, drawRotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}