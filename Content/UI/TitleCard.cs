using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    public class TitleCard : UIState
    {
        private Texture2D badge;
        private string name;
        private string title;
        private int showLength;

        private int timer;

        public static bool visible = false;

        public void SetTitle(Texture2D badge, string name, string title, int showLength)
        {
            this.badge = badge;
            this.name = name;
            this.title = title;
            this.showLength = showLength;

            visible = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            int xPosition = (int)(Main.screenWidth * Main.UIScale) / 2;
            int yPosition = 75;

            int titleSize = (int)(Terraria.GameContent.FontAssets.DeathText.Value.MeasureString(title).X) / 2;
            int nameSize = (int)(Terraria.GameContent.FontAssets.DeathText.Value.MeasureString(name).X) / 2;

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag").Value;
            float backOffset = (Main.screenWidth / 2);
            spriteBatch.Draw(backDrop, new Vector2(xPosition + 50, yPosition + 100), null, Color.White, 0f, backDrop.Size() / 2, 1f, SpriteEffects.None, 1f);

            Texture2D borderTop = ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag_Top").Value;
            Texture2D borderBottom = ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag_Bottom").Value;

            spriteBatch.Draw(borderBottom, new Vector2(xPosition + 50, yPosition + 106), null, Color.White, 0f, borderBottom.Size() / 2, 1f, SpriteEffects.None, 1f);
            spriteBatch.Draw(borderTop, new Vector2(xPosition + 50, yPosition + 100), null, Color.White, 0f, borderTop.Size() / 2, 1f, SpriteEffects.None, 1f);

            spriteBatch.DrawString(Terraria.GameContent.FontAssets.DeathText.Value, title, new Vector2(xPosition, yPosition + 25), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Terraria.GameContent.FontAssets.DeathText.Value, name, new Vector2(xPosition - nameSize / 2, yPosition + 65), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            if (timer == showLength)
            {
                timer = 0;
                visible = false;
            }
            else
            {
                if (!Main.gamePaused) timer++;
            }
        }
    }
}