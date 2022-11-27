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
    // Base template for the physical tile piles, would probably be inherited by classes like ModTilePile_3x3 or ModTilePile_2x2 or something
    public abstract class ModTilePile<TEntity> : ModTile where TEntity : BaseTilePile
    {
        public override bool CanExplode(int i, int j) => false;
        public override bool CreateDust(int i, int j, ref int type) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override bool KillSound(int i, int j, bool fail) => false;
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); // Probably should be changeable within the child
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BasicLoot>().Hook_AfterPlacement, -1, 0, true);

            MinPick = 55; // debugging
            TileObjectData.addTile(Type);
        }

        // Used for debugging, set to true to draw the tile matrix for the associated tile
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            bool activeObjects = false;

            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX != 0 || tile.TileFrameY != 0) return;

            TEntity pile = FindTE(i, j);

            if (pile == null) return;

            //foreach (TileObject tileObject in OvermorrowModSystem.Instance.tilePiles[GetTilePileIndex(i, j)]?.PileContents)
            foreach (TileInfo tileObject in pile.PileContents)
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

                    if (Main.LocalPlayer.HeldItem.pick > 0 && Main.LocalPlayer.itemAnimation > 0 && tileObject.interactType == (int)TileInfo.InteractionType.Mine)
                    {
                        Main.NewText("trying to mine the thing");

                        if (Main.rand.NextBool(180))
                        {
                            int d = Dust.NewDust(new Vector2(tileObject.rectangle.X, tileObject.rectangle.Y), rect.Width, rect.Height, DustID.Dirt, 0f, 0f, 254, Color.White, 0.5f);
                            Main.dust[d].velocity *= 0f;
                        }
                    }

                    if (Main.LocalPlayer.HeldItem.axe > 0 && Main.LocalPlayer.itemAnimation > 0 && tileObject.interactType == (int)TileInfo.InteractionType.Chop)
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
            TEntity pile = FindTE(i, j);

            if (pile == null) return;

            foreach (TileInfo tileObject in pile.PileContents)
            {
                if (!tileObject.active) continue;

                if (Main.MouseWorld.Between(tileObject.rectangle.TopLeft(), tileObject.rectangle.BottomRight()))
                {
                    tileObject.selected = true;

                    switch (tileObject.interactType)
                    {
                        case (int)TileInfo.InteractionType.Click:
                            Main.instance.MouseText($"Take {tileObject.name}");
                            break;
                        case (int)TileInfo.InteractionType.Chop:
                            Main.instance.MouseText($"Chop {tileObject.name}");
                            break;
                        case (int)TileInfo.InteractionType.Mine:
                            Main.instance.MouseText($"Mine {tileObject.name}");
                            break;
                    }
                }
            }
        }

        public override bool RightClick(int i, int j)
        {
            TEntity pile = FindTE(i, j);

            if (pile != null)
            {
                foreach (TileInfo tileObject in pile.PileContents)
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

        public static TEntity FindTE(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            int index = ModContent.GetInstance<TEntity>().Find(left, top);
            if (index == -1)
            {
                return null;
            }

            TEntity entity = (TEntity)TileEntity.ByID[index];
            return entity;

        }
    }

    public class LootPile : ModTilePile<BasicLoot>
    {
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3); // Probably should be changeable within the child
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<BasicLoot>().Hook_AfterPlacement, -1, 0, true);

            MinPick = 55; // debugging
            TileObjectData.addTile(Type);
        }
    }

    // The Tile Pile type that is tied to the in-world Tile
    public class BasicLoot : BaseTilePile
    {
        public override TileStyle Style => TileStyle.Style3x3;

        public override void CreateTilePile()
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    PileContents = new TileInfo[2];
                    PileContents[0] = new TileInfo("BookStack_S3", position, 16, 38, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo("BookStack_S2", position, 18, 28, 0, (int)TileInfo.InteractionType.Chop);
                    break;
                case 1:
                    PileContents = new TileInfo[4];
                    PileContents[0] = new TileInfo("Crate_S", position, 4, 32, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo("Crate_M", position, 18, 28, -1, (int)TileInfo.InteractionType.Chop);
                    PileContents[2] = new TileInfo("Cloth_L", position, 18, 26, 1, (int)TileInfo.InteractionType.Mine);
                    PileContents[3] = new TileInfo("BookStack_S3", position, 24, 14, 2, (int)TileInfo.InteractionType.Click);
                    break;
                case 2:
                    PileContents = new TileInfo[4];
                    PileContents[0] = new TileInfo("Crate_S", position, 18, 32, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo("Cloth_S", position, 18, 30, 0, (int)TileInfo.InteractionType.Mine);
                    PileContents[2] = new TileInfo("BookStack_S3", position, 22, 18, 1, (int)TileInfo.InteractionType.Click);
                    PileContents[3] = new TileInfo("Sack_S", position, 4, 30, -1, (int)TileInfo.InteractionType.Chop);
                    break;
                case 3:
                    PileContents = new TileInfo[3];
                    PileContents[0] = new TileInfo("Crate_S", position, 6, 32, -1, (int)TileInfo.InteractionType.Click);
                    PileContents[1] = new TileInfo("Crate_S", position, 8, 14, 0, (int)TileInfo.InteractionType.Mine);
                    PileContents[2] = new TileInfo("Backpack_Sr", position, 26, 34, -1, (int)TileInfo.InteractionType.Chop);
                    break;
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            BasicLoot te = ByID[id] as BasicLoot;
            te.SetPosition(new Vector2(i + 1, j + 2)); // TODO: The origin is in the top left corner, change to scale based on Style
            te.CreateTilePile();

            return id;
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<LootPile>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<LootPile>();
        }
    }
}
