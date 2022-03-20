using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SoulTrail : SimpleTrail
    {
        public SoulTrail() : base(30, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail1"))
        {
        }
    }

    public class SpikeTrail : SimpleTrail
    {
        public SpikeTrail() : base(12, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail2"))
        {
        }
    }
}