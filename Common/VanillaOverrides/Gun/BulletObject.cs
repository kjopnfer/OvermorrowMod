using System;
using OvermorrowMod.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class BulletObject
    {
        public int DrawCounter = 0;
        public int DeathCounter = 0;

        public bool isActive = true;
        public bool startDeath = false;

        private string BulletTexture;
        public BulletObject(string BulletTexture, int DrawCounter = 0)
        {
            this.BulletTexture = BulletTexture;
            this.DrawCounter = DrawCounter;
        }

        public void Update()
        {
            if (!isActive) return;

            if (startDeath)
            {
                DeathCounter++;

                if (DeathCounter == 15)
                {
                    isActive = false;
                }
            }

            DrawCounter++;
        }
        
        /// <summary>
        /// Flags the bullet display to start it's death code, does not instantly set isActive to false.
        /// </summary>
        public void Deactivate()
        {
            startDeath = true;
        }

        public void Reset()
        {
            DeathCounter = 0;

            startDeath = false;
            isActive = true;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Texture2D activeBullets = ModContent.Request<Texture2D>(AssetDirectory.UI + BulletTexture).Value;
            float scale = 1;

            Vector2 positionOffset = Vector2.UnitY * MathHelper.Lerp(-1, 1, (float)Math.Sin(DrawCounter / 30f) * 0.5f + 0.5f);
            float rotation = MathHelper.Lerp(MathHelper.ToRadians(-8), MathHelper.ToRadians(8), (float)Math.Sin(DrawCounter / 40f) * 0.5f + 0.5f);

            if (startDeath)
            {
                if (DeathCounter < 8)
                {
                    scale = MathHelper.Lerp(1f, 1.5f, DeathCounter / 8f);
                }
                else
                {
                    scale = MathHelper.Lerp(1.5f, 0, (DeathCounter - 8) / 7f);
                }
            }

            spriteBatch.Draw(activeBullets, position + positionOffset - Main.screenPosition, null, Color.White, rotation, activeBullets.Size() / 2f, scale, SpriteEffects.None, 1);
        }
    }
}