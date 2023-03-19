using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace OvermorrowMod.Common.TilePiles
{
    public class TileInfo
    {
        internal string name;
        internal Rectangle rectangle;
        internal Texture2D texture;
        internal int itemID;
        private int minStack;
        private int maxStack;
        internal bool active;
        internal bool selected;
        internal int dependency;
        internal int wiggleTimer;
        public int hitDelay;

        public string identifier;
        public Vector2 coordinates;
        public int x;
        public int y;

        public int breakCount;
        public int tileDurability;
        public int tileDust;
        public SoundStyle hitSound;
        public SoundStyle deathSound;
        public SoundStyle grabSound;
        public bool canWiggle;

        public enum InteractionType
        {
            Mine = 0,
            Chop = 1,
            Click = 2,
        }

        public int interactType;

        public TileInfo(string identifier, Vector2 coordinates, int x, int y, int dependency, int interactType)
        {
            this.identifier = identifier;
            this.coordinates = coordinates;
            this.x = x;
            this.y = y;

            this.interactType = interactType;
            this.dependency = dependency;

            TileObject tileObject = TileObject.GetTileObject(identifier);
            rectangle.Width = tileObject.Width;
            rectangle.Height = tileObject.Height;
            texture = TileObjects.TileObjectTextures[identifier];
            itemID = tileObject.ItemID;
            name = tileObject.Name;
            minStack = tileObject.MinStack;
            maxStack = tileObject.MaxStack;
            tileDurability = tileObject.Durability;
            tileDust = tileObject.TileDust;
            hitSound = tileObject.HitSound;
            deathSound = tileObject.DeathSound;
            grabSound = tileObject.GrabSound;
            canWiggle = tileObject.CanWiggle;

            rectangle.X = ((int)coordinates.X - 1) * 16 + x;
            rectangle.Y = ((int)coordinates.Y - 2) * 16 + y;
            active = true;
            selected = false;

            wiggleTimer = 0;
        }

        public int GetRandomStack() => Main.rand.Next(maxStack - minStack) + minStack;
    }
}