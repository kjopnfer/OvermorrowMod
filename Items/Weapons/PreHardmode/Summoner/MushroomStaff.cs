using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner
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
            item.rare = ItemRarityID.Blue;
            item.width = 32;
            item.height = 32;
            item.damage = 15;
            item.summon = true;
            item.noMelee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.UseSound = SoundID.Item82;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.sentry = true;
            item.autoReuse = true;
            item.knockBack = 0.1f;
            item.shoot = ModContent.ProjectileType<MushroomSumm>();
            item.shootSpeed = 0f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GlowingMushroom, 35);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
            return true;
        }
    }
}