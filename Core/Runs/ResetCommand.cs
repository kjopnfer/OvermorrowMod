using SubworldLibrary;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Runs
{
    public class ResetCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "reset";

        public override string Description => "Deletes the saved run";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            bool inSubworld = SubworldSystem.AnyActive();
            RunReset.Reset(caller.Player);
            caller.Reply(inSubworld ? "Leaving to reset the run" : "Run reset: saved biomes, run items, collectibles and rerolls cleared");
        }
    }
}
