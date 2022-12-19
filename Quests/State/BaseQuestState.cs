using OvermorrowMod.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public class BaseQuestState : TagSerializable
    {
        public BaseQuest Quest { get; private set; }

        public BaseQuestState(BaseQuest quest, IEnumerable<BaseRequirementState> requirementStates)
        {
            RequirementStates = requirementStates.ToDictionary(req => req.Requirement.ID);
            Quest = quest;
        }

        public virtual TagCompound SerializeData()
        {
            var tag = new TagCompound
            {
                ["completed"] = Completed,
                ["questID"] = Quest.QuestID
            };
            foreach (var req in RequirementStates.Values)
            {
                tag[req.Requirement.ID] = req;
            }
            return tag;
        }

        public virtual void Load(TagCompound tag)
        {
            Completed = tag.GetBool("completed");
            foreach (var req in RequirementStates.Values)
            {
                var reqState = tag.GetCompound(req.Requirement.ID);
                if (reqState != null)
                {
                    req.Load(reqState);
                }
            }
        }

        public bool Completed { get; set; }
        public Dictionary<string, BaseRequirementState> RequirementStates { get; }

        public virtual void Reset()
        {
            Completed = false;
            foreach (var req in RequirementStates.Values)
            {
                req.Reset();
            }
        }

        public BaseRequirementState GetRequirementState(IQuestRequirement req)
        {
            return RequirementStates.GetValueOrDefault(req.ID);
        }
    }
}
