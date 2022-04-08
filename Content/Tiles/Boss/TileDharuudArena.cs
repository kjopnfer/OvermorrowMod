using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Boss
{
    /**
     * The following handles collision for the doors of the boss arena, it is a completely invisible texture
     * The way that SLR's barrier "tiles" work is that the collision tiles are spawned upon WorldGen for the desired area
     * When the boss is spawned or a condition is met, these invisible tiles will become active
     * These are used in conjunction with a texture that you can freely manipulate in order to do something like boss doors
     */
    public class DharuudArena : ModTile
    {
        public override void SetDefaults()
        {
            //TileID.Sets.DrawsWalls[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileSolid[Type] = true;
            minPick = 1;
        }

        /**
         * Set the texture of the tile to be invisible
         */
        public override bool Autoload(ref string name, ref string texture)
        {
            texture = AssetDirectory.Assets + "Empty";

            return base.Autoload(ref name, ref texture);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            /*if (tile.frameX == 0 && tile.frameY == 0)
            {
                Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "Boss/BG");
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                Rectangle drawRectangle = new Rectangle(0, texture.Height, texture.Width, texture.Height);

                spriteBatch.Draw(texture, drawPos /*- texture.Size() / 2, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }*/

            Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "Boss/BG");
            Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
            Rectangle drawRectangle = new Rectangle(0, texture.Height, texture.Width, texture.Height);

            spriteBatch.Draw(texture, drawPos - texture.Size() / 2, null, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}