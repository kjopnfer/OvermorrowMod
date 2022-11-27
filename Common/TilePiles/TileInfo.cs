using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Core;

namespace OvermorrowMod.Common.TilePiles
{
    public class TileInfo
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

        public enum InteractionType
        {
            Mine = 0,
            Chop = 1,
            Click = 2,
        }

        public int interactType;

        //public virtual int InteractType => (int)InteractionType.Click;

                public TileInfo(string identifier, Vector2 coordinates, int x, int y, int dependency, int interactType)
        {
            this.identifier = identifier;
            this.coordinates = coordinates;
            this.x = x;
            this.y = y;

            this.interactType = interactType;

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

        /*public TileInfo(string identifier, Vector2 coordinates, int x, int y, int dependency, int interactType)
        {
            this.identifier = identifier;
            this.coordinates = coordinates;
            this.x = x;
            this.y = y;

            this.interactType = interactType;

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
        }*/

        public int GetRandomStack() => Main.rand.Next(maxStack - minStack) + minStack;
    }
}