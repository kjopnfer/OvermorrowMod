using Terraria;
using Terraria.ID;
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
            Item.damage = 28;
            Item.width = 52;
            Item.height = 52;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = false;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 35, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1;
        }
    }
}
