using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.MetaUpgrades
{
    public class MetaUpgradePlayer : ModPlayer
    {
        private readonly Dictionary<string, int> ranks = new();

        public int Rank(MetaUpgrade upgrade) => Math.Min(ranks.GetValueOrDefault(upgrade.Name), upgrade.MaxRank);

        public int Value(MetaUpgrade upgrade) => upgrade.Values[Rank(upgrade)];

        public bool IsMaxed(MetaUpgrade upgrade) => Rank(upgrade) >= upgrade.MaxRank;

        public StoneCost[] NextCost(MetaUpgrade upgrade) => IsMaxed(upgrade) ? null : upgrade.Costs[Rank(upgrade)];

        public bool CanAfford(MetaUpgrade upgrade)
        {
            StoneCost[] cost = NextCost(upgrade);
            MonkeyStonePlayer wallet = Player.GetModPlayer<MonkeyStonePlayer>();
            return cost != null && cost.All(stone => wallet.Get(stone.Tier) >= stone.Amount);
        }

        public bool TryBuy(MetaUpgrade upgrade)
        {
            if (!CanAfford(upgrade)) return false;

            MonkeyStonePlayer wallet = Player.GetModPlayer<MonkeyStonePlayer>();
            foreach (StoneCost stone in NextCost(upgrade))
                wallet.TrySpend(stone.Tier, stone.Amount);

            ranks[upgrade.Name] = Rank(upgrade) + 1;
            return true;
        }

        public void SetRank(MetaUpgrade upgrade, int rank) => ranks[upgrade.Name] = Math.Clamp(rank, 0, upgrade.MaxRank);

        public override void PostUpdateEquips()
        {
            if (RunState.InRun)
                Player.potionDelayTime = Math.Min(Player.potionDelayTime, ModUtils.SecondsToTicks(Value(MetaUpgrade.QuickRecovery)));
        }

        public override void SaveData(TagCompound tag)
        {
            tag["MetaUpgradeNames"] = ranks.Keys.ToList();
            tag["MetaUpgradeRanks"] = ranks.Values.ToList();
        }

        public override void LoadData(TagCompound tag)
        {
            ranks.Clear();
            IList<string> names = tag.GetList<string>("MetaUpgradeNames");
            IList<int> values = tag.GetList<int>("MetaUpgradeRanks");
            for (int i = 0; i < names.Count; i++)
                ranks[names[i]] = values[i];
        }
    }
}
