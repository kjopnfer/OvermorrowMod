using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class InkWormBody : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

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
            NPC.dontTakeDamage = true;
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

        List<NPC> inkTentacles = new List<NPC>();
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
                inkTentacles.Add(NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<InkWorm>(), 0, ai0: NPC.whoAmI));
            }
        }

        public override void AI()
        {
            int activeTentacles = inkTentacles.Count(tentacle => tentacle.active);
            if (activeTentacles == 0)
            {
                Main.NewText("all dead");
                NPC.life = 0;
                NPC.HitEffect(0, 0);
                NPC.checkDead();
            }

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
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => (AICase)AIState != AICase.Panic && (AICase)AIState != AICase.Death;

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 20;
            NPC.lifeMax = 120;
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
        NPC Parent => Main.npc[(int)ParentID];


        float randomAnimationOffset = 0;
        float initialDistance = 0;
        private float persistentTime = 0;

        public override void OnSpawn(IEntitySource source)
        {
            randomAnimationOffset = Main.rand.Next(0, 9) * 10;
            initialDistance = (NPC.Center - Parent.Center).Length();
            persistentTime = randomAnimationOffset / 20f;

            AIState = (int)AICase.Idle;
        }

        public override void AI()
        {
            if (!Parent.active)
            {
                NPC.active = false;
                return;
            }

            InkWormBody parentState = Parent.ModNPC as InkWormBody;

            Vector2 direction = NPC.Center - Parent.Center;
            float distance = initialDistance;

            NPC.Opacity = 1f;
            NPC.dontTakeDamage = true;

            // Timer for sine wave animation
            if ((AICase)AIState != AICase.Death)
                persistentTime = (float)(Main.GameUpdateCount + randomAnimationOffset) / 20f;

            if ((AICase)AIState != AICase.Panic && (AICase)AIState != AICase.Death)
                AIState = parentState.AIState;

            float amplitude = 1;
            switch ((AICase)AIState)
            {
                case AICase.Death:
                    // Doesn't like zero for some reason
                    amplitude = 0.01f;

                    if (AICounter++ >= 120)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 0);
                        NPC.checkDead();
                    }
                    break;
                case AICase.Panic:
                    amplitude = MathHelper.Lerp(3, 5, AICounter / 150f);

                    if (AICounter++ >= 150)
                    {
                        AICounter = 0;
                        AIState = (float)AICase.Death;
                    }
                    break;
                case AICase.Hidden:
                    NPC.Opacity = 0f;
                    break;
                case AICase.Extend:
                    distance = initialDistance - MathHelper.Lerp(initialDistance, 0, parentState.AICounter / 15f);
                    break;
                case AICase.Idle:
                    // Calculate direction and distance from the parent
                    distance = initialDistance;
                    NPC.dontTakeDamage = false;
                    break;
                case AICase.Retract:
                    distance = initialDistance - MathHelper.Lerp(0, initialDistance, parentState.AICounter / 120f);
                    NPC.Opacity = MathHelper.Lerp(1f, 0f, (parentState.AICounter - 100) / 20f);
                    break;
            }

            direction.Normalize();

            // Calculate sine wave offset for the head
            float waveOffset = (float)Math.Sin(persistentTime) * amplitude;

            // Perpendicular vector for the sine wave motion
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * waveOffset;

            NPC.rotation = direction.ToRotation();
            NPC.Center = Parent.Center + direction * distance + perpendicular;
        }

        public override bool CheckDead()
        {
            if ((AICase)AIState == AICase.Death) return true;

            if ((AICase)AIState != AICase.Panic)
            {
                AIState = (float)AICase.Panic;

                NPC.life = NPC.lifeMax;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;

                return false;
            }

            return (AICase)AIState == AICase.Panic;
        }

        Texture2D hand = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormHand", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        Texture2D body = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormBody", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        Texture2D head = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormHead", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Calculate direction and distance between the NPC and its parent
            Vector2 direction = NPC.Center - Parent.Center;
            float distance = direction.Length();

            // Calculate the number of iterations based on the height of the body texture
            int bodyHeight = body.Height - 6; // Slight overlap adjustment
            int iterations = Math.Min((int)(distance / bodyHeight), 1000); // Limit iterations to avoid overflow

            // Normalize the direction vector
            direction.Normalize();

            // Initialize the starting position at the parent's center
            Vector2 currentPosition = Parent.Center;

            // List to store segment positions for calculating rotation
            Vector2[] segmentPositions = new Vector2[iterations + 1];
            segmentPositions[0] = currentPosition;

            // Precompute values that don't change inside the loop
            Vector2 perpendicularBase = new Vector2(-direction.Y, direction.X); // Base perpendicular vector

            // Calculate positions for all segments
            for (int i = 1; i <= iterations; i++)
            {
                // Calculate sine wave offset
                float waveOffset = (float)Math.Sin(persistentTime + i) * 3f; // Amplitude of 3f, phase offset for animation

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

            DrawWigglingHand(spriteBatch, drawColor, persistentTime, NPC.rotation + MathHelper.ToRadians(-30), -18, SpriteEffects.None);
            DrawWigglingHand(spriteBatch, drawColor, persistentTime + MathHelper.Pi, NPC.rotation + MathHelper.ToRadians(30), 18, SpriteEffects.FlipVertically);
            DrawWigglingHand(spriteBatch, drawColor, persistentTime + MathHelper.PiOver2, NPC.rotation + MathHelper.ToRadians(-90), 18, SpriteEffects.FlipVertically);

            spriteBatch.Draw(head, NPC.Center - Main.screenPosition, null, drawColor * NPC.Opacity, NPC.rotation, head.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

            return base.DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }

        private void DrawWigglingHand(SpriteBatch spriteBatch, Color drawColor, float time, float baseRotation, float offsetY, SpriteEffects spriteEffects)
        {
            float wiggleAmount = MathHelper.ToRadians(25);
            float handRotation = baseRotation + (float)Math.Sin(time) * wiggleAmount;
            Vector2 handOffset = new Vector2(0, offsetY).RotatedBy(handRotation);

            spriteBatch.Draw(hand, NPC.Center + handOffset - Main.screenPosition, null, drawColor * NPC.Opacity, handRotation, hand.Size() / 2f, NPC.scale, spriteEffects, 0);
        }
    }
}