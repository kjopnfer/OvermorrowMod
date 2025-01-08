using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Common.Primitives
{
    public interface ITrailEntity
    {
        IEnumerable<TrailConfig> TrailConfigurations();
    }

    public class TrailConfig
    {
        public Type TrailType { get; }
        public Func<float, Color> TrailColor { get; }
        public Func<float, float> TrailSize { get; }

        public TrailConfig(Type trailType, Func<float, Color> trailColor, Func<float, float> trailSize)
        {
            TrailType = trailType;
            TrailColor = trailColor;
            TrailSize = trailSize;
        }
    }
}