using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
//using TestMod.Primitives;

namespace OvermorrowMod
{
    public static class TRay
    {
        public static Vector2 Cast(Vector2 start, Vector2 direction, float length)
        {
            direction = direction.SafeNormalize(Vector2.UnitY);
            Vector2 output = start;
            for (int i = 0; i < length; i++)
            {
                if (Collision.CanHitLine(output, 0, 0, output + direction, 0, 0))
                {
                    output += direction;
                }
                else
                {
                    break;
                }
            }
            return output;
        }
        public static float CastLength(Vector2 start, Vector2 direction, float length)
        {
            Vector2 end = Cast(start, direction, length);
            return (end - start).Length();
        }
    }
}