using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Dungeons.Archive.Cells;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using System;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace OvermorrowMod.Core.WorldGeneration.TestSubworld
{
    public class TestGenPass : GenPass
    {
        private readonly Func<GridRoom> _portalB;
        private readonly Func<GridRoom> _portalC;
        private readonly Func<DungeonContent> _contentFactory;

        public TestGenPass(string name, double loadWeight, Func<GridRoom> portalB, Func<GridRoom> portalC, Func<DungeonContent> contentFactory) : base(name, loadWeight)
        {
            _portalB = portalB;
            _portalC = portalC;
            _contentFactory = contentFactory;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating test world";

            Main.worldSurface = new TestSubworld().Height - 200;
            Main.rockLayer = new TestSubworld().Height;

            int centerX = new TestSubworld().Width / 2;
            int centerY = new TestSubworld().Height / 2;

            var rand = new Random(Environment.TickCount);

            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                var layout = new DungeonLayout();
                int a = layout.Add(_contentFactory());
                int b = layout.Add(_contentFactory());
                int c = layout.Add(_contentFactory());
                // Branch B and C off A in two random distinct directions. East/West become spine
                // endpoint doors; the other six become vertical forks.
                var dirs = (LayoutDirection[])Enum.GetValues(typeof(LayoutDirection));
                int d1 = rand.Next(dirs.Length);
                int d2 = rand.Next(dirs.Length - 1);
                if (d2 >= d1) d2++;
                layout.Connect(a, dirs[d1], b);
                layout.Connect(a, dirs[d2], c);
                if (_portalB != null) layout.AddRoom(b, _portalB);
                if (_portalC != null) layout.AddRoom(c, _portalC);
                // Only the root dungeon (the one the player spawns in) gets a StartingRoom.
                layout.AddRoom(a, () => new StartingRoom());
                layout.AddRoom(a, () => new ShopRoom());
                layout.SetRoot(a);
                layout.Build(new Point(centerX, centerY), rand);

                sw.Stop();
                Terraria.ModLoader.Logging.PublicLogger.Info($"OvermorrowDungeon layout built in {sw.ElapsedMilliseconds} ms");
            }
            finally
            {
                Common.TextureMapping.TexGen.ClearCache();
            }
        }
    }
}
