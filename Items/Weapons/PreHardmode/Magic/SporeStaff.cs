using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class SporeStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Staff");
            Tooltip.SetDefault("Creates a bouncing spore, inflicts poison");
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 21;
            item.magic = true;
            item.mana = 5;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0.5f;
            item.shoot = mod.ProjectileType("SporeMagic");
            item.shootSpeed = 7f;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(331, 9);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}
