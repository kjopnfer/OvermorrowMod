using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class WoodenBow_Held : HeldBow
    {
        public override Color StringColor => new Color(83, 65, 50);
        public override int ParentItem => Terraria.ID.ItemID.WoodenBow;
    }
}