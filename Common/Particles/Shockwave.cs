using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Shockwave : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "PulseCircle";
        public float maxSize = 1;
        public float maxTime = 0;
        public override void OnSpawn()
        {
            //if (particle.customData[1] == 0) particle.customData[1] = 60;
            //if (particle.customData[2] == 0) particle.customData[2] = 1;
            //if (particle.customData[3] == 0) particle.customData[3] = 1;

            maxTime = /*particle.customData[1] == 0 ? 60 : particle.customData[1];*/ 60;
            maxSize = particle.scale;
            particle.scale = 0f;
        }

        public override void Update()
        {
            particle.velocity = Vector2.Zero;
            //float progress = particle.activeTime / maxTime;

            float progress = ModUtils.EaseOutQuad(particle.activeTime / maxTime);
            //particle.scale = MathHelper.SmoothStep(particle.scale, maxSize, progress);
            //particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            /*spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);*/

            Vector2 offset = Vector2.UnitY.RotatedBy(particle.rotation);

            float progress2 = Utils.Clamp(particle.customData[0]++, 0, 25f) / 25f;
            float progress3 = Utils.Clamp(particle.customData[1]++, 0, 20f) / 25f;

            DrawRing(AssetDirectory.Textures + "Crosshair", spriteBatch, particle.position + offset - Main.screenPosition, 1, 1, Main.GameUpdateCount / 40f, progress2, new Color(244, 188, 91));
            DrawRing(AssetDirectory.Textures + "Crosshair", spriteBatch, particle.position - Main.screenPosition, 3f, 3f, Main.GameUpdateCount / 20f, progress3, new Color(244, 188, 91));
        }

        private void DrawRing(string texture, SpriteBatch spriteBatch, Vector2 position, float width, float height, float rotation, float prog, Color color)
        {
            var texRing = ModContent.Request<Texture2D>(texture).Value;
            Effect effect = OvermorrowModFile.Instance.Ring.Value;

            effect.Parameters["uProgress"].SetValue(rotation);
            effect.Parameters["uColor"].SetValue(color.ToVector3());
            effect.Parameters["uImageSize1"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uOpacity"].SetValue(prog);
            effect.CurrentTechnique.Passes["BowRingPass"].Apply();

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.Additive, default, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

            var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
            spriteBatch.Draw(texRing, target, null, color * prog, particle.rotation + MathHelper.PiOver2, texRing.Size() / 2, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }
    }
}