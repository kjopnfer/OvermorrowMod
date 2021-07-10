using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria.ID;
using Terraria;
using System;
using OvermorrowMod.Effects;

namespace OvermorrowMod
{
    public static class ModUtils
    {
        public static void Draw(this Rectangle rect, bool collides = false)
        {
            PrimitivePacket packet = new PrimitivePacket();
            packet.Type = PrimitiveType.TriangleList;
            packet.Pass = "Basic";
            Vector2 pos1 = rect.TopLeft();
            Vector2 pos2 = rect.BottomLeft();
            Vector2 pos3 = rect.TopRight();
            Vector2 pos4 = rect.BottomRight();
            Color color = collides ? Color.Green : Color.Red;
            packet.Add(pos1, color, Vector2.Zero);
            packet.Add(pos2, color, Vector2.Zero);
            packet.Add(pos3, color, Vector2.Zero);
            packet.Add(pos4, color, Vector2.Zero);
            packet.Add(pos3, color, Vector2.Zero);
            packet.Add(pos2, color, Vector2.Zero);
            packet.Send();
        }
        public static Vector2 NearestPoint(this Vector2 vec, Rectangle rect)
        {
            float nearX = vec.X > rect.Right ? rect.Right : vec.X < rect.Left ? rect.Left : vec.X;
            float nearY = vec.Y > rect.Bottom ? rect.Bottom : vec.Y < rect.Top ? rect.Top : vec.Y;
            return new Vector2(nearX, nearY);
        }
        public static bool CollisionCheck(Rectangle rect1, Rectangle rect2)
        {
            Vector2 nearest1 = NearestPoint(rect1.Center.ToVector2(), rect2);
            Vector2 nearest2 = NearestPoint(rect2.Center.ToVector2(), rect1);
            return rect1.Contains(nearest1.ToPoint()) || rect2.Contains(nearest2.ToPoint());;
        }
        public static Vector2 GetRequiredVelocity(Vector2 from, Vector2 to, float rps, float atr = (float)Math.PI)
        {
            float mult = (float)atr / rps;
            mult /= 10;
            mult *= 2;
            float aboutLength = mult * (float)atr;
            float length = (to - from).Length();
            float startRot = (to - from).ToRotation();
            return new Vector2(0, length / aboutLength).RotatedBy(startRot - atr);
        }
        public static Vector2 GetRequiredVelocity2(Vector2 from, Vector2 to, float time, float atr = (float)Math.PI)
        {
            float timeToRot = atr / time;
            return GetRequiredVelocity(from, to, timeToRot, atr);
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
        public static void ClampDistance(ref Vector2 start, ref Vector2 end, float maxDistance)
		{
			if (Vector2.Distance(end, start) > maxDistance)
			{
				end = start + Vector2.Normalize(end - start) * maxDistance;
			}
		}
		/// <summary> Adjusts a velocity to a gravity </summary>
        public static Vector2 AdjustToGravity(this Vector2 velocity, float gravity, float time)
        {
            velocity.X = velocity.X / time;
            velocity.Y = velocity.Y / time - 0.5f * gravity * time;
            return velocity;
        }
        public static Vector3 RotatedBy(this Vector3 vec, float yaw, float pitch, float roll)
        {
            return Vector3.Transform(vec, Matrix.CreateFromYawPitchRoll(yaw, pitch, roll));
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
        public static void Reload(this SpriteBatch spriteBatch, BlendState blendState = null)
        {
            if ((bool)spriteBatch.GetType().GetField("inBeginEndPair", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch))
			{
				spriteBatch.End();
			}
			SpriteSortMode sortMode = SpriteSortMode.Deferred;
			if (blendState == null) blendState = (BlendState)spriteBatch.GetField("blendState");
			SamplerState state = (SamplerState)spriteBatch.GetField("samplerState");
			DepthStencilState state2 = (DepthStencilState)spriteBatch.GetField("depthStencilState");
			RasterizerState state3 = (RasterizerState)spriteBatch.GetField("rasterizerState");
			Effect effect = (Effect)spriteBatch.GetField("customEffect");
			Matrix matrix = (Matrix)spriteBatch.GetField("transformMatrix");
			spriteBatch.Begin(sortMode, blendState, state, state2, state3, effect, matrix);
        }
        public static object GetField(this object obj, string name)
        {
            return obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }
        public static Vector2 HandPosition(this Player player)
        {
            Vector2 center = player.MountedCenter;
            if (player.bodyFrame.Y == player.bodyFrame.Height * 3)
            {
                center.X += 8 * player.direction;
                center.Y += 2 * player.gravDir;
            }
            else if (player.bodyFrame.Y == player.bodyFrame.Height * 2)
            {
                center.X += 6 * player.direction;
                center.Y += -12 * player.gravDir;
            }
            else if (player.bodyFrame.Y == player.bodyFrame.Height * 4)
            {
                center.X += 6 * player.direction;
                center.Y += 8 * player.gravDir;
            }
            else if (player.bodyFrame.Y == player.bodyFrame.Height)
            {
                center.X += -10 * player.direction;
                center.Y += -14 * player.gravDir;
            }
            return center;
        }
        public static void Move(this NPC npc, Vector2 pos, float speed, float divider)
        {
            Vector2 vel = npc.DirectionTo(pos) * speed;
            npc.velocity = (npc.velocity * divider + vel) / (divider + 1);
        }
        public static void DirectMove(this NPC npc, Vector2 pos, float speed, float time, float mult)
        {
            Vector2 vel = (pos - npc.Center) / time;
            npc.velocity = vel * mult;
        }
    }
}