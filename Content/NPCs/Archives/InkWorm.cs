using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class InkWormBody : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
        }

        public enum AICase
        {
            Death = -2,
            Panic = -1,
            Hidden = 0,
            Extend = 1,
            Idle = 2,
            Retract = 3
        }

        private float Rotation;

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        List<NPC> tentacle = new List<NPC>();
        public override void OnSpawn(IEntitySource source)
        {
            int spawnCount = 3; // Number of NPCs to spawn
            float startAngle = MathHelper.ToRadians(-70); // Start of the arc (pi radians)
            float endAngle = MathHelper.ToRadians(70); // End of the arc (0 radians)
            float arcStep = (startAngle - endAngle) / (spawnCount - 1); // Angle step for even distribution

            for (int i = 0; i < spawnCount; i++)
            {
                // Calculate the angle for the current spawn
                float angle = startAngle - (arcStep * i);

                // Calculate the spawn position based on the angle
                Vector2 spawnOffset = new Vector2(0, -60).RotatedBy(angle);
                Vector2 spawnPosition = NPC.Center + spawnOffset;

                // Spawn the NPC at the calculated position
                tentacle.Add(NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<InkWorm>(), 0, ai0: NPC.whoAmI));
            }
        }

        public override void AI()
        {
            switch ((AICase)AIState)
            {
                case AICase.Hidden:
                    if (AICounter++ == 120)
                    {
                        AICounter = 0;
                        AIState = (int)AICase.Extend;
                    }
                    break;
                case AICase.Extend:
                    if (AICounter++ == 15)
                    {
                        AICounter = 0;
                        AIState = (int)AICase.Idle;
                    }
                    break;
                case AICase.Idle:
                    if (AICounter++ == 120)
                    {
                        AICounter = 0;
                        AIState = (int)AICase.Retract;
                    }
                    break;
                case AICase.Retract:
                    if (AICounter++ == 120)
                    {
                        AICounter = 0;
                        AIState = (int)AICase.Hidden;
                    }
                    break;
            }
        }

        private void ChangeTentacleStates(AICase state)
        {
            foreach (NPC npc in tentacle)
            {
                if (npc.ModNPC is InkWorm inkWorm)
                {
                    inkWorm.AIState = (int)state;
                }
            }
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            base.DrawNPCBestiary(spriteBatch, drawColor);
        }

        public override void DrawBehindOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.DrawBehindOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }

        public override bool CheckDead()
        {

            return true;
        }
    }

    public class InkWorm : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool CheckActive() => false;
        public override void SetDefaults()
        {
            NPC.width = NPC.height = 20;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.damage = 12;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
        }

        public override void DrawBehind(int index)
        {
            NPC.hide = true;
            Main.instance.DrawCacheNPCProjectiles.Add(index);
        }

        public enum AICase
        {
            Death = -2,
            Panic = -1,
            Hidden = 0,
            Extend = 1,
            Idle = 2,
            Retract = 3
        }

        public ref float ParentID => ref NPC.ai[0];
        public ref float AIState => ref NPC.ai[1];
        public ref float AICounter => ref NPC.ai[2];
        NPC parent => Main.npc[(int)ParentID];


        float randomAnimationOffset = 0;
        float initialDistance = 0;
        public override void OnSpawn(IEntitySource source)
        {
            randomAnimationOffset = Main.rand.Next(0, 9) * 10;
            initialDistance = (NPC.Center - parent.Center).Length();
            AIState = (int)AICase.Idle;
        }

        public override void AI()
        {
            if (!parent.active) NPC.active = false;
            InkWormBody parentState = parent.ModNPC as InkWormBody;

            Vector2 direction = NPC.Center - parent.Center;
            float distance = initialDistance /*+ MathHelper.Lerp(0, 120, AICounter++ / 20f)*/;
            NPC.Opacity = 1f;

            AIState = parentState.AIState;

            switch ((AICase)AIState)
            {
                case AICase.Hidden:
                    NPC.Opacity = 0f;
                    break;
                case AICase.Extend:
                    distance = initialDistance - MathHelper.Lerp(initialDistance, 0, parentState.AICounter / 15f);
                    break;
                case AICase.Idle:
                    // Calculate direction and distance from the parent
                    distance = initialDistance;
                    break;
                case AICase.Retract:
                    distance = initialDistance - MathHelper.Lerp(0, initialDistance, parentState.AICounter / 120f);
                    NPC.Opacity = MathHelper.Lerp(1f, 0f, (parentState.AICounter - 100) / 20f);
                    break;
            }

            direction.Normalize();

            // Timer for sine wave animation
            float time = (float)(Main.GameUpdateCount + randomAnimationOffset) / 20f;

            // Calculate sine wave offset for the head
            float waveOffset = (float)Math.Sin(time); // Adjust amplitude as needed

            // Perpendicular vector for the sine wave motion
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * waveOffset;

            NPC.rotation = direction.ToRotation();
            NPC.Center = parent.Center + direction * distance + perpendicular;
        }

        Texture2D body = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormBody", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        Texture2D head = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormHead").Value;
        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Calculate direction and distance between the NPC and its parent
            Vector2 direction = NPC.Center - parent.Center;
            float distance = direction.Length();

            // Calculate the number of iterations based on the height of the body texture
            int bodyHeight = body.Height - 6; // Slight overlap adjustment
            int iterations = Math.Min((int)(distance / bodyHeight), 1000); // Limit iterations to avoid overflow

            // Normalize the direction vector
            direction.Normalize();

            // Timer for sine wave animation (ensures movement over time)
            float time = (float)(Main.GameUpdateCount + randomAnimationOffset) / 20f;

            // Initialize the starting position at the parent's center
            Vector2 currentPosition = parent.Center;

            // List to store segment positions for calculating rotation
            Vector2[] segmentPositions = new Vector2[iterations + 1];
            segmentPositions[0] = currentPosition;

            // Precompute values that don't change inside the loop
            Vector2 perpendicularBase = new Vector2(-direction.Y, direction.X); // Base perpendicular vector

            // Calculate positions for all segments
            for (int i = 1; i <= iterations; i++)
            {
                // Calculate sine wave offset
                float waveOffset = (float)Math.Sin(time + i) * 3f; // Amplitude of 3f, phase offset for animation

                // Perpendicular vector for sine wave motion (no need to calculate inside loop)
                Vector2 perpendicular = perpendicularBase * waveOffset;

                // Update position with sine wave offset
                currentPosition += direction * bodyHeight + perpendicular;

                // Ensure we're not overflowing the segment position array
                if (i < segmentPositions.Length)
                {
                    segmentPositions[i] = currentPosition;
                }
            }

            // Precompute the rotation values outside the loop
            Vector2 previousPosition, nextPosition, segmentDirection;
            float rotation;
            for (int i = 0; i < iterations; i++)
            {
                previousPosition = segmentPositions[i];
                nextPosition = (i < iterations - 1) ? segmentPositions[i + 1] : NPC.Center;
                segmentDirection = nextPosition - previousPosition;
                rotation = (float)Math.Atan2(segmentDirection.Y, segmentDirection.X) + MathHelper.PiOver2;

                spriteBatch.Draw(body, previousPosition - Main.screenPosition, null, drawColor * NPC.Opacity, rotation, body.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }

            // Draw the head at the NPC's position
            Texture2D hand = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormHand").Value;

            // Timer for smooth oscillation
            float wiggleAmount = MathHelper.ToRadians(25);  // Maximum wiggle angle in radians

            // First hand
            float handRotation = NPC.rotation + MathHelper.ToRadians(-30) + (float)Math.Sin(time) * wiggleAmount;
            Vector2 handOffset = new Vector2(0, -18).RotatedBy(handRotation);
            spriteBatch.Draw(hand, NPC.Center + handOffset - Main.screenPosition, null, drawColor * NPC.Opacity, handRotation, hand.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            // Second hand
            handRotation = NPC.rotation + MathHelper.ToRadians(30) + (float)Math.Sin(time + MathHelper.Pi) * wiggleAmount; // Offset the sine wave for variation
            handOffset = new Vector2(0, 18).RotatedBy(handRotation);
            spriteBatch.Draw(hand, NPC.Center + handOffset - Main.screenPosition, null, drawColor * NPC.Opacity, handRotation, hand.Size() / 2f, NPC.scale, SpriteEffects.FlipVertically, 0);

            // Third hand
            handRotation = NPC.rotation + MathHelper.ToRadians(-90) + (float)Math.Sin(time + MathHelper.PiOver2) * wiggleAmount; // Different phase offset for variety
            handOffset = new Vector2(0, 18).RotatedBy(handRotation);
            spriteBatch.Draw(hand, NPC.Center + handOffset - Main.screenPosition, null, drawColor * NPC.Opacity, handRotation, hand.Size() / 2f, NPC.scale, SpriteEffects.FlipVertically, 0);

            spriteBatch.Draw(head, NPC.Center - Main.screenPosition, null, drawColor * NPC.Opacity, NPC.rotation, head.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            return base.DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }

    }
}