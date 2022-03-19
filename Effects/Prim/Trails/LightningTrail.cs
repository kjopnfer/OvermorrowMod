using OvermorrowMod.Common;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class LightningTrail : SimpleTrail
    {
        public LightningTrail() : base(40, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail5"), true)
        {
        }
    }
}