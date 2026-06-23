using OvermorrowMod.Core.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Detours
{
    public class RewardPauseOverride : ILoadable
    {
        public void Load(Mod mod)
        {
            if (Main.dedServ) return;
            Terraria.On_Main.CanPauseGame += ForcePauseForRewards;
        }

        public void Unload() { }

        private bool ForcePauseForRewards(Terraria.On_Main.orig_CanPauseGame orig)
        {
            if (RewardSelection.IsInteractive && Main.netMode == NetmodeID.SinglePlayer) return true;
            return orig();
        }
    }
}
