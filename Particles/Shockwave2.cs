using Terraria;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Particles
{
    public class Shockwave2 : CustomParticle
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
            particle.scale = MathHelper.Lerp(particle.scale, particle.customData[0], progress);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, progress);
            Color col = new Color(particle.customData[1], particle.customData[2], particle.customData[3]);
            particle.color = Color.Lerp(particle.color, col, progress);
            if (particle.activeTime > maxTime) particle.Kill();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            
            // TODO: Needs the actual shader
            var shader = GameShaders.Misc["OvermorrowMod: Shockwave"];
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