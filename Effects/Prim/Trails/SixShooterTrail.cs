using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class SixShooterTrail : Trail
    {
        public SixShooterTrail() : base(20)
        {
        }

        private float Offset;
        public override void SetDefaults()
        {
            Effect = OvermorrowModFile.Mod.TrailShader;
        }
        public override void Update()
        {
            Offset += (1f / Positions.Capacity);
            Positions.PushBack(Entity.Center);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.PopFront();
        }
       
        public override void PrepareTrail()
        {
            Color darkest = new Color(6, 106, 255);
            Color lightest = new Color(196, 247, 258);

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Positions.Capacity;
                float prog2 = (float)(i + 1) / (float)Positions.Capacity;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];

                if (pos1 == pos2) continue;

                Vector2 off1 = PrimitiveHelper.GetRotation(Positions, i) * TrailEntity.TrailSize(prog1) * prog1;
                Vector2 off2 = PrimitiveHelper.GetRotation(Positions, i + 1) * TrailEntity.TrailSize(prog2) * prog2;
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