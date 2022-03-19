using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Projectiles.Magic.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic
{
    public class TopazStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Topaz Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 7;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 14;
            item.useTurn = false;
            item.useAnimation = 22;
            item.useTime = 22;
            item.width = 48;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<TopazProj>();
            item.shootSpeed = 9.5f;
            item.knockBack = 4.5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TopazStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}