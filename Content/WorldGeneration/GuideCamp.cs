using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Base;
using OvermorrowMod.Common.WorldGeneration;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.Tiles.GuideCamp;
using OvermorrowMod.Content.Tiles.TilePiles;
using OvermorrowMod.Content.Tiles.Town;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class GuideCamp : ModSystem
    {
        public static Vector2 FeydenCavePosition;
        public override void SaveWorldData(TagCompound tag)
        {
            tag["FeydenCavePosition"] = FeydenCavePosition;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            FeydenCavePosition = tag.Get<Vector2>("FeydenCavePosition");
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GuideIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Quick Cleanup"));
            if (GuideIndex != -1)
            {
                tasks.Insert(GuideIndex + 1, new PassLegacy("Spawn Camp", GenerateCamp));
            }

            int SurfaceCaves = tasks.FindIndex(genpass => genpass.Name.Equals("Rock Layer Caves"));
            if (SurfaceCaves != -1)
            {
                tasks.Insert(SurfaceCaves + 1, new PassLegacy("Feyden Cave", GenerateSlimeCave));
            }
        }

        private void GenerateSlimeCave(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Falling into a cave";

            float flatDelay = Main.maxTilesX * 0.05f;
            int x = (int)((Main.maxTilesX / 7 * 4) + flatDelay);
            int y = 0;

            bool validArea = false;
            while (!validArea)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                while (!tile.HasTile)
                {
                    y++;
                    tile = Framing.GetTileSafely(x, y);
                }

                // We have the tile but we want to check if its a grass block, if it isn't restart the process
                if (/*!aboveTile.HasTile*/Main.tileSolid[tile.TileType])
                {
                    validArea = true;
                }
            }

            FeydenCave feydenCave = new FeydenCave();
            feydenCave.Place(new Point(x, y), GenVars.structures);
        }

        private void GenerateCamp(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Setting up camp";

            int startX = Main.maxTilesX / 2;
            int startY = 0;

            bool validArea = false;

            int x = startX;
            int y = startY/* - 15*/;

            while (!validArea)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                while (!tile.HasTile)
                {
                    y++;
                    tile = Framing.GetTileSafely(x, y);
                }

                Tile aboveTile = Framing.GetTileSafely(x, y - 1);

                // We have the tile but we want to check if its a grass block, if it isn't restart the process
                if (/*!aboveTile.HasTile*/Main.tileSolid[tile.TileType])
                {
                    validArea = true;
                }
            }

            WorldGen.PlaceTile(x, y, TileID.Adamantite, false, true);

            for (int i = 0; i < 2; i++)
            {
                PlaceCamp(x + 3, y + 8);
            }
        }

        public static void PlaceCamp(int x, int y)
        {
            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(42, 100, 46)] = TileID.Grass,
                [new Color(71, 38, 28)] = TileID.Dirt,
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/GuideCamp_Clear").Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/GuideCamp").Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            //WorldGen.PlaceTile(x - (TileGen.width / 2), y - TileGen.height, TileID.Adamantite, false, true);

            Vector2 origin = new Vector2(x - (TileGen.width / 2), y - TileGen.height);

            ModUtils.PlaceObject((int)(origin.X + 20), (int)(origin.Y + 5), ModContent.TileType<GuideCampfire>());
            ModContent.GetInstance<GuideCampfire_TE>().Place((int)(origin.X + 19), (int)(origin.Y + 4));

            ModUtils.PlaceTilePile<BowRock, BowRockObjects>((int)(origin.X + 12), (int)(origin.Y + 4));
            //WorldGen.PlaceTile((int)(origin.X + 12), (int)(origin.Y + 4), TileID.Adamantite, false, true);
            ModUtils.PlaceTilePile<GuideStool, GuideStoolObjects>((int)(origin.X + 16), (int)(origin.Y + 5));
            //WorldGen.PlaceTile((int)(origin.X + 26), (int)(origin.Y + 5), TileID.Adamantite, false, true);

            ModUtils.PlaceTilePile<GuideLog, GuideLogObjects>((int)(origin.X + 26), (int)(origin.Y + 4));
            ModUtils.PlaceTilePile<GuideTent, GuideTentObjects>((int)(origin.X + 34), (int)(origin.Y + 3));
            ModUtils.PlaceTilePile<BookRock, BookRockObjects>((int)(origin.X + 39), (int)(origin.Y + 3));
            ModUtils.PlaceTilePile<AxeStump, AxeStumpObjects>((int)(origin.X + 42), (int)(origin.Y + 3));
        }
    }

    public class FeydenCave : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            FastNoiseLite noise = new FastNoiseLite(WorldGen._genRandSeed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            noise.SetFractalOctaves(6);
            noise.SetFractalLacunarity(4);
            noise.SetFrequency(0.025f);
            noise.SetFractalGain(0.1f);

            Vector2 end = origin.ToVector2() + new Vector2(0, 150).RotatedBy(MathHelper.PiOver4);
            FeydenTunneler tunnel = new FeydenTunneler(origin.ToVector2(), end, noise, 2);
            tunnel.Run(out _);

            structures.AddProtectedStructure(new Rectangle(origin.X, origin.Y, 300, 300));

            return true;
        }
    }

    public class FeydenTunneler : PerlinWorm
    {
        public int repeatWorm = 0;

        public FeydenTunneler(Vector2 startPosition, Vector2 endPosition, FastNoiseLite noise, int repeatWorm = 1) : base(startPosition, endPosition, noise)
        {
            this.repeatWorm = repeatWorm;
        }

        public int endDistance = 300;
        public override void OnRunStart(Vector2 position)
        {
        }

        public override void RunAction(Vector2 position, Vector2 endPosition, int currentIteration)
        {
            int size = Main.rand.Next(3, 7);
            WorldGen.digTunnel((int)position.X, (int)position.Y, 0, 0, 1, size, false);
        }

        public override void OnRunEnd(Vector2 position)
        {
            if (repeatWorm <= 1)
            {
                bool withinBounds = position.X > 0 && position.X < Main.maxTilesX && position.Y > 0 && position.Y < Main.maxTilesY;
                if (withinBounds)
                {
                    int repeat = Main.rand.Next(2, 4);
                    for (int i = 0; i < repeat; i++)
                    {
                        float xScale = 0.8f + Main.rand.NextFloat() * 0.5f; // Randomize the width of the shrine area
                        float yScale = Main.rand.NextFloat(0.6f, 0.8f);
                        int radius = Main.rand.Next(32, 48);
                        Point shapePosition = new Point((int)position.X + -15 * i, (int)position.Y + Main.rand.Next(-5, 5));

                        ShapeData slimeShapeData = new ShapeData();
                        WorldUtils.Gen(shapePosition, new Shapes.Slime(20, xScale, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));
                        WorldUtils.Gen(shapePosition, new ModShapes.InnerOutline(slimeShapeData, true), Actions.Chain(new Modifiers.Blotches(3, 0.65f), new Modifiers.IsSolid(), new Actions.SetTile(TileID.SlimeBlock, true)));

                        if (i == repeat - 1)
                        {
                            GuideCamp.FeydenCavePosition = shapePosition.ToVector2() * 16;
                            NPC.NewNPC(null, shapePosition.X * 16, shapePosition.Y * 16, ModContent.NPCType<Feyden_Bound>());
                        }
                    }
                }
            }
            else
            {
                Vector2 branchEndpoint = position + new Vector2(0, 150).RotatedBy(-MathHelper.PiOver2);
                FeydenTunneler tunnel = new FeydenTunneler(position, branchEndpoint, noise, --repeatWorm);
                tunnel.Run(out _);
            }

            base.OnRunEnd(position);
        }
    }
}