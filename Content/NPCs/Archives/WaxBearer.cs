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
        int segLen = 48;
        int numSegs = 2;
        public override void OnSpawn(IEntitySource source)
        {
            robotarm = new RobotArm(NPC.Center.X, NPC.Center.Y, numSegs, segLen, 0);
        }

        public override void AI()
        {
            NPC.TargetClosest();
            robotarm.BasePosition = NPC.Center;
            robotarm.Update(Main.MouseWorld);

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

            //robotarm.Update(Main.MouseWorld.X, Main.MouseWorld.Y);
            //robotarm.Show(spriteBatch, magicPixel);
            robotarm.Draw(spriteBatch);

            base.PostDraw(spriteBatch, screenPos, drawColor);
        }
    }
    public class Segment
    {
        public Vector2 A { get; set; } // Changed to allow setting externally
        public Vector2 B { get; private set; } // Only allow setting B inside the class
        public float Length { get; private set; }
        public float Angle { get; set; }
        public Segment Parent { get; set; }

        public Segment(float x, float y, float length, float angle)
        {
            A = new Vector2(x, y);
            Length = length;
            Angle = angle;
            Recalculate();
        }

        public void Follow(Vector2 target)
        {
            Vector2 direction = target - A;
            Angle = direction.ToRotation();

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
            Texture2D pixel = ModContent.Request<Texture2D>("Terraria/Images/MagicPixel").Value;
            Vector2 scale = new Vector2(Length, 4f); // Adjust thickness as needed
            float rotation = Angle;
            Rectangle rect = new Rectangle(0, 0, 1, 1);

            spriteBatch.Draw(
                pixel,
                A - Main.screenPosition,
                rect,
                color,
                rotation,
                new Vector2(0, 0.5f),
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }

    public class RobotArm
    {
        public Vector2 BasePosition { get; set; }
        public Segment[] Segments;

        public RobotArm(float x, float y, int numSegments, float segmentLength, float initialAngle)
        {
            BasePosition = new Vector2(x, y);
            Segments = new Segment[numSegments];

            Segments[0] = new Segment(x, y, segmentLength, initialAngle);
            for (int i = 1; i < numSegments; i++)
            {
                Segments[i] = new Segment(0, 0, segmentLength, 0);
                Segments[i - 1].Parent = Segments[i];
            }
        }

        public void Update(Vector2 target)
        {
            // Follow target starting from the last segment
            for (int i = Segments.Length - 1; i >= 0; i--)
            {
                if (i == Segments.Length - 1)
                {
                    Segments[i].Follow(target);
                }
                else
                {
                    Segments[i].Follow(Segments[i + 1].A);
                }
                Segments[i].Update();
            }

            // Recalculate positions starting from the base
            Segments[0].A = BasePosition;
            Segments[0].Recalculate();
            for (int i = 1; i < Segments.Length; i++)
            {
                Segments[i].A = Segments[i - 1].B;
                Segments[i].Recalculate();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color color = Color.Red; // Replace with desired coloring logic
            foreach (Segment segment in Segments)
            {
                segment.Draw(spriteBatch, color);
            }
        }
    }
}