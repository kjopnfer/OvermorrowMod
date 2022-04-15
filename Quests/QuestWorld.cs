using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests
{
    public class QuestWorld : ModWorld
    {
        public override void PreUpdate()
        {
            if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.talkNPC == -1) Quests.ResetUi();
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                ["globalCompletedQuests"] = Quests.GlobalCompletedQuests.ToList()
            };
        }

        public override void Load(TagCompound tag)
        {
            Quests.GlobalCompletedQuests.Clear();
            var quests = tag.GetList<string>("globalCompletedQuests");
            foreach (var q in quests) Quests.GlobalCompletedQuests.Add(q);
        }
    }
}
