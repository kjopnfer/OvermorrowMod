using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Melee
{
    public class GraniteGreatsword : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 13;
            item.melee = true;
            item.width = 50;
            item.height = 50;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6;
            item.value = 10000;
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shootSpeed = 0.7f;
            item.scale = 1f;
        }
    }
}