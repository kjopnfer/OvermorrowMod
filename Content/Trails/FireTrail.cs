using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class FireTrail : SimpleTrail
    {
        public FireTrail() : base(22, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Fire").Value, true)
        {
        }
    }
}