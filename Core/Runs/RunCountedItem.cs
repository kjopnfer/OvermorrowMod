using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Runs
{
    public class RunCountedItem : GlobalItem
    {
        public bool Counted;

        public override bool InstancePerEntity => true;

        public static void Mark(Item item)
        {
            if (item.TryGetGlobalItem(out RunCountedItem counted)) counted.Counted = true;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            if (Counted) tag["RunCounted"] = true;
        }

        public override void LoadData(Item item, TagCompound tag) => Counted = tag.GetBool("RunCounted");
    }
}
