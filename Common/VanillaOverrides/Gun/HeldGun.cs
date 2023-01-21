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
    public enum GunType
    {
        Revolver,
        Pistol,
        Shotgun,
        Rifle
    }

    public abstract class HeldGun : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        /// <summary>
        /// Determines whether the gun consumes any ammo on use.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanConsumeAmmo(Player player) => true;

        public virtual Vector2 PositionOffset => new Vector2(15, 0);

        public virtual bool TwoHanded => false;

        public virtual int MaxShots => 6;

        public virtual float ProjectileScale => 1;

        public SoundStyle ReloadFinishSound => new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/RevolverReload");

        public abstract int ParentItem { get; }

        /// <summary>
        /// Determines if the bow fires a unique type of bullet. Uses Projectile ID instead of Item ID.
        /// </summary>
        public virtual int BulletType => ProjectileID.None;

        /// <summary>
        /// Determines what bullet type is needed in order to convert the bullet to if BulletType is given. Uses Item ID.
        /// </summary>
        public virtual int ConvertBullet => ItemID.None;

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
            // TODO: make the projectile load from a player dictionary containing the gun and whichever information is available
            // this is so that the gun doesnt reset whenever the player switches items
            LoadGunInfo();

            for (int _ = 0; _ < MaxShots; _++)
            {
                BulletDisplay.Add(new BulletObject(Main.rand.Next(0, 9) * 7));
            }

            // deactivate any bullets that were previously fired and stored
            for (int i = 0; i < ShotsFired; i++)
            {
                BulletDisplay[BulletDisplay.Count - 1 - i].isActive = false;
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

                    ModUtils.AutofillAmmoSlots(player, AmmoID.Bullet);

                    if (FindAmmo()) HandleGunUse();
                }
            }
            else
            {
                HandleReloadAction();
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

        public override void Kill(int timeLeft)
        {
            SaveGunInfo();
        }

        private void LoadGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                ShotsFired = gunPlayer.playerGunInfo[ParentItem].shotsFired;
            }
        }

        private void SaveGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (!gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                gunPlayer.playerGunInfo.Add(ParentItem, new HeldGunInfo(ShotsFired));
            }
            else
            {
                gunPlayer.playerGunInfo[ParentItem] = new HeldGunInfo(ShotsFired);
            }
        }

        public Projectile LoadedBullet { private set; get; }
        public int LoadedBulletType { private set; get; }
        public int LoadedBulletItemType { private set; get; }

        private int AmmoSlotID;

        /// <summary>
        /// Loops through the ammo slots, loads in the first bullet found into the bow.
        /// </summary>
        /// <returns></returns>
        private bool FindAmmo()
        {
            LoadedBulletItemType = -1;
            if (ConvertBullet != ItemID.None) // There is a bullet given for conversion, try to find that bullet.
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    // The bullet needed to convert is found, so convert the bullet and exit the loop.
                    if (item.type == ConvertBullet)
                    {
                        LoadedBulletType = BulletType;
                        LoadedBulletItemType = item.type;

                        AmmoSlotID = 54 + i;

                        return true;
                    }
                }
            }

            // If here, then there is no conversion bullet OR no conversion bullet was found.
            // Thus, run the default behavior to find any bullets to fire.
            if (LoadedBulletItemType == -1)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Item item = player.inventory[54 + i];
                    if (item.type == ItemID.None || item.ammo != AmmoID.Bullet) continue;

                    LoadedBulletType = item.shoot;
                    LoadedBulletItemType = item.type;

                    AmmoSlotID = 54 + i;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Consumes the given ammo if allowed and handles any exception cases
        /// </summary>
        private void ConsumeAmmo()
        {
            if (!CanConsumeAmmo(player)) return;

            if (player.inventory[AmmoSlotID].type != ItemID.EndlessMusketPouch)
                player.inventory[AmmoSlotID].stack--;
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
                    reloadBuffer = 10;

                    return;
                }
                else // Don't want the gun to consume a bullet if it is going into the reload state
                {
                    ConsumeAmmo();
                }
            }

            if (shootCounter > 0)
            {
                if (shootCounter % shootAnimation == 0)
                {
                    OnGunShoot();

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

                    Projectile.NewProjectile(null, shootPosition, velocity, LoadedBulletType, Projectile.damage, Projectile.knockBack, player.whoAmI);
                }

                if (shootCounter > 0) shootCounter--;
            }
        }

        /// <summary>
        /// Allows for the implementation of any actions whenever the gun has fired a bullet.
        /// Useful for spawning dropped bullet casings or spawning additional projectiles.
        /// </summary>
        public virtual void OnGunShoot() { }

        private bool reloadFail = false;
        private bool reloadSuccess = false;

        public int reloadTime = 0;
        private int maxReloadTime = 60;

        private int clickDelay = 0;
        private int reloadDelay = 0;
        private int reloadBuffer = 10;
        private void HandleReloadAction()
        {
            if (reloadTime == maxReloadTime)
            {
                OnReloadStart();
            }

            if (reloadTime > 0) reloadTime--;
            if (clickDelay > 0) clickDelay--;
            if (reloadBuffer > 0)
            {
                reloadBuffer--;
                return;
            }

            if (player.controlUseItem && clickDelay == 0 && !reloadFail)
            {
                float clickPercentage = (1 - (float)reloadTime / maxReloadTime);
                clickDelay = 15;

                if (CheckInZone(clickPercentage))
                {
                    reloadSuccess = true;
                    OnReloadEventSuccess();
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

                OnReloadEnd();

                SoundEngine.PlaySound(ReloadFinishSound);
            }
        }

        /// <summary>
        /// Called whenever the gun exits the reloading state
        /// </summary>
        public virtual void OnReloadEnd() { }

        /// <summary>
        /// Called whenever the gun enters the reloading state
        /// </summary>
        public virtual void OnReloadStart() { }

        /// <summary>
        /// Called whenever the player has successfully triggered the event during the reloading state
        /// </summary>
        public virtual void OnReloadEventSuccess()
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

        public virtual List<(int, int)> ClickZones => new List<(int, int)>() { (40, 55) };
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
    }
}