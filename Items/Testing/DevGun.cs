using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Testing
{
    public class DevGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dev Gun");
            Tooltip.SetDefault("Use this to spawn test projectiles while you do not have an item for them");
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 10;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("GraniteLaser");
            item.shootSpeed = 11f;
        }
    }
}