using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using OvermorrowMod.Effects;

namespace OvermorrowMod.Effects
{
    public class PrimitivePacket
    {
        public delegate short[] Indices(int count);
        public delegate int GetCount(int limit);
        public bool UsesCustomIndices = true;
        public Indices CustomIndices;
        public bool UsesCustomCount;
        public GetCount CustomCount;
        // type of primitive, TriangleList, TriangleStrip, LineList, LineStrip
        public PrimitiveType type;
        // texture for the next thing
        public Texture2D texture;
        // the pass it uses, options are:
        public string pass;
        // positions it gotta draw
        public List<VertexPositionColorTexture> draws = new List<VertexPositionColorTexture>();
        // the shader thats made
        private Effect effect = OvermorrowModFile.Mod.VertexShader;
        private TrailConfig config;
        // self explanatory
        public PrimitivePacket(TrailConfig config = null)
        {
            if (config != null)
            {
                effect = config.effect;
                this.config = config;
            }
        }

        public Vector3 ToVector3(Vector2 input)
        {
            return new Vector3(input.X, input.Y, 0);
        }

        public void Add(Vector2 pos, Color color, Vector2 TexCoord, Vector2 offset = default)
        {
            pos += -Main.screenPosition + offset;
            draws.Add(new VertexPositionColorTexture(ToVector3(pos), color, TexCoord));
        }
        public void Add(Vector3 pos, Color color, Vector2 TexCoord, Vector2 offset = default)
        {
            pos -= ToVector3(Main.screenPosition) + ToVector3(offset);
            draws.Add(new VertexPositionColorTexture(pos, color, TexCoord));
        }
        public void Add(VertexPositionColorTexture pos)
        {
            pos.Position -= ToVector3(Main.screenPosition);
            draws.Add(pos);
        }
        public void AddAsStrip(Vector2 pos, Color color, float prog, float rot, float size)
        {
            Vector2 offset = (rot + MathHelper.Pi / 2).ToRotationVector2() * size;
            Add(pos + offset, color, new Vector2(prog, 0));
            Add(pos - offset, color, new Vector2(prog, 1));
        }
        // self explanatory
        public void Set(VertexPositionColorTexture[] positions)
        {
            draws = new List<VertexPositionColorTexture>(positions);
        }
        public void Set(List<VertexPositionColorTexture> positions)
        {
            draws = positions;
        }
        // self explanatory
        public void Clear()
        {
            type = default;
            texture = null;
            pass = "Basic";
            draws.Clear();
            effect = OvermorrowModFile.Mod.VertexShader;
        }
        // this is literally just the amount of times it gotta draw
        public int PrimitiveCount
        {
            get
            {            
                int count = 0;
                if (!UsesCustomCount)
                switch(type)
                {
                    case PrimitiveType.LineList:
                    count = draws.Count / 2;
                    break;
                    case PrimitiveType.LineStrip:
                    count = draws.Count - 1;
                    break;
                    case PrimitiveType.TriangleList:
                    count = draws.Count / 3;
                    break;
                    case PrimitiveType.TriangleStrip:
                    count = draws.Count - 2;
                    break;
                }
                else
                count = CustomCount(count);
                return count;
            }
        }
        // indices for drawing them, dont mess with them yet
        public short[] GetIndices()
        {
            short[] indexes = new short[1];
            int count = draws.Count - 1;
            if (!UsesCustomIndices)
            {
            switch(type)
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
                for (short i = 0; i < count; i = (short)(i+1))
                {
                    short indexInArray = (short)(i);
                    int num = i * 2;
                    indexes[indexInArray] = (short)num;
                }
                break;
            }
            }
            else if (CustomIndices != null)
            {
                indexes = CustomIndices(count);
            }
            return indexes;
            /*
            LineList
            1 --- 2
            3 --- 4
            LineStrip
            1 --- 2 --- 3 --- 4
            TriangleList
            1 --- 2 --- 3     4 --- 5 --- 6
            TriangleStrip
            1 --- 2 --- 3     3 --- 4 --- 5
            */
        }
        // Makes the primitives actually draw
        public void Send()
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            VertexBuffer vertex = new VertexBuffer(device, typeof(VertexPositionColorTexture), draws.Count, BufferUsage.WriteOnly);
            IndexBuffer index = new IndexBuffer(device, typeof(short), GetIndices().Length, BufferUsage.WriteOnly);
            device.SetVertexBuffer(null);

            vertex.SetData(draws.ToArray());
            index.SetData(GetIndices());

            device.SetVertexBuffer(vertex);
            device.Indices = index;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            if (config == null)
            {
                effect.Parameters["WVP"].SetValue(WVP);
                effect.Parameters["strength"].SetValue(1f);
                if (texture != null)
                effect.Parameters["imageTexture"].SetValue(texture);
                
                effect.CurrentTechnique.Passes[pass].Apply();
            }
            else
            {
                Main.graphics.GraphicsDevice.Textures[0] = texture;
                effect.Parameters["Pixelate"].SetValue(config.Pixelate);
                effect.Parameters["PixelMult"].SetValue(config.PixelateMult);
                effect.Parameters["TAlpha"].SetValue(config.TAlpha);
                effect.Parameters["TClone"].SetValue(config.TClone);
                effect.Parameters["wvp"].SetValue(WVP);

                effect.CurrentTechnique.Passes["Basic"].Apply();
            }

            device.DrawPrimitives(type, 0, PrimitiveCount);
        }
        // just the matrix for it
        public Matrix WVP
        {
            get
            {
                GraphicsDevice graphics = Main.graphics.GraphicsDevice;
		    	Vector2 zoom = Main.GameViewMatrix.Zoom;
		    	int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;
                Matrix Zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) * Matrix.CreateTranslation(width / 2, height / -2, 0) * Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(zoom.X, zoom.Y, 1f);
		    	Matrix Projection = Matrix.CreateOrthographic(width, height, 0, 1000);
			    return Zoom * Projection;
            }
        }
    }
}