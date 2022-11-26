using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using Terraria.ModLoader.IO;
using Terraria.GameContent.Metadata;

namespace OvermorrowMod.Common.TilePiles
{
    internal class TilePiles : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool KillSound(int i, int j, bool fail) => false;
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);

            TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TilePile>().Hook_AfterPlacement, -1, 0, true);

            MinPick = 55;

            TileObjectData.addTile(Type);
        }

        // Used for debugging, set to true to draw the tile matrix for the associated tile
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool activeObjects = false;

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX != 0 || tile.TileFrameY != 0) return;

            TilePile pile = FindTE(i, j);

            if (pile == null) return;

            //foreach (TileObject tileObject in OvermorrowModSystem.Instance.tilePiles[GetTilePileIndex(i, j)]?.PileContents)
            foreach (TileObject tileObject in pile.PileContents)
            {
                if (tileObject.dependency >= 0)
                {
                    if (!pile.PileContents[tileObject.dependency].active)
                    {
                        if (tileObject.active)
                        {
                            tileObject.active = false;
                            Item.NewItem(new EntitySource_Misc("TilePileLoot"), tileObject.rectangle, tileObject.ID, tileObject.GetRandomStack());
                            //play breaking sound
                        }
                    }
                }

                if (!tileObject.active) continue;

                Rectangle rect = tileObject.rectangle;
                Vector2 pos = new Vector2(rect.X, rect.Y) - Main.screenPosition;
                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
                Vector2 wiggleOffset = Vector2.Zero;
                float wiggleRotation = 0f;

                if (tileObject.selected)
                {
                    if (tileObject.wiggleTimer++ < 20 && tileObject.wiggleTimer > 3)
                    {
                        wiggleOffset = new Vector2((float)Math.Sin(tileObject.wiggleTimer * 4), -1f);
                        wiggleRotation = (float)Math.Sin(tileObject.wiggleTimer * 2) / 10f;
                    }

                    if (tileObject.wiggleTimer > 60) tileObject.wiggleTimer = 0;

                    if (Main.LocalPlayer.HeldItem.pick > 0 && Main.LocalPlayer.itemAnimation > 0 && tileObject.interactType == (int)TileObject.InteractionType.Mine)
                    {
                        Main.NewText("trying to mine the thing");

                        if (Main.rand.NextBool(180))
                        {
                            int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, DustID.Dirt, 0f, 0f, 254, Color.White, 0.5f);
                            Main.dust[d].velocity *= 0f;
                        }
                    }

                    if (Main.LocalPlayer.HeldItem.axe > 0 && Main.LocalPlayer.itemAnimation > 0 && tileObject.interactType == (int)TileObject.InteractionType.Chop)
                    {
                        Main.NewText("trying to chop the thing");

                        if (Main.rand.NextBool(180))
                        {
                            int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, DustID.Dirt, 0f, 0f, 254, Color.White, 0.5f);
                            Main.dust[d].velocity *= 0f;
                        }
                    }
                }
                else
                {
                    tileObject.wiggleTimer += 2;
                }

                rect.X = (int)(pos.X + zero.X + wiggleOffset.X) + rect.Width / 2;
                rect.Y = (int)(pos.Y + zero.Y + wiggleOffset.Y) + rect.Height / 2;

                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                Color hoverColor = tileObject.selected ? Color.Yellow : Lighting.GetColor(i, j);

                spriteBatch.Draw(tileObject.texture, pos + offScreenRange, null, hoverColor, wiggleRotation, Vector2.Zero, 1f, SpriteEffects.None, 0);

                if (Main.rand.NextBool(180))
                {
                    //int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, DustID.TintableDustLighted, 0f, 0f, 254, Color.White, 0.5f);
                    //Main.dust[d].velocity *= 0f;
                }

                tileObject.selected = false;
                activeObjects = true;

            }

            if (!activeObjects) WorldGen.KillTile(i, j);

        }

        public override void MouseOver(int i, int j)
        {
            TilePile pile = FindTE(i, j);

            if (pile == null) return;

            foreach (TileObject tileObject in pile.PileContents)
            {
                if (!tileObject.active) continue;

                if (Main.MouseWorld.Between(tileObject.rectangle.TopLeft(), tileObject.rectangle.BottomRight()))
                {
                    tileObject.selected = true;

                    switch (tileObject.interactType)
                    {
                        case (int)TileObject.InteractionType.Click:
                            Main.instance.MouseText($"Take {tileObject.name}");
                            break;
                        case (int)TileObject.InteractionType.Chop:
                            Main.instance.MouseText($"Chop {tileObject.name}");
                            break;
                        case (int)TileObject.InteractionType.Mine:
                            Main.instance.MouseText($"Mine {tileObject.name}");
                            break;
                    }
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            TilePile pile = FindTE(i, j);

            if (pile != null)
            {
                foreach (TileObject tileObject in pile.PileContents)
                {
                    if (Main.MouseWorld.Between(tileObject.rectangle.TopLeft(), tileObject.rectangle.BottomRight()) && tileObject.active)
                    {
                        tileObject.active = false;
                        Item.NewItem(new EntitySource_Misc("TilePileLoot"), tileObject.rectangle, tileObject.ID, tileObject.GetRandomStack());
                        //play grab sound
                        break;
                    }
                }
            }

            return true;
        }

        public static TilePile FindTE(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            int index = ModContent.GetInstance<TilePile>().Find(left, top);
            if (index == -1)
            {
                return null;
            }

            TilePile entity = (TilePile)TileEntity.ByID[index];
            return entity;

        }
    }

    public class TilePile : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x3;

        public override void CreateTilePile()
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    PileContents = new TileObject[2];
                    PileContents[0] = new TileObject("BookStack_S3", position, 16, 38, -1, (int)TileObject.InteractionType.Click);
                    PileContents[1] = new TileObject("BookStack_S2", position, 18, 28, 0, (int)TileObject.InteractionType.Chop);
                    break;
                case 1:
                    PileContents = new TileObject[4];
                    PileContents[0] = new TileObject("Crate_S", position, 4, 32, -1, (int)TileObject.InteractionType.Click);
                    PileContents[1] = new TileObject("Crate_M", position, 18, 28, -1, (int)TileObject.InteractionType.Chop);
                    PileContents[2] = new TileObject("Cloth_L", position, 18, 26, 1, (int)TileObject.InteractionType.Mine);
                    PileContents[3] = new TileObject("BookStack_S3", position, 24, 14, 2, (int)TileObject.InteractionType.Click);
                    break;
                case 2:
                    PileContents = new TileObject[4];
                    PileContents[0] = new TileObject("Crate_S", position, 18, 32, -1, (int)TileObject.InteractionType.Click);
                    PileContents[1] = new TileObject("Cloth_S", position, 18, 30, 0, (int)TileObject.InteractionType.Mine);
                    PileContents[2] = new TileObject("BookStack_S3", position, 22, 18, 1, (int)TileObject.InteractionType.Click);
                    PileContents[3] = new TileObject("Sack_S", position, 4, 30, -1, (int)TileObject.InteractionType.Chop);
                    break;
                case 3:
                    PileContents = new TileObject[3];
                    PileContents[0] = new TileObject("Crate_S", position, 6, 32, -1, (int)TileObject.InteractionType.Click);
                    PileContents[1] = new TileObject("Crate_S", position, 8, 14, 0, (int)TileObject.InteractionType.Mine);
                    PileContents[2] = new TileObject("Backpack_Sr", position, 26, 34, -1, (int)TileObject.InteractionType.Chop);
                    break;
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            TilePile te = ByID[id] as TilePile;
            te.SetPosition(new Vector2(i + 1, j + 2)); // TODO: The origin is in the top left corner, change to scale based on Style
            te.CreateTilePile();

            return id;
        }
    }
}
