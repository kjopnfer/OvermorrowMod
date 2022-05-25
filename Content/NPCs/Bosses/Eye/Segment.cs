using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class Segment
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;

        public float Length;
        public float Time;

        public float Angle;
        public float SelfAngle;
        public float InitialAngle; // Store the initial angle for the root segment

        public Segment Parent = null;
        public Segment Child = null;

        public Segment(Vector2 Position, float Length, float Angle, float Time)
        {
            Parent = null;
            StartPoint = Position;
            InitialAngle = Angle;

            this.Length = Length;
            this.Angle = Angle;
            this.Time = Time;

            CalculateEndpoint();
        }

        public Segment(Segment Segment, float Length, float Angle, float Time)
        {
            Parent = Segment;
            StartPoint = Segment.EndPoint;
            this.Length = Length;
            this.Angle = Angle;
            this.Time = Time;
            SelfAngle = Angle;

            CalculateEndpoint();
        }

        /// <summary>
        /// Re-maps a number from one range to another.
        /// </summary>
        /// <param name="value">the incoming value to be converted</param>
        /// <param name="start1">lower bound of the value's current range</param>
        /// <param name="stop1">upper bound of the value's current range</param>
        /// <param name="start2">lower bound of the value's target range</param>
        /// <param name="stop2">upper bound of the value's target range</param>
        /// <returns></returns>
        public float Map(float value, float start1, float stop1, float start2, float stop2)
        {
            float outgoing = start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));

            return outgoing;
        }

        public void Move()
        {
            float MinAngle = -0.05f;
            float MaxAngle = 0.05f;

            SelfAngle = Map((float)Math.Sin(Time), -1, 1, MaxAngle, MinAngle);
            Time += 0.03f;
        }

        public void Update()
        {
            Angle = SelfAngle;
            if (Parent != null)
            {
                StartPoint = Parent.EndPoint;
                Angle += Parent.Angle;
            }
            else
            {
                // The initial angle which the base of the tentacle faces
                Angle += InitialAngle;
            }

            CalculateEndpoint();
        }

        public void CalculateEndpoint()
        {
            float xChange = (float)(Length * Math.Cos(Angle));
            float yChange = (float)(Length * Math.Sin(Angle));

            EndPoint = new Vector2(StartPoint.X + xChange, StartPoint.Y + yChange);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Draw(texture, StartPoint - Main.screenPosition, null, Color.Wheat, Angle - MathHelper.PiOver2, texture.Size() / 2, 1f, SpriteEffects.None, 0);
        }
    }
}
