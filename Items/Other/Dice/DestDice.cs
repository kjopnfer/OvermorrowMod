using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Other.Dice
{
    public class DestDice : ModItem
    {
        public override void SetDefaults()
        {
            item.magic = true;
            item.noMelee = true;
            item.damage = 1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 60;
            item.mana = 50;
            item.noUseGraphic = true;
            item.useAnimation = 60;
            item.knockBack = 0;
            item.rare = ItemRarityID.Blue;
            item.autoReuse = false;
            item.shoot = mod.ProjectileType("RandomDice");
            item.shootSpeed = 10f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Destiny Die");
            Tooltip.SetDefault("Does 1 of 6 random effects");
        }

        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(ItemID.Grenade, 1);
            recipe1.AddIngredient(ItemID.DiamondStaff, 1);
            recipe1.AddIngredient(ItemID.WoodenArrow, 1);
            recipe1.AddIngredient(ItemID.Torch, 1);
            recipe1.AddIngredient(ItemID.DemoniteOre, 1);
            recipe1.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();

            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(ItemID.Grenade, 1);
            recipe2.AddIngredient(ItemID.AmethystStaff, 1);
            recipe2.AddIngredient(ItemID.WoodenArrow, 1);
            recipe2.AddIngredient(ItemID.Torch, 1);
            recipe2.AddIngredient(ItemID.DemoniteOre, 1);
            recipe2.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();

            ModRecipe recipe3 = new ModRecipe(mod);
            recipe3.AddIngredient(ItemID.Grenade, 1);
            recipe3.AddIngredient(ItemID.TopazStaff, 1);
            recipe3.AddIngredient(ItemID.WoodenArrow, 1);
            recipe3.AddIngredient(ItemID.Torch, 1);
            recipe3.AddIngredient(ItemID.DemoniteOre, 1);
            recipe3.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe3.AddTile(TileID.Anvils);
            recipe3.SetResult(this);
            recipe3.AddRecipe();

            ModRecipe recipe4 = new ModRecipe(mod);
            recipe4.AddIngredient(ItemID.Grenade, 1);
            recipe4.AddIngredient(ItemID.SapphireStaff, 1);
            recipe4.AddIngredient(ItemID.WoodenArrow, 1);
            recipe4.AddIngredient(ItemID.Torch, 1);
            recipe4.AddIngredient(ItemID.DemoniteOre, 1);
            recipe4.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe4.AddTile(TileID.Anvils);
            recipe4.SetResult(this);
            recipe4.AddRecipe();

            ModRecipe recipe5 = new ModRecipe(mod);
            recipe5.AddIngredient(ItemID.Grenade, 1);
            recipe5.AddIngredient(ItemID.EmeraldStaff, 1);
            recipe5.AddIngredient(ItemID.WoodenArrow, 1);
            recipe5.AddIngredient(ItemID.Torch, 1);
            recipe5.AddIngredient(ItemID.DemoniteOre, 1);
            recipe5.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe5.AddTile(TileID.Anvils);
            recipe5.SetResult(this);
            recipe5.AddRecipe();

            ModRecipe recipe6 = new ModRecipe(mod);
            recipe6.AddIngredient(ItemID.Grenade, 1);
            recipe6.AddIngredient(ItemID.RubyStaff, 1);
            recipe6.AddIngredient(ItemID.WoodenArrow, 1);
            recipe6.AddIngredient(ItemID.Torch, 1);
            recipe6.AddIngredient(ItemID.DemoniteOre, 1);
            recipe6.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe6.AddTile(TileID.Anvils);
            recipe6.SetResult(this);
            recipe6.AddRecipe();

            ModRecipe recipe7 = new ModRecipe(mod);
            recipe7.AddIngredient(ItemID.Grenade, 1);
            recipe7.AddIngredient(ItemID.RubyStaff, 1);
            recipe7.AddIngredient(ItemID.WoodenArrow, 1);
            recipe7.AddIngredient(ItemID.Torch, 1);
            recipe7.AddIngredient(ItemID.CrimtaneOre, 1);
            recipe7.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe7.AddTile(TileID.Anvils);
            recipe7.SetResult(this);
            recipe7.AddRecipe();

            ModRecipe recipe8 = new ModRecipe(mod);
            recipe8.AddIngredient(ItemID.Grenade, 1);
            recipe8.AddIngredient(ItemID.EmeraldStaff, 1);
            recipe8.AddIngredient(ItemID.WoodenArrow, 1);
            recipe8.AddIngredient(ItemID.Torch, 1);
            recipe8.AddIngredient(ItemID.CrimtaneOre, 1);
            recipe8.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe8.AddTile(TileID.Anvils);
            recipe8.SetResult(this);
            recipe8.AddRecipe();

            ModRecipe recipe9 = new ModRecipe(mod);
            recipe9.AddIngredient(ItemID.Grenade, 1);
            recipe9.AddIngredient(ItemID.SapphireStaff, 1);
            recipe9.AddIngredient(ItemID.WoodenArrow, 1);
            recipe9.AddIngredient(ItemID.Torch, 1);
            recipe9.AddIngredient(ItemID.CrimtaneOre, 1);
            recipe9.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe9.AddTile(TileID.Anvils);
            recipe9.SetResult(this);
            recipe9.AddRecipe();

            ModRecipe recipe10 = new ModRecipe(mod);
            recipe10.AddIngredient(ItemID.Grenade, 1);
            recipe10.AddIngredient(ItemID.TopazStaff, 1);
            recipe10.AddIngredient(ItemID.WoodenArrow, 1);
            recipe10.AddIngredient(ItemID.Torch, 1);
            recipe10.AddIngredient(ItemID.CrimtaneOre, 1);
            recipe10.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe10.AddTile(TileID.Anvils);
            recipe10.SetResult(this);
            recipe10.AddRecipe();

            ModRecipe recipe11 = new ModRecipe(mod);
            recipe11.AddIngredient(ItemID.Grenade, 1);
            recipe11.AddIngredient(ItemID.AmethystStaff, 1);
            recipe11.AddIngredient(ItemID.WoodenArrow, 1);
            recipe11.AddIngredient(ItemID.Torch, 1);
            recipe11.AddIngredient(ItemID.CrimtaneOre, 1);
            recipe11.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe11.AddTile(TileID.Anvils);
            recipe11.SetResult(this);
            recipe11.AddRecipe();

            ModRecipe recipe12 = new ModRecipe(mod);
            recipe12.AddIngredient(ItemID.Grenade, 1);
            recipe12.AddIngredient(ItemID.DiamondStaff, 1);
            recipe12.AddIngredient(ItemID.WoodenArrow, 1);
            recipe12.AddIngredient(ItemID.Torch, 1);
            recipe12.AddIngredient(ItemID.CrimtaneOre, 1);
            recipe12.AddIngredient(ItemID.LesserHealingPotion, 1);
            recipe12.AddTile(TileID.Anvils);
            recipe12.SetResult(this);
            recipe12.AddRecipe();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(speedX, speedY)) * 0f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }
            return true;
        }
    }
}
