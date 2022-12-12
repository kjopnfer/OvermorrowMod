using OvermorrowMod.Core.Interfaces;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Quests.State
{
    public abstract class BaseRequirementState : TagSerializable
    {
        public IQuestRequirement Requirement { get; }

        public BaseRequirementState(IQuestRequirement requirement)
        {
            Requirement = requirement;
        }

        public abstract void Load(TagCompound tag);

        public abstract TagCompound SerializeData();

        public virtual void Reset()
        {
        }
    }

    public class CompletableRequirementState : BaseRequirementState
    {
        public bool IsCompleted { get; set; }

        public CompletableRequirementState(IQuestRequirement requirement) : base(requirement)
        {
        }

        public override void Load(TagCompound tag)
        {
            IsCompleted = tag.GetBool("isCompleted");
        }

        public override TagCompound SerializeData()
        {
            return new TagCompound
            {
                ["isCompleted"] = IsCompleted
            };
        }

        public override void Reset()
        {
            IsCompleted = false;
        }
    }
}
