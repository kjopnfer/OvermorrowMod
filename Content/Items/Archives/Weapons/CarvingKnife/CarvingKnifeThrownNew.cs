using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public class CarvingKnifeThrownNew : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveItems + "CarvingKnife";
        protected Player Owner => Main.player[Projectile.owner];

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


        private float GetBackTime() => 15f;
        private float GetForwardTime() => 4f;
        private float GetHoldTime() => 4f;

        public override void AI()
        {
            AICounter++;
            switch (AIState)
            {
                case 0:
                    HandleThrowAnimation();
                    break;
                case 1:
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

            Vector2 mousePosition = Main.MouseWorld;
            Projectile.spriteDirection = mousePosition.X < Owner.Center.X ? -1 : 1;
            Owner.direction = Projectile.spriteDirection;

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

            float weaponRotation = Owner.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -Owner.direction;
            Projectile.rotation = weaponRotation;

            if (AICounter >= totalAnimTime)
            {
                AIState = 1;
                AICounter = 0;
                Owner.heldProj = -1;
            }
            else
            {
                Owner.itemTime = Owner.itemAnimation = 2;
            }

            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
        }

        private void HandleFlightPhase()
        {
            Projectile.velocity.X *= 0.99f;
            Projectile.rotation += 0.48f * (Projectile.velocity.X > 0 ? 1 : -1);

            if (Projectile.ai[0]++ > 10)
                Projectile.velocity.Y += 0.25f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveItems + "CarvingKnife").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Main.player[Projectile.owner].gfxOffY), null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}