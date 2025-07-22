using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core
{
    public partial class OvermorrowModFile : Mod
    {
        public Asset<Effect> BarrierShader;
        public Asset<Effect> ColorFill;
        public Asset<Effect> Outline;
        public Asset<Effect> Ring;
        public Asset<Effect> TrailShader;

        public Asset<Texture2D> BarrierNoiseTexture;

        private void LoadEffects()
        {
            BarrierShader = Assets.Request<Effect>("Effects/Barrier");
            ColorFill = Assets.Request<Effect>("Effects/ColorFill");
            Outline = Assets.Request<Effect>("Effects/Outline");
            Ring = Assets.Request<Effect>("Effects/Ring");
            TrailShader = Assets.Request<Effect>("Effects/TrailShader");

            BarrierNoiseTexture = Assets.Request<Texture2D>("Assets/Textures/TextureMaps/color_range", AssetRequestMode.ImmediateLoad);
        }

        private void UnloadEffects()
        {
            BarrierShader = null;
            ColorFill = null;
            Outline = null;
            Ring = null;
            TrailShader = null;
        }
    }
}
