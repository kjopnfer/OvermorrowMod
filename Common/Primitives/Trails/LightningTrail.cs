using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class LightningTrail : SimpleTrail
    {
        public LightningTrail() : base(20, OvermorrowModFile.Instance.GetTexture(AssetDirectory.Trails + "Trail5"), true)
        {
        }
    }
}