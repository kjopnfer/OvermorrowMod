using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using System.IO;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.TextureMapping
{
    public class TexGenData
    {
        public readonly Color[] Data;
        public readonly int Width;
        public readonly int Height;

        public Color this[int index] => Data[index];

        public int Length => Data.Length;

        public TexGenData(Color[] data, int width, int height)
        {
            Data = data;
            Width = width;
            Height = height;
        }

        public static TexGenData FromStream(Stream stream)
        {
            byte[] colorBytes = ImageIO.ReadRaw(stream, out int width, out int height);
            Color[] colors = new Color[width * height];

            for (int i = 0; i < width * height * 4; i += 4)
            {
                colors[i / 4].PackedValue = (uint)(colorBytes[i + 3] << 24 | colorBytes[i + 2] << 16 | colorBytes[i + 1] << 8 | colorBytes[i]);
            }

            return new(colors, width, height);
        }

        public static TexGenData FromTexture2D(Texture2D texture)
        {
            Color[] data = new Color[texture.Width * texture.Height];
            SystemUtils.InvokeOnMainThread(() => texture.GetData(0, texture.Bounds, data, 0, texture.Width * texture.Height));
            return new(data, texture.Width, texture.Height);
        }
    }
}