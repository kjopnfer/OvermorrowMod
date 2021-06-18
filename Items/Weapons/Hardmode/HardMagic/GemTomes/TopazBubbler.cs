using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    public class TopazBubbler : ModItem
    {
        public override void SetDefaults()
        {
            item.magic = true;
            item.noMelee = true;
            item.damage = 30;
            item.useTime = 20;
            item.useAnimation = 17;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 7;
            item.mana = 3;
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item111;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("TopazBubble");
            item.shootSpeed = 14f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Topaz Bubbler");
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(1523, 1);
            recipe.AddIngredient(547, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
