using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Bow
{
    public class BowPlayer : ModPlayer
    {
        public int ArrowArmorPenetration;
        public int PracticeTargetCounter = 0;

        public override void ResetEffects()
        {
            ArrowArmorPenetration = 0;
        }
    }
}