using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Content.Items.Materials;
using Microsoft.Xna.Framework;

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
            item.width = 32;
            item.height = 32;
            item.damage = 32;
            item.melee = true;
            item.noMelee = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = ModContent.ProjectileType<SoulSpin>();
            item.shootSpeed = 11f;
            item.channel = true;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.Green;

        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.ownedProjectileCounts[type] < 2)
            {
                Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 1);
            }

            return false;
        }

        public override void AddRecipes() //prolly needs a change
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 35);
            recipe.AddIngredient(ModContent.ItemType<SoulFire>(), 1); //idk how this is obtained so...
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}