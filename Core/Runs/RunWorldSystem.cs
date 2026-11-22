using OvermorrowMod.Core.WorldGeneration;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Runs
{
    public class RunWorldSystem : ModSystem
    {
        /// <summary>
        /// SubworldLibrary switches its current subworld before saving the world being left, so <see cref="RunState.InRun"/> already describes the destination during that save.
        /// </summary>
        public static bool RunWorldLoaded { get; private set; }

        public override void OnWorldLoad()
        {
            RunWorldLoaded = RunState.InRun;
            SubworldTimer.Reset();
            TestSubworld.GhostSpawned = false;
        }

        public override void OnWorldUnload()
        {
            RunSave.WaitForPendingSave();
            RunWorldLoaded = false;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (!RunWorldLoaded) return;

            tag["TimerTicks"] = SubworldTimer.RemainingTicks;
            tag["TimerRunning"] = SubworldTimer.Running;
            tag["TimerBonusCount"] = SubworldTimer.BonusCount;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (!tag.ContainsKey("TimerTicks")) return;

            SubworldTimer.Restore(tag.GetInt("TimerTicks"), tag.GetBool("TimerRunning"), tag.GetInt("TimerBonusCount"));
        }
    }
}
