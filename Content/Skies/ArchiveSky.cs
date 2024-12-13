using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Skies
{
    public class ArchiveSky : CustomSky
    {
        private bool isActive = false;
        private const float ParallaxMultiplier = 0.4f;
        public override float GetCloudAlpha() => 0f;

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            int biomeHeight = ((int)Main.worldSurface + (int)Main.worldSurface) / 2;
            float width = Main.screenWidth / 2f;
            float height = Main.screenHeight / 2f;

            Color textureColor = Color.Lerp(Main.ColorOfTheSkies, Color.White, 0.45f);
            Vector2 origin = new Vector2(0f, biomeHeight);


            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Backgrounds + "ArchiveBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            if (maxDepth >= 3E+38f && minDepth < 3E+38f)
            {
                int x = (int)(Main.screenPosition.X * 0.4f * ParallaxMultiplier);
                x %= (int)(texture.Width);

                int y = (int)(Main.screenPosition.Y * 0.5f * ParallaxMultiplier);

                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector2(width - x + texture.Width * k, height - y);
                    spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y - 400, Main.screenWidth, Main.screenHeight), Color.Lerp(Color.Black, Color.White, 0.5f));
                }
                //var pos = new Vector2(width - x + texture.Width / 2, height - y);
                //spriteBatch.Draw(texture, new Rectangle((int)pos.X, (int)pos.Y, Main.screenWidth, Main.screenHeight), Color.White);
            }

            if (maxDepth >= 7f && minDepth < 7f)
            {
                //DrawMidTextures(spriteBatch, width, height, textureColor, origin);
            }
        }

        private void DrawMidTextures(SpriteBatch spriteBatch, float width, float height, Color textureColor, Vector2 origin)
        {
            float midScale = 1f;
            Texture2D midTexture = ModContent.Request<Texture2D>(AssetDirectory.Backgrounds + "ArchiveBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int x = (int)(Main.screenPosition.X * 0.8f * ParallaxMultiplier);
            x %= (int)(midTexture.Width * midScale);

            int y = (int)(Main.screenPosition.Y * 0.5f * ParallaxMultiplier);
            //y -= 1420; // 1000

            Texture2D startTexture = midTexture;           
            Vector2 position = midTexture.Size() / 2f * midScale;

            for (int k = -1; k <= 1; k++)
            {
                var pos = new Vector2(width - x + midTexture.Width * k * midScale, height - y);
                spriteBatch.Draw(startTexture, pos - position, null, textureColor, 0f, origin, midScale, SpriteEffects.None, 0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive) { }
        }

        public override bool IsActive()
        {
            return isActive;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }
    }
}