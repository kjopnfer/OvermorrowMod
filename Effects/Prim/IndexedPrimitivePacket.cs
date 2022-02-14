using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Effects.Prim
{
    public class IndexedPrimitivePacket
    {
        // Not a collection, should only be enumerated once for efficiency, if possible.
        private readonly IEnumerable<VertexPositionColorTexture> vertices;
        private readonly PrimitiveType type;
        // Keep separate from vertices enumerable to avoid enumerating it.
        private readonly int vertexCount;

        public Effect Effect = OvermorrowModFile.Mod.TrailShader;
        public string Pass = "Basic";

        public IndexedPrimitivePacket(IEnumerable<VertexPositionColorTexture> vertices, PrimitiveType type, int vertexCount)
        {
            this.vertices = vertices;
            this.type = type;
            this.vertexCount = vertexCount;
        }

        private int Count
        {
            get
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
            }
        }
        private short[] GetIndices()
        {
            int count = vertexCount - 1;
            switch (type)
            {
                case PrimitiveType.TriangleList:
                    {
                        const int IPV = 3; // indexes per vertex
                        int length = count * IPV; // length of index array
                        var indexes = new short[length];

                        for (short i = 0; i < count; i++)
                        {
                            short indexInArray = (short)(i * IPV);
                            int num = i * 2;
                            indexes[indexInArray] = (short)num; // resuming: connect first one
                            indexes[indexInArray + 1] = (short)(num + 1); // to second one
                            indexes[indexInArray + 2] = (short)(num + 2); // then to third one
                        }

                        return indexes;
                    }
                case PrimitiveType.TriangleStrip:
                    {
                        const int IPV = 2;
                        var indexes = new short[count * IPV];

                        for (short i = 0; i < count; i++)
                        {
                            short indexInArray = (short)(i * IPV);
                            int num = i * 2;
                            indexes[indexInArray] = (short)num; // connect first one
                            indexes[indexInArray + 1] = (short)(num + 1); // to second one
                        }

                        return indexes;
                    }
                case PrimitiveType.LineList:
                    {
                        const int IPV = 2;
                        var indexes = new short[count * IPV];

                        for (short i = 0; i < count; i++)
                        {
                            short indexInArray = (short)(i * IPV);
                            int num = i * 2;
                            indexes[indexInArray] = (short)num; // connect first
                            indexes[indexInArray + 1] = (short)(num + 1); // to second
                        }
                        return indexes;
                    }
                    
                case PrimitiveType.LineStrip:
                    {
                        var indexes = new short[count];

                        for (short i = 0; i < count; i++)
                        {
                            short indexInArray = i;
                            int num = i * 2;
                            indexes[indexInArray] = (short)num;
                        }

                        return indexes;
                    }
                default: return new short[0];
                    
            }
        }
        public void Send()
        {
            if (Count > 0)
            {
                GraphicsDevice device = Main.graphics.GraphicsDevice;

                var indices = GetIndices();
                var verticesAsArray = vertices as VertexPositionColorTexture[] ?? vertices.ToArray();

                VertexBuffer buffer = new VertexBuffer(device, typeof(VertexPositionColorTexture), vertexCount, BufferUsage.WriteOnly);
                IndexBuffer index = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.WriteOnly);

                device.SetVertexBuffer(null);

                buffer.SetData(verticesAsArray);
                index.SetData(indices);

                device.SetVertexBuffer(buffer);
                device.Indices = index;

                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.CullMode = CullMode.None;
                device.RasterizerState = rasterizerState;

                Effect.Parameters["WorldViewProjection"].SetValue(BeamHelper.GetMatrix());
                Effect.CurrentTechnique.Passes[Pass].Apply();

                device.DrawPrimitives(type, 0, Count);
            }
        }
    }
}