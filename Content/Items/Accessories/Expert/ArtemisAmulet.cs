using OvermorrowMod.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories.Expert
{
    public class ArtemisAmulet : ModItem
    {
        //public override string Texture => "Terraria/Item_" + ItemID.CharmofMyths;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Artemis's Amulet");
            Tooltip.SetDefault("Summons Artemis's arrow rune to shoot arrows to the sky \n Press idk");
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = 17500;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OvermorrowModPlayer>().ArtemisAmulet = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine tooltip in tooltips)
            {
                if (tooltip.mod == "Terraria" && tooltip.Name == "Tooltip1")
                {
                    string bind = "";
                    var keys = OvermorrowModFile.AmuletKey.GetAssignedKeys();
                    foreach (string key in keys)
                    {
                        bind = key;
                    }
                    tooltip.text = $"Press [{bind}] to summon the rune";
                }
            }
        }
    }
}