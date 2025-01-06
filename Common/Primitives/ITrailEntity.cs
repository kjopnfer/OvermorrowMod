using Microsoft.Xna.Framework;
using System;

namespace OvermorrowMod.Common.Primitives
{
    public interface ITrailEntity
    {
        Type TrailType();
        Color TrailColor(float progress);
        float TrailSize(float progress);
    }
}