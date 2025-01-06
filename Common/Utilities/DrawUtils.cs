using ReLogic.Content;
using System.Threading;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Common.Utilities
{
    public static class DrawUtils
    {
        public static Color ColorLerp3(Color a, Color b, Color c, float t)
        {
            if (t < 0.5f) // 0.0 to 0.5 goes to a -> b
                return Color.Lerp(a, b, t / 0.5f);
            else // 0.5 to 1.0 goes to b -> c
                return Color.Lerp(b, c, (t - 0.5f) / 0.5f);
        }

        public static bool HasParameter(this Effect effect, string name)
        {
            return effect.Parameters[name] != null;
        }

        public static void SafeSetParameter<T>(this Effect effect, string name, T value)
        {
            if (!effect.HasParameter(name))
                return;

            var param = effect.Parameters[name];
            switch (value)
            {
                case float f:
                    param.SetValue(f);
                    break;
                case Vector2 v2:
                    param.SetValue(v2);
                    break;
                case Color color:
                    param.SetValue(color.ToVector3());
                    break;
                case Texture2D tex:
                    param.SetValue(tex);
                    break;
                case Matrix mat:
                    param.SetValue(mat);
                    break;
                default:
                    throw new ArgumentException($"Unsupported type {typeof(T)} for parameter {name}");
            }
        }

        public static bool HasBegun(this SpriteBatch spriteBatch)
        {
            return (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
        }

        public static object GetField(this object obj, string name)
        {
            return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        public static void Reload(this SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if (spriteBatch.HasBegun()) spriteBatch.End();

            BlendState blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            SamplerState samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            DepthStencilState depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            RasterizerState rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Effect effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            Matrix matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
            spriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, matrix);
        }

        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = null, SpriteSortMode sortMode = default)
        {
            if (spriteBatch.HasBegun()) spriteBatch.End();
            if (blendState == null) blendState = (BlendState)spriteBatch.GetField("blendState");

            SamplerState state = (SamplerState)spriteBatch.GetField("samplerState");
            DepthStencilState state2 = (DepthStencilState)spriteBatch.GetField("depthStencilState");
            RasterizerState state3 = (RasterizerState)spriteBatch.GetField("rasterizerState");
            Effect effect = (Effect)spriteBatch.GetField("customEffect");
            Matrix matrix = (Matrix)spriteBatch.GetField("transformMatrix");
            spriteBatch.Begin(sortMode, blendState, state, state2, state3, effect, matrix);
        }
    }
}