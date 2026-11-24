using OvermorrowMod.Core.Runs;
using OvermorrowMod.Core.UI.RunResume;
using SubworldLibrary;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.UI.ClassSelection
{
    public class EnterCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "enter";

        public override string Description => "Opens the class picker to enter or regenerate a run";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!SubworldSystem.AnyActive() && caller.Player.GetModPlayer<RunSavePlayer>().HasSavedRun) RunResumeManager.Show();
            else ClassSelectionManager.Show();
        }
    }
}
