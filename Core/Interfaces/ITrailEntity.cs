using Microsoft.Xna.Framework;
using System;

namespace OvermorrowMod.Core.Interfaces
{
    public interface ITrailEntity
    {
        Type TrailType();
        Color TrailColor(float progress);
        float TrailSize(float progress);
    }
}