using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class FireTrail : SimpleTrail
    {
        public FireTrail() : base(22, OvermorrowModFile.Instance.GetTexture(AssetDirectory.Trails + "Trail3"), true)
        {
        }
    }
}