using OvermorrowMod.Content.Dungeons.Inkwell;
using SubworldLibrary;

namespace OvermorrowMod.Core.WorldGeneration
{
    public static class RunState
    {
        public static bool InRun => SubworldSystem.IsActive<ArchiveSubworld.ArchiveSubworld>() || SubworldSystem.IsActive<TestSubworld.TestSubworld>() || SubworldSystem.IsActive<TestSubworld.TestSubworld2>() || SubworldSystem.IsActive<InkwellSubworld>();
    }
}
