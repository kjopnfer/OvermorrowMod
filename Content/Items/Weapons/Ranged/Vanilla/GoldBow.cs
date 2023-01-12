using Microsoft.Xna.Framework;
using OvermorrowMod.Common.VanillaOverrides.Bow;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla
{
    public class GoldBow_Held : HeldBow
    {
        public override Color StringColor => new Color(75, 86, 95);
        public override int ParentItem => Terraria.ID.ItemID.GoldBow;
    }
}