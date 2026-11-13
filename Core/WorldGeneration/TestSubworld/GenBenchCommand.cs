using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.TestSubworld
{
    public class GenBenchCommand : ModCommand
    {
        private static int _running;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "genbench";

        public override string Usage => "/genbench [count] [seed] [validate] [sequential]";

        public override string Description => "Plans dungeon layouts in the background and writes a timing report to OvermorrowDungeonDumps";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            int count = args.Length > 0 && int.TryParse(args[0], out int c) ? Math.Clamp(c, 1, GenBench.MaxCount) : GenBench.DefaultCount;
            int seed = args.Length > 1 && int.TryParse(args[1], out int s) ? s : Environment.TickCount;
            var options = new PlanOptions { ValidateReachability = args.Contains("validate"), Sequential = args.Contains("sequential") };

            if (Interlocked.Exchange(ref _running, 1) == 1)
            {
                caller.Reply("A genbench run is already in progress.");
                return;
            }

            caller.Reply($"genbench: planning {count} layouts from seed {seed}.");
            Task.Run(() =>
            {
                try
                {
                    var result = GenBench.Run(count, seed, options);
                    string folder = Path.Combine(Main.SavePath, "OvermorrowDungeonDumps");
                    Directory.CreateDirectory(folder);
                    string fileName = $"genbench_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    File.WriteAllText(Path.Combine(folder, fileName), result.Report);
                    Main.QueueMainThreadAction(() => Main.NewText($"genbench: average {result.AverageMs} ms, worst {result.WorstMs} ms. Report: {fileName}"));
                }
                catch (Exception ex)
                {
                    Mod.Logger.Error("genbench failed", ex);
                    Main.QueueMainThreadAction(() => Main.NewText($"genbench failed: {ex.Message}", Color.Red));
                }
                finally
                {
                    Volatile.Write(ref _running, 0);
                }
            });
        }
    }
}
