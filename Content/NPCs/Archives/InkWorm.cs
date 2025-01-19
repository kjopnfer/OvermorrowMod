using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Core;
using System;
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
            Attack = 1,
            Wiggle = 2
        }

        private float Rotation;

        public ref float AIState => ref NPC.ai[0];

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
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, ModContent.NPCType<InkWorm>(), 0, ai0: NPC.whoAmI);
            }
        }

        public override void AI()
        {
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
        public ref float ParentID => ref NPC.ai[0];
        public ref float AIState => ref NPC.ai[1];

        float randomAnimationOffset = 0;
        public override void OnSpawn(IEntitySource source)
        {
            randomAnimationOffset = Main.rand.Next(0, 9) * 10;
            base.OnSpawn(source);
        }

        public override void AI()
        {
            NPC parent = Main.npc[(int)ParentID];
            if (!parent.active) NPC.active = false;

            // Calculate direction and distance from the parent
            Vector2 direction = NPC.Center - parent.Center;
            float distance = direction.Length();

            // Normalize direction vector
            direction.Normalize();

            // Timer for sine wave animation
            float time = (float)(Main.GameUpdateCount + randomAnimationOffset) / 20f;

            // Calculate sine wave offset for the head
            float waveOffset = (float)Math.Sin(time); // Adjust amplitude as needed

            // Perpendicular vector for the sine wave motion
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * waveOffset;

            // Final position of the head relative to the parent
            NPC.Center = parent.Center + direction * distance + perpendicular;
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC parent = Main.npc[(int)ParentID];

            // Load textures
            Texture2D body = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormBody").Value;
            Texture2D body2 = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormBody2").Value;
            Texture2D head = ModContent.Request<Texture2D>(AssetDirectory.ArchiveNPCs + "InkWormHead").Value;

            // Calculate direction and distance between the NPC and its parent
            Vector2 direction = NPC.Center - parent.Center;
            float distance = direction.Length();

            // Calculate the number of iterations based on the height of the body texture
            int bodyHeight = body.Height - 6; // Slight overlap adjustment
            int iterations = (int)(distance / bodyHeight);

            // Normalize the direction vector
            direction.Normalize();

            // Initialize the starting position at the parent's center
            Vector2 currentPosition = parent.Center;

            // Timer for sine wave animation (ensures movement over time)
            float time = (float)(Main.GameUpdateCount + randomAnimationOffset) / 20f;

            // List to store segment positions for calculating rotation
            Vector2[] segmentPositions = new Vector2[iterations + 1];
            segmentPositions[0] = currentPosition;

            // Calculate positions for all segments
            for (int i = 1; i <= iterations; i++)
            {
                // Calculate sine wave offset
                float waveOffset = (float)Math.Sin(time + i) * 3f; // Amplitude of 10f, phase offset of 0.5f per segment

                // Perpendicular vector for sine wave motion
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * waveOffset;

                // Update position with sine wave offset
                currentPosition += direction * bodyHeight + perpendicular;

                // Store the segment position
                segmentPositions[i] = currentPosition;
            }

            // Draw the body segments
            for (int i = 0; i < iterations; i++)
            {
                // Determine rotation
                Vector2 nextPosition = i < iterations - 1 ? segmentPositions[i + 1] : NPC.Center;
                Vector2 segmentDirection = nextPosition - segmentPositions[i];
                float rotation = (float)Math.Atan2(segmentDirection.Y, segmentDirection.X) + MathHelper.PiOver2;

                // Draw the body segment
                spriteBatch.Draw(body, segmentPositions[i] - Main.screenPosition, null, drawColor, rotation, new Vector2(body.Width / 2f, body.Height / 2f), NPC.scale, SpriteEffects.None, 0);
            }

            // Draw the head at the NPC's position
            spriteBatch.Draw(head, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, new Vector2(head.Width / 2f, head.Height / 2f), NPC.scale, SpriteEffects.None, 0);

            return base.DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }
    }
}