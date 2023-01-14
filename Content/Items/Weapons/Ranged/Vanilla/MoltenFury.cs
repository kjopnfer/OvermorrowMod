using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class MoltenFury_Held : HeldBow
    {
        public override Color StringColor => new Color(238, 102, 70);
        public override int ConvertArrow => ItemID.WoodenArrow;
        public override int ArrowType => ProjectileID.FireArrow;
        public override int ParentItem => ItemID.MoltenFury;
    }
}