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

        public static float EaseOutBounce(float x)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (x < 1 / d1)
            {
                return n1 * x * x;
            }
            else if (x < 2 / d1)
            {
                x -= 1.5f / d1;
                return n1 * x * x + 0.75f;
            }
            else if (x < 2.5f / d1)
            {
                x -= 2.25f / d1;
                return n1 * x * x + 0.9375f;
            }
            else
            {
                x -= 2.625f / d1;
                return n1 * x * x + 0.984375f;
            }
        }

        public static float EaseInOutBounce(float x)
        {
            if (x < 0.5f)
            {
                return (1f - EaseOutBounce(1f - 2f * x)) * 0.5f;
            }
            else
            {
                return (1f + EaseOutBounce(2f * x - 1f)) * 0.5f;
            }
        }

        public static float EaseInOutCubic(float x)
        {
            return x < 0.5f
                ? 4f * x * x * x
                : 1f - MathF.Pow(-2f * x + 2f, 3f) / 2f;
        }

        public static float EaseInOutQuint(float x)
        {
            return x < 0.5f
                ? 16f * x * x * x * x * x
                : 1f - MathF.Pow(-2f * x + 2f, 5f) / 2f;
        }

    }
}