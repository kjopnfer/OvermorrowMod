using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.WormStaff
{
    public class StaffofWorms : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of the Worms");
            Tooltip.SetDefault("Spits out worms");
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.damage = 15;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.mana = 7;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.UseSound = SoundLoader.GetLegacySoundSlot("OvermorrowMod/Sounds/Items/Hork");
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.knockBack = 0;
            Item.shoot = ModContent.ProjectileType<WormT1>();
            Item.shootSpeed = 16.7f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}