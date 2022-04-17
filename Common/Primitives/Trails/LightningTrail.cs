using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class LightningTrail : SimpleTrail
    {
        public LightningTrail() : base(20, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail5").Value, true)
        {
        }
    }
}