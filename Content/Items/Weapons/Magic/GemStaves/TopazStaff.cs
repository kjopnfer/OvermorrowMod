using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class TopazStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Topaz Staff");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.mana = 7;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 14;
            Item.useTurn = false;
            Item.useAnimation = 22;
            Item.useTime = 22;
            Item.width = 48;
            Item.height = 48;
            Item.shoot = ModContent.ProjectileType<TopazProj>();
            Item.shootSpeed = 9.5f;
            Item.knockBack = 4.5f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.TopazStaff)
                .AddIngredient<ManaBar>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}