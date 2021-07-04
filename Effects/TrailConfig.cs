using Terraria;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace OvermorrowMod.Effects
{
    public class TrailConfig
    {
        public delegate void Prepare(ref List<VertexPositionColorTexture> positions, Vector2 current, Vector2 next, float progress1, float progress2);
        public delegate float TSize(float progress);
        public delegate Color TColor(float progress);
        public int Length = 20;
        public Prepare CustomPrepare = null;
        public TSize size = delegate(float progress) {return 1f;};
        public TColor color = delegate(float progress) {return Color.White;};
        public bool Pixelate = false;
        public float PixelateMult = 0f;
        public Texture2D texture = null;
        public bool TAlpha = false;
        public bool TClone = false;
        public Effect effect = OvermorrowModFile.Mod.VertexShader;
    }
}