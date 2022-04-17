using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.MushroomStaff
{
    public class MushroomStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mushroom Staff");
            Tooltip.SetDefault("Summons a mushroom sentry to fight for you");
        }
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 32;
            Item.height = 32;
            Item.damage = 15;
            Item.DamageType = DamageClass.Summon;
            Item.noMelee = true;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item82;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.sentry = true;
            Item.autoReuse = true;
            Item.knockBack = 0.1f;
            Item.shoot = ModContent.ProjectileType<MushroomSumm>();
            Item.shootSpeed = 0f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.GlowingMushroom, 35)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
        }
    }
}