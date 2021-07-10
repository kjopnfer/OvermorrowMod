using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Effects.Prim
{
    public class ShiroTrail : Trail
    {
        public override void SetDefaults()
        {
            Width = 40;
            Length = 10;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.Red;
            Pixelated = false;
        }
        public override void Update()
        {
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2) 
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.Parameters["WorldViewProjection"].SetValue(GetWVP());
            Main.graphics.GraphicsDevice.Textures[0] = OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/ShiroTrail");
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {
            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * (prog1 * 0.1f + 0.9f);
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * (prog2 * 0.1f + 0.9f);
                float coord1 = off1.Length() < off2.Length() ? 1 : 0;
                float coord2 = off1.Length() < off2.Length() ? 0 : 1;
                Color col1 = Color.Lerp(Color.Green, Color.LightGreen, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Green, Color.LightGreen, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1, coord1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1, coord2));
                AddVertex(pos2 + off2, col2, new Vector2(prog2, coord1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2, coord1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2, coord2));
                AddVertex(pos1 - off1, col1, new Vector2(prog1, coord2));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }
}