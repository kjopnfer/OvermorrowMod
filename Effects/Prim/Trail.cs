using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Effects.Prim
{
    public class Trail
    {
        public Effect Effect { get; set; }
        public Entity Entity;
        public List<Vector2> Positions = new List<Vector2>();
        public List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
        public bool Pixelated;
        public DrawType Type;
        public bool Dead { get; set; }
        public bool Dying { get; set; } = false;
        public Color Color { get; set; }
        public float Width { get; set; }
        public int Length { get; set; }
        public void AddVertex(Vector2 position, Color color, Vector2 TexCoord)
        {
            int divider = Pixelated ? 2 : 1;
            Vertices.Add(new VertexPositionColorTexture((position - Main.screenPosition).ToVector3() / divider, color, TexCoord));
        }
        public static Vector2 GetRotation(Vector2[] oldPos, int index)
        {
            if (oldPos.Length == 1)
                return oldPos[0];

            if (index == 0)
            {
                return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(MathHelper.Pi / 2);
            }

            return (index == oldPos.Length - 1
                ? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
                : Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
        }
        public virtual void SetDefaults()
        {
            Effect = OvermorrowModFile.Mod.TrailShader;
        }
        public virtual void Update() { }
        public virtual void UpdateDead() { }
        public static Matrix GetWVP()
        {
            GraphicsDevice graphics = Main.graphics.GraphicsDevice;
            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix View = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(graphics.Viewport.Width / 2, graphics.Viewport.Height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
            Matrix Projection = Matrix.CreateOrthographic(graphics.Viewport.Width, graphics.Viewport.Height, 0, 1000);
            return View * Projection;
        }
        public virtual void PrepareEffect()
        {
            Effect.Parameters["WorldViewProjection"].SetValue(GetWVP());
            Effect.CurrentTechnique.Passes[0].Apply();
        }
        public virtual void PrepareTrail() { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}