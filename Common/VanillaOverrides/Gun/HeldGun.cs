using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using OvermorrowMod.Core;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public abstract class HeldGun : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        public virtual int MaxShots => 6;

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

        private bool inReloadState = false;
        public Player player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem) Projectile.Kill();

            player.heldProj = Projectile.whoAmI;

            HandleGunDrawing();

            if (!inReloadState)
            {
                HandleGunUse();
            }
            else
            {
                HandleReloadAction();
            }
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

        public int ShotsFired = 0;
        private int shootCounter = 0;
        private int shootTime => player.HeldItem.useTime;
        private int shootAnimation => player.HeldItem.useAnimation;
        private void HandleGunUse()
        {
            if (player.controlUseItem && shootCounter == 0)
            {
                Projectile.timeLeft = 120;
                shootCounter = shootTime;

                ShotsFired++;
                if (ShotsFired > MaxShots)
                {
                    Main.NewText("reset");

                    shootCounter = 0;
                    inReloadState = true;
                    reloadTime = 60;

                    return;
                }
            }

            if (shootCounter > 0)
            {
                if (shootCounter % shootAnimation == 0)
                {
                    Main.NewText("fire");

                    Vector2 velocity = Vector2.Normalize(Projectile.Center.DirectionTo(Main.MouseWorld)) * 16;
                    Vector2 shootPosition = Projectile.Center + new Vector2(5, -5).RotatedBy(Projectile.rotation) * player.direction;
                    SoundEngine.PlaySound(SoundID.Item41);

                    Projectile.NewProjectile(null, shootPosition, velocity, ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                }

                if (shootCounter > 0) shootCounter--;
            }
        }

        private int reloadTime = 60;
        private void HandleReloadAction()
        {
            if (reloadTime > 0) reloadTime--;

            if (reloadTime == 0)
            {
                inReloadState = false;
                ShotsFired = 0;
            }
        }

        private void DrawAmmo()
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet_Used").Value;

            //int xOffset = texture.Width / 2;
            int xOffset = 0;

            for (int i = 0; i < 6; i++)
            {
                int textureOffset = 14 * i;
                Vector2 offset = new Vector2(-30 + 12 * i, 42);
                //Vector2 offset = new Vector2(-xOffset , 42);

                Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, ProjectileScale, SpriteEffects.None, 1);
            }

            Texture2D activeBullets = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet").Value;
            int numBullets = MaxShots - ShotsFired;
            for (int i = 0; i < numBullets; i++)
            {
                int textureOffset = 14 * i;
                Vector2 offset = new Vector2(-30 + 12 * i, 42);
                //Vector2 offset = new Vector2(-xOffset , 42);

                Main.spriteBatch.Draw(activeBullets, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, ProjectileScale, SpriteEffects.None, 1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            DrawAmmo();

            return false;
        }
    }
}