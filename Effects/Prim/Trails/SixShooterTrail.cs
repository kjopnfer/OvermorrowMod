using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SixShooterTrail : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 10;
            Length = 20;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
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
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail9"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {
            Color darkest = new Color(6, 106, 255);
            Color lightest = new Color(196, 247, 258);

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(darkest, lightest, prog1) * prog1;
                Color col2 = Color.Lerp(darkest, lightest, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
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