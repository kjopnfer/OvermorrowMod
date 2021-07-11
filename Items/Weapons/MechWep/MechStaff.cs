/*using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.MechWep
{
    public class MechStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mech Staff");
            Tooltip.SetDefault("Shoot a ball arround your mouse");
        }

        public override void SetDefaults()
        {
            item.damage = 63;
            item.noMelee = true;
            item.magic = true;
            item.mana = 1;
            item.rare = ItemRarityID.Pink;
            item.width = 28;
            item.height = 30;
            item.useTime = 18;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.shootSpeed = 3.5f;
            item.autoReuse = true;
            item.useAnimation = 18;
            item.shoot = mod.ProjectileType("MechCircle");
            item.value = Item.sellPrice(silver: 3);
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3, 0);
        }
    }
}*/
