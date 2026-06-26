using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Guns
{
    public abstract partial class HeldGun
    {
        public virtual bool PreDrawAmmo(Player player, SpriteBatch spriteBatch) { return true; }

        public override bool PreDraw(ref Color lightColor)
        {
            if (PreDrawGun(player, Main.spriteBatch, ShotsFired, shootCounter, lightColor))
                DrawGun(lightColor);

            DrawGunOnShoot(player, Main.spriteBatch, lightColor, shootCounter, ShootTime + CurrentStats.UseTimeModifier);

            if (reloadTime == 0 && PreDrawAmmo(player, Main.spriteBatch))
            {
                if (CanReload()) DrawAmmo();
            }
            else
                if (CanReload()) DrawReloadBar();

            // These need to be here otherwise the player arm gets drawn additively for some reason
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        private void HandleGunDrawing()
        {
            if (recoilTimer > 0) recoilTimer--;

            float recoilRotation = MathHelper.Lerp(0, MathHelper.ToRadians(-RecoilAmount * player.direction), Utils.Clamp(recoilTimer, 0, RECOIL_TIME) / (float)RECOIL_TIME);

            float gunRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation() + recoilRotation;
            Projectile.rotation = gunRotation;
            Projectile.spriteDirection = gunRotation > MathHelper.PiOver2 || gunRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = (player.direction == -1 ? PositionOffset.Item1 : PositionOffset.Item2).RotatedBy(gunRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionOffset;

            if (TwoHanded)
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2 + recoilRotation);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 + recoilRotation);
        }

        protected float reloadRotation = 0;

        public virtual bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor) { return true; }

        private void DrawGun(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            // Handle revolver spin effect
            if (CurrentStats.SpinCylinderOnReload)
            {
                if (reloadDelay > 0 && reloadSuccess)
                {
                    float spinRate = MathHelper.Lerp(0.09f, 0.99f, reloadDelay / 30f);
                    reloadRotation -= spinRate * player.direction;
                }
                else
                    reloadRotation = 0;
            }

            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, lightColor,
                Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);
        }

        public virtual void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime) { }

        private void DrawReloadBar()
        {
            float scale = 1;
            if (clickDelay > 0) scale = MathHelper.Lerp(1.25f, 1f, 1 - (clickDelay / 15f));

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "GunReloadFrame").Value;
            Vector2 offset = new Vector2(-2, 41);
            Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            foreach (ReloadZone clickZone in ClickZones)
            {
                // Calculate the actual pixel positions for this zone
                float zoneStartPixel = (clickZone.StartPercentage / 100f) * texture.Width;
                float zoneEndPixel = (clickZone.EndPercentage / 100f) * texture.Width;
                float zoneWidthPixels = zoneEndPixel - zoneStartPixel;

                Vector2 zoneOffset = new Vector2(-texture.Width / 2f + zoneStartPixel, 41f);
                Vector2 zonePosition = player.Center + zoneOffset;

                Texture2D clickTexture = clickZone.HasClicked ?
                    ModContent.Request<Texture2D>(AssetDirectory.GunUI + "ReloadZone_Clicked").Value :
                    ModContent.Request<Texture2D>(AssetDirectory.GunUI + "ReloadZone").Value;

                // Create a rectangle for the specific zone width
                Rectangle drawRectangle = new Rectangle(0, 0, (int)zoneWidthPixels, clickTexture.Height);

                Main.spriteBatch.Draw(clickTexture, zonePosition - Main.screenPosition, drawRectangle, Color.White, 0f,
                    new Vector2(0, clickTexture.Height / 2f), scale, SpriteEffects.None, 1);
            }

            // Draw reload cursor
            float cursorProgress = (1 - (float)reloadTime / MaxReloadTime) * texture.Width;
            Texture2D cursor = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "ReloadCursor").Value;
            Vector2 cursorOffset = new Vector2(-texture.Width / 2f + cursorProgress, 42.5f);
            Vector2 cursorPosition = player.Center + cursorOffset;
            Main.spriteBatch.Draw(cursor, cursorPosition - Main.screenPosition, null, Color.White, 0f, cursor.Size() / 2f, scale, SpriteEffects.None, 1);

            // Draw bullet icon
            string bulletTexture = GetBulletTexture();
            Texture2D bullet = ModContent.Request<Texture2D>(AssetDirectory.GunUI + bulletTexture).Value;
            Vector2 bulletPosition = player.Center + new Vector2(-texture.Width / 2f - 2, 40f);
            Main.spriteBatch.Draw(bullet, bulletPosition - Main.screenPosition, null, Color.White, 0f, bullet.Size() / 2f, scale, SpriteEffects.None, 1);
        }

        private void DrawAmmo()
        {
            if (Main.gamePaused || Main.LocalPlayer != Main.player[Projectile.owner]) return;

            string bulletTexture = GetBulletTexture();
            float textureWidth = ModContent.Request<Texture2D>(AssetDirectory.GunUI + bulletTexture).Value.Width;

            // Count active bullets for display purposes
            int activeBullets = 0;
            foreach (var bullet in BulletDisplay)
            {
                if (bullet.isActive) activeBullets++;
            }

            int bulletCounts = activeBullets % 10;
            if (bulletCounts == 0 && activeBullets > 0) bulletCounts = 10;

            float gapOffset = 6 * Utils.Clamp(bulletCounts - 1, 0, MaxShots);
            float total = textureWidth * bulletCounts + gapOffset;

            float startOffset = bulletTexture == "GunBullet_Shotgun" ? 12 : 8;
            float startPosition = (-total / 2) + startOffset;

            // Draw only the active bullets
            var offsetCounter = 0;
            foreach (var bullet in BulletDisplay)
            {
                if (!bullet.isActive) continue;

                bullet.Update();

                Vector2 offset = new Vector2(startPosition + 18 * offsetCounter, 42);
                bullet.Draw(Main.spriteBatch, player.Center + offset);

                offsetCounter++;
            }

            DrawAmmoCounter(startPosition, bulletCounts);
        }

        private void DrawAmmoCounter(float startPosition, int bulletCounts)
        {
            if (BulletDisplay.Count > 10)
            {
                Texture2D xTexture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "OverflowDisplay_X").Value;

                Vector2 counterOffset = new Vector2(startPosition + 18 * bulletCounts, 42);
                Main.spriteBatch.Draw(xTexture, player.Center + counterOffset - Main.screenPosition, null, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                Texture2D counterTexture = ModContent.Request<Texture2D>(AssetDirectory.GunUI + "OverflowDisplay_Numbers").Value;
                int counterTextureWidth = counterTexture.Width / 10;

                int initialCount = BulletDisplay.Count - 1;
                int firstPlace = GetPlace(initialCount, 100);

                counterOffset = new Vector2(startPosition + 18 * (bulletCounts + 1), 40);
                Rectangle drawRectangle = new Rectangle(counterTextureWidth * firstPlace, 0, 14, counterTexture.Height);
                Main.spriteBatch.Draw(counterTexture, player.Center + counterOffset - Main.screenPosition, drawRectangle, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                int secondPlace = GetPlace(initialCount, 10);

                counterOffset = new Vector2(startPosition + 18 * (bulletCounts + 2), 40);
                drawRectangle = new Rectangle(counterTextureWidth * secondPlace, 0, 14, counterTexture.Height);
                Main.spriteBatch.Draw(counterTexture, player.Center + counterOffset - Main.screenPosition, drawRectangle, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }

        public int GetPlace(int value, int place)
        {
            return ((value % (place * 10)) - (value % place)) / place;
        }
    }
}
