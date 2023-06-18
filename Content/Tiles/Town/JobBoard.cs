using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.UI.JobBoard;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class JobBoard : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(2, 3);

            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };

            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Job Board");
            AddMapEntry(new Color(37, 37, 37), name);
        }

        bool isHovering = false;
        public override void MouseOver(int i, int j)
        {
            isHovering = true;
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
        }

        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen, new Vector2(i * 16, j * 16));
            UIJobBoardSystem.Instance.BoardState.showBoard = true;
            return true;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            isHovering = false;
            base.AnimateTile(ref frame, ref frameCounter);
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Content/Tiles/Town/JobBoard_Highlight").Value;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

                Rectangle boardHitbox = new Rectangle(i * 16, j * 16, 16 * 7, 16 * 4);
                if (isHovering)
                {
                    Vector2 offset = new Vector2(-2, -2);
                    Main.spriteBatch.Draw(texture, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero + offset, null, Color.PaleGoldenrod * 0.75f, 0f, default, 1f, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(i, j, spriteBatch);
        }
    }
}