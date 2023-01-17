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
using OvermorrowMod.Common.Particles;
using Terraria.DataStructures;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class BulletObject
    {
        public int DrawCounter = 0;
        public int DeathCounter = 0;

        public bool isActive = true;
        public bool startDeath = false;

        public BulletObject(int DrawCounter = 0)
        {
            this.DrawCounter = DrawCounter;
        }

        public void Update()
        {
            if (!isActive) return;

            if (startDeath)
            {
                DeathCounter++;

                if (DeathCounter == 15)
                {
                    isActive = false;
                }
            }

            DrawCounter++;
        }

        public void Deactivate()
        {
            startDeath = true;
        }

        public void Reset()
        {
            DeathCounter = 0;

            startDeath = false;
            isActive = true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Texture2D activeBullets = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet").Value;
            float scale = 1;

            Vector2 positionOffset = Vector2.UnitY * MathHelper.Lerp(-1, 1, (float)Math.Sin(DrawCounter / 30f) * 0.5f + 0.5f);
            float rotation = MathHelper.Lerp(MathHelper.ToRadians(-8), MathHelper.ToRadians(8), (float)Math.Sin(DrawCounter / 40f) * 0.5f + 0.5f);

            if (startDeath)
            {
                if (DeathCounter < 8)
                {
                    scale = MathHelper.Lerp(1f, 1.5f, DeathCounter / 8f);
                }
                else
                {
                    scale = MathHelper.Lerp(1.5f, 0, (DeathCounter - 8) / 7f);
                }
            }
            
            spriteBatch.Draw(activeBullets, position + positionOffset - Main.screenPosition, null, Color.White, rotation, activeBullets.Size() / 2f, scale, SpriteEffects.None, 1);
        }
    }

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

        public override void OnSpawn(IEntitySource source)
        {
            for (int _ = 0; _ < MaxShots; _++)
            {
                BulletDisplay.Add(new BulletObject(Main.rand.Next(0, 9) * 7));
            }
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
                if (reloadDelay > 0) reloadDelay--;

                if (reloadDelay == 0)
                {
                    reloadSuccess = false;
                    HandleGunUse();
                }
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

        private void PopBulletDisplay()
        {
            for (int i = BulletDisplay.Count - 1; i >= 0; i--)
            {
                if (BulletDisplay[i].isActive)
                {
                    BulletDisplay[i].Deactivate();
                    return;
                }
            }
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

                PopBulletDisplay();

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

                    // TODO: use overrideable shootOffset, make it so facing left multiples y offset by 3 and makes it positive
                    Vector2 shootOffset = player.direction == 1 ? new Vector2(15, -5) : new Vector2(15, 15);
                    Vector2 shootPosition = Projectile.Center + shootOffset.RotatedBy(Projectile.rotation);
                    SoundEngine.PlaySound(SoundID.Item41);

                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.05f, 0.12f)).RotatedByRandom(MathHelper.ToRadians(25));
                        Particle.CreateParticle(Particle.ParticleType<Smoke>(), shootPosition, particleVelocity, Color.DarkGray);
                    }

                    Projectile.NewProjectile(null, shootPosition, velocity, ProjectileID.Bullet, Projectile.damage, Projectile.knockBack, player.whoAmI);
                }

                if (shootCounter > 0) shootCounter--;
            }
        }

        private bool reloadFail = false;
        private bool reloadSuccess = false;

        private int reloadTime = 0;
        private int maxReloadTime = 60;

        private int clickDelay = 0;
        private int reloadDelay = 0;
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

            if (player.controlUseItem && clickDelay == 0 && !reloadFail)
            {
                float clickPercentage = (1 - (float)reloadTime / maxReloadTime);
                clickDelay = 15;

                if (CheckInZone(clickPercentage))
                {
                    reloadSuccess = true;
                    ReloadSuccess();
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/youmissedthatone") with
                    {
                        Volume = 3f
                    }, player.Center);
                    reloadFail = true;
                }
            }

            if (reloadTime == 0)
            {
                reloadFail = false;

                reloadDelay = 30;

                inReloadState = false;
                ShotsFired = 0;
                clickDelay = 0;

                for (int i = 0; i < BulletDisplay.Count; i++)
                    BulletDisplay[i].Reset();

                SoundEngine.PlaySound(ReloadFinishSound);
            }
        }

        public virtual void ReloadSuccess()
        {
            reloadTime = 0;
        }

        private bool CheckInZone(float clickPercentage)
        {
            clickPercentage = clickPercentage * 100;

            foreach ((int, int) clickZone in ClickZones)
            {
                if (clickPercentage >= clickZone.Item1 && clickPercentage <= clickZone.Item2)
                {
                    return true;
                }
            }

            //Main.NewText("you missed lol");
            return false;
        }

        public List<BulletObject> BulletDisplay = new List<BulletObject>();
        private void DrawAmmo()
        {
            if (Main.gamePaused) return;

            for (int i = 0; i < BulletDisplay.Count; i++)
            {
                if (!BulletDisplay[i].isActive) continue;

                BulletDisplay[i].Update();

                Vector2 offset = new Vector2(-38 + 18 * i, 42);
                BulletDisplay[i].Draw(Main.spriteBatch, player.Center + offset);
            }

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet_Used").Value;

            //int xOffset = texture.Width / 2;
            int xOffset = 0;

            for (int i = 0; i < 6; i++)
            {
                int textureOffset = 14 * i;
                Vector2 offset = new Vector2(-30 + 12 * i, 42);
                //Vector2 offset = new Vector2(-xOffset , 42);

                //Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 1);
            }

            Texture2D activeBullets = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet").Value;
            int numBullets = MaxShots - ShotsFired;
            for (int i = 0; i < numBullets; i++)
            {
                int textureOffset = 14 * i;
                Vector2 offset = new Vector2(-30 + 14 * i, 42);
                //Vector2 offset = new Vector2(-xOffset , 42);

                //Main.spriteBatch.Draw(activeBullets, player.Center + offset - Main.screenPosition, null, Color.White, 0f, activeBullets.Size() / 2f, 1f, SpriteEffects.None, 1);
            }
        }

        private int recoilTimer = 0;
        private int RECOIL_TIME = 15;
        float reloadRotation = 0;
        private void DrawGun(Color lightColor)
        {
            RECOIL_TIME = 15;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Vector2 directionOffset = Vector2.Zero;
            if (player.direction == -1)
            {
                directionOffset = new Vector2(0, -10);
            }

            if (reloadDelay > 0 && reloadSuccess)
            {
                float spinRate = MathHelper.Lerp(0.09f, 0.99f, reloadDelay / 30f);
                reloadRotation -= spinRate * player.direction;
            }
            else
                reloadRotation = 0;

            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, lightColor, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

            if (shootCounter > 13)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                Texture2D muzzleFlash = ModContent.Request<Texture2D>(AssetDirectory.Textures + "muzzle_05").Value;

                Vector2 muzzleDirectionOffset = player.direction == 1 ? new Vector2(28, -5) : new Vector2(28, 5);
                Vector2 muzzleOffset = Projectile.Center + directionOffset + muzzleDirectionOffset.RotatedBy(Projectile.rotation);
                spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Main.spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Red * 0.85f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.05f, spriteEffects, 1);
                Main.spriteBatch.Draw(muzzleFlash, muzzleOffset - Main.screenPosition, null, Color.Orange * 0.6f, Projectile.rotation + MathHelper.PiOver2, muzzleFlash.Size() / 2f, 0.05f, spriteEffects, 1);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        private List<(int, int)> ClickZones = new List<(int, int)>() { (40, 55) };
        private void DrawReloadBar()
        {
            float scale = 1;
            if (clickDelay > 0) scale = MathHelper.Lerp(1.25f, 1f, 1 - (clickDelay / 15f));

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunReloadFrame").Value;
            Vector2 offset = new Vector2(-2, 41);
            Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            foreach ((int, int) clickZone in ClickZones)
            {
                float startOffset = (clickZone.Item1 / 100f) * texture.Width;

                Vector2 zoneOffset = new Vector2(-2 + startOffset, 41f);
                Vector2 zonePosition = player.Center + zoneOffset;

                float clickWidth = (clickZone.Item2 - clickZone.Item1) / 100f;
                Texture2D clickTexture = ModContent.Request<Texture2D>(AssetDirectory.UI + "ReloadZone").Value;
                Rectangle drawRectangle = new Rectangle(0, 0, (int)(clickTexture.Width * clickWidth), clickTexture.Height);
                Main.spriteBatch.Draw(clickTexture, zonePosition - Main.screenPosition, drawRectangle, Color.White, 0f, clickTexture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            float cursorProgress = MathHelper.Lerp(0, texture.Width, 1 - ((float)reloadTime / maxReloadTime));
            Texture2D cursor = ModContent.Request<Texture2D>(AssetDirectory.UI + "ReloadCursor").Value;
            Vector2 cursorOffset = new Vector2(-66 + cursorProgress, 42.5f);
            Vector2 cursorPosition = player.Center + cursorOffset;
            Main.spriteBatch.Draw(cursor, cursorPosition - Main.screenPosition, null, Color.White, 0f, cursor.Size() / 2f, scale, SpriteEffects.None, 1);

            Texture2D bullet = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet").Value;
            Vector2 bulletPosition = player.Center + new Vector2(-68 * scale, 40f);
            Main.spriteBatch.Draw(bullet, bulletPosition - Main.screenPosition, null, Color.White, 0f, bullet.Size() / 2f, scale, SpriteEffects.None, 1);
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