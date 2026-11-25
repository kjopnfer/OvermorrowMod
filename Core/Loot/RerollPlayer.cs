using OvermorrowMod.Core.MetaUpgrades;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Loot
{
    public class RerollPlayer : ModPlayer
    {
        public int Rerolls;

        public bool TrySpend()
        {
            if (Rerolls <= 0) return false;
            Rerolls--;
            return true;
        }

        public void Reset() => Rerolls = Player.GetModPlayer<MetaUpgradePlayer>().Value(MetaUpgrade.SpareReroll);

        public override void SaveData(TagCompound tag) => tag["Rerolls"] = Rerolls;

        public override void LoadData(TagCompound tag) => Rerolls = tag.GetInt("Rerolls");
    }
}
