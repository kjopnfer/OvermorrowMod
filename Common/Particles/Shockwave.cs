using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Shockwave : Particle
    {
        public float maxSize { get { return CustomData[0]; } set { CustomData[0] = value; } }
        float maxTime = 60f;
        public override void OnSpawn()
        {
            if (CustomData[1] == 0) CustomData[1] = 1;
            if (CustomData[2] == 0) CustomData[2] = 1;
            if (CustomData[3] == 0) CustomData[3] = 1;
            maxSize = Scale;
            Scale = 0f;
        }
        public override void Update()
        {
            Velocity = Vector2.Zero;
            float progress = (float)ActiveTime / maxTime;
            Scale = MathHelper.Lerp(Scale, maxSize, progress);
            Alpha = MathHelper.Lerp(Alpha, 0, progress);
            Color col = new Color(CustomData[1], CustomData[2], CustomData[3]);
            Color = Color.Lerp(Color, col, progress);
            if (ActiveTime > maxTime) Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // 1.5 X scale cuz the vanilla shader halves X size
            Vector2 scale = new Vector2(Scale * 1.5f, Scale);
            // restart spritebatch
            spriteBatch.Reload(SpriteSortMode.Immediate);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Perlin").Value;
            // make a new drawdata(spritebatch draw but saved inside a class)
            DrawData data = new DrawData(texture,
                Position - Main.screenPosition,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color * Alpha,
                Rotation,
                new Vector2(texture.Width, texture.Height) / 2,
                scale,
                SpriteEffects.None,
            0);
            // vanilla effect used in pillar shield
            var effect = GameShaders.Misc["ForceField"];
            effect.UseColor(Color);
            effect.Apply(data);
            // make it actually draw
            data.Draw(spriteBatch);
            // restart spritebatch again so effect doesnt continue to be applied
            spriteBatch.Reload(SpriteSortMode.Deferred);
        }
    }
}