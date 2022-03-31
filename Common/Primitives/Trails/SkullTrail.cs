using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SkullTrail : SimpleTrail
    {
        public SkullTrail() : base(12, OvermorrowModFile.Instance.GetTexture(AssetDirectory.Trails + "Trail6"), true)
        {
        }
    }
}