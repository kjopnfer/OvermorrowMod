using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    public class MonkeyStone : ModItem
    {
        public int itemFrame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monkey Stone");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(0, 3));
            Tooltip.SetDefault("'It resonates at a unique frequency, doesn't seem very useful but it might have value for someone.'");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
        }

        public override bool OnPickup(Player player)
        {
            foreach (Item playerItem in player.inventory)
            {
                if (playerItem.type == item.type && playerItem.stack < 999)
                {
                    itemFrame = ((MonkeyStone)item.modItem).itemFrame;
                }
            }

            return base.OnPickup(player);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Vector2 pos = ModUtils.GetInventoryPosition(position, frame, origin, scale);

            Texture2D texture = Main.itemTexture[item.type];
            Vector2 textureOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Rectangle drawRectangle = new Rectangle(0, texture.Height / 3 * itemFrame, texture.Width, texture.Height / 3);
            spriteBatch.Draw(texture, pos + new Vector2(0, (item.height / 2) + (item.height / 4) + 8), drawRectangle, drawColor, 0f, textureOrigin, scale, SpriteEffects.None, 0);

            return false;
        }

        private const int TEXTURE_HEIGHT = 26;
        private const int MAX_FRAMES = 3;
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = Main.itemTexture[item.type];
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            Rectangle drawRectangle = new Rectangle(0, texture.Height / 3 * itemFrame, texture.Width, texture.Height / 3);
            //Vector2 position = item.Center + new Vector2(0, (TEXTURE_HEIGHT / 2 * MAX_FRAMES) + 4 - (TEXTURE_HEIGHT / 2)) - Main.screenPosition;
            spriteBatch.Draw(texture, item.Center + new Vector2(0, item.height) - Main.screenPosition, drawRectangle, lightColor, rotation, origin, scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
