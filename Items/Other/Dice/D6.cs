/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Other.Dice
{
    public class D6 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Die");
            Tooltip.SetDefault("Either kills or Heals and Rerolls Enemies \nDoes not work on bossed \nExtremely Unstable");
        }
        public override void SetDefaults()
        {
            item.damage = 1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 60;
            item.magic = true;
            item.mana = 50;
            item.useAnimation = 60;
            item.shootSpeed = 10f;
            item.shoot = mod.ProjectileType("ZAWARUDObomb");
            item.noUseGraphic = true;
            item.crit = 4;
        }
        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }
    }
}*/
