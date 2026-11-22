using OvermorrowMod.Content.Misc;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.IO;

namespace OvermorrowMod.Core.Runs
{
    public static class RunSave
    {
        private static Task pendingSave = Task.CompletedTask;

        /// <summary>
        /// Saves the subworld and the player together in the background so a crash restores a matching pair.
        /// </summary>
        public static void Checkpoint()
        {
            if (!RunWorldSystem.RunWorldLoaded || Main.netMode != NetmodeID.SinglePlayer || RunDeathPlayer.LocalDead || CombatOrchestrator.AnyFightRunning()) return;

            Main.LocalPlayer.GetModPlayer<RunSavePlayer>().PrepareCheckpoint();
            pendingSave = pendingSave.ContinueWith(_ =>
            {
                WorldFile.SaveWorld();
                Player.SavePlayer(Main.ActivePlayerFileData);
            });
        }

        /// <summary>
        /// <see cref="WorldFile.SaveWorld()"/> returns without saving while another save holds its lock, and SubworldLibrary clears the world right after its exit save.
        /// Call before leaving a run world so the exit save runs and the world isn't cleared under a checkpoint still writing.
        /// </summary>
        public static void WaitForPendingSave() => Task.WaitAny(pendingSave);
    }
}
