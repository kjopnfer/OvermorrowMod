using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Animations;
using Terraria.ID;
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


        RobotArm lanternArm;
        RobotArm backArm;
        RobotArm frontLeg;
        RobotArm backLeg;

        int segLen = 64;
        int numSegs = 2;
        public override void OnSpawn(IEntitySource source)
        {
            Texture2D armTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + Name + "Arm").Value;
            Texture2D legTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + Name + "Leg").Value;

            Texture2D[] armTextures = new Texture2D[]{ armTexture, armTexture};
            Texture2D[] legTextures = new Texture2D[] { legTexture, legTexture };

            float[] segmentLengths = new float[] { 60f, 120f }; // Different lengths for each segment
            lanternArm = new RobotArm(NPC.Center.X, NPC.Center.Y, 2, segmentLengths, 0, armTextures);
            backArm = new RobotArm(NPC.Center.X, NPC.Center.Y, 2, segmentLengths, 0, armTextures);
            frontLeg = new RobotArm(NPC.Center.X, NPC.Center.Y + 20, 2, segmentLengths, 0, legTextures);
            backLeg = new RobotArm(NPC.Center.X, NPC.Center.Y + 20, 2, segmentLengths, 0, legTextures);

            backArm.Update(NPC.Center + new Vector2(0, 200));
            lanternArm.Update(NPC.Center + new Vector2(0, 200));

            frontLeg.Update(NPC.Center + new Vector2(0, 200));
            backLeg.Update(NPC.Center + new Vector2(-10, 200));
        }

        public override void AI()
        {
            NPC.TargetClosest();

            lanternArm.BasePosition = NPC.Center;
            backArm.BasePosition = NPC.Center;
            frontLeg.BasePosition = NPC.Center + new Vector2(0, 20);
            backLeg.BasePosition = NPC.Center + new Vector2(0, 20);

            backArm.Update(NPC.Center + new Vector2(-80, -40));
            lanternArm.Update(NPC.Center + new Vector2(-80, 20));

            backLeg.Update(NPC.Center + new Vector2(50, 200));

            Dust.NewDust(TileUtils.FindNearestGround(Main.player[NPC.target].Center), 1, 1, DustID.Torch);
            frontLeg.Update(TileUtils.FindNearestGround(frontLeg.GetEndPosition()));

            NPC.velocity.Y = 4;

            var tileDistance = RayTracing.CastTileCollisionLength(NPC.Bottom, Vector2.UnitY, 1000);
            if (tileDistance <= 120)
            {
                NPC.velocity.Y = 0;
            }

            NPC.velocity.X = 0;
        }

        float link1Length = 48f;  // Upper arm length
        float link2Length = 48f;  // Lower arm length
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel
            backArm.Draw(spriteBatch);
            backLeg.Draw(spriteBatch);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel

            //lanternArm.Update(Main.MouseWorld.X, Main.MouseWorld.Y);
            //lanternArm.Show(spriteBatch, magicPixel);
            lanternArm.Draw(spriteBatch);
            frontLeg.Draw(spriteBatch);

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
        public Texture2D Texture { get; set; }  // Texture property for each segment

        public Segment(float x, float y, float length, float angle, Texture2D texture)
        {
            A = new Vector2(x, y);
            Length = length;
            Angle = angle;
            Texture = texture;

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
            if (Texture == null)
                Texture = ModContent.Request<Texture2D>("Terraria/Images/MagicPixel").Value;

            Texture2D pixel = ModContent.Request<Texture2D>("Terraria/Images/MagicPixel").Value;
            Vector2 scale = new Vector2(Length, 4f); // Adjust thickness as needed
            float rotation = Angle;
            Rectangle rect = new Rectangle(0, 0, 1, 1);

            spriteBatch.Draw(
                Texture,
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

        // Constructor now accepts an array of lengths for each segment
        public RobotArm(float x, float y, int numSegments, float[] segmentLengths, float initialAngle, Texture2D[] segmentTextures)
        {
            if (segmentLengths.Length != numSegments)
            {
                Main.NewText("The number of segment lengths must match the number of segments.");
            }

            BasePosition = new Vector2(x, y);
            Segments = new Segment[numSegments];

            // Create the first segment at the base
            Segments[0] = new Segment(x, y, segmentLengths[0], initialAngle, segmentTextures[0]);

            // Create the remaining segments with their respective lengths
            for (int i = 1; i < numSegments; i++)
            {
                Segments[i] = new Segment(0, 0, segmentLengths[i], 0, segmentTextures[i]);
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
            // Draw each segment, customize the color as needed
            foreach (Segment segment in Segments)
            {
                segment.Draw(spriteBatch, Color.White);
            }
        }

        /// <summary>
        /// Gets the position of the last segment's endpoint, which represents the moving limb's tip.
        /// </summary>
        /// <returns>The <see cref="Vector2"/> position of the last segment's endpoint.</returns>
        public Vector2 GetEndPosition()
        {
            // Return the 'B' position of the last segment, which is the endpoint of the last limb
            return Segments[Segments.Length - 1].B;
        }
    }
}