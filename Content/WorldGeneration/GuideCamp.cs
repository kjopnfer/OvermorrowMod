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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Content.Tiles.Underground;

namespace OvermorrowMod.Content.WorldGeneration
{
    public class GuideCamp : ModSystem
    {
        public static Vector2 FeydenCavePosition;
        public static Vector2 SlimeCaveEntrance;
        public override void SaveWorldData(TagCompound tag)
        {
            tag["SlimeCaveEntrance"] = FeydenCavePosition;
            tag["FeydenCavePosition"] = FeydenCavePosition;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            FeydenCavePosition = tag.Get<Vector2>("FeydenCavePosition");
            SlimeCaveEntrance = tag.Get<Vector2>("SlimeCaveEntrance");
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int SurfaceCaves = tasks.FindIndex(genpass => genpass.Name.Equals("Rock Layer Caves"));
            if (SurfaceCaves != -1)
            {
                tasks.Insert(SurfaceCaves + 1, new PassLegacy("Spawn Camp", GenerateCamp));
            }

            int TunnelIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Rocks In Dirt"));
            if (TunnelIndex != -1)
            {
                tasks.Insert(TunnelIndex + 1, new PassLegacy("Feyden Cave", GenerateSlimeCave));
            }
            //int GuideIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Quick Cleanup"));
            //if (GuideIndex != -1) tasks.Insert(GuideIndex + 1, new PassLegacy("Spawn Camp", GenerateCamp));
        }

        private void GenerateSlimeCave(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Falling into a cave";

            float tilePadding = Main.maxTilesX * 0.05f;

            Vector2 startPosition = new Vector2((int)((Main.maxTilesX / 7 * 4) + tilePadding), 0);
            Vector2 cavePosition = ModUtils.FindNearestGround(startPosition, false);
            SlimeCaveEntrance = cavePosition * 16;

            FeydenCave feydenCave = new FeydenCave();
            feydenCave.Place(new Point((int)cavePosition.X, (int)cavePosition.Y), GenVars.structures);
        }

        private void GenerateCamp(GenerationProgress progress, GameConfiguration config)
        {
            progress.Message = "Setting up camp";

            Vector2 startPosition = new Vector2(Main.maxTilesX / 2, 0);
            Vector2 campPosition = ModUtils.FindNearestGround(startPosition, false);
            //WorldGen.PlaceTile(x, y, TileID.Adamantite, false, true);

            SpawnCamp spawnCamp = new SpawnCamp();
            spawnCamp.Place(new Point((int)campPosition.X + 3, (int)campPosition.Y + 5), GenVars.structures);
        }
    }

    public class SpawnCamp : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            int x = origin.X;
            int y = origin.Y;

            Dictionary<Color, int> TileMapping = new Dictionary<Color, int>
            {
                [new Color(42, 100, 46)] = TileID.Grass,
                [new Color(71, 38, 28)] = TileID.Dirt,
            };

            Dictionary<Color, int> TileRemoval = new Dictionary<Color, int>
            {
                [new Color(0, 0, 0)] = -2
            };

            Texture2D ClearMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/GuideCamp_Clear", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TexGen TileClear = BaseWorldGenTex.GetTexGenerator(ClearMap, TileRemoval, ClearMap, TileRemoval);
            TileClear.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            Texture2D TileMap = ModContent.Request<Texture2D>(AssetDirectory.WorldGen + "Textures/GuideCamp", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            TexGen TileGen = BaseWorldGenTex.GetTexGenerator(TileMap, TileMapping, TileMap);
            TileGen.Generate(x - (TileClear.width / 2), y - (TileClear.height), true, true);

            //WorldGen.PlaceTile(x - (TileGen.width / 2), y - TileGen.height, TileID.Adamantite, false, true);

            Vector2 position = new Vector2(x - (TileGen.width / 2), y - TileGen.height);
            ModUtils.PlaceObject((int)(position.X + 20), (int)(position.Y + 5), ModContent.TileType<GuideCampfire>());
            ModContent.GetInstance<GuideCampfire_TE>().Place((int)(position.X + 19), (int)(position.Y + 4));

            ModUtils.PlaceTilePile<BowRock, BowRockObjects>((int)(position.X + 12), (int)(position.Y + 4));
            //WorldGen.PlaceTile((int)(position.X + 12), (int)(position.Y + 4), TileID.Adamantite, false, true);
            ModUtils.PlaceTilePile<GuideStool, GuideStoolObjects>((int)(position.X + 16), (int)(position.Y + 5));
            //WorldGen.PlaceTile((int)(position.X + 26), (int)(position.Y + 5), TileID.Adamantite, false, true);

            ModUtils.PlaceTilePile<GuideLog, GuideLogObjects>((int)(position.X + 26), (int)(position.Y + 4));
            ModUtils.PlaceTilePile<GuideTent, GuideTentObjects>((int)(position.X + 34), (int)(position.Y + 3));
            ModUtils.PlaceTilePile<BookRock, BookRockObjects>((int)(position.X + 39), (int)(position.Y + 3));
            ModUtils.PlaceTilePile<AxeStump, AxeStumpObjects>((int)(position.X + 42), (int)(position.Y + 3));

            structures.AddProtectedStructure(new Rectangle(origin.X - (TileClear.width / 2), origin.Y - (TileClear.height), TileClear.width, TileClear.height));

            return true;
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

            Vector2 end = origin.ToVector2() + new Vector2(0, 25);
            FeydenTunneler tunnel = new FeydenTunneler(origin.ToVector2(), end, noise, 3);
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
            //WorldGen.digTunnel((int)position.X, (int)position.Y, 0, 0, 1, size, false);

            Point shapePosition = new Point((int)position.X, (int)position.Y);
            float xScale = 0.8f + Main.rand.NextFloat() * 0.5f; // Randomize the width of the shrine area

            ShapeData slimeShapeData = new ShapeData();
            int scale = Main.rand.Next(5, 10);

            ushort tileType = Main.rand.NextBool() ? (ushort)ModContent.TileType<SlimedStone>() : TileID.Stone;

            WorldUtils.Gen(shapePosition, new Shapes.Slime(10, xScale, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));
            if (Main.rand.NextBool(3)) WorldUtils.Gen(shapePosition, new ModShapes.InnerOutline(slimeShapeData, true), Actions.Chain(new Modifiers.Blotches(scale, 0.65f), new Modifiers.IsSolid(), new Actions.SetTile(tileType, true)));

            GenerateSlimeRocks(shapePosition.ToVector2(), tileType == (ushort)ModContent.TileType<SlimedStone>());

            WorldUtils.Gen(shapePosition, new ModShapes.All(slimeShapeData), Actions.Chain(new Actions.PlaceWall(WallID.DirtUnsafe)));
        }

