using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows
{
    public class LeadBow_Held : HeldBow
    {
        public override Color StringColor => new Color(89, 51, 53);
        public override int ParentItem => Terraria.ID.ItemID.LeadBow;
    }
}