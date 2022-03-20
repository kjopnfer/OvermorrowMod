using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Effects.Prim
{
    public class BeamPacket
    {
        public List<VertexPositionColorTexture> Vertices = new List<VertexPositionColorTexture>();
        public PrimitiveType Type = PrimitiveType.TriangleList;
        public Effect Effect = OvermorrowModFile.Mod.BeamShader;
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
        public void Add2(Vector2 pos, Color color, Vector2 TexCoord)
        {
            Vertices.Add(new VertexPositionColorTexture(new Vector3(pos.X, pos.Y, 0), color, TexCoord));
        }
        public void AddStrip(Vector2 pos1, Vector2 pos2, float size1, float size2, float progress1, float progress2, Color color1, Color color2)
        {
            Vector2 dir = (pos2 - pos1).SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);
            Vector2 offset1 = dir * size1;
            Vector2 offset2 = dir * size2;
            Add(pos1 + offset1, color1, new Vector2(progress1, 1));
            Add(pos1 - offset1, color1, new Vector2(progress1, 0));
            Add(pos2 + offset2, color2, new Vector2(progress2, 1));

            Add(pos2 - offset2, color2, new Vector2(progress2, 0));
            Add(pos2 + offset2, color2, new Vector2(progress2, 1));
            Add(pos1 - offset1, color1, new Vector2(progress1, 0));
        }
        public static void SetTexture(int index, Texture2D texture)
        {
            Main.graphics.GraphicsDevice.Textures[index] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[index] = SamplerState.PointWrap;
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

                Effect.Parameters["WVP"].SetValue(BeamHelper.GetMatrix());
                Effect.CurrentTechnique.Passes[Pass].Apply();

                device.DrawPrimitives(Type, 0, Count);
            }
        }
    }
}