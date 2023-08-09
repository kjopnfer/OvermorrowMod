using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class WoodenStaff : ModItem
    {
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wooden Staff");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
            ItemID.Sets.Spears[Item.type] = true; // This allows the game to recognize our new item as a spear.
            Tooltip.SetDefault("{Keyword:Focus}: Gain increased damage and knockback");
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.rare = ItemRarityID.Green; // Assign this item a rarity level of Pink
            Item.value = Item.sellPrice(silver: 10); // The number and type of coins item can be sold for to an NPC

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.useAnimation = 30; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useTime = 36; // The length of the item's use time in ticks (60 ticks == 1 second.)
            //Item.UseSound = SoundID.Item1; // The sound that this item plays when used.
            Item.autoReuse = false; // Allows the player to hold click to automatically use the item again. Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

            // Weapon Properties
            Item.damage = 12;
            Item.knockBack = 3f;
            Item.noUseGraphic = true; // When true, the item's sprite will not be visible while the item is in use. This is true because the spear projectile is what's shown so we do not want to show the spear sprite as well.
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true; // Allows the item's animation to do damage. This is important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.

            // Projectile Properties
            Item.shootSpeed = 2.2f; // The speed of the projectile measured in pixels per frame.
            Item.shoot = ModContent.ProjectileType<WoodenStaff_Held>(); // The projectile that is fired from this weapon
        }

        public int attackIndex = 1;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            attackIndex++;
            if (attackIndex > 2) attackIndex = 0;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, attackIndex);

            return false;
        }

        public override void HoldItem(Player player)
        {
            //if (player.ownedProjectileCounts[player.HeldItem.shoot] < 1)
            //    Projectile.NewProjectile(null, player.Center, Vector2.Zero, Item.shoot, Item.damage, Item.knockBack, player.whoAmI);
        }

        public override bool? UseItem(Player player)
        {
            // Because we're skipping sound playback on use animation start, we have to play it ourselves whenever the item is actually used.
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }

            return null;
        }
    }

    public class WoodenStaff_Held : ModProjectile
    {
        public override bool? CanHitNPC(NPC target) => !target.friendly && inSwingState;

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (HoldCounter >= heavySwingThreshold)
            {
                damage = (int)(damage * 1.5f);
                knockback *= 2;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wooden Staff");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
        }

        Player player => Main.player[Projectile.owner];

        public ref float ComboIndex => ref Projectile.ai[0];
        public ref float AICounter => ref Projectile.ai[1];


        // TODO: OnSpawn Hook with combo initializer
        public override void OnSpawn(IEntitySource source)
        {
            InitializeValues();
        }

        /// <summary>
        /// Initializes swing and damage values based on the combo index
        /// </summary>
        private void InitializeValues()
        {
            switch (ComboIndex)
            {
                default:
                    break;
            }
        }

        /// <summary>
        /// How long the player has gone without taking an action. Used to determine combo time.
        /// </summary>
        public float InactiveCounter = 0;
        public float InactiveLimit = 60;

        /// <summary>
        /// How long the player has held down the weapon, separate from the counter that handles actions
        /// </summary>
        public float HoldCounter = 0; // TODO: Sync this with ExtraAI

        private bool IsExecutingAction = false;
        public override void AI()
        {
            if (player.active && player.HeldItem.type == ModContent.ItemType<WoodenStaff>()) Projectile.timeLeft = 10;

            Projectile.width = Projectile.height = 40;
            player.heldProj = Projectile.whoAmI;

            if (InactiveCounter == InactiveLimit) Projectile.Kill();
            if (!IsExecutingAction) InactiveCounter++;

            HandleArmDrawing();
            HandleWeaponUse();
        }

        Vector2 positionOffset;
        private void HandleWeaponUse()
        {
            if (player.controlUseItem && !justReleasedWeapon) HandleWeaponHold();
            else if (HoldCounter > 0) HandleWeaponRelease();

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter);
        }

        public int maxHoldCount = 60;
        /// <summary>
        /// Executed whenever the player holds down left mouse, used to draw the weapon moving back or any other prep effects.
        /// </summary>
        private void HandleWeaponHold()
        {
            InactiveCounter = 0;
            IsExecutingAction = true;

            if (HoldCounter < maxHoldCount) HoldCounter++;
            if (HoldCounter > heavySwingThreshold && flashCounter <= 15) flashCounter++;

            if (AICounter <= backTime)
            {
                AICounter++;
                switch (ComboIndex)
                {
                    case 0:
                        positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(Projectile.rotation);
                        swingAngle = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        break;
                    case 2:
                        float xOffset = MathHelper.Lerp(20, 6, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        positionOffset = new Vector2(xOffset, 0);

                        break;
                    default:
                        positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(Projectile.rotation);
                        swingAngle = MathHelper.Lerp(0, -135, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        break;
                }
            }
        }

        float backTime
        {
            get
            {
                switch (ComboIndex)
                {
                    case 1:
                        return 20;
                    default:
                        return 15;
                }
            }
        }

        float forwardTime
        {
            get
            {
                switch (ComboIndex)
                {
                    case 1:
                        return HoldCounter > heavySwingThreshold ? 7 : 12;
                    case 2:
                        return HoldCounter > heavySwingThreshold ? 8 : 12;
                    default:
                        return HoldCounter > heavySwingThreshold ? 5 : 10;
                }
            }
        }

        float holdTime
        {
            get
            {
                switch (ComboIndex)
                {
                    case 1:
                        return 10;
                    default:
                        return 7;
                }
            }
        }

        int heavySwingThreshold = 15;

        private bool justReleasedWeapon = false;
        private bool inSwingState = false;
        private Vector2 lastMousePosition;
        private void HandleWeaponRelease()
        {
            // On weapon release is when we execute the attack animation
            if (!justReleasedWeapon)
            {
                flashCounter = 0;
                justReleasedWeapon = true;
                lastMousePosition = Main.MouseWorld;

                //if (HoldCounter < heavySwingThreshold) Main.NewText("light attack");
                //if (HoldCounter >= heavySwingThreshold) Main.NewText("heavy attack");
            }

            // Position
            switch (ComboIndex)
            {
                case 2:
                    float xOffset = 0f;
                    if (AICounter <= backTime)
                        xOffset = MathHelper.Lerp(20, 6, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        xOffset = MathHelper.Lerp(6, 32, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                        xOffset = MathHelper.Lerp(32, 12, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    positionOffset = new Vector2(xOffset, 0);
                    break;
                default:
                    positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(Projectile.rotation);
                    break;
            }

            AICounter++;

            // Angle
            switch (ComboIndex)
            {
                case 0:
                    if (AICounter <= backTime)
                        swingAngle = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        swingAngle = MathHelper.Lerp(100, -75, ModUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                    {
                        inSwingState = false;
                        swingAngle = MathHelper.Lerp(-75, 0, ModUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                    }
                    break;
                case 2:
                    break;
                default:
                    if (AICounter <= backTime)
                        swingAngle = MathHelper.Lerp(0, -135, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

                    if (AICounter > backTime && AICounter <= backTime + forwardTime)
                    {
                        if (!inSwingState) SoundEngine.PlaySound(SoundID.Item1, player.Center);

                        inSwingState = true;
                        swingAngle = MathHelper.Lerp(-135, 75, ModUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
                    }

                    if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                    {
                        inSwingState = false;
                        swingAngle = MathHelper.Lerp(75, 0, ModUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                    }
                    break;
            }

            // HoldCounter gets reset to zero at the end of the attack animation
            if (AICounter >= backTime + forwardTime + holdTime)
            {
                AICounter = 0;
                HoldCounter = 0;
                swingAngle = 0;

                justReleasedWeapon = false;
                IsExecutingAction = false;
                InactiveCounter = 0;

                Projectile.Kill();
            }
        }

        int flashCounter = 0;
        Vector2 spriteOffset = Vector2.Zero;
        Vector2 spriteCenter = Vector2.Zero;
        private void HandleWeaponDrawing(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 spritePositionOffset = Vector2.Zero;
            float rotationOffset = 0f;
            switch (ComboIndex)
            {
                case 0:
                    spritePositionOffset = new Vector2(-8, 8 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = 0;
                    break;
                case 1:
                    spritePositionOffset = new Vector2(8, 18 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(45 * player.direction);
                    break;
                case 2:
                    spritePositionOffset = new Vector2(-2, 2 * player.direction).RotatedBy(Projectile.rotation);
                    rotationOffset = MathHelper.ToRadians(45 * player.direction);
                    break;
            }

            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 5f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Color lerpColor = Color.Lerp(lightColor, Color.White, flashProgress);

            Main.spriteBatch.Draw(texture, spriteCenter + spritePositionOffset - Main.screenPosition, null, lerpColor, Projectile.rotation + rotationOffset, texture.Size() / 2f, Projectile.scale, spriteEffects, 1);
        }

        public float swingAngle = 0;
        private void HandleArmDrawing()
        {
            Vector2 mousePosition = justReleasedWeapon ? lastMousePosition : Main.MouseWorld;
            Projectile.spriteDirection = mousePosition.X < player.Center.X ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            float staffRotation = player.Center.DirectionTo(mousePosition).ToRotation() + MathHelper.ToRadians(swingAngle) * -player.direction;
            Projectile.rotation = staffRotation;

            float backRotation = player.direction == -1 ? -150 : -30;

            switch (ComboIndex)
            {
                case 2:
                    if (AICounter < backTime + 2)
                    {
                        float progress = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
                        if (progress < 50)
                        {
                            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation + MathHelper.ToRadians(backRotation));
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation + MathHelper.ToRadians(-90));
                        }
                        else
                        {
                            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation + MathHelper.ToRadians(backRotation));
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation + MathHelper.ToRadians(-90));
                        }
                    }

                    if (justReleasedWeapon) // For some reason the arm resets while holding the staff back
                    {
                        if (AICounter > backTime && AICounter <= backTime + forwardTime)
                        {
                            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(backRotation));
                            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                        }

                        if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
                        {
                            float progress = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
                            if (progress < 50)
                            {
                                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation + MathHelper.ToRadians(backRotation));
                                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation + MathHelper.ToRadians(-90));
                            }
                            else
                            {
                                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation + MathHelper.ToRadians(backRotation));
                                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation + MathHelper.ToRadians(-90));
                            }
                        }
                    }

                    break;
                default:
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(backRotation));
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
                    break;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = 50;
            hitbox.Height = 50;

            Vector2 hitboxOffset;
            switch (ComboIndex)
            {
                case 2:
                    hitbox.Height = 45;
                    hitboxOffset = positionOffset.RotatedBy(Projectile.rotation);

                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
                default:
                    hitbox.Width = 60;

                    hitboxOffset = new Vector2(35, -15 * player.direction).RotatedBy(Projectile.rotation);
                    hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + hitboxOffset.X);
                    hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + hitboxOffset.Y);
                    break;
            }

            spriteCenter = new Vector2(hitbox.X + (hitbox.Width / 2f), hitbox.Y + (hitbox.Height / 2f));

            base.ModifyDamageHitbox(ref hitbox);
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            HandleWeaponDrawing(lightColor);

            return false;
        }
    }
}