using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SimpleTrail : Trail
    {
        protected float offset;
        private bool useOffset;
        public SimpleTrail(int length, Texture2D texture, bool useOffset = false, Effect effect = null)
            : base(length, texture, effect)
        {
            this.useOffset = useOffset;
        }

        public override void Draw()
        {
            if (Vertices.Count < 6) return;

            Effect.SafeSetParameter("WorldViewProjection", PrimitiveHelper.GetMatrix());
            Effect.SafeSetParameter("uImage0", Texture);
            Effect.CurrentTechnique.Passes["Texturized"].Apply();

            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }

        public override void PrepareTrail()
        {
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
                Color col1 = TrailEntity.TrailColor(prog1);
                Color col2 = TrailEntity.TrailColor(prog2);

                AddVertex(pos1 + off1, col1, new Vector2(prog1 + offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + offset, 0));
            }
        }

        public override void Update()
        {
            if (useOffset) offset += (1f / Positions.Capacity);
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
    }
}
