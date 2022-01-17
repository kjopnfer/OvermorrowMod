using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SixShooterTrail : SimpleTrail
    {
        public SixShooterTrail() : base(20, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail6"), true)
        {
        }
    }
}