using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.Items.Testing
{
    public class XPosFind : ModItem
    {

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = 100;
            item.rare = ItemRarityID.Blue;
            item.useStyle = ItemUseStyleID.SwingThrow;
        }

        public override bool UseItem(Player player)
        {
            CombatText.NewText(player.getRect(), Color.LightBlue, (int)Math.Round(player.position.X));
                        return true;
        }
    }
}