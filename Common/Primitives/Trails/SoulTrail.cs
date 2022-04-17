using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SoulTrail : SimpleTrail
    {
        public SoulTrail() : base(30, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail1").Value)
        {
        }
    }

    public class SpikeTrail : SimpleTrail
    {
        public SpikeTrail() : base(12, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Trail2").Value)
        {
        }
    }
}