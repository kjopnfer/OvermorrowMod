using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.NPCs.Bosses.SandstormBoss;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.DesertTemple
{
    /**
     * The following handles collision for the doors of the boss arena, it is a completely invisible texture
     * The way that SLR's barrier "tiles" work is that the collision tiles are spawned upon WorldGen for the desired area
     * When the boss is spawned or a condition is met, these invisible tiles will become active
     * These are used in conjunction with a texture that you can freely manipulate in order to do something like boss doors
     */
    public class snek : ModTile
    {
        public override void SetStaticDefaults()
        {
            //TileID.Sets.DrawsWalls[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileSolid[Type] = true;
            MinPick = 1;
        }

        public override string Texture => AssetDirectory.Assets + "Empty";

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Tiles + "DesertTemple/snek").Value;

            int progress = (int)(20 / 120f * texture.Height);
            foreach (NPC npc in Main.npc)
            {
                /*if (npc.active && npc.type == ModContent.NPCType<SandstormBoss>())
                {
                    progress = (int)(npc.localAI[2] / 120f * texture.Height);
                }*/
            }

            /*if (tile.frameX == 0 && tile.frameY == 0)
            {
                Texture2D texture = ModContent.GetTexture(AssetDirectory.Tiles + "Boss/BG");
                Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
                Rectangle drawRectangle = new Rectangle(0, texture.Height, texture.Width, texture.Height);
                spriteBatch.Draw(texture, drawPos /*- texture.Size() / 2, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }*/

            Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

            spriteBatch.Draw(texture, new Rectangle((int)drawPos.X, (int)drawPos.Y - progress, texture.Width, progress),
            new Rectangle(0, 0, texture.Width, progress), Color.Orange);
            /*Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;
            Rectangle drawRectangle = new Rectangle(0, texture.Height, texture.Width, texture.Height);

            spriteBatch.Draw(texture, drawPos - texture.Size() / 2, null, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);*/

            return false;
        }
    }
}