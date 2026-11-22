using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Runs
{
    public class RunCountedItem : GlobalItem
    {
        public bool Counted;
        public bool FromKit;

        public override bool InstancePerEntity => true;

        public static void MarkKit(Item item)
        {
            if (!item.TryGetGlobalItem(out RunCountedItem counted)) return;

            counted.Counted = true;
            counted.FromKit = true;
        }

        public static bool IsKit(Item item) => item.TryGetGlobalItem(out RunCountedItem counted) && counted.FromKit;

        public override void SaveData(Item item, TagCompound tag)
        {
            if (Counted) tag["RunCounted"] = true;
            if (FromKit) tag["RunKit"] = true;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            Counted = tag.GetBool("RunCounted");
            FromKit = tag.GetBool("RunKit");
        }
    }
}
