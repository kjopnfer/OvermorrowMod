using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class BowPlayer : ModPlayer
    {
        public int ArrowArmorPenetration;
        //public int PracticeTargetCounter = 0;
        public List<IBowModifier> ActiveModifiers { get; private set; } = new List<IBowModifier>();
        public List<IBowDrawEffects> ActiveDrawEffects { get; private set; } = new List<IBowDrawEffects>();

        public override void ResetEffects()
        {
            ArrowArmorPenetration = 0;
            ActiveModifiers.Clear();
            ActiveDrawEffects.Clear();
        }

        public override void PostUpdateEquips()
        {
            //if (!Player.GetModPlayer<OvermorrowModPlayer>().PracticeTarget)
            //    PracticeTargetCounter = 0;
        }

        public void AddBowModifier(IBowModifier modifier)
        {
            ActiveModifiers.Add(modifier);
        }

        public void AddBowDrawEffect(IBowDrawEffects drawEffect)
        {
            ActiveDrawEffects.Add(drawEffect);
        }
    }
}