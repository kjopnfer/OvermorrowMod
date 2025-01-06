using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace OvermorrowMod.Core
{
    public partial class OvermorrowModFile : Mod
    {
        public Asset<Effect> TrailShader;

        private void LoadEffects()
        {
            TrailShader = Assets.Request<Effect>("Effects/TrailShader");
        }

        private void UnloadEffects()
        {
            TrailShader = null;
        }
    }
}
