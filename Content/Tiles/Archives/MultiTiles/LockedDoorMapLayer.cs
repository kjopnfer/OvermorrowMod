using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Archives;
using Terraria;
using Terraria.DataStructures;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.Tiles.Archives
{
    /// <summary>
    /// Marks every seen, still-locked <see cref="ArchiveDoor_TE"/> on the maps while the local player holds an <see cref="ArchiveKey"/>.
    /// </summary>
    public class LockedDoorMapLayer : ModMapLayer
    {
        public override void Draw(ref MapOverlayDrawContext context, ref string text)
        {
            if (!Main.LocalPlayer.HasItem(ModContent.ItemType<ArchiveKey>())) return;

            Texture2D icon = ModContent.Request<Texture2D>(ModContent.GetInstance<ArchiveKey>().Texture).Value;
            float pulse = 1.4f + 0.2f * (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 4f);
            foreach (var te in TileEntity.ByID.Values)
            {
                if (te is not ArchiveDoor_TE door || !door.IsLocked || !door.Seen) continue;
                context.Draw(icon, door.TileArea.Center.ToVector2(), Color.White, new SpriteFrame(1, 1), pulse, pulse + 0.4f, Alignment.Center);
            }
        }
    }
}
