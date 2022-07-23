using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.Dusts;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class MiniServant : ModNPC, ITrailEntity
    {
        private float randomAmplitude;
        private float rotateDirection = 1;
        private float moveSpeed;
        private float turnResistance;
        private Vector2 positionOffset;
        public Color TrailColor(float progress) => Color.Black;
        public float TrailSize(float progress) => 16;
        public Type TrailType() => typeof(LightningTrail);

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drainer of Cthulhu");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 16;
            NPC.lifeMax = 10;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
        }

        public ref float AICounter => ref NPC.ai[1];
        public override void OnSpawn(IEntitySource source)
        {
            moveSpeed = Main.rand.Next(12, 15) * 2;
            turnResistance = Main.rand.Next(16, 19) * 5;
            positionOffset = new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)) * 10;
            rotateDirection = Main.rand.NextBool() ? 1 : -1;
            randomAmplitude = Main.rand.NextFloat(0.1f, 0.25f);
        }

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];

            NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

            if (++AICounter < 150)
            {
                NPC.Move(player.Center, moveSpeed, turnResistance);
            }
            else if (AICounter == 150)
            {
                rotateDirection = Main.rand.NextBool() ? 1 : -1;
                NPC.velocity = NPC.velocity.RotatedByRandom(MathHelper.PiOver4) * 2;
            }

            NPC.velocity = NPC.velocity.RotatedBy(Math.Sin(AICounter * randomAmplitude) * randomAmplitude * rotateDirection);

            if (AICounter == 210) AICounter = 0;
        }

        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0) NPC.frame.Y += frameHeight;

            if (NPC.frame.Y >= frameHeight * 2) NPC.frame.Y = 0;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/MiniServant_Glow").Value;
            Color color = Color.Lerp(Color.White, Color.Transparent, NPC.alpha / 255f);

            spriteBatch.Draw(glow, NPC.Center - screenPos, null, color, NPC.rotation + MathHelper.PiOver2, glow.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        }
    }
}