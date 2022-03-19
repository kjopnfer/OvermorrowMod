using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class TorchTrail : SimpleTrail
    {
        public TorchTrail() : base(22, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail3"), true)
        {
        }
    }
}