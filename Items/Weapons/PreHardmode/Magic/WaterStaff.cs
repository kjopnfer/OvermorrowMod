using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class WaterStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Blue;
            item.mana = 9;
            item.UseSound = SoundID.Item21;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 16;
            item.useTurn = false;
            item.useAnimation = 32;
            item.useTime = 32;
            item.width = 50;
            item.height = 56;
            item.shoot = ModContent.ProjectileType<NatureBolt>();
            item.shootSpeed = 7f;
            item.knockBack = 3f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WaterBar>(), 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}