        public override void OnRunEnd(Vector2 position)
        {
            if (repeatWorm <= 1)
            {
                bool withinBounds = position.X > 0 && position.X < Main.maxTilesX && position.Y > 0 && position.Y < Main.maxTilesY;
                if (withinBounds)
                {
                    int repeat = 3;
                    for (int i = 0; i < repeat; i++)
                    {
                        float xScale = 0.8f + Main.rand.NextFloat() * 0.5f;
                        Point shapePosition = new Point((int)position.X + -15 * i, (int)position.Y + Main.rand.Next(5, 10));

                        int scale = Main.rand.Next(5, 10);

                        ShapeData slimeShapeData = new ShapeData();
                        WorldUtils.Gen(shapePosition, new Shapes.Slime(20, xScale, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(frameNeighbors: true).Output(slimeShapeData)));
                        WorldUtils.Gen(shapePosition, new ModShapes.InnerOutline(slimeShapeData, true), Actions.Chain(new Modifiers.Blotches(scale, 0.65f), new Modifiers.IsSolid(), new Actions.SetTile((ushort)ModContent.TileType<SlimedStone>(), true)));
                        WorldUtils.Gen(shapePosition, new ModShapes.All(slimeShapeData), Actions.Chain(new Actions.PlaceWall(WallID.DirtUnsafe), /*new Modifiers.Blotches(3, 0.65f), new Modifiers.Dither(.85),*/ new Actions.PlaceWall(WallID.Slime, true)));

                        GenerateSlimeRocks(shapePosition.ToVector2(), true);

                        if (i == repeat - 1)
                        {
                            GuideCamp.FeydenCavePosition = shapePosition.ToVector2() * 16;
                            NPC.NewNPC(null, shapePosition.X * 16, shapePosition.Y * 16, ModContent.NPCType<Feyden>());
                        }
                    }
                }
            }
            else
            {
                Vector2 branchEndpoint = position + new Vector2(250, 0); // Turn to the right
                if (repeatWorm == 3) branchEndpoint = position + new Vector2(0, 150).RotatedBy(MathHelper.ToRadians(70)); // Go diagonally

                FeydenTunneler tunnel = new FeydenTunneler(position, branchEndpoint, noise, --repeatWorm);
                tunnel.Run(out _);
            }

            base.OnRunEnd(position);
        }

        private void GenerateSlimeRocks(Vector2 shapePosition, bool isSlime)
        {
            for (int x = 0; x < Main.rand.Next(6, 8); x++)
            {
                Vector2 randomOffset = Vector2.UnitX * Main.rand.Next(-20, 20);
                Vector2 rockPosition = ModUtils.FindNearestGround(shapePosition + randomOffset, false);

                //var logger = OvermorrowModFile.Instance.Logger;
                //logger.Debug(rockPosition + " ?? " + tile.HasTile + " : " + tile.ToString());
                
                int variant = Main.rand.Next(1, 8);

                // Mod.Find and ModContent.Find won't work. This works so I don't give a fuck anymore.
                int type = variant switch
                {
                    1 => isSlime ? ModContent.TileType<SlimeRock1>() : ModContent.TileType<Rock1>(),
                    2 => isSlime ? ModContent.TileType<SlimeRock2>() : ModContent.TileType<Rock2>(),
                    3 => isSlime ? ModContent.TileType<SlimeRock3>() : ModContent.TileType<Rock3>(),
                    4 => isSlime ? ModContent.TileType<SlimeRock4>() : ModContent.TileType<Rock4>(),
                    5 => isSlime ? ModContent.TileType<SlimeRock5>() : ModContent.TileType<Rock5>(),
                    6 => isSlime ? ModContent.TileType<SlimeRock6>() : ModContent.TileType<Rock6>(),
                    7 => isSlime ? ModContent.TileType<SlimeRock7>() : ModContent.TileType<Rock7>(),
                    _ => ModContent.TileType<Rock1>()
                };


                WorldGen.PlaceTile((int)rockPosition.X, (int)rockPosition.Y - 1, type, true, false);
            }
        }
    }
}