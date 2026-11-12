using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// One painted candidate from a cell's Spawns layer. Holds its world position,
    /// painted color, and grid cell coords. Pool and resolved NPC type are filled
    /// in by the selector.
    /// </summary>
    public class SpawnSlot
    {
        public Point WorldPos;
        public (byte R, byte G, byte B) Color;
        public Point GridCoord;
        public SpawnPool Pool;
        public int ResolvedNpcType = -1;
    }
}
