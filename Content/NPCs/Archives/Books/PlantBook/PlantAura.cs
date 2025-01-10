using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Common;
using System;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class PlantAura : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.timeLeft = 120;
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
        }

        private enum TileType
        {
            FullBlock,
            HalfBlock,
            SlopeUpLeft,
            SlopeUpRight,
            SlopeDownLeft,
            SlopeDownRight
        }

        private List<(Point TilePosition, TileType Type, Color DisplayColor)> surfaceTiles = new();

        public override void OnSpawn(IEntitySource source)
        {
            // Adjust position to the nearest ground tile
            Point tilePosition = Projectile.Center.ToTileCoordinates();

            for (int y = tilePosition.Y; y < Main.maxTilesY; y++)
            {
                Tile tile = Main.tile[tilePosition.X, y];
                if (tile.HasTile && Main.tileSolid[tile.TileType])
                {
                    // Snap to the ground
                    Projectile.position.Y = y * 16 - Projectile.height / 2;
                    break;
                }
            }

            // Categorize tiles around the projectile
            FindTiles();
        }

        private void FindTiles()
        {
            Point centerTile = Projectile.Center.ToTileCoordinates();
            Vector2 elevatedCenter = Projectile.Center - new Vector2(0, 5 * 18); // Start 3 tiles above

            int radius = 5; // Radius to check around the projectile (in tiles)

            Dust.NewDust((Projectile.Center - new Vector2(0, 3 * 18)), 1, 1, DustID.DemonTorch);
            for (int x = centerTile.X - radius; x <= centerTile.X + radius; x++)
            {
                for (int y = centerTile.Y - radius; y <= centerTile.Y + radius; y++)
                {
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {

                        Tile tile = Main.tile[x, y];
                        if (tile.HasTile && Main.tileSolid[tile.TileType] && IsOuterTile(x, y))
                        {
                            TileType type = CategorizeTile(tile);
                            Color displayColor = GetTileColor(type);
                            surfaceTiles.Add((new Point(x, y), type, displayColor));
                        }
                    }
                }
            }
        }

        private bool IsOuterTile(int x, int y)
        {
            // Track if we have encountered any non-solid tile next to this one
            bool hasNonSolidNeighbor = false;

            // Check adjacent tiles (top, bottom, left, and right)
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue; // Skip the current tile

                    if (Math.Abs(dx) == Math.Abs(dy)) // Skip diagonals
                        continue;

                    int checkX = x + dx;
                    int checkY = y + dy;

                    // Ensure we are within the bounds of the map
                    if (checkX >= 0 && checkX < Main.maxTilesX && checkY >= 0 && checkY < Main.maxTilesY)
                    {
                        Tile adjacentTile = Main.tile[checkX, checkY];

                        // If an adjacent tile is non-solid, mark it
                        if (!adjacentTile.HasTile || !Main.tileSolid[adjacentTile.TileType])
                        {
                            hasNonSolidNeighbor = true;
                        }
                    }
                }
            }

            // We want this to be a surface tile, so:
            // 1. There should be at least one non-solid neighboring tile (checking for "outer").
            // 2. We don't want this to be part of an inner corner, meaning we avoid cases where all 4 surrounding tiles are solid.
            return hasNonSolidNeighbor;
        }


        private TileType CategorizeTile(Tile tile)
        {
            if (tile.IsHalfBlock)
                return TileType.HalfBlock;

            if (tile.Slope == SlopeType.SlopeUpLeft)
                return TileType.SlopeUpLeft;

            if (tile.Slope == SlopeType.SlopeUpRight)
                return TileType.SlopeUpRight;

            if (tile.Slope == SlopeType.SlopeDownLeft)
                return TileType.SlopeDownLeft;

            if (tile.Slope == SlopeType.SlopeDownRight)
                return TileType.SlopeDownRight;

            return TileType.FullBlock;
        }

        private Color GetTileColor(TileType type)
        {
            return type switch
            {
                TileType.FullBlock => Color.Green,
                TileType.HalfBlock => Color.Yellow,
                TileType.SlopeUpLeft => Color.Blue,
                TileType.SlopeUpRight => Color.Red,
                TileType.SlopeDownLeft => Color.Purple,
                TileType.SlopeDownRight => Color.Orange,
                _ => Color.White
            };
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Draw categorized tiles with designated colors
            foreach (var (tilePosition, _, displayColor) in surfaceTiles)
            {
                Vector2 tileWorldPosition = new Vector2(tilePosition.X * 16, tilePosition.Y * 16);
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    tileWorldPosition - Main.screenPosition,
                    new Rectangle(0, 0, 16, 16),
                    displayColor * 0.5f);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}