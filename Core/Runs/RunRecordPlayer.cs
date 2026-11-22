using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.WorldGeneration;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Runs
{
    public enum KillTier { Enemy, Elite, Miniboss, Boss }

    public class RunRecordPlayer : ModPlayer
    {
        public int RunTicks;
        public int RoomsCleared;
        public readonly int[] KillsByTier = new int[Enum.GetValues<KillTier>().Length];
        public readonly int[] ItemsByRarity = new int[Enum.GetValues<Rarity>().Length];

        public void Clear()
        {
            RunTicks = 0;
            RoomsCleared = 0;
            Array.Clear(KillsByTier);
            Array.Clear(ItemsByRarity);
        }

        public void RecordKill(NPC npc)
        {
            KillTier? tier = npc.ModNPC is ITieredEnemy tiered ? tiered.Tier switch
            {
                EnemyTier.Common => KillTier.Enemy,
                EnemyTier.Elite => KillTier.Elite,
                EnemyTier.Miniboss => KillTier.Miniboss,
                EnemyTier.Boss => KillTier.Boss,
                _ => null,
            } : npc.boss ? KillTier.Boss : KillTier.Enemy;

            if (tier is KillTier kill) KillsByTier[(int)kill]++;
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || !RunState.InRun) return;

            RunTicks++;

            foreach (Item[] slots in new[] { Player.inventory, Player.armor, Player.miscEquips })
            {
                foreach (Item item in slots) CountItem(item);
            }
            CountItem(Main.mouseItem);
        }

        private void CountItem(Item item)
        {
            if (item.IsAir || !item.TryGetGlobalItem(out RunCountedItem counted) || counted.Counted || !LootMetadata.TryGetAny(item.type, out LootMetadataEntry meta)) return;

            counted.Counted = true;
            ItemsByRarity[(int)meta.Rarity]++;
        }

        public override void SaveData(TagCompound tag)
        {
            tag["RunTicks"] = RunTicks;
            tag["RoomsCleared"] = RoomsCleared;
            tag["KillsByTier"] = KillsByTier;
            tag["ItemsByRarity"] = ItemsByRarity;
        }

        public override void LoadData(TagCompound tag)
        {
            RunTicks = tag.GetInt("RunTicks");
            RoomsCleared = tag.GetInt("RoomsCleared");
            tag.GetIntArray("KillsByTier").AsSpan().CopyTo(KillsByTier);
            tag.GetIntArray("ItemsByRarity").AsSpan().CopyTo(ItemsByRarity);
        }
    }
}
