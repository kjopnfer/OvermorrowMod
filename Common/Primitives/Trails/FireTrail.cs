using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class FireTrail : SimpleTrail
    {
        public FireTrail() : base(22, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail3").Value, true)
        {
        }
    }
}