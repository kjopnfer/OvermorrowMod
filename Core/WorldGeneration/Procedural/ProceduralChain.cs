using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public static class ProceduralChain
    {
        private const int FillPadding = 20;

        public static List<ProceduralRoom> Build(
            Point start, Point target, int roomCount,
            List<IRoomTemplate> roomPool,
            int fillTileType, int liningTileType, Random rand)
        {
            var rooms = new List<ProceduralRoom>();

            int minX = Math.Min(start.X, target.X) - FillPadding;
            int maxX = Math.Max(start.X, target.X) + 200 + FillPadding;
            int minY = Math.Min(start.Y, target.Y) - 100 - FillPadding;
            int maxY = Math.Max(start.Y, target.Y) + 100 + FillPadding;

            // Fill with stone
            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    WorldGen.PlaceTile(x, y, fillTileType, true, true);

            // Seed cursor from start point (floor-level position)
            SocketAnchor cursor = new SocketAnchor
            {
                Position = start,
                Facing = SocketDirection.Right
            };

            for (int i = 0; i < roomCount; i++)
            {
                var template = roomPool[rand.Next(roomPool.Count)];

                // First room uses start directly; subsequent rooms align to cursor
                Point roomPos = (i == 0) ? start : template.AlignTo(cursor);
                var room = template.Generate(roomPos, fillTileType, liningTileType);
                rooms.Add(room);

                if (i < roomCount - 1)
                {
                    var outputSocket = room.Right;
                    if (outputSocket != null && outputSocket.Accepted != null && outputSocket.Accepted.Count > 0)
                    {
                        var connector = outputSocket.Accepted[rand.Next(outputSocket.Accepted.Count)];
                        cursor = connector.Build(outputSocket.ToAnchor(), fillTileType, liningTileType);
                    }
                }
            }

            return rooms;
        }

    }
}
