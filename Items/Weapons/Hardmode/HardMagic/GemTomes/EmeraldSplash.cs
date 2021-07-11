/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMagic.GemTomes
{
    public class EmeraldSplash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Emerald Splash");
            Tooltip.SetDefault("'Can not be deflected'");
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.noMelee = true;
            item.magic = true;
            item.channel = true;
            item.rare = ItemRarityID.Green;
            item.width = 28;
            item.knockBack = 3;
            item.height = 30;
            item.useTime = 4;
            item.UseSound = SoundID.Item20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 12f;
            item.useAnimation = 12;
            item.useTime = 8;
            item.autoReuse = true;
            item.reuseDelay = 14;
            item.mana = 3;
            item.shoot = ModContent.ProjectileType<EmeraldMeteor>();
            item.value = Item.sellPrice(silver: 50);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(1525, 1);
            recipe.AddIngredient(549, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}*/
