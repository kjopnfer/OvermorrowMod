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

namespace OvermorrowMod.Common.TilePiles
{
    internal class TilePiles : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileNoAttach[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TilePile>().Hook_AfterPlacement, -1, 0, true);
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

            if (pile != null)
            {
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

                    if (tileObject.active)
                    {
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

                            if (tileObject.wiggleTimer > 60)
                                tileObject.wiggleTimer = 0;
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
                }

                if (!activeObjects) WorldGen.KillTile(i, j);
            }
        }

        public override void MouseOver(int i, int j)
        {
            TilePile pile = FindTE(i, j);

            if (pile != null)
            {
                foreach (TileObject tileObject in pile.PileContents)
                {
                    if (tileObject.active)
                    {
                        if (Main.MouseWorld.Between(tileObject.rectangle.TopLeft(), tileObject.rectangle.BottomRight()))
                        {
                            Main.instance.MouseText($"Take {tileObject.name}");
                            tileObject.selected = true;
                        }
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

    public abstract class BaseTilePile : ModTileEntity
    {
        private Vector2 _position;
        internal Vector2 position
        {
            get => _position;
        }

        internal TileObject[] PileContents;

        public enum TileStyle
        {
            Style3x3,
            Style4x4,
            Style5x4,
        }

        public virtual TileStyle Style => TileStyle.Style3x3;

        public override void SaveData(TagCompound tag)
        {
            tag["_position"] = _position;
            tag["PileContents"] = PileContents;
        }

        public override void LoadData(TagCompound tag)
        {
            _position = tag.Get<Vector2>("_position");
            PileContents = tag.Get<TileObject[]>("PileContents");
        }

        public virtual void CreateTilePile() { }

        public void SetPosition(Vector2 position) => _position = position;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<TilePiles>())
            {
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<TilePiles>();
        }
    }

    public class TileObjectSerializer : TagSerializer<TileObject, TagCompound>
    {
        public override TagCompound Serialize(TileObject value) => new TagCompound
        {
            ["identifier"] = value.identifier,
            ["xCoordinate"] = value.coordinates.X,
            ["yCoordinate"] = value.coordinates.Y,
            ["x"] = value.x,
            ["y"] = value.y,
            ["dependency"] = value.dependency
        };

        public override TileObject Deserialize(TagCompound tag) => new TileObject(
            tag.GetString("identifier"),
            new Vector2(tag.GetFloat("xCoordinate"), tag.GetFloat("yCoordinate")),
            tag.GetInt("x"),
            tag.GetInt("y"),
            tag.GetInt("dependency"));
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
                    PileContents[0] = new TileObject("BookStack_S3", position, 16, 38, -1);
                    PileContents[1] = new TileObject("BookStack_S2", position, 18, 28, 0);
                    break;
                case 1:
                    PileContents = new TileObject[4];
                    PileContents[0] = new TileObject("Crate_S", position, 4, 32, -1);
                    PileContents[1] = new TileObject("Crate_M", position, 18, 28, -1);
                    PileContents[2] = new TileObject("Cloth_L", position, 18, 26, 1);
                    PileContents[3] = new TileObject("BookStack_S3", position, 24, 14, 2);
                    break;
                case 2:
                    PileContents = new TileObject[4];
                    PileContents[0] = new TileObject("Crate_S", position, 18, 32, -1);
                    PileContents[1] = new TileObject("Cloth_S", position, 18, 30, 0);
                    PileContents[2] = new TileObject("BookStack_S3", position, 22, 18, 1);
                    PileContents[3] = new TileObject("Sack_S", position, 4, 30, -1);
                    break;
                case 3:
                    PileContents = new TileObject[3];
                    PileContents[0] = new TileObject("Crate_S", position, 6, 32, -1);
                    PileContents[1] = new TileObject("Crate_S", position, 8, 14, 0);
                    PileContents[2] = new TileObject("Backpack_Sr", position, 26, 34, -1);
                    break;
            }
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int id = Place(i, j);
            TilePile te = ByID[id] as TilePile;
            te.SetPosition(new Vector2(i + 1, j + 2));
            te.CreateTilePile();

            return id;
        }
    }

    public class TileObject
    {
        internal string name;
        internal Rectangle rectangle;
        internal Texture2D texture;
        internal int ID;
        private int minStack;
        private int maxStack;
        internal bool active;
        internal bool selected;
        internal int dependency;
        internal int wiggleTimer;

        public string identifier;
        public Vector2 coordinates;
        public int x;
        public int y;

        public TileObject(string identifier, Vector2 coordinates, int x, int y, int dependency)
        {
            this.identifier = identifier;
            this.coordinates = coordinates;
            this.x = x;
            this.y = y;


            switch (identifier)
            {
                case "Crate_S":
                    name = "Crate";
                    rectangle.Width = 28;
                    rectangle.Height = 18;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Crate_S", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.WoodenCrate;
                    minStack = 1;
                    maxStack = 1;
                    break;
                case "Crate_M":
                    name = "Crate";
                    rectangle.Width = 32;
                    rectangle.Height = 22;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Crate_M", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.WoodenCrate;
                    minStack = 1;
                    maxStack = 1;
                    break;
                case "Cloth_S":
                    name = "Silk";
                    rectangle.Width = 26;
                    rectangle.Height = 10;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Cloth_S", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.Silk;
                    minStack = 2;
                    maxStack = 5;
                    break;
                case "Cloth_L":
                    name = "Silk";
                    rectangle.Width = 30;
                    rectangle.Height = 12;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Cloth_L", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.Silk;
                    minStack = 3;
                    maxStack = 8;
                    break;
                case "BookStack_S1":
                    name = "Books";
                    rectangle.Width = 18;
                    rectangle.Height = 10;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "BookStack_S1", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.Book;
                    minStack = 2;
                    maxStack = 4;
                    break;
                case "BookStack_S2":
                    name = "Books";
                    rectangle.Width = 20;
                    rectangle.Height = 10;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "BookStack_S2", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.Book;
                    minStack = 2;
                    maxStack = 4;
                    break;
                case "BookStack_S3":
                    name = "Books";
                    rectangle.Width = 20;
                    rectangle.Height = 12;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "BookStack_S3", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.Book;
                    minStack = 2;
                    maxStack = 4;
                    break;
                case "Backpack_S":
                    name = "Backpack";
                    rectangle.Width = 20;
                    rectangle.Height = 16;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Backpack_S", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.StoneBlock;
                    minStack = 1;
                    maxStack = 1;
                    break;
                case "Backpack_Sr":
                    name = "Backpack";
                    rectangle.Width = 20;
                    rectangle.Height = 16;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Backpack_Sr", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.StoneBlock;
                    minStack = 1;
                    maxStack = 1;
                    break;
                case "Sack_S":
                    name = "Sack";
                    rectangle.Width = 18;
                    rectangle.Height = 20;
                    texture = ModContent.Request<Texture2D>(AssetDirectory.TilePiles + "Sack_S", AssetRequestMode.ImmediateLoad).Value;
                    ID = ItemID.DirtBlock;
                    minStack = 3;
                    maxStack = 9;
                    break;
            }

            this.dependency = dependency;

            rectangle.X = ((int)coordinates.X - 1) * 16 + x;
            rectangle.Y = ((int)coordinates.Y - 2) * 16 + y;
            active = true;
            selected = false;

            wiggleTimer = 0;
        }

        public int GetRandomStack() => Main.rand.Next(maxStack - minStack) + minStack;
    }
}
