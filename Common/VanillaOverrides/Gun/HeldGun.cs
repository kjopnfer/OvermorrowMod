using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using OvermorrowMod.Core;
using System.Collections.Generic;
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

        /// <summary>
        /// Used to determine where the gun is held for the left and right directions, respectively.
        /// </summary>
        public virtual (Vector2, Vector2) PositionOffset => (new Vector2(15, 0), new Vector2(15, 0));

        public virtual bool TwoHanded => false;

        private int _maxShots = 6;
        public int MaxShots { get { return _maxShots; } set { _maxShots = value <= 0 ? 1 : value; } }

        public SoundStyle ShootSound { get; set; } = SoundID.Item41;

        /// <summary>
        /// Defines the shot bullet positions for the gun for left and right directions, respectively.
        /// </summary>
        public virtual (Vector2, Vector2) BulletShootPosition => (new Vector2(15, -5), new Vector2(15, 15));
        public virtual float ProjectileScale => 1;

        public SoundStyle ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/RevolverReload");

        public abstract int ParentItem { get; }

        /// <summary>
        /// Determines if the bow fires a unique type of bullet. Uses Projectile ID instead of Item ID.
        /// </summary>
        public virtual int BulletType => ProjectileID.None;

        /// <summary>
        /// Determines what bullet type is needed in order to convert the bullet to if BulletType is given. Uses Item ID.
        /// </summary>
        public virtual int ConvertBullet => ItemID.None;

        public abstract GunType GunType { get; }

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
            LoadGunInfo();

            ReloadBulletDisplay();

            _clickZones = ClickZones;

            // deactivate any bullets that were previously fired and stored

            for (int i = 0; i < ShotsFired; i++)
            {
                BulletDisplay[BulletDisplay.Count - 1 - i].isActive = false;
            }
        }

        private bool inReloadState = false;
        private Player player => Main.player[Projectile.owner];
        public ref float PrimaryCounter => ref Projectile.ai[0];
        public ref float SecondaryCounter => ref Projectile.ai[1];

        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem)
            {
                //Main.NewText("death");
                Projectile.Kill();
            }

            player.heldProj = Projectile.whoAmI;

            HandleGunDrawing();
            UpdateBulletDisplay();

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
            if (PreDrawGun(player, Main.spriteBatch, ShotsFired, shootCounter, lightColor))
                DrawGun(lightColor);

            DrawGunOnShoot(player, Main.spriteBatch, lightColor, shootCounter, shootTime);

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
                BonusBullets = gunPlayer.playerGunInfo[ParentItem].bonusBullets;
                BonusDamage = gunPlayer.playerGunInfo[ParentItem].bonusDamage;
                BonusAmmo = gunPlayer.playerGunInfo[ParentItem].bonusAmmo;
            }
        }

        private void SaveGunInfo()
        {
            GunPlayer gunPlayer = player.GetModPlayer<GunPlayer>();

            if (!gunPlayer.playerGunInfo.ContainsKey(ParentItem))
            {
                gunPlayer.playerGunInfo.Add(ParentItem, new HeldGunInfo(ShotsFired, BonusBullets, BonusDamage, BonusAmmo));
            }
            else
            {
                gunPlayer.playerGunInfo[ParentItem] = new HeldGunInfo(ShotsFired, BonusBullets, BonusDamage, BonusAmmo);
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

        public int RecoilAmount { get; set; } = 10;
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

        private void ReloadBulletDisplay()
        {
            for (int _ = 0; _ < MaxShots + BonusAmmo; _++)
            {
                BulletDisplay.Add(new BulletObject(Main.rand.Next(0, 9) * 7));
            }
        }

        private void UpdateBulletDisplay()
        {
            List<BulletObject> removedList = BulletDisplay;

            for (int i = BulletDisplay.Count - 1; i >= 0; i--)
            {
                if (!BulletDisplay[i].isActive) removedList.RemoveAt(i);
            }

            BulletDisplay = removedList;
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

                if (ShotsFired == MaxShots + BonusAmmo)
                {

                    shootCounter = 0;
                    inReloadState = true;
                    reloadTime = MaxReloadTime;
                    reloadBuffer = 10;

                    return;
                }
                else // Don't want the gun to consume a bullet if it is going into the reload state
                {
                    ShotsFired++;
                    ConsumeAmmo();
                }
            }

            if (shootCounter > 0)
            {
                if (shootCounter % shootAnimation == 0)
                {
                    recoilTimer = RECOIL_TIME;

                    Vector2 velocity = Vector2.Normalize(player.Center.DirectionTo(Main.MouseWorld)) * 16;

                    Vector2 shootOffset = player.direction == 1 ? BulletShootPosition.Item2 : BulletShootPosition.Item1;
                    Vector2 shootPosition = Projectile.Center + shootOffset.RotatedBy(Projectile.rotation);

                    SoundEngine.PlaySound(ShootSound);

                    OnShootEffects(player, Main.spriteBatch, velocity, shootPosition, BonusBullets);

                    float damage = Projectile.damage + BonusDamage;
                    OnGunShoot(player, velocity, shootPosition, (int)damage, LoadedBulletType, Projectile.knockBack, BonusBullets);
                }

                if (shootCounter > 0) shootCounter--;
            }
        }

        /// <summary>
        /// Called whenever the gun is fired, used to add miscellaneous effects like particles and dust
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void OnShootEffects(Player player, SpriteBatch spriteBatch, Vector2 velocity, Vector2 shootPosition, int bonusBullets) { }

        /// <summary>
        /// Allows for the implementation of any actions whenever the gun has fired a bullet.
        /// Useful for spawning dropped bullet casings or spawning additional projectiles.
        /// </summary>
        public virtual void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            Projectile.NewProjectile(null, shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);
        }

        private bool reloadFail = false;
        public bool reloadSuccess { get; private set; } = false;

        public int reloadTime = 0;
        public int MaxReloadTime { get; set; } = 60;

        private int clickDelay = 0;
        public int reloadDelay { get; private set; } = 0;
        private int reloadBuffer = 10;
        private void HandleReloadAction()
        {
            if (reloadTime == MaxReloadTime)
            {
                BonusDamage = 0; // Set bonus damage to zero, re-apply any bonus damage on reload end (i.e., via GlobalGun)
                BonusAmmo = 0;
                BonusBullets = 0;

                OnReloadStart(player);
            }

            if (reloadTime > 0) reloadTime--;
            if (clickDelay > 0) clickDelay--;
            if (reloadBuffer > 0)
            {
                reloadBuffer--;
                return;
            }

            // Keeps the projectile alive during the reload state
            // There was a bug where the projectile died immediately after exiting the reload state making it stuck in the reload state forever
            Projectile.timeLeft = 120;

            if (player.controlUseItem && clickDelay == 0 && !reloadFail)
            {
                float clickPercentage = (1 - (float)reloadTime / MaxReloadTime);
                clickDelay = 15;

                if (CheckInZone(clickPercentage, out int zoneIndex))
                {
                    _clickZones[zoneIndex].HasClicked = true;

                    ReloadEventTrigger(player, ref reloadTime, ref BonusBullets, ref BonusAmmo, ref BonusDamage, Projectile.damage, GetClicksLeft());
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

                if (CheckEventSuccess())
                {
                    reloadSuccess = true;
                    OnReloadEventSuccess(player, ref reloadTime, ref BonusBullets, ref BonusAmmo, ref BonusDamage, Projectile.damage);
                }
                else
                    OnReloadEventFail(player);

                ReloadBulletDisplay();

                OnReloadEnd(player);
                ResetReloadZones();

                SoundEngine.PlaySound(ReloadFinishSound);
            }
        }

        private bool CheckEventSuccess()
        {
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (!clickZone.HasClicked) return false;
            }

            return true;
        }

        private int BonusBullets = 0;
        private int BonusDamage = 0;
        private int BonusAmmo = 0;

        private int GetClicksLeft()
        {
            var numLeft = ClickZones.Count;
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (clickZone.HasClicked) numLeft--;
            }

            return numLeft;
        }

        /// <summary>
        /// Called whenever the gun exits the reloading state
        /// </summary>
        public virtual void OnReloadEnd(Player player) { }

        /// <summary>
        /// Called whenever the gun enters the reloading state
        /// </summary>
        public virtual void OnReloadStart(Player player) { }

        /// <summary>
        /// Called after the player clicks within any of the reload zones. Used to add incremental effects like extra ammo.
        /// </summary>
        public virtual void ReloadEventTrigger(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, int clicksLeft) { }

        /// <summary>
        /// Called whenever the player has successfully completed the event during the reloading state. Used to modify reload time or damage.
        /// </summary>
        public virtual void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage) { }

        private void ResetReloadZones()
        {
            foreach (ReloadZone clickZone in _clickZones)
            {
                clickZone.HasClicked = false;
            }
        }

        public virtual void OnReloadEventFail(Player player) { }

        private bool CheckInZone(float clickPercentage, out int zoneIndex)
        {
            clickPercentage = clickPercentage * 100;

            int zoneCounter = 0;
            foreach (ReloadZone clickZone in _clickZones)
            {
                if (clickPercentage >= clickZone.StartPercentage && clickPercentage <= clickZone.EndPercentage)
                {
                    zoneIndex = zoneCounter;
                    return true;
                }

                zoneCounter++;
            }

            zoneIndex = -1;

            return false;
        }


        public List<BulletObject> BulletDisplay = new List<BulletObject>();
        private void DrawAmmo()
        {
            if (Main.gamePaused) return;

            float textureWidth = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunBullet").Value.Width;

            float gapOffset = 6 * Utils.Clamp(BulletDisplay.Count - 1, 0, MaxShots + BonusAmmo);
            float total = textureWidth * BulletDisplay.Count + gapOffset;

            float startPosition = (-total / 2) + 8;

            for (int i = 0; i < BulletDisplay.Count; i++)
            {
                if (!BulletDisplay[i].isActive) continue;

                BulletDisplay[i].Update();

                //Vector2 offset = new Vector2(-38 + 18 * i, 42);
                Vector2 offset = new Vector2(startPosition + 18 * i, 42);

                BulletDisplay[i].Draw(Main.spriteBatch, player.Center + offset);
            }
        }

        private int recoilTimer = 0;
        private int RECOIL_TIME = 15;
        public float reloadRotation = 0;

        /// <summary>
        /// Allows for drawing things behind the projectile. Return false to prevent the default drawing from running.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <returns></returns>
        public virtual bool PreDrawGun(Player player, SpriteBatch spriteBatch, float shotsFired, float shootCounter, Color lightColor) { return true; }

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

            if (GunType == GunType.Revolver)
            {
                if (reloadDelay > 0 && reloadSuccess)
                {
                    float spinRate = MathHelper.Lerp(0.09f, 0.99f, reloadDelay / 30f);
                    reloadRotation -= spinRate * player.direction;
                }
                else
                    reloadRotation = 0;
            }

            Main.spriteBatch.Draw(texture, Projectile.Center + directionOffset - Main.screenPosition, null, lightColor, Projectile.rotation + reloadRotation, texture.Size() / 2f, ProjectileScale, spriteEffects, 1);

        }


        /// <summary>
        /// Called whenever the gun is fired within the PreDraw hook, used to add effects such as muzzle flashes. Always gets called regardless of PreDraw.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="lightColor"></param>
        public virtual void DrawGunOnShoot(Player player, SpriteBatch spriteBatch, Color lightColor, float shootCounter, float maxShootTime) { }


        private List<ReloadZone> _clickZones;
        public abstract List<ReloadZone> ClickZones { get; }

        private void DrawReloadBar()
        {
            float scale = 1;
            if (clickDelay > 0) scale = MathHelper.Lerp(1.25f, 1f, 1 - (clickDelay / 15f));

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "GunReloadFrame").Value;
            Vector2 offset = new Vector2(-2, 41);
            Main.spriteBatch.Draw(texture, player.Center + offset - Main.screenPosition, null, Color.White, 0f, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            foreach (ReloadZone clickZone in _clickZones)
            {
                float startOffset = (clickZone.StartPercentage / 100f) * texture.Width;

                Vector2 zoneOffset = new Vector2(-2 + startOffset, 41f);
                Vector2 zonePosition = player.Center + zoneOffset;

                float clickWidth = (clickZone.EndPercentage - clickZone.StartPercentage) / 100f;
                Texture2D clickTexture = clickZone.HasClicked ? ModContent.Request<Texture2D>(AssetDirectory.UI + "ReloadZone_Clicked").Value : ModContent.Request<Texture2D>(AssetDirectory.UI + "ReloadZone").Value;
                Rectangle drawRectangle = new Rectangle(0, 0, (int)(clickTexture.Width * clickWidth), clickTexture.Height);
                Main.spriteBatch.Draw(clickTexture, zonePosition - Main.screenPosition, drawRectangle, Color.White, 0f, clickTexture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            float cursorProgress = MathHelper.Lerp(0, texture.Width, 1 - ((float)reloadTime / MaxReloadTime));
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