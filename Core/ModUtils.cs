using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using System;
using System.Reflection;
using Terraria;

namespace OvermorrowMod.Core
{
    public static class ModUtils
    {
        public static MethodInfo startRain;
        public static MethodInfo stopRain;
        public static void Load(bool unload)
        {
            if (unload)
            {
                startRain = null;
                stopRain = null;
            }
            else
            {
                startRain = typeof(Main).GetMethod("StartRain", BindingFlags.Static | BindingFlags.NonPublic);
                stopRain = typeof(Main).GetMethod("StopRain", BindingFlags.Static | BindingFlags.NonPublic);
            }
        }
        public static void StartRain()
        {
            startRain.Invoke(null, null);
        }
        public static void StopRain()
        {
            stopRain.Invoke(null, null);
        }
        public static Vector2 NearestPoint(this Vector2 vec, Rectangle rect)
        {
            float nearX = vec.X > rect.Right ? rect.Right : vec.X < rect.Left ? rect.Left : vec.X;
            float nearY = vec.Y > rect.Bottom ? rect.Bottom : vec.Y < rect.Top ? rect.Top : vec.Y;
            return new Vector2(nearX, nearY);
        }
        private static float Bezier(float x1, float x2, float x3, float x4, float t)
        {
            return (float)(
                x1 * Math.Pow(1 - t, 3) +
                x2 * 3 * t * Math.Pow(1 - t, 2) +
                x3 * 3 * Math.Pow(t, 2) * (1 - t) +
                x4 * Math.Pow(t, 3)
                );
        }
        public static Vector2 Bezier(Vector2 from, Vector2 to, Vector2 cp1, Vector2 cp2, float amount)
        {
            Vector2 output = new Vector2();
            output.X = Bezier(from.X, cp1.X, cp2.X, to.X, amount);
            output.Y = Bezier(from.Y, cp1.Y, cp2.Y, to.Y, amount);
            return output;
        }
        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0);
        }
        public static Vector2 AdjustToGravity(this Vector2 velocity, float gravity, float time)
        {
            velocity.X = velocity.X / time;
            velocity.Y = velocity.Y / time - 0.5f * gravity * time;
            return velocity;
        }
        public static bool HasParameter(this Effect effect, string name)
        {
            foreach (EffectParameter param in effect.Parameters)
            {
                if (param.Name == name) return true;
            }
            return false;
        }
        public static OvermorrowModPlayer Overmorrow(this Player player)
        {
            return player.GetModPlayer<OvermorrowModPlayer>();
        }
        public static void SafeSetParameter(this Effect effect, string name, float value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void SafeSetParameter(this Effect effect, string name, Color value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value.ToVector3());
        }
        public static void SafeSetParameter(this Effect effect, string name, Texture2D value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void SafeSetParameter(this Effect effect, string name, Matrix value)
        {
            if (effect.HasParameter(name)) effect.Parameters[name].SetValue(value);
        }
        public static void Reload(this SpriteBatch spriteBatch, SpriteSortMode sortMode = SpriteSortMode.Deferred)
        {
            if (spriteBatch.HasBegun())
            {
                spriteBatch.End();
            }
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
            if ((bool)spriteBatch.GetType().GetField("inBeginEndPair", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
            {
                spriteBatch.End();
            }
            if (blendState == null) blendState = (BlendState)spriteBatch.GetField("blendState");
            SamplerState state = (SamplerState)spriteBatch.GetField("samplerState");
            DepthStencilState state2 = (DepthStencilState)spriteBatch.GetField("depthStencilState");
            RasterizerState state3 = (RasterizerState)spriteBatch.GetField("rasterizerState");
            Effect effect = (Effect)spriteBatch.GetField("customEffect");
            Matrix matrix = (Matrix)spriteBatch.GetField("transformMatrix");
            spriteBatch.Begin(sortMode, blendState, state, state2, state3, effect, matrix);
        }
        public static bool HasBegun(this SpriteBatch spriteBatch)
        {
            return (bool)spriteBatch.GetType().GetField("inBeginEndPair", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
        }
        public static object GetField(this object obj, string name)
        {
            return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }
        public static void Move(this NPC npc, Vector2 pos, float speed, float divider)
        {
            Vector2 vel = npc.DirectionTo(pos) * speed;
            npc.velocity = (npc.velocity * divider + vel) / (divider + 1);
        }

        public static float isLeft(Vector2 P0, Vector2 P1, Vector2 P2)
        {
            return ((P1.X - P0.X) * (P2.X - P0.X) - (P2.X - P0.X) * (P1.X - P0.X));
        }
        public static bool PointInShape(Vector2 P, params Vector2[] args)
        {
            bool result = true;
            for (int i = 0; i < args.Length; i++)
            {
                int i1 = i;
                int i2 = (i + 1) % args.Length;
                Vector2 a = args[i1];
                Vector2 b = args[i2];
                Vector2 c = P;
                if (isLeft(a, b, c) < 0) result = false;
            }
            return result;
        }
    }
}