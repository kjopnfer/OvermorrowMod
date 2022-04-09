using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class TorchTrail : SimpleTrail
    {
        public TorchTrail() : base(15, OvermorrowModFile.Instance.GetTexture(AssetDirectory.Trails + "Trail1"), true)
        {
        }
    }
}