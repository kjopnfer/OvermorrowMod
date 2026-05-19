using OvermorrowMod.Common;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Misc
{
    /// <summary>
    /// General-purpose empty wall. 
    /// Used for objects to anchor to without needing an actual wall texture behind it.
    /// </summary>
    public class InvisibleWall : ModWall
    {
        public override string Texture => AssetDirectory.Tiles + Name;
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}
