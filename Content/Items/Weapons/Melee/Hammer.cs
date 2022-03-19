using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class Hammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hamber");
            Tooltip.SetDefault("you all are lazy fucks");
        }

        public override void SetDefaults()
        {
            item.damage = 28;
            item.width = 52;
            item.height = 52;
            item.melee = true;
            item.autoReuse = false;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4;
            item.value = Item.sellPrice(0, 0, 35, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item1;
        }
    }
}
