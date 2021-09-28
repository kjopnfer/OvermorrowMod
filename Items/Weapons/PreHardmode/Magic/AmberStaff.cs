using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Magic.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class AmberStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Amber Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 15;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 20;
            item.useTurn = false;
            item.useAnimation = 33;
            item.useTime = 33;
            item.width = 48;
            item.height = 48;
            item.shoot = ModContent.ProjectileType<AmbeProj>();
            item.shootSpeed = 5f;
            item.knockBack = 6f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AmberStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}