using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee.Gecko
{
    public class AerodynamicGecko : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 20;
            item.value = Item.sellPrice(silver: 5);
            item.rare = ItemRarityID.White;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useAnimation = 40;
            item.useTime = 40;
            item.knockBack = 4f;
            item.damage = 6;
            item.shoot = mod.ProjectileType("GeckoT");
            item.shootSpeed = 15.1f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.crit = 2;
            item.channel = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Acorn, 20);
            recipe.AddIngredient(ItemID.Wood, 100);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
