using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Common.Primitives
{
    public static class PrimitiveHelper
    {
        private static int width;
        private static int height;
        private static Vector2 zoom;

        private static bool CheckGraphicsChanged()
        {
            var device = Main.graphics.GraphicsDevice;
            bool changed = device.Viewport.Width != width
                || device.Viewport.Height != height
                || Main.GameViewMatrix.Zoom != zoom;

            if (!changed) return false;

            width = device.Viewport.Width;
            height = device.Viewport.Height;
            zoom = Main.GameViewMatrix.Zoom;

            return true;
        }

        private static Matrix view;
        private static Matrix projection;
        public static Matrix GetMatrix()
        {
            if (CheckGraphicsChanged())
            {
                var device = Main.graphics.GraphicsDevice;
                int width = device.Viewport.Width;
                int height = device.Viewport.Height;
                Vector2 zoom = Main.GameViewMatrix.Zoom;
                view =
                    Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up)
                    * Matrix.CreateTranslation(width / 2, height / -2, 0)
                    * Matrix.CreateRotationZ(MathHelper.Pi)
                    * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
                projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            }

            return view * projection;
        }

        public static VertexPositionColorTexture AsVertex(Vector2 position, Color color, Vector2 TexCoord)
        {
            return new VertexPositionColorTexture((position - Main.screenPosition).ToVector3(), color, TexCoord);
        }

        public static VertexPositionTexture AsVertex(Vector2 position, Vector2 TexCoord)
        {
            return new VertexPositionTexture((position - Main.screenPosition).ToVector3(), TexCoord);
        }

        public static VertexPositionTexture[] GetRectangleStrip(Rectangle rect)
        {
            return new VertexPositionTexture[]
            {
                AsVertex(rect.BottomLeft(), new Vector2(0, 1)),
                AsVertex(rect.TopLeft(), new Vector2(0, 0)),
                AsVertex(rect.BottomRight(), new Vector2(1, 1)),
                AsVertex(rect.TopRight(), new Vector2(1, 0)),
            };
        }

        public static void SetTexture(int index, Texture2D texture)
        {
            Main.graphics.GraphicsDevice.Textures[index] = texture;
        }

        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0);
        }
        public static Vector2 GetRotation(IReadOnlyList<Vector2> oldPos, int index)
        {
            if (oldPos.Count == 1)
                return oldPos[0];

            if (index == 0)
            {
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);
            }

            return (index == oldPos.Count - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }
        public static void SetBasicEffectParameters(this Effect effect)
        {
            effect.Parameters["WVP"].SetValue(GetMatrix());
        }
    }
}