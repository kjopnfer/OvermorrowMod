using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.BackgroundObjects
{
    // TODO: On creation, assign a list of tree segment textures that are randomly generated and then saved
    public class BackgroundTree : BaseBackgroundObject
    {
        /// <summary>
        /// The tree's height in tiles
        /// </summary>
        public int TreeHeight = 10;

        /// <summary>
        /// The tree texture segments with the base of the tree as index 0
        /// </summary>
        public List<string> TreeSegments = new List<string>();

        public override void LoadData(TagCompound tag)
        {
            // TODO: Save TreeHeight and TreeSegments
            base.LoadData(tag);
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }


        public override bool BackgroundObjectTileCheck(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile;
        }

        public override void DrawObject(SpriteBatch spriteBatch)
        {
            base.DrawObject(spriteBatch);
        }

        public override (float, float) Size() => new(420, 420);
    }
}