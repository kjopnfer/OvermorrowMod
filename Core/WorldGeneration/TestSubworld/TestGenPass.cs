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
        private readonly Func<GridRoom> _sideExit;
        private readonly Func<GridRoom> _downExit;
        private readonly Func<DungeonContent> _contentFactory;

        public TestGenPass(string name, double loadWeight, Func<GridRoom> sideExit, Func<GridRoom> downExit, Func<DungeonContent> contentFactory) : base(name, loadWeight)
        {
            _sideExit = sideExit;
            _downExit = downExit;
            _contentFactory = contentFactory;
        }

        /// <summary>
        /// Declares this subworld's dungeons: the root, a dungeon below it with an exit at its bottom, and a dungeon east or
        /// west of the root with an exit at its far end.
        /// </summary>
        public DungeonLayout CreateLayout(Random rand)
        {
            var layout = new DungeonLayout();
            LayoutDirection side = rand.Next(2) == 0 ? LayoutDirection.East : LayoutDirection.West;

            int root = layout.Add(_contentFactory());
            int below = layout.Add(_contentFactory());
            int beside = layout.Add(_contentFactory());
            layout.Connect(root, LayoutDirection.South, below);
            layout.Connect(root, side, beside);
            layout.SetZone(below, 1);
            layout.SetZone(beside, 2);

            if (_downExit != null) layout.AddExit(below, LayoutDirection.South, _downExit);
            if (_sideExit != null) layout.AddExit(beside, side, _sideExit);
            layout.AddRoom(root, () => new StartingRoom());
            layout.AddRoom(root, () => new ShopRoom());
            layout.SetRoot(root);
            return layout;
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
                CreateLayout(rand).Build(new Point(centerX, centerY), rand);

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
