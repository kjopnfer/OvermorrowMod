using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class WaveTrail : SimpleTrail
    {
        public WaveTrail() : base(35, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Wave").Value, false)
        {
        }
    }
}