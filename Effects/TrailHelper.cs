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
        public Effect effect = OvermorrowModFile.Mod.Sword;
        public delegate Color TColor(float progress);
        public delegate float TSize(float progress);
        public List<VertexPositionColorTexture> positions = new List<VertexPositionColorTexture>();
        public Projectile entity;
        private TColor color;
        private TSize size;
        private int length;
        private Texture2D texture;
        public int type;
        public string pass;

        private int activeLenght;

        public TrailHelper(Projectile Entity, TColor Color, TSize Size, int Length = 10, string pass = "Basic", Texture2D Texture = null)
        {
            entity = Entity;
            color = Color;
            size = Size;
            //type = 1;
            length = Length;
            texture = Texture;
            this.pass = pass;
        }


        public static void ClampDistance(ref Vector2 start, ref Vector2 end, float maxDistance)
        {
            if (Vector2.Distance(end, start) > maxDistance)
            {
                end = start + Vector2.Normalize(end - start) * maxDistance;
            }
        }

        public void Update()
        { 
            if (!(Main.gameMenu && Main.gamePaused && Main.gameInactive))
            {
                switch(type)
                {
                    case 0:
                        entity.oldPos[0] = entity.Center;
                        for (int i = (entity.oldPos.Length - 1); i > 0; i--)
                            entity.oldPos[i] = entity.oldPos[i - 1];

                        entity.oldRot[0] = entity.rotation;
                        for (int i = (entity.oldRot.Length - 1); i > 0; i--)
                            entity.oldRot[i] = entity.oldRot[i - 1];
                        break;
                    case 1:
                        for (int i = 0; i < entity.oldPos.Length; i++)
                        {
                            if (entity.oldPos[i] == Vector2.Zero)
                                entity.oldPos[i] = entity.Center;
                        }
                        for (int i = 1; i < entity.oldPos.Length; i++)
                        {
                            ClampDistance(ref entity.oldPos[i - 1], ref entity.oldPos[i], 8f);
                        }
                        break;
                }
            }
        }

     
        public Vector3 ToVector3(Vector2 input)
        {
            return new Vector3(input.X, input.Y, 0);
        }

        private void CreateTrail()
        {
            /*if (entity.oldPos.Length != lenght)
            Array.Resize(ref entity.oldPos, lenght);
            if (entity.oldRot.Length != lenght)
            Array.Resize(ref entity.oldRot, lenght);
            for (int i = 0; i < lenght; i++)
            {
                Vector2 pos = entity.oldPos[i] - Main.screenPosition;
                if (pos == Vector2.Zero) break;
                float rot = MathHelper.WrapAngle(entity.oldRot[i] + MathHelper.Pi / 2);
                float progress = (float)i / (float)lenght;
                Color color1 = color(progress);
                float size1 = size(progress);
                Vector2 offset = rot.ToRotationVector2() * size1;
                positions.Add(new VertexPositionColorTexture(ToVector3(pos + offset), color1, new Vector2(progress, 1)));
                positions.Add(new VertexPositionColorTexture(ToVector3(pos - offset), color1, new Vector2(progress, 0)));
            }*/
            if (entity.oldPos.Length != length)
                Array.Resize(ref entity.oldPos, length);
            if (entity.oldRot.Length != length)
                Array.Resize(ref entity.oldRot, length);
            int len = entity.oldPos.Length;
            for (int i = 0; i < len - 1; i++)
            {
                if (entity.oldPos[i] == Vector2.Zero || entity.oldPos[i + 1] == Vector2.Zero)
                {
                    break;
                }
                Vector2 pos1 = entity.oldPos[i] - Main.screenPosition;
                Vector2 pos2 = entity.oldPos[i + 1] - Main.screenPosition;
                float progress = (float)i / (float)entity.oldPos.Length;
                float progress2 = (float)(i + 1) / (float)entity.oldPos.Length;
                float size1 = size(progress);
                float size2 = size(progress2);
                Color color1 = color(progress);
                Color color2 = color(progress2);
                Vector2 offset = MathHelper.WrapAngle(entity.oldRot[i] + MathHelper.Pi / 2).ToRotationVector2() * size1;
                Vector2 offset2 = MathHelper.WrapAngle(entity.oldRot[i + 1] + MathHelper.Pi / 2).ToRotationVector2() * size1;
                AddVertex(pos1 + offset, color1, new Vector2(progress, 1));
                AddVertex(pos1 - offset, color1, new Vector2(progress, 0));
                AddVertex(pos2 + offset2, color2, new Vector2(progress2, 1));

                AddVertex(pos2 - offset2, color2, new Vector2(progress2, 0));
                AddVertex(pos2 + offset2, color2, new Vector2(progress2, 1));
                AddVertex(pos1 - offset, color1, new Vector2(progress, 0));
            }
        }

        public void AddVertex(Vector2 pos, Color color, Vector2 TexCoord)
        {
            positions.Add(new VertexPositionColorTexture(ToVector3(pos), color, TexCoord));
        }

        public void Draw()
        {
            Update();
            CreateTrail();
            /*PrimitivePacket packet = new PrimitivePacket();
            packet.type = PrimitiveType.TriangleStrip;
            packet.pass = "Basic";
            if (texture != null) packet.texture = texture;
            foreach (VertexPositionColorTexture vertex in positions)
            {
                packet.Add(vertex);
            }
            packet.Send();*/
            if (positions.Count > 6)
            {
                // this is the first method
                /*
                Main.graphics.GraphicsDevice.SetVertexBuffer(null);
                VertexBuffer buffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionColorTexture), positions.Count, BufferUsage.WriteOnly);
                buffer.SetData(positions.ToArray());

                Main.graphics.GraphicsDevice.SetVertexBuffer(buffer);
                Main.graphics.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, positions.Count / 3);
                */
                PrimitivePacket packet = new PrimitivePacket();
                packet.type = PrimitiveType.TriangleList;
                if (texture != null)
                {
                    packet.texture = texture;
                }
                packet.pass = pass;
                packet.Set(positions.ToArray());
                packet.Send();
            }
        }
    }
}