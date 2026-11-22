using OvermorrowMod.Content.Misc;
using Terraria;
using Terraria.ID;
using Terraria.IO;

namespace OvermorrowMod.Core.Runs
{
    public static class RunSave
    {
        /// <summary>
        /// Saves the subworld and the player together so a crash restores a matching pair.
        /// </summary>
        public static void Checkpoint()
        {
            if (!RunWorldSystem.RunWorldLoaded || Main.netMode != NetmodeID.SinglePlayer || CombatOrchestrator.AnyFightRunning()) return;

            Main.LocalPlayer.GetModPlayer<RunSavePlayer>().PrepareCheckpoint();
            WorldFile.SaveWorld();
            Player.SavePlayer(Main.ActivePlayerFileData);
        }
    }
}
