using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SpinTrail : SimpleTrail
    {
        //public SpinTrail() : base(15, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail2"), true)
        public SpinTrail() : base(/*60*/120, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Extra_209"), false)
        {

        }
    }
}