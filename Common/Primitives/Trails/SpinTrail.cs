using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SpinTrail : SimpleTrail
    {
        //public SpinTrail() : base(15, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail2"), true)
        public SpinTrail() : base(/*60*/120, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Extra_209").Value, false)
        {

        }
    }
}