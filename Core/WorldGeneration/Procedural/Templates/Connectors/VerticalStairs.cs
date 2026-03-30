using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public class VerticalStairs : IProcedural
    {
        private const int ShaftWidth = 38;
        private const int ShaftHeight = 34;

        public SocketAnchor Build(SocketAnchor entry, int fillTileType, int liningTileType)
        {
            int startX = entry.Position.X - ShaftWidth / 2;
            int startY = entry.Position.Y;

            for (int x = startX; x < startX + ShaftWidth; x++)
                for (int y = startY; y < startY + ShaftHeight; y++)
                    WorldGen.KillTile(x, y, false, false, true);

            int wood = ModContent.TileType<ArchiveWood>();

            // Top Left
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(startX + x, startY + y, TileID.Emerald, true, true);

            // Bottom Left
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(startX + x, startY + ShaftHeight - y - 1, TileID.Diamond, true, true);

            // Top Right
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(entry.Position.X + ShaftWidth / 2 - x - 1, startY + y, TileID.Emerald, true, true);

            // Bottom Right
            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 4; y++)
                    WorldGen.PlaceTile(entry.Position.X + ShaftWidth / 2 - x - 1, startY + ShaftHeight - y - 1, TileID.Diamond, true, true);

            return new SocketAnchor
            {
                Position = new Point(entry.Position.X, startY + ShaftHeight),
                Facing = SocketDirection.Down
            };
        }
    }
}
