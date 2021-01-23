using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.WardenClass.Weapons.ChainWeapons
{
    public class VilePiercer : PiercingItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Piercer");
        }

        public override void SafeSetDefaults()
        {
            item.CloneDefaults(ItemID.ChainKnife);
            item.damage = 32;
            item.width = 30;
            item.height = 10;
            item.useTime = 10;
            item.shootSpeed = 24f;

            item.noUseGraphic = true;
            item.useStyle = 5;
            item.color = Color.Purple;
            item.knockBack = 0;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item71;
        }
    }
}