using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Common.Rendering
{
    public class BeginData
    {
        private readonly BlendState blendState;
        private readonly DepthStencilState depthStencilState;
        private readonly Effect effect;
        private readonly RasterizerState rasterizerState;
        private readonly SamplerState samplerState;
        private readonly SpriteSortMode sortMode;
        private readonly Matrix transformMatrix;

        public BeginData(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix)
        {
            this.sortMode = sortMode;
            this.blendState = blendState;
            this.samplerState = samplerState;
            this.depthStencilState = depthStencilState;
            this.rasterizerState = rasterizerState;
            this.effect = effect;
            this.transformMatrix = transformMatrix;
        }

        public void Apply(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect,
                transformMatrix);
        }

        public static BeginData GetData(SpriteBatch spriteBatch)
        {
            return new BeginData(
                (SpriteSortMode)spriteBatch.GetField("sortMode"),
                (BlendState)spriteBatch.GetField("blendState"),
                (SamplerState)spriteBatch.GetField("samplerState"),
                (DepthStencilState)spriteBatch.GetField("depthStencilState"),
                (RasterizerState)spriteBatch.GetField("rasterizerState"),
                (Effect)spriteBatch.GetField("customEffect"),
                (Matrix)spriteBatch.GetField("transformMatrix")
            );
        }
    }

    public static class DrawUtils
    {
        public static bool HasBegun(this SpriteBatch spriteBatch)
        {
            return (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(spriteBatch);
        }

        public static object GetField(this object obj, string name)
        {
            return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = null,
            SpriteSortMode sortMode = default)
        {
            if (spriteBatch.HasBegun()) spriteBatch.End();
            blendState ??= (BlendState)spriteBatch.GetField("blendState");
            var state = (SamplerState)spriteBatch.GetField("samplerState");
            var state2 = (DepthStencilState)spriteBatch.GetField("depthStencilState");
            var state3 = (RasterizerState)spriteBatch.GetField("rasterizerState");
            var effect = (Effect)spriteBatch.GetField("customEffect");
            var matrix = (Matrix)spriteBatch.GetField("transformMatrix");
            spriteBatch.Begin(sortMode, blendState, state, state2, state3, effect, matrix);
        }
        public static Color UseAlpha(this Color color, int alpha)
        {
            return new Color(color.R, color.G, color.B, (byte)(alpha));
        }
    }
}