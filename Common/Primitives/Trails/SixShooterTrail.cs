using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Primitives.Trails
{
    public class SixShooterTrail : SimpleTrail
    {
        public SixShooterTrail() : base(20, OvermorrowModFile.Mod.GetTexture(AssetDirectory.Trails + "Trail6"), true)
        {
        }
    }
}