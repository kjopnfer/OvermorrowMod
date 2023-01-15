using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using Terraria.Audio;
using OvermorrowMod.Core;
using System.Collections.Generic;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public abstract class HeldGun : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        public virtual bool TwoHanded => false;

        public virtual int MaxShots => 6;

        public virtual float ProjectileScale => 1;

        public SoundStyle ReloadFinishSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/RevolverReload");

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
            if (recoilTimer > 0) recoilTimer--;
            float recoilRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-10 * player.direction), Utils.Clamp(recoilTimer, 0, RECOIL_TIME) / (float)RECOIL_TIME);

            float gunRotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation() + recoilRotation;
            Projectile.rotation = gunRotation;
            Projectile.spriteDirection = gunRotation > MathHelper.PiOver2 || gunRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = PositionOffset.RotatedBy(gunRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            if (TwoHanded)
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2 + recoilRotation);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 + recoilRotation);
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
                    shootCounter = 0;
                    inReloadState = true;
                    reloadTime = maxReloadTime;
                    fuckingStop = 10;

                    return;
                }
            }

            if (shootCounter > 0)
            {
                if (shootCounter % shootAnimation == 0)
                {
                    recoilTimer = RECOIL_TIME;

                    Vector2 velocity = Vector2.Normalize(Projectile.Center.DirectionTo(Main.MouseWorld)) * 16;
                    Vector2 shootPosition = Projectile.Center + new Vector2(5, -5).RotatedBy(Projectile.rotation) * player.direction;
                    SoundEngine.PlaySound(SoundID.Item41);

                    Projectile.NewProjectile(null, shootPosition, velocity, ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                }

                if (shootCounter > 0) shootCounter--;
            }
        }

        private int reloadTime = 0;
        private int maxReloadTime = 60;
        private int clickDelay = 0;

        private int fuckingStop = 10;
        private void HandleReloadAction()
        {
            if (reloadTime > 0) reloadTime--;
            if (clickDelay > 0) clickDelay--;
            if (fuckingStop > 0)
            {
                fuckingStop--;
                return;
            }

            if (player.controlUseItem && clickDelay == 0)
            {
                float clickPercentage = (1 - (float)reloadTime / maxReloadTime);
                CheckInTick(clickPercentage);
                clickDelay = 15;
            }

            if (reloadTime == 0)
            {
                inReloadState = false;
                ShotsFired = 0;
                clickDelay = 0;

                SoundEngine.PlaySound(ReloadFinishSound);
            }
        }

        private void CheckInTick(float clickPercentage)
        {
            clickPercentage = clickPercentage * 100;

            //Main.NewText("click at " + clickPercentage);
            foreach (int actionTick in ActionTicks)
            {
                if (clickPercentage >= actionTick - 7 && clickPercentage <= actionTick + 7)
                {
                    Main.NewText("hit: " + clickPercentage);
                    return;
                }
            }

            Main.NewText("you missed lol");
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

                Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
            }

            Texture2D activeBullets = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet").Value;
            int numBullets = MaxShots - ShotsFired;
            for (int i = 0; i < numBullets; i++)
            {
                int textureOffset = 14 * i;
                Vector2 offset = new Vector2(-30 + 12 * i, 42);
                //Vector2 offset = new Vector2(-xOffset , 42);

                Main.spriteBatch.Draw(activeBullets, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
            }
        }

        private int recoilTimer = 0;
        private int RECOIL_TIME = 15;
        private void DrawGun(Color lightColor)
        {
            RECOIL_TIME = 15;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float recoilRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-5), Utils.Clamp(recoilTimer, 0, RECOIL_TIME) / (float)RECOIL_TIME);

            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

        }

        private List<int> ActionTicks = new List<int>() { 45, 75 };
        private void DrawReloadBar()
        {
            float scale = 1;
            if (clickDelay > 0) scale = MathHelper.Lerp(1.25f, 1f, 1 - (clickDelay / 15f));

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunReloadFrame").Value;
            Vector2 offset = new Vector2(-2, 41);

            Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            Texture2D bar = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunReloadBar").Value;
            Vector2 barOffset = new Vector2(3, 42.5f);

            float progress = MathHelper.Lerp(0, bar.Width, 1 - ((float)reloadTime / maxReloadTime));
            Rectangle drawRectangle = new Rectangle(0, 0, (int)progress, bar.Height);
            Vector2 barPosition = player.Center + barOffset;

            Main.spriteBatch.Draw(bar, barPosition - Main.screenPosition, drawRectangle, Color.White, 0f, bar.Size() / 2f, scale, SpriteEffects.None, 1);

            foreach (int actionTick in ActionTicks)
            {
                float positionPercentage = (float)actionTick / 100;
                float tickOffset = positionPercentage * bar.Width;

                Texture2D tick = ModContent.Request<Texture2D>(AssetDirectory.UI + "ReloadTick").Value;
                Vector2 tickPosition = barPosition + new Vector2(-51 + tickOffset, 0);

                Main.spriteBatch.Draw(tick, tickPosition - Main.screenPosition, null, Color.White, 0f, tick.Size() / 2f, 1f, SpriteEffects.None, 1);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawGun(lightColor);

            if (reloadTime == 0)
                DrawAmmo();
            else
                DrawReloadBar();

            return false;
        }
    }
}