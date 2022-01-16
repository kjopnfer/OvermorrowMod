using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SoulTrail : SimpleTrail
    {
        public SoulTrail() : base(30, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail1"))
        {
        }
    }
}