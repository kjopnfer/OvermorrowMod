using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class DiamondStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Diamond Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 11;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 20;
            item.useTurn = false;
            item.useAnimation = 28;
            item.useTime = 14;
            item.width = 48;
            item.height = 48;
            item.shoot = ProjectileID.DiamondBolt;
            item.shootSpeed = 14f;
            item.knockBack = 6f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DiamondStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}