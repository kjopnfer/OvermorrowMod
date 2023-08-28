using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using System.Collections.Generic;
using OvermorrowMod.Content.Dusts;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class MiniServant : ModNPC, ITrailEntity
    {
        private float randomAmplitude;
        private float rotateDirection = 1;
        private float moveSpeed;
        private float turnResistance;

        public Player latchPlayer;
        private Vector2 latchPoint;
        private int latchCounter = 0;

        private bool isLeft = false;
        private bool isRight = false;

        private int lineOffset;

        public bool shadowForm = false;
        public int shadowCounter = 0;
        public Color TrailColor(float progress) => AICase == (int)AIStates.Latch ? Color.Transparent : Color.Black;
        public float TrailSize(float progress) => 16;
        public Type TrailType() => typeof(LightningTrail);

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Drainer of Cthulhu");
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.TrailCacheLength[NPC.type] = 100;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
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

        public enum AIStates
        {
            Fly = 0,
            Latch = 1,
            ShakeOff = 2
        }
        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref float HealCounter => ref NPC.ai[2];
        public ref float ParentID => ref NPC.ai[3];
        public override void OnSpawn(IEntitySource source)
        {
            moveSpeed = Main.rand.Next(12, 15) * 2;
            turnResistance = Main.rand.Next(16, 19) * 5;
            rotateDirection = Main.rand.NextBool() ? 1 : -1;
            randomAmplitude = Main.rand.NextFloat(0.1f, 0.25f);
            lineOffset = (Main.rand.Next(-24, 14) + 5) * 20;
        }

        public List<float> particles = new List<float>();

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            NPC parentNPC = Main.npc[(int)ParentID];

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
            }

            NPC.dontTakeDamage = AICase == (int)AIStates.Latch;

            if (shadowCounter > 0 && shadowForm)
            {
                NPC.dontTakeDamage = true;
                shadowCounter--;
            }
            else if (shadowCounter == 0 && shadowForm)
            {
                for (int i = 0; i < Main.rand.Next(12, 16); i++)
                {
                    Vector2 randomSpeed = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(1, 4);
                    Dust.NewDust(NPC.Center, 2, 2, ModContent.DustType<ShadowForm>(), randomSpeed.X, randomSpeed.Y, 0, default, Main.rand.Next(1, 3));
                }

                NPC.dontTakeDamage = false;
                shadowForm = false;
                shadowCounter = 0;
            }

            switch (AICase)
            {
                case (int)AIStates.Fly:
                    NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver2;

                    if (++AICounter < 150 && !player.dead)
                    {
                        NPC.Move(player.Center, moveSpeed, turnResistance);
                    }
                    else if (AICounter == 150)
                    {
                        rotateDirection = Main.rand.NextBool() ? 1 : -1;
                        NPC.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(MathHelper.PiOver4) * (moveSpeed / 2);
                    }

                    NPC.velocity = NPC.velocity.RotatedBy(Math.Sin(AICounter * randomAmplitude) * randomAmplitude * rotateDirection);

                    if (AICounter == 210) AICounter = 0;

                    foreach (Player latchTarget in Main.player)
                    {
                        if (!latchTarget.active) continue;

                        if (NPC.Hitbox.Intersects(latchTarget.getRect()))
                        {
                            latchPoint = NPC.Center - player.Center;
                            latchPlayer = player;

                            AICase = (int)AIStates.Latch;
                            AICounter = 0;
                            NPC.localAI[0] = 0;
                        }
                    }
                    break;
                case (int)AIStates.Latch:
                    NPC.Center = latchPlayer.Center + latchPoint;

                    // First damage the player and add a particle
                    if (HealCounter++ % 240 == 0)
                    {
                        particles.Add(0f);

                        /*if (parentNPC.active && parentNPC.type == NPCID.EyeofCthulhu)
                        {
                            parentNPC.HealEffect(10);
                            parentNPC.life += 10;
                        }*/

                    }
                    else if (HealCounter % 240 == 180) // and after a second has passed( particle has touched eoc)
                    {
                        // heal eoc
                        if (parentNPC.active && parentNPC.type == NPCID.EyeofCthulhu)
                        {
                            parentNPC.HealEffect(10);
                            parentNPC.life += 10;
                        }

                        latchPlayer.HurtDirect(PlayerDeathReason.ByCustomReason(latchPlayer.name + " was reduced to a husk."), 10, false, true);
                    }

                    #region Shake Off Detection
                    // The reason that we don't use the control keys for left/right is to account for bottle-type accessories
                    if (player.direction == 1 && !isRight)
                    {
                        isRight = true;
                        isLeft = false;

                        latchCounter += Main.rand.Next(3, 6);
                    }

                    if (player.direction == -1 && !isLeft)
                    {
                        isLeft = true;
                        isRight = false;

                        latchCounter += Main.rand.Next(3, 6);
                    }

                    if (latchPlayer.justJumped)
                    {
                        latchCounter += Main.rand.Next(4, 7);
                    }

                    if (latchCounter > 0 && AICounter++ % 3 == 0) latchCounter -= Main.rand.Next(1, 3);
                    if (latchCounter < 0) latchCounter = 0;

                    if (latchCounter >= 25 || player.wet || player.HasBuff(BuffID.OnFire) || latchPlayer.statLife <= 0)
                    {
                        NPC.velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 8;
                        lineOffset = (Main.rand.Next(-15, 14) + 5) * 20;
                        latchCounter = 0;

                        AICase = (int)AIStates.ShakeOff;
                        AICounter = 0;
                        HealCounter = 0;
                    }
                    #endregion
                    break;
                case (int)AIStates.ShakeOff:
                    NPC.velocity = NPC.velocity.RotatedBy(Math.Sin(AICounter * randomAmplitude) * randomAmplitude * rotateDirection);

                    if (AICounter++ == 90)
                    {
                        AICase = (int)AIStates.Fly;
                        AICounter = 0;
                    }
                    break;
            }

            // update every particle
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i] += 1 / 180f;
                if (particles[i] >= 1)
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (++NPC.frameCounter % 5 == 0) NPC.frame.Y += frameHeight;

            if (NPC.frame.Y >= frameHeight * 2) NPC.frame.Y = 0;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            #region Boss Line
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/LineIndicator").Value;

            NPC parent = Main.npc[(int)ParentID];
            foreach (NPC npc in Main.npc)
            {
                if (npc.type != NPCID.EyeofCthulhu) continue;

                parent = npc;
            }

            if (parent.type == NPCID.EyeofCthulhu && AICase == (int)AIStates.Latch)
            {
                Vector2 mountedCenter = parent.Center;
                var drawPosition = NPC.Center;

                var remainingVectorToParent = mountedCenter - drawPosition;
                float rotation = remainingVectorToParent.ToRotation() - MathHelper.PiOver2;

                float CHAIN_LENGTH = 1;
                float distance = Vector2.Distance(parent.Center, NPC.Center);
                float iterations = distance / CHAIN_LENGTH;
                Vector2 midPoint1 = parent.Center + new Vector2(lineOffset, distance / 3).RotatedBy(parent.rotation - MathHelper.PiOver4);
                Vector2 midPoint2 = NPC.Center + new Vector2(0, -distance / 3).RotatedBy(NPC.rotation);

                float alpha = MathHelper.Lerp(0, 0.1f, (float)Math.Sin(NPC.localAI[0]++ / 60f) / 2 + 0.5f);
                for (int i = 0; i < iterations; i++)
                {
                    float progress = i / iterations;

                    Vector2 position = ModUtils.Bezier(parent.Center, NPC.Center, midPoint1, midPoint2, progress);
                    Main.EntitySpriteDraw(texture, position - Main.screenPosition, null, Color.Orange * alpha, rotation, texture.Size() / 2, 1f, SpriteEffects.None, 0);
                }

                // draw a glow in the area the particles are at
                Texture2D tex2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
                for (int i = 0; i < particles.Count; i++)
                {
                    Vector2 pos = ModUtils.Bezier(parent.Center, NPC.Center, midPoint1, midPoint2, ModUtils.EaseOutQuad(1f - particles[i]));
                    Main.EntitySpriteDraw(tex2, pos - Main.screenPosition, null, Color.Orange, 0f, tex2.Size() / 2, 0.15f, SpriteEffects.None, 0);
                }
            }
            #endregion
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/MiniServant_Glow").Value;
            Color color = Color.Lerp(Color.White, Color.Transparent, NPC.alpha / 255f);

            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            if (shadowForm)
            {
                Texture2D glow2 = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/MiniServant_Glow2").Value;

                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 drawPos = NPC.oldPos[k] + NPC.Size / 2 - Main.screenPosition;
                    var trailLength = ProjectileID.Sets.TrailCacheLength[NPC.type];
                    var fadeMult = 1f / trailLength;
                    Color afterImageColor = Color.White * (1f - fadeMult * k);

                    spriteBatch.Draw(glow2, drawPos, NPC.frame, afterImageColor, NPC.oldRot[k] + MathHelper.PiOver2, NPC.frame.Size() / 2f, NPC.scale * (trailLength - k) / trailLength, SpriteEffects.None, 0f);
                }
            }
        }
    }
}