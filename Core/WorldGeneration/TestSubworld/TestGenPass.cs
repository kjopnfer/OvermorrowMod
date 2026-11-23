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
        private readonly Func<GridRoom> _sidePortal;
        private readonly Func<GridRoom> _exitPortal;
        private readonly Func<DungeonContent> _contentFactory;

        public TestGenPass(string name, double loadWeight, Func<GridRoom> sidePortal, Func<GridRoom> exitPortal, Func<DungeonContent> contentFactory) : base(name, loadWeight)
        {
            _sidePortal = sidePortal;
            _exitPortal = exitPortal;
            _contentFactory = contentFactory;
        }

        /// <summary>
        /// Declares this subworld's dungeons: the root, the exit dungeon one step in the biome's onward direction,
        /// and a side dungeon east or west of the root with a diagonal door onward into the exit dungeon.
        /// </summary>
        public DungeonLayout CreateLayout(Random rand)
        {
            var layout = new DungeonLayout();
            var rootContent = _contentFactory();
            LayoutDirection onward = rootContent.OnwardDirection;
            LayoutDirection side = rand.Next(2) == 0 ? LayoutDirection.East : LayoutDirection.West;

            int root = layout.Add(rootContent);
            int exit = layout.Add(_contentFactory());
            int sideDungeon = layout.Add(_contentFactory());
            layout.Connect(root, onward, exit);
            layout.Connect(root, side, sideDungeon);
            layout.Connect(sideDungeon, LayoutDirections.FromDelta(side.Opposite().Delta() + onward.Delta()), exit, toAtSpineEnd: true);

            if (_exitPortal != null) layout.SetExit(exit, _exitPortal);
            if (_sidePortal != null) layout.AddRoom(sideDungeon, _sidePortal);
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
