using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Loot
{
    public class RerollPlayer : ModPlayer
    {
        public const int StartingRerolls = 1;

        public int Rerolls;

        public bool TrySpend()
        {
            if (Rerolls <= 0) return false;
            Rerolls--;
            return true;
        }

        public void Reset() => Rerolls = StartingRerolls;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) => Reset();

        public override void SaveData(TagCompound tag) => tag["Rerolls"] = Rerolls;

        public override void LoadData(TagCompound tag) => Rerolls = tag.GetInt("Rerolls");
    }
}
