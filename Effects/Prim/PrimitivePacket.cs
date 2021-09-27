using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Effects.Prim
{
    public class PrimitivePacket
    {
        public List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
        public PrimitiveType Type = PrimitiveType.TriangleList;
        public Effect Effect = OvermorrowModFile.Mod.TrailShader;
        public string Pass = "Basic";
        public int Count
        {
            get
            {
                int count = 0;
                switch (Type)
                {
                    case PrimitiveType.LineList:
                        count = Vertices.Count / 2;
                        break;
                    case PrimitiveType.LineStrip:
                        count = Vertices.Count - 1;
                        break;
                    case PrimitiveType.TriangleList:
                        count = Vertices.Count / 3;
                        break;
                    case PrimitiveType.TriangleStrip:
                        count = Vertices.Count - 2;
                        break;
                }
                return count;
            }
        }
        public void Add(Vector2 position, Color color, Vector2 TexCoord)
        {
            Vector2 pos = position - Main.screenPosition;
            Vector3 pos2 = new Vector3(pos.X, pos.Y, 0f);
            Vertices.Add(new VertexPositionColorTexture(pos2, color, TexCoord));
        }
        public static void SetTexture(int index, Texture2D texture)
        {
            Main.graphics.GraphicsDevice.Textures[index] = texture;
        }

        public short[] GetIndices()
        {
            short[] indexes = new short[1];
            int count = Vertices.Count - 1;
            switch (Type)
            {
                case PrimitiveType.TriangleList:
                    int IPV = 3; // indexes per vertex
                    int length = count * IPV; // length of index array
                    if (indexes.Length < length)
                    {
                        Array.Resize(ref indexes, length);
                    }
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i * IPV);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num; // resuming: connect first one
                        indexes[indexInArray + 1] = (short)(num + 1); // to second one
                        indexes[indexInArray + 2] = (short)(num + 2); // then to third one
                    }
                    break;
                case PrimitiveType.TriangleStrip:
                    int IPV1 = 2;
                    int length1 = count * IPV1;
                    if (indexes.Length < length1)
                    {
                        Array.Resize(ref indexes, length1);
                    }
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i * IPV1);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num; // connect first one
                        indexes[indexInArray] = (short)(num + 1); // to second one
                    }
                    break;
                case PrimitiveType.LineList:
                    int IPV2 = 2;
                    int length2 = count * IPV2;
                    if (indexes.Length < length2)
                    {
                        Array.Resize(ref indexes, length2);
                    }
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i * IPV2);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num; // connect first
                        indexes[indexInArray] = (short)(num + 1); // to second
                    }
                    break;
                case PrimitiveType.LineStrip:
                    int length3 = count;
                    for (short i = 0; i < count; i = (short)(i + 1))
                    {
                        short indexInArray = (short)(i);
                        int num = i * 2;
                        indexes[indexInArray] = (short)num;
                    }
                    break;
            }
            return indexes;
        }
        public void Send()
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            if (Count > 0)
            {
                VertexBuffer buffer = new VertexBuffer(device, typeof(VertexPositionColorTexture), Vertices.Count, BufferUsage.WriteOnly);
                IndexBuffer index = new IndexBuffer(device, typeof(short), GetIndices().Length, BufferUsage.WriteOnly);

                device.SetVertexBuffer(null);

                buffer.SetData(Vertices.ToArray());
                index.SetData(GetIndices());

                device.SetVertexBuffer(buffer);
                device.Indices = index;

                RasterizerState rasterizerState = new RasterizerState();
                rasterizerState.CullMode = CullMode.None;
                device.RasterizerState = rasterizerState;

                Effect.Parameters["WorldViewProjection"].SetValue(Trail.GetWVP());
                Effect.CurrentTechnique.Passes[Pass].Apply();

                device.DrawPrimitives(Type, 0, Count);
            }
        }
    }
}