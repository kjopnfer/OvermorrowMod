using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader;
using Terraria;

namespace OvermorrowMod.Common.InverseKinematics
{
    public class InverseKinematicSegment
    {
        public Vector2 A { get; set; } // Changed to allow setting externally
        public Vector2 B { get; private set; } // Only allow setting B inside the class
        public float Length { get; private set; }
        public float Angle { get; set; }
        public float MinAngle { get; set; } = -MathHelper.Pi; // Default: -180 degrees
        public float MaxAngle { get; set; } = MathHelper.Pi;  // Default: 180 degrees
        public InverseKinematicSegment Parent { get; set; }
        public Texture2D Texture { get; set; }  // Texture property for each segment
        public Vector2 Origin { get; set; }

        public InverseKinematicSegment(float x, float y, float length, float angle, Texture2D texture, Vector2? origin = null, Vector2? textureOffset = null)
        {
            A = new Vector2(x, y);
            Length = length;
            Angle = angle;
            Texture = texture;

            Origin = origin ?? new Vector2(Texture.Width / 2, 0f);  // Default to the center of the texture

            Recalculate();
        }

        public void Follow(Vector2 target)
        {
            Vector2 direction = target - A;
            //Angle = direction.ToRotation();

            float targetAngle = direction.ToRotation();
            targetAngle = (targetAngle + MathHelper.TwoPi) % MathHelper.TwoPi;
            Angle = MathHelper.Clamp(targetAngle, MinAngle, MaxAngle);

            direction = direction.SafeNormalize(Vector2.Zero) * Length;
            A = target - direction;
        }

        public void Recalculate()
        {
            Vector2 offset = new Vector2((float)Math.Cos(Angle), (float)Math.Sin(Angle)) * Length;
            B = A + offset;
        }

        public void Update()
        {
            Recalculate();
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            if (Texture == null)
                Texture = ModContent.Request<Texture2D>("Terraria/Images/MagicPixel").Value;

            Texture2D pixel = ModContent.Request<Texture2D>("Terraria/Images/MagicPixel").Value;
            //Vector2 scale = new Vector2(Length, 4f); // Adjust thickness as needed
            float rotation = Angle;
            Rectangle rect = new Rectangle(0, 0, 1, 1);

            // the sprite is probably placed in the wrong direction, the origin of the sprite should be in the middle of the 
            // left side rectangle and not to the right of it completely


            spriteBatch.Draw(
                texture: Texture,
                position: A - Main.screenPosition,
                sourceRectangle: null,
                color,
                rotation - MathHelper.PiOver2,
                origin: Origin,
                scale: 1f,
                SpriteEffects.None,
                0f
            );

            Vector2 textureSize = new Vector2(Texture.Width, Texture.Height);
            Rectangle boxRect = new Rectangle((int)(A.X - Main.screenPosition.X), (int)(A.Y - Main.screenPosition.Y), (int)textureSize.X, (int)textureSize.Y);

            // Draw a simple rectangle around the texture (debugging purpose)
            spriteBatch.Draw(
                texture: pixel,
                position: new Vector2(boxRect.X, boxRect.Y),
                sourceRectangle: new Rectangle(boxRect.X, boxRect.Y, boxRect.Width, (int)Length),
                color: Color.Red * 0.25f,  // You can change the color of the box
                rotation: rotation - MathHelper.PiOver2,
                origin: Vector2.Zero,
                scale: 1f, // Set the size of the box
                SpriteEffects.None,
                0f
            );
        }
    }
}