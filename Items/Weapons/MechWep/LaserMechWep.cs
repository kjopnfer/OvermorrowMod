/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.MechWep
{
    public class LaserMechWep : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rapid Fire Laser Gun");
            Tooltip.SetDefault("Shoots Lasers at a high rate \nVery inaccurate");
        }

        public override void SetDefaults()
        {
            item.damage = 38;
            item.ranged = true;
            item.width = 56;
            item.noUseGraphic = true;
            item.height = 22;
            item.useTime = 7;
            item.useAnimation = 25;
            item.channel = true;
            item.noMelee = true;
            item.tileBoost++;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 6;
            item.value = Item.buyPrice(0, 22, 50, 0);
            item.rare = ItemRarityID.Cyan;
            item.UseSound = SoundID.Item23;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("LaserMechWepProj");
            item.shootSpeed = 6f;
        }
    }
}*/