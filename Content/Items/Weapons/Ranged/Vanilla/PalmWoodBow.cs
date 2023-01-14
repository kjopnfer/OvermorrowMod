using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class PalmWoodBow_Held : HeldBow
    {
        public override Color StringColor => new Color(44, 34, 28);
        public override int ParentItem => Terraria.ID.ItemID.PalmWoodBow;
    }
}