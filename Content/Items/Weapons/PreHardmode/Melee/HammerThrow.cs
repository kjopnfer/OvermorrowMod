using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.PreHardmode.Melee
{
    public class HammerThrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hammer Throw");
            Tooltip.SetDefault("Use this to spawn test projectiles while you do not have an item for them");
            Item.staff[item.type] = true;
        }
        public override void SetDefaults()
        {

            item.width = 32;
            item.height = 32;
            item.damage = 10;
            item.ranged = true;
            item.noMelee = true;
            item.useTime = 51;
            item.useAnimation = 51;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = false;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("VeloIncrease");
            item.shootSpeed = 11f;
            item.channel = true;
        }
    }
}