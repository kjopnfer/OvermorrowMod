using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class TorchTrail : SimpleTrail
    {
        public TorchTrail() : base(32, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail1"), true)
        {
        }
    }

    public class MeteorTrail : SimpleTrail
    {
        public MeteorTrail() : base(64, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail1"), true)
        {
        }
    }
}