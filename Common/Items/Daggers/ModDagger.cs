using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items.Daggers;
using OvermorrowMod.Core.Items.Daggers;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Daggers
{
    public abstract class ModDagger<HeldProjectile, ThrownProjectile> : ModItem
        where HeldProjectile : HeldDagger
        where ThrownProjectile : ThrownDagger
    {
        public override bool AltFunctionUse(Player player) => true;

        public int ComboCount { get; private set; } = 0;
        int slashDirection = 1;

        protected virtual int MaxComboCount => 3;
        protected virtual bool HasCrossSlash => true;
        protected virtual int MaxThrownDaggers => 2;

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<HeldProjectile>()] <= 0 &&
                  player.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] < MaxThrownDaggers;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                // Right click for throwing
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<ThrownProjectile>(), damage, knockback, player.whoAmI, 0f);
            }
            else
            {
                // Left click for combo slash attacks
                if (Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<HeldProjectile>()))
                    return false;

                int offhand = 1;
                slashDirection = -slashDirection;

                bool isCrossSlash = HasCrossSlash && ComboCount == MaxComboCount &&
                                  player.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] != 1;

                if (isCrossSlash)
                {
                    slashDirection = player.direction == 1 ? -1 : 1;
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeldProjectile>(), damage, knockback, player.whoAmI, slashDirection);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeldProjectile>(), damage, knockback, player.whoAmI, slashDirection, offhand);
                }
                else
                {
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeldProjectile>(), damage, knockback, player.whoAmI, slashDirection);

                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] != 1)
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeldProjectile>(), damage, knockback, player.whoAmI, slashDirection, offhand);
                }

                ComboCount++;

                // Reset combo count based on available daggers
                int maxCombo = player.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] == 1 ?
                              (MaxComboCount - 1) : MaxComboCount;

                if (ComboCount > maxCombo)
                    ComboCount = 0;
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Main.GetItemDrawFrame(Item.type, out var itemTexture, out var itemFrame);
            Vector2 drawOrigin = itemFrame.Size() / 2f;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, drawOrigin.Y);

            spriteBatch.Draw(texture, drawPosition, null, lightColor, 0f, drawOrigin, scale, SpriteEffects.FlipHorizontally, 1);
            spriteBatch.Draw(texture, drawPosition, null, lightColor, 0f, drawOrigin, scale, SpriteEffects.None, 1);

            return false;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Color backKnifeColor = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] == MaxThrownDaggers ? Color.Black : drawColor;
            Color frontKnifeColor = Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ThrownProjectile>()] >= 1 ? Color.Black : drawColor;

            spriteBatch.Draw(texture, position, frame, backKnifeColor, 0f, origin, scale, SpriteEffects.FlipHorizontally, 1);
            spriteBatch.Draw(texture, position, frame, frontKnifeColor, 0f, origin, scale, SpriteEffects.None, 1);

            return false;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 102;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;

            Item.knockBack = 2;
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
            Item.damage = 13;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<ThrownProjectile>();

            SafeSetDefaults();
        }

        public virtual void SafeSetDefaults() { }
    }
}