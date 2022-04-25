using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class SapphireStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Sapphire Staff");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Orange;
            Item.mana = 8;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 15;
            Item.useTurn = false;
            Item.useAnimation = 19;
            Item.useTime = 19;
            Item.width = 48;
            Item.height = 48;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SapphireProj>();
            Item.shootSpeed = 10.5f;
            Item.knockBack = 4.5f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SapphireStaff)
                .AddIngredient<ManaBar>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}