using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.WaterSword
{
    public class WaterSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Falchion");
            Tooltip.SetDefault("Launches a short ranged water slash");
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.damage = 20;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.width = 56;
            Item.height = 54;
            Item.shoot = ModContent.ProjectileType<WaterSlash>();
            Item.shootSpeed = 8f;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<WaterBar>(9)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}