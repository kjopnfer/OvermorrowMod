using OvermorrowMod.Content.Dungeons.Inkwell;
using SubworldLibrary;

namespace OvermorrowMod.Core.WorldGeneration
{
    public static class RunState
    {
        /// <summary>
        /// True while the player is inside a roguelike run subworld.
        /// </summary>
        public static bool InRun => SubworldSystem.IsActive<ArchiveSubworld.ArchiveSubworld>() || SubworldSystem.IsActive<TestSubworld.TestSubworld>() || SubworldSystem.IsActive<TestSubworld.TestSubworld2>() || SubworldSystem.IsActive<InkwellSubworld>();
    }
}
