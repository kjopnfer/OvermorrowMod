using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class LaserTrail : SimpleTrail
    {
        public LaserTrail() : base(35, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Laser", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, false)
        {
        }
    }
}