using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Other
{
    public class BrazilFlag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brazilian Flag");
            Tooltip.SetDefault("Usable transportation item");
        }
        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 64;
            item.rare = ItemRarityID.Green;
            item.rare = Item.sellPrice(gold: 1);
            item.consumable = true;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }
        public override bool UseItem(Player player)
        {
            Main.NewText("You're going to Brazil!");
            Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/Custom/Yeet"));
            player.velocity.Y = -50f;
            return true;
        }
    }
}
