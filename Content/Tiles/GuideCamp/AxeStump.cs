using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common.TilePiles;

using static OvermorrowMod.Common.TilePiles.TileObjects;
using OvermorrowMod.Core;
using OvermorrowMod.Content.Tiles.GuideCamp.TileObjects;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Quests;
using System;

namespace OvermorrowMod.Content.Tiles.GuideCamp
{
    public class AxeStump : ModTilePile<AxeStumpObjects>
    {
        public override BaseTilePile.TileStyle GridStyle => BaseTilePile.TileStyle.Style2x2;

        int offsetCounter = 0;
        int markerFrame = 2;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            offsetCounter = frameCounter;
            if (offsetCounter % 24 == 0) markerFrame = markerFrame == 2 ? 3 : 2;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
            {

                if (questPlayer.FindActiveQuest("GuideCampfire") && !questPlayer.grabbedAxe)
                {
                    Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                    Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                    float yOffset = MathHelper.Lerp(8, 12, (float)Math.Sin(offsetCounter / 24f));


                    Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Quests/QuestAlert").Value;
                    Rectangle drawRectangle = new Rectangle(0, texture.Height / 6 * markerFrame, texture.Width, texture.Height / 6);

                    spriteBatch.Draw(
                        texture,
                        new Vector2(drawPos.X + 16, drawPos.Y - yOffset),
                        drawRectangle,
                        Color.White,
                        0f,
                        new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                        1f,
                        SpriteEffects.None,
                        0f);
                }
            }

            return base.PreDraw(i, j, spriteBatch);
        }
    }

    public class AxeStumpObjects : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style2x2;

        public override void CreateTilePile()
        {
            PileContents = new TileInfo[2];
            PileContents[0] = new TileInfo(ObjectType<Axe>(), position, 10, 22, 1, (int)TileInfo.InteractionType.Click);
            PileContents[1] = new TileInfo(ObjectType<Stump>(), position, 12, 38, -1, (int)TileInfo.InteractionType.Chop);
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            AxeStumpObjects te = ByID[id] as AxeStumpObjects;
            te.SetPosition(new Vector2(i, j));
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<AxeStump>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<AxeStump>();
        }
    }
}