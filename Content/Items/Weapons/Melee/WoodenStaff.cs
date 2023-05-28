using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Item.damage = 25;
            Item.knockBack = 6.5f;
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

    /*public class WoodenStaff_Held : ModProjectile
    {
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
        }

        Player player => Main.player[Projectile.owner];
        public override void AI()
        {
            if (player.active && player.HeldItem.type == ModContent.ItemType<WoodenStaff>()) Projectile.timeLeft = 10;

            Projectile.width = Projectile.height = 40;

            player.heldProj = Projectile.whoAmI;

            float staffRotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();

            Projectile.rotation = staffRotation;
            Projectile.spriteDirection = staffRotation > MathHelper.PiOver2 || staffRotation < -MathHelper.PiOver2 ? -1 : 1;
            player.direction = Projectile.spriteDirection;

            //Vector2 positionOffset = (player.direction == -1 ? PositionOffset.Item1 : PositionOffset.Item2).RotatedBy(gunRotation);
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

        public override void Kill(int timeLeft)
        {
            Main.NewText("dide");
            base.Kill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            var spriteEffects = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2f, Projectile.scale, spriteEffects, 1);

            return false;
        }
    }*/

    public class WoodenStaff_Held : ModProjectile
    {
        // Define the range of the Spear Projectile. These are overrideable properties, in case you'll want to make a class inheriting from this one.
        protected virtual float HoldoutRangeMin => 24f;
        protected virtual float HoldoutRangeMax => 96f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner]; // Since we access the owner player instance so much, it's useful to create a helper local variable for this
            int duration = player.itemAnimationMax; // Define the duration the projectile will exist in frames

            player.heldProj = Projectile.whoAmI; // Update the player's held projectile id

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            //Projectile.velocity = Vector2.Normalize(Projectile.velocity); // Velocity isn't used in this spear implementation, but we use the field to store the spear's attack direction.
            Vector2 attackDirection = Vector2.Normalize(Projectile.velocity);

            float halfDuration = duration * 0.5f;
            float progress;

            // Here 'progress' is set to a value that goes from 0.0 to 1.0 and back during the item use animation.
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(attackDirection * HoldoutRangeMin, attackDirection * HoldoutRangeMax, progress);

            // Apply proper rotation to the sprite.
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            return false; // Don't execute vanilla AI.
        }
    }
}