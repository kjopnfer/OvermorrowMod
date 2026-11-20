using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.Waypoints
{
    /// <summary>
    /// A tile the player lights once, then uses to open the full map and travel to any other lit waypoint.
    /// </summary>
    public abstract class Waypoint_TE : ModTileEntity
    {
        public bool IsLit;

        public abstract Rectangle TileArea { get; }

        public Vector2 WorldCenter => TileArea.Center.ToVector2() * 16f;

        /// <summary>
        /// The player's top-left position when standing centered on the floor below <see cref="TileArea"/>.
        /// </summary>
        public Vector2 LandingPoint(Player player)
        {
            Rectangle area = TileArea;
            return new Vector2(area.Center.X * 16f - player.width / 2f, area.Bottom * 16f - player.height);
        }

        public override void SaveData(TagCompound tag) => tag["IsLit"] = IsLit;

        public override void LoadData(TagCompound tag) => IsLit = tag.GetBool("IsLit");
    }
}
