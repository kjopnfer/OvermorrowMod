using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace OvermorrowMod.Common.Particles
{
    public class Shockwave : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "Perlin";
        public float maxSize { get { return particle.customData[0]; } set { particle.customData[0] = value; } }
        float maxTime = 60f;
        public override void OnSpawn()
        {
            if (particle.customData[1] == 0) particle.customData[1] = 1;
            if (particle.customData[2] == 0) particle.customData[2] = 1;
            if (particle.customData[3] == 0) particle.customData[3] = 1;
            maxSize = particle.scale;
            particle.scale = 0f;
        }
        public override void Update()
        {
            particle.velocity = Vector2.Zero;
            float progress = (float)particle.activeTime / maxTime;
            particle.scale = MathHelper.Lerp(particle.scale, maxSize, progress);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, progress);
            Color col = new Color(particle.customData[1], particle.customData[2], particle.customData[3]);
            particle.color = Color.Lerp(particle.color, col, progress);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // 1.5 X scale cuz the vanilla shader halves X size
            Vector2 scale = new Vector2(particle.scale * 1.5f, particle.scale);
            // restart spritebatch
            spriteBatch.Reload(SpriteSortMode.Immediate);
            Texture2D texture = Particle.ParticleTextures[particle.type];
            // make a new drawdata(spritebatch draw but saved inside a class)
            DrawData data = new DrawData(texture,
                particle.position - Main.screenPosition,
                new Rectangle(0, 0, texture.Width, texture.Height),
                particle.color * particle.alpha,
                particle.rotation,
                new Vector2(texture.Width, texture.Height) / 2,
                scale,
                SpriteEffects.None,
            0);
            // vanilla effect used in pillar shield
            var effect = GameShaders.Misc["ForceField"];
            effect.UseColor(particle.color);
            effect.Apply(data);
            // make it actually draw
            data.Draw(spriteBatch);
            // restart spritebatch again so effect doesnt continue to be applied
            spriteBatch.Reload(SpriteSortMode.Deferred);
        }
    }
}