using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Effects
{
    /// <summary>
    /// Simple primitive renderer for triangle strips
    /// </summary>
    public static class PrimitiveRenderer
    {
        private static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

        /// <summary>
        /// Draws a triangle strip
        /// </summary>
        /// <param name="spriteBatch">Current SpriteBatch (will be ended/begun)</param>
        /// <param name="vertices">List of vertices for the triangle strip</param>
        /// <param name="texture">Texture to apply</param>
        /// <param name="blendState">Blend state to use (null for AlphaBlend)</param>
        public static void Draw(SpriteBatch spriteBatch, List<VertexPositionColorTexture> vertices, Texture2D texture, BlendState blendState = null)
        {
            if (vertices.Count < 3) return;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, blendState ?? BlendState.AlphaBlend, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            Effect trailShader = OvermorrowModFile.Instance.TrailShader.Value;

            if (trailShader.Parameters["WorldViewProjection"] != null)
                trailShader.Parameters["WorldViewProjection"].SetValue(model * projection);
            else if (trailShader.Parameters["WVP"] != null)
                trailShader.Parameters["WVP"].SetValue(model * projection);
            else if (trailShader.Parameters.Count > 0)
                trailShader.Parameters[0].SetValue(model * projection);

            trailShader.SafeSetParameter("uImage0", texture);
            trailShader.CurrentTechnique.Passes["Texturized"].Apply();

            Main.instance.GraphicsDevice.Textures[0] = texture;

            var vertexArray = vertices.ToArray();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertexArray, 0, vertices.Count / 3);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}