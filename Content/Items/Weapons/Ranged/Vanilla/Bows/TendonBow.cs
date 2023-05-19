using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows
{
    public class TendonBow_Held : HeldBow
    {
        public override Color StringColor => new Color(224, 12, 17);
        public override int ParentItem => Terraria.ID.ItemID.TendonBow;
    }
}