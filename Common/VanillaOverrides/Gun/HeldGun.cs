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
using System.IO;
using System;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public enum GunType
    {
        None,
        Revolver,
        Pistol,
        Shotgun,
        Musket,
        Rifle,
        Minigun,
        Launcher,
        Sniper
    }

    public abstract partial class HeldGun : ModProjectile
    {
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        /// <summary>
        /// Determines whether the gun consumes any ammo on use.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanConsumeAmmo(Player player) => true;

        public virtual bool CanUseGun(Player player) => true;

        /// <summary>
        /// Determines whether or not the gun can reload after firing all the ammo.
        /// <para>If false, the gun has no ammo clip and the ammo clip is not drawn. Miniguns default to false.</para>
        /// </summary>
        /// <returns></returns>
        public virtual bool CanReload() => GunType == GunType.Minigun ? false : true;

        /// <summary>
        /// Used for any general update tasks such as unsetting a conditional boolean.
        /// </summary>
        public virtual void Update(Player player) { }

        /// <summary>
        /// Used to determine where the gun is held for the left and right directions, respectively.
        /// </summary>
        public virtual (Vector2, Vector2) PositionOffset => (new Vector2(15, 0), new Vector2(15, 0));

        public virtual bool CanRightClick => false;
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

        /// <summary>
        /// Determines whether the gun consumes ammo per bullet fired.
        /// <para>Used for when the gun fires multiple bullets in a single click.</para>
        /// </summary>
        public virtual bool ConsumePerShot => false;

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

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(inReloadState);
            writer.Write(rightClickDelay);
            writer.Write(shootCounter);
            writer.Write(chargeCounter);
            writer.Write(ShotsFired);
            writer.Write(reloadTime);
            writer.Write(BonusDamage);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            inReloadState = reader.ReadBoolean();
            rightClickDelay = reader.ReadInt16();
            shootCounter = reader.ReadInt16();
            chargeCounter = reader.ReadInt16();
            ShotsFired = reader.ReadInt16();
            reloadTime = reader.ReadInt16();
            BonusDamage = reader.ReadInt16();
        }

        private bool inReloadState = false;
        private Player player => Main.player[Projectile.owner];
        public ref float PrimaryCounter => ref Projectile.ai[0];
        public ref float SecondaryCounter => ref Projectile.ai[1];

        public int rightClickDelay = 0;
        public override void AI()
        {
            if (Main.myPlayer != player.whoAmI) return;
            if (player.HeldItem.type != ParentItem || !player.active)
                Projectile.Kill();
            else
                Projectile.timeLeft = 5;

            player.heldProj = Projectile.whoAmI;

            HandleGunDrawing();
            UpdateBulletDisplay();
            ForceCorrectBulletDisplay();
            Update(player);

            if (rightClickDelay > 0) rightClickDelay--;

            if (!inReloadState)
            {
                if (reloadDelay > 0) reloadDelay--;

                if (CanRightClick && rightClickDelay == 0 && shootCounter == 0 && Main.mouseRight)
                {
                    if (UsesRightClickDelay) rightClickDelay = 10;

                    RightClickEvent(player, ref BonusDamage, Projectile.damage);

                    Projectile.netUpdate = true;
                }

                if (reloadDelay == 0)
                {
                    reloadSuccess = false;

                    ModUtils.AutofillAmmoSlots(player, AmmoID.Bullet);
                    if (FindAmmo() && rightClickDelay == 0) HandleGunUse();
                }
            }
            else
            {
                HandleReloadAction();
            }
        }

        /// <summary>
        /// Allows for an action to occur whenever the player right clicks. Must have set CanRightClick to true for this method to work.
        /// </summary>
        /// <param name="player"></param>
        public virtual void RightClickEvent(Player player, ref int BonusDamage, int baseDamage) { }

        public virtual bool PreDrawAmmo(Player player, SpriteBatch spriteBatch) { return true; }
        public override bool PreDraw(ref Color lightColor)
        {
            if (PreDrawGun(player, Main.spriteBatch, ShotsFired, shootCounter, lightColor))
                DrawGun(lightColor);

            DrawGunOnShoot(player, Main.spriteBatch, lightColor, shootCounter, shootTime + useTimeModifier);

            if (reloadTime == 0 && PreDrawAmmo(player, Main.spriteBatch))
            {
                if (CanReload()) DrawAmmo();
            }
            else
                if (CanReload()) DrawReloadBar();

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SaveGunInfo();
        }

        public Projectile LoadedBullet { private set; get; }
        public int LoadedBulletType { private set; get; }
        public int LoadedBulletItemType { private set; get; }

        private int AmmoSlotID;

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

        #region Gun Use
        public int ShotsFired = 0;
        private int shootCounter = 0;
        public virtual int shootTime => player.HeldItem.useTime;
        public virtual int shootAnimation => player.HeldItem.useAnimation;

        private int useTimeModifier = 0;

        public int chargeCounter { private set; get; } = 0;
        public int maxChargeTime = 120;
        public bool hasReleased = false;
        private void HandleGunUse()
        {
            if (GunType == GunType.Minigun)
            {
                HandleMinigunUse();
                return;
            }

            HandleAmmoAction();
            HandleShootAction();
        }

        private void HandleMinigunUse()
        {
            if (player.controlUseItem && !hasReleased && CanUseGun(player))
            {
                OnChargeUpEffects(player, chargeCounter);

                if (chargeCounter < maxChargeTime) chargeCounter++;

                if (chargeCounter == maxChargeTime)
                {
                    OnChargeShootEffects(player);

                    // If the gun consumes ammo for every bullet fired, then this is handled in HandleShootAction() instead
                    if (player.controlUseItem && shootCounter == 0 && !ConsumePerShot)
                    {
                        shootCounter = shootTime + useTimeModifier;

                        if (CanReload()) PopBulletDisplay();

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
                            if (CanReload()) ShotsFired++;
                            ConsumeAmmo();
                        }

                        Projectile.netUpdate = true;
                    }

                    HandleShootAction();
                }
            }
            else
            {
                OnChargeReleaseEffects(player, chargeCounter);

                shootCounter = 0;

                if (chargeCounter > 0)
                {
                    hasReleased = true;
                    chargeCounter -= 2;
                }

                if (chargeCounter < 0) chargeCounter = 0;

                if (chargeCounter == 0)
                {
                    hasReleased = false;
                }
            }
        }

        private void HandleAmmoAction()
        {
            if (player.controlUseItem && shootCounter == 0 && CanUseGun(player))
            {
                shootCounter = shootTime + useTimeModifier;

                // If the gun consumes ammo for every bullet fired, then this is handled in HandleShootAction() instead
                if (!ConsumePerShot)
                {
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
                        if (CanReload()) ShotsFired++;
                        ConsumeAmmo();
                    }
                }

                Projectile.netUpdate = true;
            }
        }

        private void HandleShootAction()
        {
            if (shootCounter > 0)
            {
                if (shootCounter % (shootAnimation + useTimeModifier) == 0)
                {
                    if (ConsumePerShot)
                    {
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
                            if (CanReload()) ShotsFired++;
                            ConsumeAmmo();
                        }
                    }

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
        /// Allows for the implementation of any actions whenever the gun has fired a bullet or manually firing bullets.
        /// Useful for spawning dropped bullet casings or spawning additional projectiles.
        /// <para>Overriding this will block the original projectile spawning from running.</para>
        /// </summary>
        public virtual void OnGunShoot(Player player, Vector2 velocity, Vector2 shootPosition, int damage, int bulletType, float knockBack, int BonusBullets)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, bulletType, "HeldGun"), shootPosition, velocity, LoadedBulletType, damage, knockBack, player.whoAmI);
        }

        /// <summary>
        /// This method is only called for minigun types.
        /// </summary>
        public virtual void OnChargeUpEffects(Player player, int chargeCounter) { }

        public virtual void OnChargeReleaseEffects(Player player, int chargeCounter) { }

        public virtual void OnChargeShootEffects(Player player) { }

        private bool reloadFail = false;
        public bool reloadSuccess { get; private set; } = false;

        public int reloadTime = 0;
        public int MaxReloadTime { get; set; } = 60;
        public bool UsesRightClickDelay = true;

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
                useTimeModifier = 0;

                OnReloadStart(player);
            }

            if (reloadTime > 0) reloadTime--;
            if (clickDelay > 0) clickDelay--;
            if (reloadBuffer > 0)
            {
                reloadBuffer--;
                return;
            }

            // Prevent the player from switching items if they are reloading
            player.itemTime = 2;
            player.itemAnimation = 2;

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
                    OnReloadEventSuccess(player, ref reloadTime, ref BonusBullets, ref BonusAmmo, ref BonusDamage, Projectile.damage, ref useTimeModifier);
                }
                else
                    OnReloadEventFail(player, ref BonusAmmo, ref useTimeModifier);

                ReloadBulletDisplay();

                OnReloadEnd(player);
                ResetReloadZones();

                SoundEngine.PlaySound(ReloadFinishSound);

                Projectile.netUpdate = true;
            }
        }

        private int BonusBullets = 0;
        private int BonusDamage = 0;
        private int BonusAmmo = 0;

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
        /// <para>BonusBullets is additional bullets per shot, BonusAmmo is additional shots you can fire.</para>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="reloadTime"></param>
        /// <param name="BonusBullets">The amount of additional bullets fired from the gun</param>
        /// <param name="BonusAmmo">The amount of additional shots that the gun will fire</param>
        /// <param name="BonusDamage"></param>
        /// <param name="baseDamage"></param>
        /// <param name="useTimeModifier"></param>
        public virtual void OnReloadEventSuccess(Player player, ref int reloadTime, ref int BonusBullets, ref int BonusAmmo, ref int BonusDamage, int baseDamage, ref int useTimeModifier) { }
        #endregion

        /// <summary>
        /// Used to apply effects whenever the player fails the skill check.
        /// <para>Decreasing the player's next clip can be done by passing in a negative value. </para> 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="BonusAmmo"></param>
        public virtual void OnReloadEventFail(Player player, ref int BonusAmmo, ref int useTimeModifier) { }

        public List<BulletObject> BulletDisplay = new List<BulletObject>();

        #region Ammo Drawing
        private void DrawAmmo()
        {
            if (Main.gamePaused || Main.LocalPlayer != Main.player[Projectile.owner]) return;

            float textureWidth = ModContent.Request<Texture2D>(AssetDirectory.UI + BulletTexture()).Value.Width;

            int bulletCounts = BulletDisplay.Count % 10;
            if (bulletCounts == 0 && BulletDisplay.Count > 0) bulletCounts = 10;

            float gapOffset = 6 * Utils.Clamp(bulletCounts - 1, 0, MaxShots + BonusAmmo);
            float total = textureWidth * bulletCounts + gapOffset;

            float startOffset = BulletTexture() == "GunBullet_Shotgun" ? 12 : 8;
            float startPosition = (-total / 2) + startOffset;

            int startIndex = BulletDisplay.Count - bulletCounts;

            var offsetCounter = 0;
            for (int i = startIndex; i < BulletDisplay.Count; i++)
            {
                if (!BulletDisplay[i].isActive) continue;

                BulletDisplay[i].Update();

                Vector2 offset = new Vector2(startPosition + 18 * offsetCounter, 42);
                BulletDisplay[i].Draw(Main.spriteBatch, player.Center + offset);

                offsetCounter++;
            }

            DrawAmmoCounter(startPosition, bulletCounts);
        }

        private void DrawAmmoCounter(float startPosition, int bulletCounts)
        {
            if (BulletDisplay.Count > 10)
            {
                Texture2D xTexture = ModContent.Request<Texture2D>(AssetDirectory.UI + "OverflowDisplay_X").Value;

                Vector2 counterOffset = new Vector2(startPosition + 18 * bulletCounts, 42);
                Main.spriteBatch.Draw(xTexture, player.Center + counterOffset - Main.screenPosition, null, Color.White, 0f, xTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                Texture2D counterTexture = ModContent.Request<Texture2D>(AssetDirectory.UI + "OverflowDisplay_Numbers").Value;
                int counterTextureWidth = counterTexture.Width / 10;
                //Main.NewText((BulletDisplay.Count));

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
        #endregion

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

            Texture2D bullet = ModContent.Request<Texture2D>(AssetDirectory.UI + BulletTexture()).Value;
            Vector2 bulletPosition = player.Center + new Vector2(-68 * scale, 40f);
            Main.spriteBatch.Draw(bullet, bulletPosition - Main.screenPosition, null, Color.White, 0f, bullet.Size() / 2f, scale, SpriteEffects.None, 1);
        }
    }
}