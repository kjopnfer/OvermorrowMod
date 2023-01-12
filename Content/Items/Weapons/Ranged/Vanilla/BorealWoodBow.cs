using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class BorealWoodBow_Held : HeldBow
    {
        public override Color StringColor => new Color(29, 24, 21);
        public override int ParentItem => Terraria.ID.ItemID.BorealWoodBow;
    }
}