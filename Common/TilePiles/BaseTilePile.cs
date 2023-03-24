using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.TilePiles
{
    public abstract class BaseTilePile : ModTileEntity
    {
        private Vector2 _position;
        internal Vector2 position
        {
            get => _position;
        }

        internal TileInfo[] PileContents;

        public enum TileStyle
        {
            Style2x2,
            Style3x2,
            Style3x3,
            Style4x3,
            Style5x4,
            Style6x3,
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
            PileContents = tag.Get<TileInfo[]>("PileContents");
        }

        public abstract void CreateTilePile();

        public void SetPosition(Vector2 position) => _position = position;
    }
}