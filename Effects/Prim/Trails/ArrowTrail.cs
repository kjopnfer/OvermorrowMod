using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Effects.Prim.Trails
{
    public class ArrowTrail : SimpleTrail
    {
        public ArrowTrail() : base(40, OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail6"), true)
        {
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
                Color col1 = TrailEntity.TrailColor(prog1) * prog1;
                Color col2 = TrailEntity.TrailColor(prog2) * prog2;

                AddVertex(pos1 + off1, col1, new Vector2(prog1 + offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + offset, 0));
            }
        }
    }
}