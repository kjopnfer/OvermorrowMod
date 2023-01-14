using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public abstract class HeldGun : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        public virtual float ProjectileScale => 1;

        public abstract int ParentItem { get; }

        public virtual void SafeSetDefaults() { }

        public sealed override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;

            SafeSetDefaults();
        }

        public Player player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem) Projectile.Kill();

            player.heldProj = Projectile.whoAmI;

            HandleGunDrawing();
            HandleGunUse();
        }


        private void HandleGunDrawing()
        {
            float gunRotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();
            Projectile.rotation = gunRotation;
            Projectile.spriteDirection = gunRotation > MathHelper.PiOver2 || gunRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = PositionOffset.RotatedBy(gunRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        private bool inFiringState = false;
        private int shootCounter = 0;
        private int shootTime => player.HeldItem.useTime;
        private int shootAnimation => player.HeldItem.useAnimation;
        private void HandleGunUse()
        {
            if (player.controlUseItem && shootCounter == 0)
            {
                Projectile.timeLeft = 120;
                shootCounter = shootTime;
            }

            if (shootCounter > 0)
            {
                if (shootCounter % shootAnimation == 0)
                {
                    Vector2 velocity = Vector2.Normalize(Projectile.Center.DirectionTo(Main.MouseWorld)) * 16;
                    Vector2 shootPosition = Projectile.Center + new Vector2(5, -5).RotatedBy(Projectile.rotation) * player.direction;
                    SoundEngine.PlaySound(SoundID.Item41);

                    Projectile.NewProjectile(null, shootPosition, velocity, ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                }
               
                if (shootCounter > 0) shootCounter--;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            return false;
        }
    }
}