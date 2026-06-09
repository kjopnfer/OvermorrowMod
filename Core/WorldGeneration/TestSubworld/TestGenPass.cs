using Microsoft.Xna.Framework;
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
                // A's east endpoint is the door to B; C is reached by a fork that
                // descends below the spine. A's west endpoint stays the bookshelf spawn.
                layout.Connect(a, LayoutDirection.East, b);
                layout.Connect(a, LayoutDirection.SouthEast, c);
                if (_portalB != null) layout.AddRoom(b, _portalB);
                if (_portalC != null) layout.AddRoom(c, _portalC);
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
