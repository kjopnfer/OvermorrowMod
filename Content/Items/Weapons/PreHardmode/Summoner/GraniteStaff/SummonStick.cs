using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Summoner.GraniteStaff
{
    public class SummonStick : ModItem
    {

        public override void SetDefaults()
        {
            item.damage = 0;
            item.width = 26;
            item.height = 52;
            item.useTime = 1;
            item.useAnimation = 1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ProjectileID.MoonlordTurretLaser;
            item.shootSpeed = 14f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }


        public override void SetStaticDefaults()
        {

            Tooltip.SetDefault("Use this with some summon weapons to make them work");
        }


        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Wood, 10);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
