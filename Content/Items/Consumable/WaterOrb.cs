using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable
{
    public class WaterOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Healing Water");
            ItemID.Sets.ItemNoGravity[item.type] = true;
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 28;
            item.maxStack = 999;
        }

        public override bool ItemSpace(Player player)
        {
            return true;
        }

        public override bool CanPickup(Player player)
        {
            return true;
        }

        public override bool OnPickup(Player player)
        {
            player.HealEffect(10);
            player.statLife += 10;
            return false;
        }
    }
}