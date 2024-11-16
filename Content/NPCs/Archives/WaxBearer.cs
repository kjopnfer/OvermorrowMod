using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class WaxBearer : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name + "Body";
        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 64;
            NPC.knockBackResist = 0.8f;
            NPC.lifeMax = 100;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.damage = 15;
            NPC.aiStyle = -1;
        }


        RobotArm robotarm;
        int segLen = 15;
        int numSegs = 40;
        public override void OnSpawn(IEntitySource source)
        {
            robotarm = new RobotArm(NPC.Center.X, NPC.Center.Y, numSegs, segLen, 0);
        }

        public override void AI()
        {
            NPC.TargetClosest();
        }

        float link1Length = 48f;  // Upper arm length
        float link2Length = 48f;  // Lower arm length
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel

            robotarm.Update(Main.MouseWorld.X, Main.MouseWorld.Y);
            robotarm.Show(spriteBatch, magicPixel);

            base.PostDraw(spriteBatch, screenPos, drawColor);
        }
    }

    public class RobotArm
    {
        private Vector2 basePosition;
        private List<Segment> segments;

        public RobotArm(float x, float y, int numSegs, int segLen, float angle)
        {
            basePosition = new Vector2(x, y);
            segments = new List<Segment>
        {
            new Segment(x, y, segLen, angle, 0)
        };

            for (int i = 1; i < numSegs; i++)
            {
                AddSegment(segLen, 0);
            }
        }

        private void AddSegment(int len, float angle)
        {
            Segment lastSegment = segments[segments.Count - 1];
            Segment newSegment = new Segment(0, 0, len, angle, segments.Count);
            lastSegment.Parent = newSegment;
            segments.Add(newSegment);
            newSegment.Follow(lastSegment.A.X, lastSegment.A.Y);
        }

        public void Update(float mouseX, float mouseY)
        {
            for (int i = 0; i < segments.Count; i++)
            {
                Segment seg = segments[i];
                seg.Update();

                if (i == 0)
                {
                    seg.Follow(mouseX, mouseY);
                }
                else
                {
                    Segment previous = segments[i - 1];
                    seg.Follow(previous.A.X, previous.A.Y);
                }
            }

            int lastIndex = segments.Count - 1;
            Segment lastSegment = segments[lastIndex];
            lastSegment.A = basePosition;
            lastSegment.ReCalculate();

            for (int i = lastIndex - 1; i >= 0; i--)
            {
                Segment current = segments[i];
                Segment next = segments[i + 1];
                current.A = next.B;
                current.ReCalculate();
            }
        }

        public void Show(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            foreach (var segment in segments)
            {
                segment.Show(spriteBatch, pixelTexture);
            }
        }
    }

    public class Segment
    {
        public Vector2 A { get; set; }
        public Vector2 B { get; private set; }
        public int Length { get; private set; }
        public float Angle { get; private set; }
        public int Id { get; private set; }
        public Segment Parent { get; set; }

        public Segment(float x, float y, int len, float angle, int id)
        {
            A = new Vector2(x, y);
            Length = len;
            Angle = angle;
            Id = id;
            ReCalculate();
        }

        public Segment CreateParent(int len, float angle, int id)
        {
            Segment parent = new Segment(0, 0, len, angle, id);
            Parent = parent;
            parent.Follow(A.X, A.Y);
            return parent;
        }

        public void ReCalculate()
        {
            float dx = (float)(Math.Cos(Angle) * Length);
            float dy = (float)(Math.Sin(Angle) * Length);
            B = new Vector2(A.X + dx, A.Y + dy);
        }

        public void Follow(float targetX, float targetY)
        {
            Vector2 target = new Vector2(targetX, targetY);
            Vector2 direction = Vector2.Subtract(target, A);
            Angle = (float)Math.Atan2(direction.Y, direction.X);

            direction = Vector2.Normalize(direction) * Length;
            direction = Vector2.Multiply(direction, -1);

            A = Vector2.Add(target, direction);
        }

        public void Update()
        {
            ReCalculate();
        }

        public void Show(SpriteBatch spriteBatch, Texture2D pixelTexture)
        {
            // Example for MagicPixel: Drawing a colored line and endpoints
            // Assume `pixelTexture` is a 1x1 white pixel texture for drawing primitives.

            // Line color based on Id
            Color lineColor = Color.Lerp(Color.Red, Color.Blue, Id / 39f);

            // Draw the segment line
            DrawLine(spriteBatch, pixelTexture, A, B, lineColor, 4);

            // Draw the endpoints
            Color endpointColor = Color.White;
            DrawCircle(spriteBatch, pixelTexture, A, 2, endpointColor); // Start point
            DrawCircle(spriteBatch, pixelTexture, B, 2, endpointColor); // End point
        }

        private void DrawLine(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = (end - start);
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spriteBatch.Draw(texture, start - Main.screenPosition, new Rectangle(0, 0, 1, 4), color, angle + MathHelper.PiOver2, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        private void DrawCircle(SpriteBatch spriteBatch, Texture2D texture, Vector2 center, int radius, Color color)
        {
            // Use the texture to draw a filled circle
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        //spriteBatch.Draw(texture, center + new Vector2(x, y) - Main.screenPosition, new Rectangle(0, 0, 1, 10), Color.White, 0f, texture.Size() / 2, 1f, SpriteEffects.None, 0f);
                        //spriteBatch.Draw(texture, center + new Vector2(x, y) - Main.screenPosition, color);
                    }
                }
            }
        }
    }
}