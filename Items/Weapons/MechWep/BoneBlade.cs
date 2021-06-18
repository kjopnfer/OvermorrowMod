using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Items.Weapons.MechWep
{
    public class BoneBlade : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 90;
            item.melee = true;
            item.width = 273;
            item.height = 241;
            item.useTime = 10;
            item.useAnimation = 40;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 10;
            item.shootSpeed = 15f;
            item.value = Item.sellPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.UseSound = SoundID.Item71;
            item.autoReuse = false;
            item.useTurn = true;
            item.shoot = mod.ProjectileType("SawBlade");
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechanical Bonesaw Blade");
            Tooltip.SetDefault("Giant sword that leaves a trail of saws");

        }
    }
}