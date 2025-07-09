using OvermorrowMod.Common.Weapons.Bows;
using OvermorrowMod.Core.Items.Bows;
using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    public interface IBowModifier
    {
        void ModifyBowStats(BowStats stats, Player player);
        void OnPowerShot(HeldBow bow, Player player);
        void OnArrowFired(HeldBow bow, Player player, Projectile arrow);
    }
}