using OvermorrowMod.Content.Dungeons.Inkwell;
using SubworldLibrary;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration
{
    public static class RunState
    {
        public static IEnumerable<Subworld> Subworlds => [ModContent.GetInstance<ArchiveSubworld.ArchiveSubworld>(), ModContent.GetInstance<TestSubworld.TestSubworld>(), ModContent.GetInstance<TestSubworld.TestSubworld2>(), ModContent.GetInstance<InkwellSubworld>()];

        public static bool InRun => SubworldSystem.Current != null && Subworlds.Contains(SubworldSystem.Current);
    }
}
