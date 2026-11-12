using Terraria.ModLoader;

namespace OvermorrowMod.Core.UI.ClassSelection
{
    public class EnterCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "enter";

        public override string Description => "Opens the class picker to enter or regenerate a run";

        public override void Action(CommandCaller caller, string input, string[] args) => ClassSelectionManager.Show();
    }
}
