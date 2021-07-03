using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardMelee
{
    public class PlasmaKnifeWep : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Knife");
            Tooltip.SetDefault("Rings arround enemies when fire is held, launches when released");
        }
        public override void SetDefaults()
        {
            item.channel = true;
            item.width = 32;
            item.height = 32;
            item.damage = 33;
            item.melee = true;
            item.noMelee = true;
            item.useTime = 11;
            item.useAnimation = 11;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item19;
            item.autoReuse = true;
            item.knockBack = 0;
            item.shoot = mod.ProjectileType("PlasmaKnife");
            item.shootSpeed = 12.5f;
        }
    }
}