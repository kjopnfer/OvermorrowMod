using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class EnergyTrail : SimpleTrail
    {
        public EnergyTrail() : base(35, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Energy").Value, false)
        {
        }
    }
}