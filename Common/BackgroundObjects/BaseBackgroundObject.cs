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

        /// <summary>
        /// The path of the object's texture, defaults to an empty texture
        /// </summary>
        public virtual string Texture => AssetDirectory.Empty;
        private Texture2D _texture => ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public abstract (float, float) Size();
        public virtual Vector2 DrawOffset => new Vector2(0, 0);
        public virtual void DrawObject(SpriteBatch spriteBatch)
        {
            Vector2 position = Position.ToVector2() * 16;
            Vector2 textureOffset = new Vector2(Size().Item1 / 2f, Size().Item2 / 2f);
            Vector2 drawOffset = new Vector2(0 + textureOffset.X + DrawOffset.X, _texture.Height / 2f + textureOffset.Y + DrawOffset.Y);

            spriteBatch.Draw(_texture, position - drawOffset - Main.screenPosition, null, Lighting.GetColor(Position.ToPoint()), 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
        }

        public bool IsOnScreen()
        {
            Vector2 position = Position.ToVector2() * 16;
            return ModUtils.OnScreen(new Rectangle((int)position.X - (int)Main.screenPosition.X, (int)position.Y - (int)Main.screenPosition.Y, (int)Size().Item1, (int)Size().Item2));
        }

        public abstract bool BackgroundObjectTileCheck(int x, int y);

        public sealed override bool IsTileValidForEntity(int x, int y)
        {
            if (!BackgroundObjectTileCheck(x, y))
                Kill(Position.X, Position.Y);

            return BackgroundObjectTileCheck(x, y);
        }

        /// <summary>
        /// Used only for testing
        /// </summary>
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int placedEntity = Place(i, j);
            return placedEntity;
        }
    }
}