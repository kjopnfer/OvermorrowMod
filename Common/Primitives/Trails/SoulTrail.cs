using OvermorrowMod.Common;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SoulTrail : SimpleTrail
    {
        public SoulTrail() : base(30, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail1"))
        {
        }
    }
}