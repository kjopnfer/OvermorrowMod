using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Common;
using System;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public partial class PlantAura : ModProjectile
    {
        private float GetRotationForTileType(TileType type)
        {
            return type switch
            {
                TileType.FullBlockExposedTop => 0f, // Upward
                TileType.FullBlockExposedBottom => MathHelper.Pi, // Downward
                TileType.FullBlockExposedLeft => -MathHelper.PiOver2, // Left
                TileType.FullBlockExposedRight => MathHelper.PiOver2, // Right
                TileType.FullBlockExposedLeftRight => Main.rand.NextBool() ? 0f : MathHelper.Pi, // Randomly choose left or right
                TileType.FullBlockExposedUpDown => Main.rand.NextBool() ? -MathHelper.PiOver2 : MathHelper.PiOver2, // Randomly choose up or down
                TileType.SlopeUpLeft => -MathHelper.PiOver4, // Upward-left
                TileType.SlopeUpRight => MathHelper.PiOver4, // Upward-right
                TileType.SlopeDownLeft => MathHelper.PiOver4, // Downward-left
                TileType.SlopeDownRight => -MathHelper.PiOver4, // Downward-right
                _ => 0f, // Default to rightward if no special case applies
            };
        }

        private void FindTiles(int radius)
        {
            Point centerTile = Projectile.Center.ToTileCoordinates();
            Vector2 elevatedCenter = Projectile.Center - new Vector2(0, 5 * 18); // Start 3 tiles above

            for (int x = centerTile.X - radius; x <= centerTile.X + radius; x++)
            {
                for (int y = centerTile.Y - radius; y <= centerTile.Y + radius; y++)
                {
                    if (x >= 0 && x < Main.maxTilesX && y >= 0 && y < Main.maxTilesY)
                    {

                        Tile tile = Main.tile[x, y];
                        if (tile.HasTile && Main.tileSolid[tile.TileType] && IsOuterTile(x, y))
                        {
                            TileType type = CategorizeTile(tile, x, y);
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


        private TileType CategorizeTile(Tile tile, int x, int y)
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

            Tile leftTile = Main.tile[x - 1, y];
            Tile rightTile = Main.tile[x + 1, y];
            Tile topTile = Main.tile[x, y - 1];
            Tile bottomTile = Main.tile[x, y + 1];
            if (!leftTile.HasTile && !rightTile.HasTile && topTile.HasTile && bottomTile.HasTile)
                return TileType.FullBlockExposedLeftRight;

            if (leftTile.HasTile && rightTile.HasTile && !topTile.HasTile && !bottomTile.HasTile)
                return TileType.FullBlockExposedUpDown;

            if (!leftTile.HasTile && rightTile.HasTile && topTile.HasTile && bottomTile.HasTile)
                return TileType.FullBlockExposedLeft;

            if (leftTile.HasTile && !rightTile.HasTile && topTile.HasTile && bottomTile.HasTile)
                return TileType.FullBlockExposedRight;

            if (leftTile.HasTile && rightTile.HasTile && !topTile.HasTile && bottomTile.HasTile)
                return TileType.FullBlockExposedTop;

            if (leftTile.HasTile && rightTile.HasTile && topTile.HasTile && !bottomTile.HasTile)
                return TileType.FullBlockExposedBottom;

            #region Trees and Shit
            // Check for FullBlockExposedLeftRight (exposed on both left and right)
            if (!Main.tileSolid[leftTile.TileType] && !Main.tileSolid[rightTile.TileType] &&
                Main.tileSolid[topTile.TileType] && Main.tileSolid[bottomTile.TileType])
            {
                return TileType.FullBlockExposedLeftRight;
            }

            // Check for FullBlockExposedUpDown (exposed on top and bottom)
            if (Main.tileSolid[leftTile.TileType] && Main.tileSolid[rightTile.TileType] &&
                !Main.tileSolid[topTile.TileType] && !Main.tileSolid[bottomTile.TileType])
            {
                return TileType.FullBlockExposedUpDown;
            }

            // Check for FullBlockExposedLeft (exposed on left side)
            if (!Main.tileSolid[leftTile.TileType] && Main.tileSolid[rightTile.TileType] &&
                Main.tileSolid[topTile.TileType] && Main.tileSolid[bottomTile.TileType])
            {
                return TileType.FullBlockExposedLeft;
            }

            // Check for FullBlockExposedRight (exposed on right side)
            if (Main.tileSolid[leftTile.TileType] && !Main.tileSolid[rightTile.TileType] &&
                Main.tileSolid[topTile.TileType] && Main.tileSolid[bottomTile.TileType])
            {
                return TileType.FullBlockExposedRight;
            }

            // Check for FullBlockExposedTop (exposed on top side)
            if (Main.tileSolid[leftTile.TileType] && Main.tileSolid[rightTile.TileType] &&
                !Main.tileSolid[topTile.TileType] && Main.tileSolid[bottomTile.TileType])
            {
                return TileType.FullBlockExposedTop;
            }

            // Check for FullBlockExposedBottom (exposed on bottom side)
            if (Main.tileSolid[leftTile.TileType] && Main.tileSolid[rightTile.TileType] &&
                Main.tileSolid[topTile.TileType] && !Main.tileSolid[bottomTile.TileType])
            {
                return TileType.FullBlockExposedBottom;
            }
            #endregion

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
                TileType.FullBlockExposedTop => Color.Cyan,
                TileType.FullBlockExposedBottom => Color.Magenta,
                TileType.FullBlockExposedLeft => Color.Pink,
                TileType.FullBlockExposedRight => Color.Teal,
                //TileType.HalfBlockExposedTop => Color.LightGreen,
                //TileType.HalfBlockExposedBottom => Color.LightPink,
                _ => Color.White
            };
        }

        private void DrawFullBlockEdges(Vector2 tileWorldPosition, TileType type, Color color)
        {
            // Draw slivers for full blocks based on the exposed edge
            Vector2 topLeft = tileWorldPosition + new Vector2(0, 0); // Top edge sliver
            Vector2 bottomLeft = tileWorldPosition + new Vector2(0, 0); // Bottom edge sliver
            Vector2 leftEdge = tileWorldPosition + new Vector2(0, 0); // Left edge sliver
            Vector2 rightEdge = tileWorldPosition + new Vector2(14, 0); // Right edge sliver

            // Example of drawing slivers based on edge exposure
            if (type == TileType.FullBlockExposedTop)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    topLeft - Main.screenPosition,
                    new Rectangle(0, 0, 16, 2), // Top edge sliver
                    color * 0.5f
                );
            }
            if (type == TileType.FullBlockExposedBottom)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    bottomLeft - Main.screenPosition,
                    new Rectangle(0, 0, 16, 2), // Bottom edge sliver
                    color * 0.5f
                );
            }
            if (type == TileType.FullBlockExposedLeft)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    leftEdge - Main.screenPosition,
                    new Rectangle(0, 0, 2, 16), // Left edge sliver
                    color * 0.5f
                );
            }
            if (type == TileType.FullBlockExposedRight)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    rightEdge - Main.screenPosition,
                    new Rectangle(0, 0, 2, 16), // Right edge sliver
                    color * 0.5f
                );
            }
            if (type == TileType.FullBlockExposedLeftRight)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    leftEdge - Main.screenPosition,
                    new Rectangle(0, 0, 2, 16), // Left edge sliver
                    color * 0.5f
                );
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    rightEdge - Main.screenPosition,
                    new Rectangle(0, 0, 2, 16), // Right edge sliver
                    color * 0.5f
                );
            }
        }

        private void DrawSlopeEdges(Vector2 tileWorldPosition, TileType type, Color color)
        {
            // For slopes, draw slivers only along the exposed part of the slope
            Vector2 slopePosition = tileWorldPosition + new Vector2(8, 14); // Adjust to slope edge

            if (type == TileType.SlopeDownRight)
            {
                float rotation = -MathHelper.PiOver4;
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    slopePosition - Main.screenPosition, // Position of the texture
                    new Rectangle(0, 0, 24, 2), // The source rectangle of the texture (width: 16, height: 8)
                    Color.Red * 0.5f, // Color with transparency
                    rotation, // Rotation angle in radians
                    new Vector2(8, 4), // Origin point for the rotation (center of the rectangle)
                    1f, // Scale factor (no scaling, so 1f)
                    SpriteEffects.None, // No flipping
                    0f // Layer depth
                );
            }
            else if (type == TileType.SlopeDownLeft)
            {
                slopePosition = tileWorldPosition + new Vector2(0, 0);
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    slopePosition - Main.screenPosition,
                    new Rectangle(0, 0, 24, 2), // Slope exposure
                    Color.Pink * 0.5f,
                    MathHelper.PiOver4,
                    Vector2.Zero, 1f, SpriteEffects.None, 0
                );
            }
            else if (type == TileType.SlopeUpLeft)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    slopePosition - Main.screenPosition,
                    new Rectangle(0, 0, 16, 8), // Slope exposure
                    Color.Red * 0.5f,
                    MathHelper.PiOver4,
                    Vector2.Zero, 1f, SpriteEffects.None, 0
                );
            }
            else if (type == TileType.SlopeUpRight)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    slopePosition - Main.screenPosition,
                    new Rectangle(0, 0, 16, 8), // Slope exposure
                    Color.Red * 0.5f,
                    MathHelper.PiOver4,
                    Vector2.Zero, 1f, SpriteEffects.None, 0
                );
            }
            // You can similarly add for other slope types (SlopeUpRight, SlopeDownLeft) here
        }

        private void DrawHalfBlockEdges(Vector2 tileWorldPosition, TileType type, Color color)
        {
            // Draw slivers for half blocks only on the exposed half (top or bottom)
            Vector2 topHalf = tileWorldPosition + new Vector2(0, 8); // Top edge sliver
            Vector2 bottomHalf = tileWorldPosition + new Vector2(0, 8); // Bottom edge sliver

            if (type == TileType.HalfBlock)
            {
                Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    topHalf - Main.screenPosition,
                    new Rectangle(0, 0, 16, 2), // Top half exposed
                    color * 0.5f
                );
            }
            //if (type == TileType.HalfBlockExposedBottom)
            //{
            //    Main.spriteBatch.Draw(
            //        Terraria.GameContent.TextureAssets.MagicPixel.Value,
            //        bottomHalf - Main.screenPosition,
            //        new Rectangle(0, 0, 16, 2), // Bottom half exposed
            //        color * 0.5f
            //    );
            //}
        }
    }
}