using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Bows
{
    public class EbonwoodBow_Held : HeldBow
    {
        public override Color StringColor => new Color(31, 29, 41);
        public override int ParentItem => Terraria.ID.ItemID.EbonwoodBow;
    }
}