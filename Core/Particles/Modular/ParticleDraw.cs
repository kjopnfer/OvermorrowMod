using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles.Modular
{
    /// <summary>
    /// Shared draw for a modular particle, used by both the world path and the contextual/UI path.
    /// Applies the spec's material shader (via the Immediate-mode Effect pattern) when one is set.
    /// </summary>
    public static class ParticleDraw
    {
        public static void Draw(SpriteBatch spriteBatch, ParticleSpec spec, ParticleState state, Vector2 screenPos, bool useShader = true)
        {
            string path = AssetDirectory.Textures + spec.Texture;
            if (!ModContent.HasAsset(path)) path = AssetDirectory.Textures + "trace_04";
            Texture2D texture = ModContent.Request<Texture2D>(path).Value;

            Effect effect = null;
            ParticleShaderRegistry.ApplyShader apply = null;
            string pass = null;
            bool usingShader = useShader && ParticleShaderRegistry.TryGet(spec.Shader, out effect, out apply, out pass);
            if (usingShader)
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);
                apply(effect, spec, state);
                effect.CurrentTechnique.Passes[pass].Apply();
            }

            SpriteEffects flip = SpriteEffects.None;
            if (spec.FlipHorizontal) flip |= SpriteEffects.FlipHorizontally;
            if (spec.FlipVertical) flip |= SpriteEffects.FlipVertically;
            float rotation = state.Rotation + MathHelper.ToRadians(spec.RotationOffsetDeg);

            spriteBatch.Draw(texture, screenPos, null, state.Color * state.Alpha, rotation,
                texture.Size() / 2f, state.Scale, flip, 0f);

            if (usingShader)
                spriteBatch.Reload(spec.Additive ? BlendState.Additive : BlendState.AlphaBlend, SpriteSortMode.Deferred);
        }
    }
}
