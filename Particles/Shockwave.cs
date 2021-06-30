using Terraria;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Particles
{
    public class Shockwave : CustomParticle
    {
        public override string Texture => "Textures/Perlin";
        public float maxSize {get {return particle.customData[0];} set{particle.customData[0] = value;}}
        float maxTime = 60f;
        public override void OnSpawn()
        {
            /*if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }*/
            particle.scale = 0f;
        }
        public override void Update()
        {
            particle.velocity *= 0;
            particle.scale = MathHelper.Lerp(particle.scale, particle.customData[0] / 256f, 0.05f);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, 0.05f);
            particle.color = Color.Lerp(particle.color, Color.White, 0.05f);
            if (particle.alpha < 0.01f || particle.activeTime > 300) particle.Kill();
        }
        public void Update2()
        {
            particle.velocity = Vector2.Zero;
            float progress = (float)particle.activeTime / maxTime;
            particle.scale = MathHelper.Lerp(particle.scale, particle.customData[0], progress);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, progress);
            particle.color = Color.Lerp(particle.color, Color.White, progress);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            var shader = GameShaders.Misc["TestMod: Shockwave"];
            shader.UseColor(particle.color);
            shader.UseSecondaryColor(particle.color);
            shader.Apply();
            Texture2D texture = Particle.ParticleTextures[particle.type];
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, new Rectangle(0, 0, texture.Width, texture.Height), particle.color * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale, SpriteEffects.None, 0);
            spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }
    }
}