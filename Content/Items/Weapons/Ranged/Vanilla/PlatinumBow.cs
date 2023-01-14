using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class PlatinumBow_Held : HeldBow
    {
        public override Color StringColor => new Color(84, 75, 95);
        public override int ParentItem => Terraria.ID.ItemID.PlatinumBow;
    }
}