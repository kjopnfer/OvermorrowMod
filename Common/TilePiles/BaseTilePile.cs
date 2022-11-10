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
}