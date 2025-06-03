using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using System;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Particles;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public partial class PlantAura : ModProjectile
    {
        // TODO: This probably needs a lot of multiplayer syncing logic
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 420;
        }

        private enum TileType
        {
            FullBlock,
            FullBlockExposedLeft,
            FullBlockExposedRight,
            FullBlockExposedTop,
            FullBlockExposedBottom,
            FullBlockExposedLeftRight,
            FullBlockExposedUpDown,
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
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            if (AICounter++ < 60)
            {
                int radius = (int)MathHelper.Lerp(1, 10, AICounter / 60f);
                surfaceTiles.Clear();
                FindTiles(radius);
            }
            else
            {
                if (AICounter >= 240) return;

                if (AICounter % 10 == 0 && surfaceTiles.Count > 0) // Spawn projectiles every 30 ticks
                {
                    // Choose a random tile from the surfaceTiles list
                    var (tilePosition, type, displayColor) = surfaceTiles[Main.rand.Next(surfaceTiles.Count)];

                    // Calculate the world position of the tile
                    Vector2 spawnPosition = new Vector2(tilePosition.X * 16, tilePosition.Y * 16);

                    // Determine rotation based on the exposed edge
                    float rotation = GetRotationForTileType(type);

                    float randomOffset = MathHelper.ToRadians(Main.rand.NextFloat(-15f, 15f));
                    rotation += randomOffset;

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        spawnPosition,
                        Vector2.Zero,
                        ModContent.ProjectileType<FlowerSummon>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner,
                        rotation
                    );
                }
            }
        }

        Texture2D[] gasTextures = new Texture2D[] {
            ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/flame_01").Value,
            ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/flame_02").Value,
            ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/flame_03").Value,
            ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/flame_04").Value
        };

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.gamePaused) return false;

            // Draw categorized tiles with designated colors
            foreach (var (tilePosition, type, displayColor) in surfaceTiles)
            {
                Vector2 tileWorldPosition = new Vector2(tilePosition.X * 16, tilePosition.Y * 16);
                /*Main.spriteBatch.Draw(
                    ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_01").Value,
                    tileWorldPosition - Main.screenPosition,
                    new Rectangle(0, 0, 16, 16),
                    displayColor * 0.5f);*/

                var plantGas = new Gas(gasTextures, 0f, 0f);
                plantGas.hasLight = true;
                plantGas.hasWaveMovement = true;
                plantGas.hasAdditiveLayers = true;
                plantGas.lightIntensity = 0.5f;

                if (Main.rand.NextBool(4))
                {
                    ParticleManager.CreateParticleDirect(plantGas, tileWorldPosition, -Vector2.UnitY, Color.LimeGreen, alpha: 1f, scale: 0.1f, 0f);
                }
                /*switch (type)
                {
                    case TileType.FullBlockExposedTop:
                    case TileType.FullBlockExposedBottom:
                    case TileType.FullBlockExposedLeft:
                    case TileType.FullBlockExposedRight:
                        // For full blocks with exposed edges, draw slivers on the exposed edges
                        DrawFullBlockEdges(tileWorldPosition, type, displayColor);
                        break;

                    case TileType.HalfBlock:
                    //case TileType.HalfBlockExposedBottom:
                        // For half blocks, draw only the exposed half
                        DrawHalfBlockEdges(tileWorldPosition, type, displayColor);
                        break;

                    case TileType.SlopeUpLeft:
                    case TileType.SlopeUpRight:
                    case TileType.SlopeDownLeft:
                    case TileType.SlopeDownRight:
                        // For slopes, draw slivers across the slope area
                        DrawSlopeEdges(tileWorldPosition, type, displayColor);
                        break;
                }*/

                /*Main.spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    tileWorldPosition - Main.screenPosition,
                    new Rectangle(0, 0, 16, 16),
                    displayColor * 0.5f);*/
            }

            return base.PreDraw(ref lightColor);
        }
    }
}