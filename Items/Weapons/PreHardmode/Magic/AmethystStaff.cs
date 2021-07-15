using OvermorrowMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Projectiles.Magic.Gems;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class AmethystStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Amethyst Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 6;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 12;
            item.useTurn = false;
            item.useAnimation = 34;
            item.useTime = 34;
            item.width = 48;
            item.height = 44;
            item.shoot = ModContent.ProjectileType<AmethystProj>();
            item.shootSpeed = 8f;
            item.knockBack = 5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AmethystStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}