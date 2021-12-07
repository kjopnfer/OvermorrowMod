using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace OvermorrowMod.Effects.Prim
{
    public interface ITrailEntity
    {
        Type TrailType();
        Color TrailColor(float progress);
        float TrailSize(float progress);
    }
}