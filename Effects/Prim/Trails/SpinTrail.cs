using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SpinTrail : Trail
    {
        float Offset;

        public override void SetDefaults()
        {
            Length = 15;
            Effect = OvermorrowModFile.Mod.TrailShader;
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
        public override void PrepareTrail()
        {
            /*if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = PrimitiveHelper.GetRotation(Positions.ToArray(), i) * 80 * (prog1 * 0.1f + 0.9f);
                Vector2 off2 = PrimitiveHelper.GetRotation(Positions.ToArray(), i + 1) * 80 * (prog2 * 0.1f + 0.9f);
                float coord1 = off1.Length() < off2.Length() ? 1 : 0;
                float coord2 = off1.Length() < off2.Length() ? 0 : 1;
                Color col1 = Color.Lerp(Color.Blue, Color.Cyan, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Blue, Color.Cyan, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1, coord1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1, coord2));
                AddVertex(pos2 + off2, col2, new Vector2(prog2, coord1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2, coord1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2, coord2));
                AddVertex(pos1 - off1, col1, new Vector2(prog1, coord2));
            }*/

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = PrimitiveHelper.GetRotation(Positions.ToArray(), i) * TrailEntity.TrailSize(prog1) * prog1;
                Vector2 off2 = PrimitiveHelper.GetRotation(Positions.ToArray(), i + 1) * TrailEntity.TrailSize(prog2) * prog2;
                Color col1 = Color.Lerp(Color.Blue, Color.Cyan, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Blue, Color.Cyan, prog2) * prog2;
                float coord1 = off1.Length() < off2.Length() ? 1 : 0;
                float coord2 = off1.Length() < off2.Length() ? 0 : 1;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, coord1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, coord2));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, coord1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, coord1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, coord2));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, coord2));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Effect.SafeSetParameter("WorldViewProjection", PrimitiveHelper.GetMatrix());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail2"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();

            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }
}