using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.EatersBlade
{
    public class EatersBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Makes enemies bleed \nBleeding ignores defence");
        }

        public override void SetDefaults()
        {
            item.damage = 10;
            item.melee = true;
            item.autoReuse = true;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 0.5f;
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item1;
        }
    }
}