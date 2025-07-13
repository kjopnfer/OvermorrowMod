using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using Terraria;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SimpleTrail : Trail
    {
        protected float offset;
        private bool useOffset;
        public SimpleTrail(int length, Texture2D defaultTexture, bool useOffset = false, Effect effect = null)
            : base(length, defaultTexture, effect)
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

            int segments = 2; // Number of segments for the semicircle
            float arcRadius = Config.TrailSize(1f); // Radius of the semicircle (based on trail size)

            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Positions.Count;
                float prog2 = (float)(i + 1) / (float)Positions.Count;

                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];

                if (pos1 == pos2) continue;

                Vector2 off1 = PrimitiveHelper.GetRotation(Positions, i) * Config.TrailSize(prog1) * prog1;
                Vector2 off2 = PrimitiveHelper.GetRotation(Positions, i + 1) * Config.TrailSize(prog2) * prog2;

                Color col1 = Config.TrailColor(prog1);
                Color col2 = Config.TrailColor(prog2);

                if (i == Positions.Count - 2) // For the head of the trail
                {
                    Vector2 headCenter = pos1; // Use pos1 as the center
                    Vector2 direction = PrimitiveHelper.GetRotation(Positions, i);

                    for (int j = 0; j < segments; j++)
                    {
                        float angle1 = -MathHelper.Pi + j * (MathHelper.Pi / segments);
                        float angle2 = -MathHelper.Pi + (j + 1) * (MathHelper.Pi / segments);

                        Vector2 arcPoint1 = headCenter + direction.RotatedBy(angle1) * arcRadius;
                        Vector2 arcPoint2 = headCenter + direction.RotatedBy(angle2) * arcRadius;

                        AddVertex(headCenter, col1, new Vector2(0.5f, 0.5f)); // Center of the semicircle
                        AddVertex(arcPoint1, col2, new Vector2(0f, 0f));  // First arc point
                        AddVertex(arcPoint2, col2, new Vector2(0.5f, 0f));  // Second arc point
                    }
                }
                else
                {
                    /*AddVertex(pos1 + off1, col1, new Vector2(prog1 + offset, 1));
                    AddVertex(pos1 - off1, col1, new Vector2(prog1 + offset, 0));
                    AddVertex(pos2 + off2, col1, new Vector2(prog1 + offset, 1));

                    AddVertex(pos2 + off2, col1, new Vector2(prog2 + offset, 1));
                    AddVertex(pos2 - off2, col1, new Vector2(prog2 + offset, 0));
                    AddVertex(pos1 - off1, col1, new Vector2(prog2 + offset, 0));*/
                    float segmentSize = 1.0f / (float)(Positions.Count - 1);
                    float startOffset = segmentSize * i;

                    AddVertex(pos1 + off1, col1, new Vector2(startOffset, 0f));
                    AddVertex(pos1 - off1, col1, new Vector2(startOffset, 1f));
                    AddVertex(pos2 + off2, col1, new Vector2(startOffset + segmentSize, 0f));

                    AddVertex(pos2 + off2, col1, new Vector2(startOffset + segmentSize, 0));
                    AddVertex(pos2 - off2, col1, new Vector2(startOffset + segmentSize, 1));
                    AddVertex(pos1 - off1, col1, new Vector2(startOffset, 1));
                }
            }
        }

        public override void Update()
        {
            offset += (1f / Positions.Count);
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
