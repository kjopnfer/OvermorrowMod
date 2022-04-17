using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class AmberStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Amber Staff");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 15;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 20;
            Item.useTurn = false;
            Item.useAnimation = 33;
            Item.useTime = 33;
            Item.width = 48;
            Item.height = 48;
            Item.shoot = ModContent.ProjectileType<AmberProj>();
            Item.shootSpeed = 5f;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AmberStaff)
                .AddIngredient<ManaBar>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}