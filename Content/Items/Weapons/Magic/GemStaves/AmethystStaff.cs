using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class AmethystStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Amethyst Staff");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.mana = 6;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 12;
            Item.useTurn = false;
            Item.useAnimation = 34;
            Item.useTime = 34;
            Item.width = 48;
            Item.height = 44;
            Item.shoot = ModContent.ProjectileType<AmethystProj>();
            Item.shootSpeed = 8f;
            Item.knockBack = 5f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AmethystStaff)
                .AddIngredient<ManaBar>(3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}