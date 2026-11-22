using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.WorldGeneration;
using SubworldLibrary;
using System.IO;
using Terraria;

namespace OvermorrowMod.Core.Runs
{
    public static class RunReset
    {
        private static readonly string[] Extensions = [".wld", ".wld.bak", ".twld", ".twld.bak"];

        /// <summary>
        /// From inside a subworld this exits first and finishes once back in the main world, since a loaded world's file can't be deleted.
        /// </summary>
        public static void Reset(Player player)
        {
            if (SubworldSystem.AnyActive())
            {
                player.GetModPlayer<RunSavePlayer>().PendingReset = true;
                SubworldSystem.Exit();
                return;
            }

            string folder = Path.Combine(Main.WorldPath, Main.ActiveWorldFileData.UniqueId.ToString());
            if (Directory.Exists(folder))
            {
                foreach (Subworld subworld in RunState.Subworlds)
                {
                    foreach (string extension in Extensions)
                        File.Delete(Path.Combine(folder, subworld.FileName + extension));
                }
            }

            player.GetModPlayer<SubworldPlayer>().ClearRunInventory();
            player.GetModPlayer<CollectiblePlayer>().Reset();
            player.GetModPlayer<RerollPlayer>().Reset();
            player.GetModPlayer<RunSavePlayer>().ClearRun();
            player.GetModPlayer<MonkeyStonePlayer>().ClearRun();
            player.GetModPlayer<RunRecordPlayer>().Clear();
        }
    }
}
