using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SpinTrail : SimpleTrail
    {
        public SpinTrail() : base(15, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail2"), true)
        {
        }
    }
}