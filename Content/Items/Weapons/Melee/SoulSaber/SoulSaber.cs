using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace OvermorrowMod.Content.Items.Weapons.Melee.SoulSaber
{
    public class SoulSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soulflame Igniters");
            Tooltip.SetDefault("Spins a double-edged spear around the player, inflicting SoulFlame\n" +
                "Right click while spinning to charge up the weapon\n" +
                "Shoots a ring of fire when ready, inflicting Greater SoulFlame on all enemies inside\n" +
                "'I am the bone of my spear'");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 32;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<SoulSpin>();
            Item.shootSpeed = 11f;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Green;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] < 2)
            {
                Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 1);
            }

            return false;
        }

        public override void AddRecipes() //prolly needs a change
        {
            CreateRecipe()
                .AddIngredient(ItemID.Bone, 35)
                .AddIngredient<SoulFire>()
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}