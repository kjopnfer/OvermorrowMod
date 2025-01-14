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

        public Asset<Texture2D> BarrierNoiseTexture;

        private void LoadEffects()
        {
            TrailShader = Assets.Request<Effect>("Effects/TrailShader");
            BarrierShader = Assets.Request<Effect>("Effects/Barrier");

            BarrierNoiseTexture = Assets.Request<Texture2D>("Assets/Textures/TextureMaps/color_range", AssetRequestMode.ImmediateLoad);
        }

        private void UnloadEffects()
        {
            TrailShader = null;
            BarrierShader = null;
        }
    }
}
