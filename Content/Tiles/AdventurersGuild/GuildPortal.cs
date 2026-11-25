using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.WorldGeneration.Guild;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.AdventurersGuild
{
    public class GuildPortal : ModTile
    {
        public override string Texture => AssetDirectory.Empty;

        public override bool CanExplode(int i, int j) => false;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 15;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(1, 14);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(102, 251, 0));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.6f;
            b = 0.1f;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemID.MagicMirror;
        }

        public override bool RightClick(int i, int j)
        {
            GuildSubworld.EnterNextBiome(Main.LocalPlayer);
            return true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
            float pulse = 0.75f + 0.25f * MathF.Sin(Main.GlobalTimeWrappedHourly * 3f + j * 0.4f);
            Color color = new Color(102, 251, 0) * 0.6f * pulse;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)drawPos.X, (int)drawPos.Y, 16, 16), color);
            return false;
        }
    }
}
