using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.Waypoints
{
    /// <summary>
    /// A tile the player lights once, then travels between through the full map. Waypoints only link
    /// to lit waypoints with the same <see cref="DungeonIndex"/>.
    /// </summary>
    public abstract class Waypoint_TE : ModTileEntity
    {
        public bool IsLit;

        /// <summary>
        /// The procedural dungeon this waypoint was generated in, or -1 for waypoints placed outside the layout.
        /// </summary>
        public int DungeonIndex = -1;

        public abstract Rectangle TileArea { get; }

        /// <summary>
        /// The player's top-left position when standing centered on the floor below <see cref="TileArea"/>.
        /// </summary>
        public Vector2 LandingPoint(Player player)
        {
            Rectangle area = TileArea;
            return new Vector2(area.Center.X * 16f - player.width / 2f, area.Bottom * 16f - player.height);
        }

        public override void SaveData(TagCompound tag)
        {
            tag["IsLit"] = IsLit;
            tag["DungeonIndex"] = DungeonIndex;
        }

        public override void LoadData(TagCompound tag)
        {
            IsLit = tag.GetBool("IsLit");
            DungeonIndex = tag.ContainsKey("DungeonIndex") ? tag.GetInt("DungeonIndex") : -1;
        }
    }
}
