using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
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
    public class WaxBearer : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + "Waxhead";
        public override void SetDefaults()
        {
            NPC.width = 65;
            NPC.height = 160;
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

        int nextFrontLegOffset = 75;
        int nextBackLegOffset = 75;
        public override void OnSpawn(IEntitySource source)
        {
            Texture2D armTexture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + Name + "Arm", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D legTexture1 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassLeg1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D legTexture2 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "BrassLeg2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2[] origins = new Vector2[] { new Vector2(legTexture1.Width / 2, 10), new Vector2(legTexture2.Width / 2, 10) };

            Texture2D[] armTextures = new Texture2D[] { armTexture, armTexture };
            Texture2D[] legTextures = new Texture2D[] { legTexture1, legTexture2 };

            float[] segmentLengths = new float[] { 60f, 120f }; // Different lengths for each segment
            lanternArm = new RobotArm(NPC.Center.X, NPC.Center.Y, 2, segmentLengths, 0, armTextures);
            backArm = new RobotArm(NPC.Center.X, NPC.Center.Y, 2, segmentLengths, 0, armTextures);

            #region Front Leg
            float[] legSegmentLengths = new float[] { 75f, 88f }; // Different lengths for each segment
            frontLeg = new RobotArm(NPC.Center.X, NPC.Center.Y, 2, legSegmentLengths, 0, legTextures, origins);

            // Set constraints for the knee joint
            frontLeg.Segments[0].MinAngle = MathHelper.PiOver2; // 90 degrees
            frontLeg.Segments[0].MaxAngle = MathHelper.Pi;  // 180 degrees

            // Set constraints for the lower leg
            //frontLeg.Segments[1].MinAngle = -MathHelper.PiOver2; // -90 degrees
            //frontLeg.Segments[1].MaxAngle = MathHelper.PiOver2;  // 90 degrees
            frontLeg.Segments[1].MinAngle = 0; // -90 degrees
            frontLeg.Segments[1].MaxAngle = MathHelper.PiOver2;  // 90 degrees

            // Set initial target to straight downwards
            frontLeg.Update(NPC.Center + new Vector2(0, 300));

            startFrontLegPosition = TileUtils.FindNearestGround(NPC.Center + new Vector2(0, 300));
            nextFrontLegPosition = TileUtils.FindNearestGround(startFrontLegPosition + new Vector2(-nextFrontLegOffset, 0));

            #endregion

            #region Back Leg

            backLeg = new RobotArm(NPC.Center.X, NPC.Center.Y, 2, legSegmentLengths, 0, legTextures, origins);

            // Set constraints for the knee joint
            backLeg.Segments[0].MinAngle = MathHelper.PiOver2; // 90 degrees
            backLeg.Segments[0].MaxAngle = MathHelper.Pi;  // 180 degrees

            // Set constraints for the lower leg
            backLeg.Segments[1].MinAngle = -MathHelper.PiOver2; // -90 degrees
            backLeg.Segments[1].MaxAngle = MathHelper.Pi;  // 90 degrees


            // Set initial target to straight downwards
            backLeg.Update(NPC.Center + new Vector2(0, 300));

            nextBackLegOffset = 100;

            currentBackLegPosition = TileUtils.FindNearestGround(NPC.Center + new Vector2(0, 300));
            nextBackLegPosition = TileUtils.FindNearestGround(currentBackLegPosition + new Vector2(-nextBackLegOffset, 0));
            #endregion

            //backArm.Update(NPC.Center + new Vector2(0, 200));
            //lanternArm.Update(NPC.Center + new Vector2(0, 200));
        }

        Vector2 startPosition;
        Vector2 targetPosition;

        Vector2 startFrontLegPosition;
        Vector2 currentFrontLegPosition;
        Vector2 nextFrontLegPosition;

        Vector2 currentBackLegPosition;
        Vector2 nextBackLegPosition;

        Vector2 currentCenterPosition;
        Vector2 nextCenterPosition;

        int moveCycle = 0;

        float maxReach = 150f; // Maximum leg reach
        float cycleTime = 30f; // Time for one step
        float arcHeight = 10f; // Height of leg arc during step
        bool firstStep = true;
        int stepCount = 0;

        float tileDistance => RayTracing.CastTileCollisionLength(NPC.Bottom, Vector2.UnitY, 1000);
        bool isOnGround = false;

        public ref float AIState => ref NPC.ai[0];
        public enum AICase
        {
            Idle = 0,
            Walk = 1,
        }

        public override void AI()
        {
            float CYCLE_TIME = 60;
            float ARC_HEIGHT = 10;

            //NPC.TargetClosest();
            NPC.direction = -1;

            lanternArm.BasePosition = NPC.Center;
            backArm.BasePosition = NPC.Center;

            backArm.Update(NPC.Center + new Vector2(-80, -40));
            lanternArm.Update(NPC.Center + new Vector2(-80, 20));

            int LEG_OFFSET = -5 * NPC.direction;
            frontLeg.BasePosition = NPC.Center + new Vector2(LEG_OFFSET, 54);
            backLeg.BasePosition = NPC.Center + new Vector2(LEG_OFFSET, 54);

            HandleGravity();

            switch ((AICase)AIState)
            {
                case AICase.Idle:
                    Neutral();
                    if (NPC.ai[1]++ == 60)
                    {
                        Main.NewText("switch to moving");
                        AIState = (int)AICase.Walk;
                        NPC.ai[1] = 0;
                    }
                    break;
                case AICase.Walk:
                    if (WalkCycle(1))
                    {
                        // TEMP COUNTER
                        Main.NewText("switch to idle");

                        // TEMP REMOVE THIS LATER
                        NPC.Center = currentCenterPosition;

                        NPC.ai[1] = 0;
                        AIState = (int)AICase.Idle;

                    }

                    break;
            }
            //WalkCycle(1);

            #region Debug
            var start = Dust.NewDustDirect(startFrontLegPosition, 1, 1, DustID.RedTorch);
            start.noGravity = true;

            var next = Dust.NewDustDirect(nextFrontLegPosition, 1, 1, DustID.RedTorch);
            next.noGravity = true;

            /*var current2 = Dust.NewDustDirect(currentBackLegPosition, 1, 1, DustID.BlueTorch);
            current2.noGravity = true;

            var next2 = Dust.NewDustDirect(nextBackLegPosition, 1, 1, DustID.BlueTorch);
            next2.noGravity = true;*/
            #endregion
            return;

            NPC.velocity.Y = 4;

            // Determine if NPC is on the ground
            float STAND_HEIGHT = 210;
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
                : nextFrontLegPosition - startFrontLegPosition;

            NPC.velocity.X = legDisplacement.X / CYCLE_TIME;

            frontLeg.Segments[0].MinAngle = MathHelper.PiOver2; // 90 degrees
            frontLeg.Segments[0].MaxAngle = MathHelper.Pi;  // 180 degrees

            Vector2 averageLegPosition = (startFrontLegPosition + currentBackLegPosition) / 2f;
            NPC.Center = Vector2.Lerp(NPC.Center, averageLegPosition + new Vector2(0, -STAND_HEIGHT), 0.2f);

            /*// STEP HAS TO GO SMALL, LARGE, LARGE, LARGE, LARGE... IN ORDER FOR IT TO NOT SHUFFLE LIKE A FUCKING IDIOT
            if (NPC.ai[1] >= CYCLE_TIME)
            {
                moveCycle++;
                stepCount++;
                if (moveCycle % 2 == 0)
                {
                    //currentBackLegPosition = nextBackLegPosition;
                    //nextBackLegPosition = ClampLegReach(TileUtils.FindNearestGround(currentBackLegPosition + new Vector2(-125, 0)), NPC.Center);
                    int stepDistance = stepCount != 0 ? -305 : -205;
                    nextBackLegPosition = TileUtils.FindNearestGround(currentBackLegPosition + new Vector2(stepDistance, 0));
                    Main.NewText("move back leg " + stepDistance + " stepCount: " + stepCount);

                    if (firstStep) firstStep = false;
                }
                else
                {
                    //currentFrontLegPosition = nextFrontLegPosition;
                    //nextFrontLegPosition = ClampLegReach(TileUtils.FindNearestGround(currentFrontLegPosition + new Vector2(-125, 0)), NPC.Center);
                    int stepDistance = stepCount != 1 ? -305 : -205;
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
            }*/

            //int LEG_OFFSET = 10 * NPC.direction;
            frontLeg.BasePosition = NPC.Center + new Vector2(LEG_OFFSET, 48);
            frontLeg.Update(startFrontLegPosition);

            backLeg.BasePosition = NPC.Center + new Vector2(LEG_OFFSET, 50);
            backLeg.Update(currentBackLegPosition);
        }

        private void Neutral()
        {
            if (isOnGround)
            {
                currentCenterPosition = NPC.Center;
                nextCenterPosition = NPC.Center + new Vector2(nextFrontLegOffset * NPC.direction, 0);
            }

            frontLeg.Update(NPC.Center + new Vector2(0, 300));
            //frontLeg.Update(Main.MouseWorld);

            backLeg.Update(currentBackLegPosition);
        }

        private bool WalkCycle(int steps)
        {
            float CYCLE_TIME = 120;
            float ARC_HEIGHT = -10;


            //float CYCLE_TIME = 60;
            //if (NPC.ai[1] >= CYCLE_TIME) return true;

            // if i want to move the front leg forwards, have it move 50 pixels away from where the back leg is
            // if i want to move the back leg forwards, have it move 50 pixels away from where the front leg is
            if (isOnGround) NPC.ai[1]++;


            bool isFrontLeg = stepCount % 2 == 0;
            if (isFrontLeg)
            {
                float delay = 10;

                // Step 2: After 10 ticks, move both vertically and horizontally
                float progress = Math.Clamp((NPC.ai[1]) / (CYCLE_TIME), 0f, 1f); // Progress after 10 ticks
                                                                                 //float yOffset = -ARC_HEIGHT * (float)Math.Sin(MathHelper.Pi * (NPC.ai[1] / CYCLE_TIME)); // Full arc motion

                //float yOffset = -ARC_HEIGHT * (float)Math.Sin(MathHelper.Pi * progress);  // Sin function to create an up-down arc
                float yOffset = ARC_HEIGHT * (float)Math.Sin((NPC.ai[1] / CYCLE_TIME) * MathHelper.Pi); // Sin(Pi) ranges from 0 -> 1 -> 0

                Main.NewText(yOffset);
                // Move horizontally from the current position towards the next position
                currentFrontLegPosition = Vector2.Lerp(startFrontLegPosition, nextFrontLegPosition, progress);
                //currentFrontLegPosition = new Vector2(0, yOffset);

                var target = currentFrontLegPosition + new Vector2(0, 200).RotatedBy(MathHelper.Lerp(0, MathHelper.PiOver2, (float)Math.Sin((NPC.ai[1] / CYCLE_TIME) * MathHelper.Pi)));
                var start = Dust.NewDustDirect(target, 1, 1, DustID.GreenTorch);
                start.noGravity = true;

                // Update the front leg's position with both horizontal and vertical movement
                frontLeg.Update(target);
                //frontLeg.Update(Main.MouseWorld);

                // Update NPC's center (for walking)
                NPC.Center = Vector2.Lerp(currentCenterPosition, nextCenterPosition, progress);


                backLeg.Update(currentBackLegPosition);
            }
            else
            {
                backLeg.Update(nextBackLegPosition);
            }

            return NPC.ai[1] >= CYCLE_TIME;
        }

        private void HandleGravity()
        {
            isOnGround = false;
            float STAND_HEIGHT = 130;

            NPC.velocity.Y = 4;
            if (tileDistance <= STAND_HEIGHT)
            {
                if (tileDistance != STAND_HEIGHT)
                {
                    NPC.velocity.Y -= 5f;
                }
                else NPC.velocity.Y = 0;

                isOnGround = true;
            }
        }

        float link1Length = 48f;  // Upper arm length
        float link2Length = 48f;  // Lower arm length
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Calculate a darker shade of the drawColor for backArm and backLeg
            Color darkerColor = Color.Lerp(drawColor, Color.Black, 0.55f); // 0.2f is the factor to darken the color

            backArm.Draw(spriteBatch, Color.White);
            backLeg.Draw(spriteBatch, darkerColor);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 spriteOffset = new Vector2(14, -16);
            spriteBatch.Draw(texture, NPC.Center + spriteOffset - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel

            lanternArm.Draw(spriteBatch, Color.White);
            frontLeg.Draw(spriteBatch, drawColor);

            //Main.NewText($"frontLeg.Segments[0] angle: {MathHelper.ToDegrees(frontLeg.Segments[0].Angle)}", Color.Red);
            DrawAngleVisualization(frontLeg.Segments[0].A - Main.screenPosition, frontLeg.Segments[0].Angle, 2f, Color.Red);

            //Main.NewText($"frontLeg.Segments[1] angle: {MathHelper.ToDegrees(frontLeg.Segments[1].Angle)}", Color.LightBlue);
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
        public Vector2 Origin { get; set; }

        public Segment(float x, float y, float length, float angle, Texture2D texture, Vector2? origin = null, Vector2? textureOffset = null)
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

    public class RobotArm
    {
        public Vector2 BasePosition { get; set; }
        public Segment[] Segments;

        // Constructor now accepts an array of lengths for each segment
        public RobotArm(float x, float y, int numSegments, float[] segmentLengths, float initialAngle, Texture2D[] segmentTextures, Vector2[] origins = null)
        {
            if (segmentLengths.Length != numSegments || (origins != null && origins.Length != numSegments))
            {
                Main.NewText("The number of segment lengths, origins, and offsets must match the number of segments.");
            }

            BasePosition = new Vector2(x, y);
            Segments = new Segment[numSegments];

            // Default origins and offsets if not provided
            origins = origins ?? new Vector2[numSegments]; // Default to Vector2.Zero

            // Create the first segment at the base
            Segments[0] = new Segment(x, y, segmentLengths[0], initialAngle, segmentTextures[0], origins[0]);

            // Create the remaining segments with their respective lengths
            for (int i = 1; i < numSegments; i++)
            {
                Segments[i] = new Segment(0, 0, segmentLengths[i], 0, segmentTextures[i], origins[i]);
                Segments[i - 1].Parent = Segments[i];
            }
        }

        /// <summary>
        /// Moves it towards this position.
        /// </summary>
        /// <param name="target"></param>
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

        public void Draw(SpriteBatch spriteBatch, Color drawColor)
        {
            // Draw each segment, customize the color as needed
            foreach (Segment segment in Segments)
            {
                segment.Draw(spriteBatch, drawColor);
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