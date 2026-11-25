using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.UI.Guild;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.AdventurersGuild
{
    public class GuildWizardStatue : NormalWizardStatue
    {
        public override string Texture => AssetDirectory.ArchiveTiles + nameof(NormalWizardStatue);

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<MonkeyStoneGreen>();
        }

        public override bool RightClick(int i, int j)
        {
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            GuildStatueDialogue.Open(bottomLeft.ToWorldCoordinates());
            return true;
        }
    }
}
