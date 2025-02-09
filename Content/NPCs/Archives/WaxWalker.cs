using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.InverseKinematics;
using OvermorrowMod.Common.Utilities;
using System;
using System.IO.Pipelines;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class WaxWalker : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
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


        InverseKinematicLimb frontLeg;
        InverseKinematicLimb backLeg;

        int segLen = 64;
        int numSegs = 2;
        public override void OnSpawn(IEntitySource source)
        {
            Texture2D footTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxWalkerFoot", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D legTexture1 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxWalkerLeg1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D legTexture2 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxWalkerLeg2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Vector2[] origins = new Vector2[] { new Vector2(legTexture1.Width / 2, 4), new Vector2(legTexture2.Width / 2, 0), new Vector2(footTexture.Width / 2, -2) };
            Texture2D[] legTextures = new Texture2D[] { legTexture1, legTexture2, footTexture };

            #region Front Leg
            float[] legSegmentLengths = new float[] { 28f, 28f, 10f }; // Different lengths for each segment
            frontLeg = new InverseKinematicLimb(NPC.Center.X, NPC.Center.Y, legTextures.Length, legSegmentLengths, 0, legTextures, origins);

            // Set constraints for the knee joint
            frontLeg.Segments[0].MinAngle = MathHelper.PiOver2; // 90 degrees
            frontLeg.Segments[0].MaxAngle = MathHelper.Pi;  // 180 degrees

            // Set constraints for the lower leg
            frontLeg.Segments[1].MinAngle = 0; // -90 degrees
            frontLeg.Segments[1].MaxAngle = MathHelper.Pi;  // 90 degrees

            frontLeg.Segments[2].MinAngle = MathHelper.PiOver2; // -90 degrees
            frontLeg.Segments[2].MaxAngle = MathHelper.PiOver2;  // 90 degrees

            frontLeg.Update(NPC.Center + new Vector2(-40, 200));

            currentFrontLegPosition = TileUtils.FindNearestGround(NPC.Center + new Vector2(0, 200));
            nextFrontLegPosition = TileUtils.FindNearestGround(currentFrontLegPosition + new Vector2(-105, 0));

            #endregion

            #region Back Leg
            backLeg = new InverseKinematicLimb(NPC.Center.X, NPC.Center.Y, legTextures.Length, legSegmentLengths, 0, legTextures, origins);

            // Set constraints for the knee joint
            backLeg.Segments[0].MinAngle = MathHelper.PiOver2; // 90 degrees
            backLeg.Segments[0].MaxAngle = MathHelper.Pi;  // 180 degrees

            // Set constraints for the lower leg
            backLeg.Segments[1].MinAngle = 0; // -90 degrees
            backLeg.Segments[1].MaxAngle = MathHelper.Pi;  // 90 degrees

            backLeg.Segments[2].MinAngle = MathHelper.PiOver2; // -90 degrees
            backLeg.Segments[2].MaxAngle = MathHelper.PiOver2;  // 90 degrees

            backLeg.Update(NPC.Center + new Vector2(-40, 200));

            currentBackLegPosition = TileUtils.FindNearestGround(NPC.Center + new Vector2(100, 0));
            nextBackLegPosition = TileUtils.FindNearestGround(currentFrontLegPosition + new Vector2(-55, 0));
            #endregion
        }

        Vector2 startPosition;
        Vector2 targetPosition;

        Vector2 currentFrontLegPosition;
        Vector2 nextFrontLegPosition;

        Vector2 currentBackLegPosition;
        Vector2 nextBackLegPosition;

        int moveCycle = 0;

        float maxReach = 150f; // Maximum leg reach
        float cycleTime = 30f; // Time for one step
        float arcHeight = 10f; // Height of leg arc during step
        bool firstStep = true;
        int stepCount = 0;
        public override void AI()
        {
            float CYCLE_TIME = 60;
            float ARC_HEIGHT = 10;

            NPC.TargetClosest();

            NPC.velocity.Y = 5;

            // Determine if NPC is on the ground
            var tileDistance = RayTracing.CastTileCollisionLength(NPC.Bottom, Vector2.UnitY, 1000);

            float STAND_HEIGHT = 60;
            // Make sure the NPC is always a certain distance above the ground
            if (tileDistance <= STAND_HEIGHT)
            {
                if (tileDistance != STAND_HEIGHT)
                {
                    NPC.velocity.Y -= 5f;
                }
                else NPC.velocity.Y = 0;

                //NPC..Y = 0;
                NPC.ai[1]++; // Advance cycle timer
            }

            // Synchronize body velocity with leg motion
            Vector2 legDisplacement = moveCycle % 2 == 0
                ? nextBackLegPosition - currentBackLegPosition
                : nextFrontLegPosition - currentFrontLegPosition;

            NPC.velocity.X = legDisplacement.X / CYCLE_TIME;

            frontLeg.Segments[0].MinAngle = MathHelper.PiOver2; // 90 degrees
            frontLeg.Segments[0].MaxAngle = MathHelper.Pi;  // 180 degrees

            Vector2 averageLegPosition = (currentFrontLegPosition + currentBackLegPosition) / 2f;
            NPC.Center = Vector2.Lerp(NPC.Center, averageLegPosition + new Vector2(0, -STAND_HEIGHT), 0.05f);


            if (NPC.ai[1] >= CYCLE_TIME)
            {
                moveCycle++;
                stepCount++;
                if (moveCycle % 2 == 0)
                {
                    //currentBackLegPosition = nextBackLegPosition;
                    //nextBackLegPosition = ClampLegReach(TileUtils.FindNearestGround(currentBackLegPosition + new Vector2(-125, 0)), NPC.Center);
                    int stepDistance = stepCount != 0 ? -55 : -55;
                    nextBackLegPosition = TileUtils.FindNearestGround(currentBackLegPosition + new Vector2(stepDistance, 0));
                    Main.NewText("move back leg " + stepDistance + " stepCount: " + stepCount);

                    if (firstStep) firstStep = false;
                }
                else
                {
                    //currentFrontLegPosition = nextFrontLegPosition;
                    //nextFrontLegPosition = ClampLegReach(TileUtils.FindNearestGround(currentFrontLegPosition + new Vector2(-125, 0)), NPC.Center);
                    int stepDistance = stepCount != 1 ? -55 : -55;
                    nextFrontLegPosition = TileUtils.FindNearestGround(currentFrontLegPosition + new Vector2(stepDistance, 0));

                    Main.NewText("move front leg " + stepDistance + " stepCount: " + stepCount);
                }

                NPC.ai[1] = 0; // Reset cycle timer
            }
            else
            {
                float progress = Math.Clamp(NPC.ai[1] / CYCLE_TIME, 0, 1f);
                float yOffset = -ARC_HEIGHT * (float)Math.Sin((NPC.ai[1] / CYCLE_TIME) * MathHelper.Pi); // Sin(Pi) ranges from 0 -> -1 -> 0
                //if (tileDistance <= 120) NPC.velocity.X = NPC.ai[1] < 15 ? -5f : 0;

                if (moveCycle % 2 == 0)
                {
                    currentBackLegPosition = Vector2.Lerp(currentBackLegPosition, nextBackLegPosition, progress) + Vector2.UnitY * yOffset;
                    currentBackLegPosition.Y = Math.Min(currentBackLegPosition.Y + yOffset, TileUtils.FindNearestGround(currentBackLegPosition).Y);
                }
                else
                {
                    //float yOffset = MathHelper.Lerp(0, -60, yOffsetCounter);
                    currentFrontLegPosition = Vector2.Lerp(currentFrontLegPosition, nextFrontLegPosition, progress) + Vector2.UnitY * yOffset;
                    currentFrontLegPosition.Y = Math.Min(currentFrontLegPosition.Y + yOffset, TileUtils.FindNearestGround(currentFrontLegPosition).Y);
                }
            }

            frontLeg.BasePosition = NPC.Center + new Vector2(0, 20);
            frontLeg.Update(currentFrontLegPosition);

            var current = Dust.NewDustDirect(currentFrontLegPosition, 1, 1, DustID.Torch);
            current.noGravity = true;

            var next = Dust.NewDustDirect(nextFrontLegPosition, 1, 1, DustID.IceTorch);
            next.noGravity = true;

            backLeg.BasePosition = NPC.Center + new Vector2(10, 20);
            backLeg.Update(currentBackLegPosition);

            var current2 = Dust.NewDustDirect(currentBackLegPosition, 1, 1, DustID.CursedTorch);
            current2.noGravity = true;

            var next2 = Dust.NewDustDirect(nextBackLegPosition, 1, 1, DustID.HallowedTorch);
            next2.noGravity = true;

            //NPC.velocity.X = NPC.ai[1] < 30 ? -4f : 0;
        }

        float link1Length = 48f;  // Upper arm length
        float link2Length = 48f;  // Lower arm length
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel

            Color darkerColor = Color.Lerp(drawColor, Color.Black, 0.55f); // 0.2f is the factor to darken the color
            backLeg.Draw(spriteBatch, darkerColor);

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel
            frontLeg.Draw(spriteBatch, drawColor);

            //Main.NewText($"frontLeg.Segments[0] angle: {MathHelper.ToDegrees(frontLeg.Segments[0].Angle)}", Color.Red);
            DrawAngleVisualization(frontLeg.Segments[0].A - Main.screenPosition, frontLeg.Segments[0].Angle, 2f, Color.Red);

            //Main.NewText($"frontLeg.Segments[1] angle: {MathHelper.ToDegrees(frontLeg.Segments[1].Angle)}", Color.LightGreen);
            DrawAngleVisualization(frontLeg.Segments[1].A - Main.screenPosition, frontLeg.Segments[1].Angle, 2f, Color.Green);

            base.PostDraw(spriteBatch, screenPos, drawColor);
        }

        void DrawAngleVisualization(Vector2 center, float angle, float radius, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            Vector2 start = center;
            Vector2 end = center + radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            // Draw a line indicating the angle
            Main.spriteBatch.Draw(
                pixel,
                start,
                null,
                color,
                (end - start).ToRotation(),
                Vector2.Zero,
                new Vector2((end - start).Length(), 2f), // Width = 2
                SpriteEffects.None,
                0f
            );
        }

        Vector2 ClampLegReach(Vector2 legTarget, Vector2 bodyPosition)
        {
            Vector2 toTarget = legTarget - bodyPosition;
            if (toTarget.Length() > maxReach)
            {
                toTarget = toTarget.SafeNormalize(Vector2.Zero) * maxReach;
            }
            return bodyPosition + toTarget;
        }
    }
    public class Segment
    {
        public Vector2 A { get; set; } // Changed to allow setting externally
        public Vector2 B { get; private set; } // Only allow setting B inside the class
        public float Length { get; private set; }
        public float Angle { get; set; }
        public float MinAngle { get; set; } = -MathHelper.Pi; // Default: -180 degrees
        public float MaxAngle { get; set; } = MathHelper.Pi;  // Default: 180 degrees
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

            spriteBatch.Draw(
                texture: Texture,
                position: A - Main.screenPosition,
                sourceRectangle: null,
                color,
                rotation - MathHelper.PiOver2,
                origin: new Vector2(0, 0.5f),
                scale: 1f,
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