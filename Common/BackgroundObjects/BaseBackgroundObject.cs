using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.BackgroundObjects
{
    public abstract class BaseBackgroundObject : ModTileEntity
    {
        // Add a texture
        // Add a detour to draw the texture

        // TODO: Write autoloading for this
        //public virtual Texture2D Texture => ModContent.Request<Texture2D>(FullName.Replace('.', '/'), ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public virtual Texture2D Texture => ModContent.Request<Texture2D>("OvermorrowMod/Common/BackgroundObjects/House", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public abstract (float, float) Size();

        public void DrawObject(SpriteBatch spriteBatch)
        {
            Vector2 position = Position.ToVector2() * 16;
            Vector2 drawOffset = new Vector2(0, Texture.Height / 2f - 16);

            spriteBatch.Draw(Texture, position - drawOffset - Main.screenPosition, null, Lighting.GetColor(Position.ToPoint()), 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }

        public bool IsOnScreen()
        {
            Vector2 position = Position.ToVector2() * 16;
            return ModUtils.OnScreen(new Rectangle((int)position.X - (int)Main.screenPosition.X, (int)position.Y - (int)Main.screenPosition.Y, (int)Size().Item1, (int)Size().Item2));
        }

        public abstract bool BackgroundObjectTileCheck(int x, int y);

        public sealed override bool IsTileValidForEntity(int x, int y)
        {
            Main.NewText(BackgroundObjectTileCheck(x, y));
            Kill(Position.X, Position.Y);

            return BackgroundObjectTileCheck(x, y);
        }

        /// <summary>
        /// Used only for testing
        /// </summary>
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            // ModTileEntity.Place() handles checking if the entity can be placed, then places it for you
            // Set "tileOrigin" to the same value you set TileObjectData.newTile.Origin to in the ModTile
            Point16 tileOrigin = new Point16(1, 1);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;
        }
    }
}