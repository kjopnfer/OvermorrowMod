using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Effects
{
    public class TrailHelper
    {
        public TrailConfig config;
        public List<VertexPositionColorTexture> positions = new List<VertexPositionColorTexture>();
        Vector2[] oldPos;
        public TrailHelper(Vector2[] oldPos, TrailConfig config)
        {
            this.config = config;
            this.oldPos = oldPos;
        }
        protected static Vector2 GetRotation(Vector2[] oldPos, int index)
		{
			if (oldPos.Length == 1)
				return oldPos[0];
                
			if (index == 0) {
				return Vector2.Normalize(oldPos[1] - oldPos[0]).RotatedBy(Math.PI / 2);
			}

			return (index == oldPos.Length - 1
				? Vector2.Normalize(oldPos[index] - oldPos[index - 1])
				: Vector2.Normalize(oldPos[index + 1] - oldPos[index - 1])).RotatedBy(MathHelper.Pi / 2);
		}
        private void CreateTrail()
        {
            if (oldPos.Length != config.Length)
            Array.Resize(ref oldPos, config.Length);
            for(int i = 0; i < oldPos.Length - 1; i++)
            {
                if (oldPos[i] == Vector2.Zero || oldPos[i + 1] == Vector2.Zero)
                {
                    break;
                }
                Vector2 pos1 = oldPos[i] - Main.screenPosition;
                Vector2 pos2 = oldPos[i + 1] - Main.screenPosition;
                float progress = (float)i / (float)oldPos.Length;
                float progress2 = (float)(i + 1) / (float)oldPos.Length;
                float size1 = config.size(progress);
                float size2 = config.size(progress2);
                Color color1 = config.color(progress);
                Color color2 = config.color(progress2);
                Vector2 offset = GetRotation(oldPos, i) * size1;
                Vector2 offset2 = GetRotation(oldPos, i + 1) * size2;
                if (config.CustomPrepare == null)
                {
                    AddVertex(pos1 + offset, color1, new Vector2(progress, 1));
                    AddVertex(pos1 - offset, color1, new Vector2(progress, 0));
                    AddVertex(pos2 + offset2, color2, new Vector2(progress2, 1));

                    AddVertex(pos2 - offset2, color2, new Vector2(progress2, 0));
                    AddVertex(pos2 + offset2, color2, new Vector2(progress2, 1));
                    AddVertex(pos1 - offset, color1, new Vector2(progress, 0));
                }
                else
                {
                    config.CustomPrepare(ref positions, pos1, pos2, progress, progress2);
                }
            }
        }

        public Vector3 ToVector3(Vector2 input)
        {
            return new Vector3(input.X, input.Y, 0);
        }

        public void AddVertex(Vector2 pos, Color color, Vector2 TexCoord)
        {
            positions.Add(new VertexPositionColorTexture(ToVector3(pos), color, TexCoord));
        }
        public void Draw()
        {
            CreateTrail();
            if (positions.Count > 6)
            {
                /*Main.graphics.GraphicsDevice.SetVertexBuffer(null);
                VertexBuffer buffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColorTexture), positions.Count, BufferUsage.WriteOnly);
                buffer.SetData(positions.ToArray());
                Main.graphics.GraphicsDevice.SetVertexBuffer(buffer);
                Main.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, positions.Count / 3);*/
                PrimitivePacket packet = new PrimitivePacket();
                packet.Type = PrimitiveType.TriangleList;
                packet.Pass = "Texturized";
                //packet.Set(positions);
                packet.Send();
            }
        }
    }
}