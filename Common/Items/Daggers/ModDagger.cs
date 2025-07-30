using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Daggers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract class ModDagger<HeldProjectile, ThrownProjectile> : ModItem, IProjectileClassification
        where HeldProjectile : HeldDagger
        where ThrownProjectile : ThrownDagger
    {
        public WeaponType WeaponType => WeaponType.Dagger;
        public virtual void SafeSetDefaults() { }

        protected bool isDualWielding => Item.stack == 2 && Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < 1;

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0 &&
                   player.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < Item.stack;
        }

        public override bool AltFunctionUse(Player player) => true; // Always allow right click, the HeldDagger will check CanThrow

        public sealed override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.maxStack = 2;
            Item.shoot = ModContent.ProjectileType<HeldProjectile>();

            SafeSetDefaults();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Right click for throwing
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, (int)DaggerAttack.Throw);
            }
            else
            {
                // Normal attack - get current attack from combo sequence
                var daggerPlayer = player.GetModPlayer<DaggerPlayer>();

                // Create main hand dagger - it will automatically use the current combo index
                Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);

                // Create off-hand dagger if dual wielding
                if (isDualWielding)
                {
                    Projectile offHand = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);

                    // Offset the off-hand dagger's AI counter to create stagger
                    offHand.ai[2] = -5f; // Start 5 ticks behind
                }
            }

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            if (Item.stack == 2)
            {
                // Draw dual wielded daggers with different colors based on thrown status
                Color backKnifeColor = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] == 2 ? Color.Black : drawColor;
                Color frontKnifeColor = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] >= 1 ? Color.Black : drawColor;

                // Draw back dagger (flipped)
                spriteBatch.Draw(texture, position, frame, backKnifeColor, 0f, origin, scale, SpriteEffects.FlipHorizontally, 1);
                // Draw front dagger
                spriteBatch.Draw(texture, position, frame, frontKnifeColor, 0f, origin, scale, SpriteEffects.None, 1);
            }
            else
            {
                Color color = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < 1 ? drawColor : Color.Black;
                spriteBatch.Draw(texture, position, frame, color, 0f, origin, scale, SpriteEffects.None, 1);
            }

            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Add dual wield info
            tooltips.Add(new TooltipLine(Mod, "DualWield", "Can be dual wielded")
            {
                OverrideColor = Color.Orange
            });

            // Add throw info  
            tooltips.Add(new TooltipLine(Mod, "Throwing", "Right click to throw")
            {
                OverrideColor = Color.Yellow
            });
        }

        public override bool ConsumeItem(Player player)
        {
            // Don't consume the item when using it
            return false;
        }
    }
}