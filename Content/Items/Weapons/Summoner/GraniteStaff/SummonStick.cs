using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff
{
    public class SummonStick : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.width = 26;
            Item.height = 52;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ProjectileID.MoonlordTurretLaser;
            Item.shootSpeed = 14f;
            Item.value = Item.sellPrice(0, 1, 0, 0);
        }


        public override void SetStaticDefaults()
        {

            Tooltip.SetDefault("Use this with some summon weapons to make them work");
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 10)
                .Register();
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
