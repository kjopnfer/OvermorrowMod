using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using ReLogic.Content;
using Terraria.ModLoader;

namespace OvermorrowMod.Core
{
    public partial class OvermorrowModFile : Mod
    {
        public Asset<Texture2D> GradientRectangle;

        private void LoadTextures()
        {
            GradientRectangle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "gradient_rectangle2", AssetRequestMode.ImmediateLoad);
        }

        private void UnloadTextures()
        {
            GradientRectangle = null;
        }
    }
}