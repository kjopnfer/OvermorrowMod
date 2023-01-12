using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class CopperBow_Held : HeldBow
    {
        public override Color StringColor => new Color(114, 81, 56);
        public override int ParentItem => Terraria.ID.ItemID.CopperBow;

    }
}