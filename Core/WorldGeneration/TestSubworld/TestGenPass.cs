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
        private readonly string _portalTarget;

        public TestGenPass(string name, double loadWeight, string portalTarget) : base(name, loadWeight)
        {
            _portalTarget = portalTarget;
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
                int a = layout.Add(new ArchiveContent());
                int b = layout.Add(new ArchiveContent());
                int c = layout.Add(new ArchiveContent());
                // A's east endpoint is the door to B; C is reached by a fork that
                // descends below the spine. A's west endpoint stays the bookshelf spawn.
                layout.Connect(a, LayoutDirection.East, b);
                layout.Connect(a, LayoutDirection.SouthEast, c);
                // B and C each get an east-facing portal door into the linked subworld.
                layout.AddSubworldPortal(b, LayoutDirection.East, _portalTarget);
                layout.AddSubworldPortal(c, LayoutDirection.East, _portalTarget);
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
