using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class FireTrail : SimpleTrail
    {
        public FireTrail() : base(22, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"), true)
        {
        }
    }
}