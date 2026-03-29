using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public class FlatCorridor : IProcedural
    {
        private const int CorridorHeight = 8;
        private const int Length = 41;

        public SocketAnchor Build(SocketAnchor entry, int fillTileType, int liningTileType)
        {
            int startX = entry.Position.X;
            int floorY = entry.Position.Y;

            WorldGen.PlaceTile(startX, floorY - CorridorHeight, TileID.Adamantite, true, true);
            WorldGen.PlaceTile(startX, floorY, TileID.Cobalt, true, true);

            // Clear the corridor air space
            for (int x = startX; x <= startX + Length; x++)
                for (int y = floorY - CorridorHeight; y <= floorY; y++)
                    WorldGen.KillTile(x, y);

            int endX = startX + Length;
            WorldGen.PlaceTile(endX, floorY - CorridorHeight, TileID.Orichalcum, true, true);
            WorldGen.PlaceTile(endX, floorY, TileID.Mythril, true, true);

            return new SocketAnchor
            {
                Position = new Point(endX, floorY),
                Facing = SocketDirection.Right
            };
        }
    }
}
