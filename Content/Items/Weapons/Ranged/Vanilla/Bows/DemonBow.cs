using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows
{
    public class DemonBow_Held : HeldBow
    {
        public override Color StringColor => new Color(65, 45, 85);
        public override int ParentItem => Terraria.ID.ItemID.DemonBow;

    }
}