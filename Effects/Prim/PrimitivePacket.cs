using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Effects.Prim
{
    public class PrimitivePacket
    {
        // Not a collection, should only be enumerated once for efficiency, if possible.
        private readonly IEnumerable<VertexPositionColorTexture> vertices;
        private readonly PrimitiveType type;
        // Keep separate from vertices enumerable to avoid enumerating it.
        private readonly int vertexCount;

        public Effect Effect = OvermorrowModFile.Mod.TrailShader;
        public string Pass = "Basic";

        public PrimitivePacket(IEnumerable<VertexPositionColorTexture> vertices, PrimitiveType type, int vertexCount)
        {
            this.vertices = vertices;
            this.type = type;
            this.vertexCount = vertexCount;
        }

        private int Count { get
        {
            switch (type)
            {
                case PrimitiveType.LineList:
                    return vertexCount / 2;
                case PrimitiveType.LineStrip:
                    return vertexCount - 1;
                case PrimitiveType.TriangleList:
                    return vertexCount / 3;
                case PrimitiveType.TriangleStrip:
                    return vertexCount - 2;
                default: return 0;
            }
        } }

        public void Send()
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;

            // ToArray is a bit expensive, in that it will copy an existing array, if the input
            // already is an array, then we can reuse it. The mutability is slightly different, but that
            // doesn't matter in this case.
            var verticesAsArray = vertices as VertexPositionColorTexture[] ?? vertices.ToArray();
            if (Count > 0)
            {
                Effect.Parameters["WorldViewProjection"].SetValue(PrimitiveHelper.GetMatrix());
                Effect.CurrentTechnique.Passes[Pass].Apply();

                device.DrawUserPrimitives(type, verticesAsArray, 0, Count);
            }
        }
    }
}
