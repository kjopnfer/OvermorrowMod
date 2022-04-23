using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests
{
    public class QuestWorld : ModSystem
    {
        public override void PreUpdateNPCs()
        {
            if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.talkNPC == -1) Quests.ResetUI();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["globalCompletedQuests"] = Quests.GlobalCompletedQuests.ToList();
            tag["perPlayerCompletedQuestsKeys"] = Quests.PerPlayerCompletedQuests.Keys.ToList();
            tag["perPlayerCompletedQuestsValues"] = Quests.PerPlayerCompletedQuests.Values.Select(v => v.ToList()).ToList();
            tag["perPlayerActiveQuestsKeys"] = Quests.PerPlayerActiveQuests.Keys.ToList();
            tag["perPlayerActiveQuestsValues"] = Quests.PerPlayerActiveQuests.Values
                    .Select(v => v.Select(q => q.QuestID).ToList()).ToList();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Quests.GlobalCompletedQuests.Clear();
            var quests = tag.GetList<string>("globalCompletedQuests");

            var validGlobals = quests.Where(qid => Quests.QuestList.TryGetValue(qid, out var quest)
                && quest.Repeatability == QuestRepeatability.OncePerWorld);
            foreach (var q in validGlobals) Quests.GlobalCompletedQuests.Add(q);

            foreach (var kvp in Quests.PerPlayerCompletedQuests) kvp.Value.Clear();

            var keys = tag.GetList<string>("perPlayerCompletedQuestsKeys");
            var values = tag.GetList<List<string>>("perPlayerCompletedQuestsValues");
            foreach (var pair in keys.Zip(values, (k, v) => (k, v)))
            {
                var valid = pair.v.Where(qid => Quests.QuestList.TryGetValue(qid, out var quest)
                    && quest.Repeatability == QuestRepeatability.OncePerWorldPerPlayer);

                Quests.PerPlayerCompletedQuests[pair.k] = new HashSet<string>(valid);
            }

            foreach (var kvp in Quests.PerPlayerActiveQuests) kvp.Value.Clear();

            var activeKeys = tag.GetList<string>("perPlayerActiveQuestsKeys");
            var activeValues = tag.GetList<List<string>>("perPlayerActiveQuestsValues");
            foreach (var pair in activeKeys.Zip(activeValues, (k, v) => (k, v)))
            {
                var qlist = pair.v.Select(qid => Quests.QuestList.TryGetValue(qid, out var quest) ? quest : null)
                    .Where(q => q != null
                        && (q.Repeatability == QuestRepeatability.OncePerWorld
                        || q.Repeatability == QuestRepeatability.OncePerWorldPerPlayer))
                    .ToList();
                Quests.PerPlayerActiveQuests[pair.k] = qlist;
            }

        }
    }
}
