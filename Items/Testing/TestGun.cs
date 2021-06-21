using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Testing
{
    public class TestGun : ModItem
    {
        public string devtooltip;
        public override void SetDefaults()
        {
            item.damage = 999999999;
            item.ranged = true;
            item.width = 26;
            item.height = 52;
            item.useTime = 1;
            item.useAnimation = 1;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.UseSound = SoundID.Item40;
            item.autoReuse = true;
            item.shoot = ProjectileID.ChlorophyteBullet;
            item.shootSpeed = 11f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test Gun");
            Tooltip.SetDefault("A gun used for testing by devs");
        }

        public override bool AltFunctionUse(Player player)//You use this to allow the item to be right clicked
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 1001; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }

            if (player.altFunctionUse == 2)//Sets what happens on right click(special ability)
            {
                item.shoot = ProjectileID.Bullet;
                item.useTime = 27;
                item.useAnimation = 27;
            }
            else //Sets what happens on left click(normal use)
            {
                item.shoot = ProjectileID.ChlorophyteBullet;
                item.useTime = 1;
                item.useAnimation = 1;
            }

            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
