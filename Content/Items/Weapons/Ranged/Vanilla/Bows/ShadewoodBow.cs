using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows
{
    public class ShadewoodBow_Held : HeldBow
    {
        public override Color StringColor => new Color(24, 31, 44);
        public override int ParentItem => Terraria.ID.ItemID.ShadewoodBow;
    }
}