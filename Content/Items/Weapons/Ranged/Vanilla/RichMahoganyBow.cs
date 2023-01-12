using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class RichMahoganyBow_Held : HeldBow
    {
        public override Color StringColor => new Color(50, 24, 26);
        public override int ParentItem => Terraria.ID.ItemID.RichMahoganyBow;
    }
}