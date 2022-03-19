using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class LightningTrail : SimpleTrail
    {
        public LightningTrail() : base(40, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail5"), true)
        {
        }
    }
}