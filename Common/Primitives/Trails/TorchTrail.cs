using OvermorrowMod.Common;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class TorchTrail : SimpleTrail
    {
        public TorchTrail() : base(22, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"), true)
        {
        }
    }
}