using System;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Content.NPCs.Mercenary
{
	public struct MathFunctions
	{
        /// <summary>
        /// Factored form Parabolas
        /// </summary>
        public class Parabola
        {
            float a;
            public float z1;
            public float z2;
            public float targX;
            public float distance;
            public float[] increment = new float[2];
            public bool backwards;
            public Vector2 point;
            public Parabola(float a1, float h1, float k1) { a = a1; z1 = h1; z2 = k1; }
            /// <summary>
            /// Returns a **positive** parabola, including 'backwards'and 'distance'; also sets point2.X to 'targX'
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="point2"></param>
            /// <returns></returns>
            public static Parabola ParabolaFromCoords(Vector2 point1, Vector2 point2)
            {
                //point1 is a zero
                //point2 is the target point
                Vector2 point = point2 - point1;
                bool bw = point.X < 0;
                point = new Vector2(AGF.Abs(point.X), AGF.Abs(point.Y));
                float[] zero = new float[2] { 0, point.X * 1.33f };
                float a1 = point.Y / ((point.X - zero[0]) * (point.X - zero[1]));
                Parabola parabola = new Parabola(a1, zero[0], zero[1]);
                parabola.distance = Vector2.Distance(Vector2.Zero, point);
                parabola.targX = point.X;
                parabola.backwards = bw;
                parabola.point = point;
                return parabola;
                #region Yucky code
                /*float[] zero = new float[2] { Abs(point1.X), Abs(((point1.X + point2.X) / 2) + point1.X) };
                float[] val = new float[2] { point2.X - zero[0], point2.X - zero[1] };
                float a1 = point2.Y / (val[0] * val[1]);
                return new Parabola(a1, zero[0], zero[1]);
                float Abs(float f) => Math.Abs(f);*/
                #endregion
            }
            /// <summary>
            /// Returns a **positive** parabola, including 'backwards'and 'distance'; also sets point2.X to 'targX'
            /// </summary>
            /// <param name="point1"></param>
            /// <param name="point2"></param>
            /// <returns></returns>
            public static Parabola ParabolaFromCoords(Point point1, Point point2)
            {
                //point1 is a zero
                //point2 is the target point
                Point point = AGF.Subtract(point2, point1);
                bool bw = point.X < 0;
                point = new Point(AGF.AbsRound(point.X), AGF.AbsRound(point.Y));
                float[] zero = new float[2] { 0, point.X * 1.33f };
                float a1 = point.Y / ((point.X - zero[0]) * (point.X - zero[1]));
                Parabola parabola = new Parabola(a1, zero[0], zero[1]);
                parabola.distance = AGF.Distance(new Point(0, 0), point);
                parabola.targX = point.X;
                parabola.backwards = bw;
                return parabola;
                #region Yucky code
                /*float[] zero = new float[2] { Abs(point1.X), Abs(((point1.X + point2.X) / 2) + point1.X) };
                float[] val = new float[2] { point2.X - zero[0], point2.X - zero[1] };
                float a1 = point2.Y / (val[0] * val[1]);
                return new Parabola(a1, zero[0], zero[1]);
                float Abs(float f) => Math.Abs(f);*/
                #endregion
            }
            public void SetIncrement(float ticks, float mult = 1) { mult = MathHelper.Clamp(mult, 1, 3); increment[0] = (targX / (ticks * ((float)Math.Sqrt(targX) * 0.1f))) * mult; increment[1] = (float)Math.Round(Math.Sqrt(ticks * increment[0]) * Math.PI); }
            /// <summary>
            /// Returns Y based on the given X
            /// </summary>
            /// <param name="x"></param>
            /// <returns></returns>
            public float GetY(float x) => a * ((x - z1) * (x - z2));
            public override string ToString()
            {
                return $"backwards: {backwards} | a: {a} | Zero(1): {z1} Zero(2): {z2} | Increment: {increment[0]} | TotalTicks = {increment[1]}";
            }
        }
        /// <summary>
        /// Analytic Geometry Functions
        /// </summary>
        public class AGF
        {
            public static int Round(float f) => (int)Math.Round(f);
            public static int AbsRound(float f) => (int)Math.Round(Math.Sqrt(f * f));
            public static float Abs(float f) => (float)Math.Sqrt(f * f);
            public static Point Subtract(Point value1, Point value2) => new Point(value1.X - value2.X, value1.Y - value2.Y);
            public static float Distance(Point p1, Point p2) => (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
            public static float Distance(Vector2 p1, Vector2 p2) => (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }
    }
}
