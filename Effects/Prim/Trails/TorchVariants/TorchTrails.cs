﻿using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace OvermorrowMod.Effects.Prim.Trails.TorchVariants
{
    public class TorchTrail1 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {
            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Red, Color.Orange, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Red, Color.Orange, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail2 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {
            
            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Cyan, Color.LightCyan, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Cyan, Color.LightCyan, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail3 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Purple, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Purple, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail4 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Green, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Green, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail5 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Yellow, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Yellow, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail6 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Red, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Red, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail7 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Pink, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Pink, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail8 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Blue, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Blue, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail9 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Tan, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Tan, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail10 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Teal, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Teal, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail11 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Aqua, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Aqua, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }

    public class TorchTrail12 : Trail
    {
        private float Offset;
        public override void SetDefaults()
        {
            Width = 9;
            Length = 22;
            Effect = OvermorrowModFile.Mod.TrailShader;
            Color = Color.White;
        }
        public override void Update()
        {
            Offset += (1f / Length);
            Positions.Add(Entity.Center);
            if (Positions.Count > Length) Positions.RemoveAt(0);
        }
        public override void UpdateDead()
        {
            if (Positions.Count < 2)
            {
                Dead = true;
                return;
            }
            Positions.RemoveAt(0);
        }
        public override void PrepareEffect()
        {
            Effect.SafeSetParameter("WorldViewProjection", GetWVP());
            Effect.SafeSetParameter("uImage0", OvermorrowModFile.Mod.GetTexture("Effects/TrailTextures/Trail3"));
            Effect.CurrentTechnique.Passes["Texturized"].Apply();
        }
        public override void PrepareTrail()
        {

            if (Positions.Count < 2) return;
            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)(i) / (float)Length;
                float prog2 = (float)(i + 1) / (float)Length;
                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];
                Vector2 off1 = GetRotation(Positions.ToArray(), i) * Width * prog1;
                Vector2 off2 = GetRotation(Positions.ToArray(), i + 1) * Width * prog2;
                Color col1 = Color.Lerp(Color.Lime, Color.White, prog1) * prog1;
                Color col2 = Color.Lerp(Color.Lime, Color.White, prog2) * prog2;
                AddVertex(pos1 + off1, col1, new Vector2(prog1 + Offset, 1));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));

                AddVertex(pos2 + off2, col2, new Vector2(prog2 + Offset, 1));
                AddVertex(pos2 - off2, col2, new Vector2(prog2 + Offset, 0));
                AddVertex(pos1 - off1, col1, new Vector2(prog1 + Offset, 0));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count < 6) return;
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            device.RasterizerState = rasterizerState;

            device.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices.ToArray(), 0, Vertices.Count / 3);
        }
    }
}