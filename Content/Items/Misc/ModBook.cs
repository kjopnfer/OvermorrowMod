using OvermorrowMod.Content.Tiles.Ambient;
using OvermorrowMod.Content.UI.ReadableBook;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    public class ModBook : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mod Book Test");
            Tooltip.SetDefault("Click while in hand to read");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            foreach (TooltipLine line in tooltips)
            {
                if (line.Mod == "Terraria" && line.Name == "Tooltip0")
                {
                    line.Text = "[c/808080:Click while in hand to read]";
                }
            }
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 40;
            Item.rare = ItemRarityID.Green;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer && !UIReadableBookSystem.Instance.BookState.showBook)
            {
                UIReadableBookSystem.Instance.BookState.ShowBook(new TestBook());

                // set book ui to open
            }

            return base.UseItem(player);
        }
    }
}