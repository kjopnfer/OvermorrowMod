using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class TorchTrail : SimpleTrail
    {
        public TorchTrail() : base(15, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail1").Value, false)
        {
        }
    }
}