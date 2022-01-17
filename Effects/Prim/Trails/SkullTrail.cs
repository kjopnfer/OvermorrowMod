using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SkullTrail : SimpleTrail
    {
        public SkullTrail() : base(12, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail6"), true)
        {
        }
    }
}