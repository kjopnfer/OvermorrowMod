using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core
{
    public partial class OvermorrowModFile : Mod
    {
        public Asset<Effect> TrailShader;
        public Asset<Effect> BarrierShader;
        public Asset<Effect> Ring;

        public Asset<Texture2D> BarrierNoiseTexture;

        private void LoadEffects()
        {
            BarrierShader = Assets.Request<Effect>("Effects/Barrier");
            TrailShader = Assets.Request<Effect>("Effects/TrailShader");
            Ring = Assets.Request<Effect>("Effects/Ring");

            BarrierNoiseTexture = Assets.Request<Texture2D>("Assets/Textures/TextureMaps/color_range", AssetRequestMode.ImmediateLoad);
        }

        private void UnloadEffects()
        {
            TrailShader = null;
            BarrierShader = null;
        }
    }
}
