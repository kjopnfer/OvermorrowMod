using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Projectiles.Magic.Gems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic
{
    public class SapphireStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Sapphire Staff");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Orange;
            item.mana = 8;
            item.UseSound = SoundID.Item43;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 15;
            item.useTurn = false;
            item.useAnimation = 19;
            item.useTime = 19;
            item.width = 48;
            item.height = 48;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SapphireProj>();
            item.shootSpeed = 10.5f;
            item.knockBack = 4.5f;
            item.magic = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SapphireStaff);
            recipe.AddIngredient(ModContent.ItemType<ManaBar>(), 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}