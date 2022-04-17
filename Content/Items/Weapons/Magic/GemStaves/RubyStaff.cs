using OvermorrowMod.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class RubyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greater Ruby Staff");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 10;
            Item.UseSound = SoundID.Item43;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 14;
            Item.useTurn = false;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.width = 64;
            Item.height = 60;
            Item.shoot = ModContent.ProjectileType<RubyProj>();
            Item.shootSpeed = 14f;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Magic;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.RubyStaff)
                .AddIngredient<ManaBar>(6)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}