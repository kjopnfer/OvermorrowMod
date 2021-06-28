using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using OvermorrowMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class Ruptor : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Boomruptor Cannon");
            Tooltip.SetDefault("Fires a bomb that sticks into enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 11;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 31;
            item.useAnimation = 31;
            item.autoReuse = true;
            item.shootSpeed = 17f;
            item.knockBack = 0;
            item.ranged = true;
            item.UseSound = SoundID.Item61;
            item.shoot = ModContent.ProjectileType<AmoungUsExplosive>();
            item.crit = 5;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe1 = new ModRecipe(mod);
            recipe1.AddIngredient(57, 12);
            recipe1.AddTile(TileID.Anvils);
            recipe1.SetResult(this);
            recipe1.AddRecipe();


            ModRecipe recipe2 = new ModRecipe(mod);
            recipe2.AddIngredient(1257, 12);
            recipe2.AddTile(TileID.Anvils);
            recipe2.SetResult(this);
            recipe2.AddRecipe();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
    }
}
