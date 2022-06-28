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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!visible) return;

            int xPosition = (int)(Main.screenWidth * Main.UIScale) / 2;
            int yPosition = (int)(Main.screenHeight * Main.UIScale) / 5;

            int titleSize = (int)(Terraria.GameContent.FontAssets.DeathText.Value.MeasureString(title).X * 0.65f) / 2;
            int nameSize = (int)(Terraria.GameContent.FontAssets.DeathText.Value.MeasureString(name).X * 0.4f) / 2;

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag").Value;
            float backOffset = (Main.screenWidth / 2);
            spriteBatch.Draw(backDrop, new Vector2(xPosition, yPosition), null, Color.White, 0f, backDrop.Size() / 2, 1f, SpriteEffects.None, 1f);

            spriteBatch.DrawString(Terraria.GameContent.FontAssets.DeathText.Value, title, new Vector2(xPosition - titleSize, yPosition + 10), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(Terraria.GameContent.FontAssets.DeathText.Value, name, new Vector2(xPosition - nameSize, yPosition + 50), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);

            if (timer++ == showLength)
            {
                timer = 0;
                visible = false;
            }
        }
    }
}