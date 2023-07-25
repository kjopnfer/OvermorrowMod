using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
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
            Item.UseSound = SoundID.Item1; // The sound that this item plays when used.
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
            if (HoldCounter >= heavySwingThreshold) damage = (int)(damage * 1.5f);
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

        Player player => Main.player[Projectile.owner];

        public ref float AICounter => ref Projectile.ai[0];

        /// <summary>
        /// How long the player has held down the weapon, separate from the counter that handles actions
        /// </summary>
        public ref float HoldCounter => ref Projectile.ai[1];

        public override void AI()
        {
            if (player.active && player.HeldItem.type == ModContent.ItemType<WoodenStaff>()) Projectile.timeLeft = 10;

            Projectile.width = Projectile.height = 40;
            player.heldProj = Projectile.whoAmI;

            HandleArmDrawing();
            HandleWeaponUse();
        }

        private void HandleWeaponUse()
        {
            // The weapon will always default to the light attack animations
            // Meaning clicking once will be enough for the light attack animations to go through their entire movement cycle
            // However, holding down click will stop the movement cycle halfway through until the player releases

            // Prevent weapon from being used again while in release state
            if (player.controlUseItem && !justReleasedWeapon) HandleWeaponHold();
            else if (HoldCounter > 0) HandleWeaponRelease();
        }

        public int maxHoldCount = 60;
        private void HandleWeaponHold()
        {
            if (HoldCounter < maxHoldCount) HoldCounter++;
            if (HoldCounter > heavySwingThreshold && flashCounter <= 15) flashCounter++;

            if (AICounter <= backTime)
            {
                AICounter++;
                swingAngle = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));
            }
        }

        float backTime => HoldCounter > heavySwingThreshold ? 20 : 15;
        float forwardTime => HoldCounter > heavySwingThreshold ? 15 : 10;
        float holdTime => HoldCounter > heavySwingThreshold ? 10 : 7;

        int heavySwingThreshold = 15;

        private bool justReleasedWeapon = false;
        private bool inReleaseState = false;
        private bool inSwingState = false;
        private void HandleWeaponRelease()
        {
            // On weapon release is when we execute the attack animation
            if (!justReleasedWeapon)
            {
                flashCounter = 0;
                justReleasedWeapon = true;

                if (HoldCounter < heavySwingThreshold) Main.NewText("light attack");
                if (HoldCounter >= heavySwingThreshold) Main.NewText("heavy attack");
            }

            AICounter++;
            if (AICounter <= backTime)
                swingAngle = MathHelper.Lerp(0, 100, ModUtils.EaseOutQuint(Utils.Clamp(AICounter, 0, backTime) / backTime));

            if (AICounter > backTime && AICounter <= backTime + forwardTime)
            {
                inSwingState = true;
                swingAngle = MathHelper.Lerp(100, -75, ModUtils.EaseInCubic(Utils.Clamp(AICounter - backTime, 0, forwardTime) / forwardTime));
            }

            if (AICounter > backTime + forwardTime && AICounter <= backTime + forwardTime + holdTime)
            {
                inSwingState = false;
                swingAngle = MathHelper.Lerp(-75, 0, ModUtils.EaseInQuart(Utils.Clamp(AICounter - (backTime + forwardTime), 0, holdTime) / holdTime));
            }

            // HoldCounter gets reset to zero at the end of the attack animation
            if (AICounter >= backTime + forwardTime + holdTime)
            {
                AICounter = 0;
                HoldCounter = 0;
                swingAngle = 0;

                justReleasedWeapon = false;
            }
        }

        int flashCounter = 0;
        private void HandleWeaponDrawing(Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 positionOffset = (player.direction == -1 ? new Vector2(8, 12) : new Vector2(8, -12)).RotatedBy(Projectile.rotation);

            float flashProgress = Utils.Clamp((float)Math.Sin(flashCounter / 5f), 0, 1);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(flashProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            Color lerpColor = Color.Lerp(lightColor, Color.White, flashProgress);

            Main.spriteBatch.Draw(texture, Projectile.Center + positionOffset - Main.screenPosition, null, lerpColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 1);
        }

        public float swingAngle = 0;
        private void HandleArmDrawing()
        {
            float staffRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.ToRadians(swingAngle) * -player.direction;

            Projectile.rotation = staffRotation;
            Projectile.spriteDirection = Main.MouseWorld.X < player.Center.X ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            Vector2 positionOffset = (player.direction == -1 ? new Vector2(14, -4) : new Vector2(10, 6)).RotatedBy(staffRotation);
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter + positionOffset);

            float backRotation = player.direction == -1 ? -150 : -30;
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(backRotation));
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.ToRadians(-90));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = 50;
            hitbox.Height = 50;

            Vector2 positionOffset = new Vector2(25, -10 * player.direction).RotatedBy(Projectile.rotation);
            hitbox.X = (int)(player.Center.X - (hitbox.Width / 2f) + positionOffset.X);
            hitbox.Y = (int)(player.Center.Y - (hitbox.Height / 2f) + positionOffset.Y);

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