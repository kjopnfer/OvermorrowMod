using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Effects
{
    public class PrimitivePacket
    {
        public List<VertexInfo> Vertices = new List<VertexInfo>();
        public PrimitiveType Type = PrimitiveType.TriangleList;
        public Effect Effect = OvermorrowModFile.Mod.TrailShader;
        public string Pass = "Basic";
        public int Count
        {
            get
            {
                int count = 0;
                switch(Type)
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
            private set{}
        }
        public void Add(Vector2 position, Color color, Vector2 TexCoord)
        {
            Vertices.Add(new VertexInfo(position - Main.screenPosition, color, TexCoord));
        }
        public static void SetTexture(int index, Texture2D texture, SamplerState state = null)
        {
            if (state == null) state = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.Textures[index] = texture;
            Main.graphics.GraphicsDevice.SamplerStates[index] = state;
        }
        public void Send()
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            if (Count > 0)
            {
                device.DrawUserPrimitives(Type, Vertices.ToArray(), 0, Count);
            }
        }
    }
}