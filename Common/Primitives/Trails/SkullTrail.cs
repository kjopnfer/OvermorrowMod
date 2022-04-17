using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SkullTrail : SimpleTrail
    {
        public SkullTrail() : base(12, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail6").Value, true)
        {
        }
    }
}