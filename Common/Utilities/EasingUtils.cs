using Microsoft.Xna.Framework;
using System;

namespace OvermorrowMod.Common.Utilities
{
    /// <summary>
    /// All functions are adapted from https://easings.net/, go there to see how the value is interpolated.
    /// </summary>
    public static class EasingUtils
    {
        public static float EaseOutQuad(float x)
        {
            return 1 - (1 - x) * (1 - x);
        }

        public static float EaseOutCirc(float x)
        {
            return (float)Math.Sqrt(1 - Math.Pow(x - 1, 2));
        }

        public static float EaseOutQuint(float x)
        {
            return (float)(1 - Math.Pow(1 - x, 5));
        }

        public static float EaseOutQuart(float x)
        {
            return (float)(1 - Math.Pow(1 - x, 4));
        }

        public static float EaseInQuad(float x)
        {
            return x * x;
        }

        public static float EaseInCubic(float x)
        {
            return x * x * x;
        }

        public static float EaseInQuart(float x)
        {
            return x * x * x * x;
        }

        public static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1;

            return 1 + c3 * (float)Math.Pow(x - 1, 3) + c1 * (float)Math.Pow(x - 1, 2);
        }
    }
}