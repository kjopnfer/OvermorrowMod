using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic
{
    public class SpliterStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Splite rStaff");
            Item.staff[item.type] = true;
            Tooltip.SetDefault("Splits foes into two on impact");
        }


        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 17;
            item.magic = true;
            item.mana = 5;
            item.noMelee = true;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("Spliter");
            item.shootSpeed = 11f;
        }
    }
}