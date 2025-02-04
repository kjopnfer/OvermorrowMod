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
            NPC.width = 65;
            NPC.height = 80;
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

        int nextFrontLegOffset = 75;
        int nextBackLegOffset = 75;
        public override void OnSpawn(IEntitySource source)
        {
            Texture2D legTexture1 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxWalkerLeg1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D legTexture2 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "WaxWalkerLeg2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2[] origins = new Vector2[] { new Vector2(legTexture1.Width / 2, 10), new Vector2(legTexture2.Width / 2, 10) };

            Texture2D[] legTextures = new Texture2D[] { legTexture1, legTexture2 };
 
            #region Front Leg
            float[] legSegmentLengths = new float[] { 28f, 24f }; // Different lengths for each segment
            frontLeg = new InverseKinematicLimb(NPC.Center.X, NPC.Center.Y, 2, legSegmentLengths, 0, legTextures, origins);

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

            nextFrontLegOffset = 50;
            startFrontLegPosition = TileUtils.FindNearestGround(NPC.Center + new Vector2(0, 300));
            nextFrontLegPosition = TileUtils.FindNearestGround(startFrontLegPosition + new Vector2(-nextFrontLegOffset, 0));

            #endregion

            #region Back Leg

            backLeg = new InverseKinematicLimb(NPC.Center.X, NPC.Center.Y, 2, legSegmentLengths, 0, legTextures, origins);

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

            int LEG_OFFSET = -10 * NPC.direction;
            frontLeg.BasePosition = NPC.Center + new Vector2(LEG_OFFSET, 12);
            backLeg.BasePosition = NPC.Center + new Vector2(LEG_OFFSET, 12);

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
            float STAND_HEIGHT = 24;

            NPC.velocity.Y = 4;
            if (tileDistance <= STAND_HEIGHT)
            {
                if (tileDistance != STAND_HEIGHT)
                {
                    NPC.velocity.Y -= 4f;
                }
                else NPC.velocity.Y = 0;

                isOnGround = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Calculate a darker shade of the drawColor for backArm and backLeg
            Color darkerColor = Color.Lerp(drawColor, Color.Black, 0.55f); // 0.2f is the factor to darken the color

            backLeg.Draw(spriteBatch, darkerColor);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Vector2 spriteOffset = new Vector2(14, -16);
            spriteBatch.Draw(texture, NPC.Center + spriteOffset - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D magicPixel = TextureAssets.MagicPixel.Value; // Load your MagicPixel

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
}