using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Magic.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class RubyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Ruby Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 10;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 14;
            item.useTurn = false;
            item.useAnimation = 28;
            item.useTime = 28;
            item.width = 64;
            item.height = 60;
            item.shoot = ModContent.ProjectileType<RubyProj>();
            item.shootSpeed = 14f;
            item.knockBack = 6f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.RubyStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}