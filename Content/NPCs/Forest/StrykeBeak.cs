using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace OvermorrowMod.Content.NPCs.Forest
{
    public class StrykeBeak : ModNPC
    {
        private const int MAX_FRAMES = 8;
        public override bool? CanBeHitByItem(Player player, Item item) => true;
        public override bool? CanBeHitByProjectile(Projectile projectile) => true;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            switch (AIState)
            {
                case (int)AICase.Dive:
                    return AICounter >= 60 && AICounter <= 120;
                case (int)AICase.Grab:
                    return AICounter >= 30;
                default:
                    return false;
            }
        }

        //public override bool CanHitPlayer(Player target, ref int cooldownSlot) => canHitPlayer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Strykebeak");
            Main.npcFrameCount[NPC.type] = MAX_FRAMES;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 38;
            NPC.damage = 20;
            NPC.defense = 6;
            NPC.lifeMax = 360;
            NPC.friendly = true;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit28;
            NPC.DeathSound = SoundID.NPCDeath31;
            NPC.value = 60f;

            // knockBackResist is the multiplier applied to the knockback the NPC receives when it takes damage
            NPC.knockBackResist = 0.5f;
        }

        public int idleDirection;
        public int idleHeight;
        public override void OnSpawn(IEntitySource source)
        {
            idleDirection = NPC.Center.X / 16 > Main.maxTilesX / 2 ? -1 : 1;
            idleHeight = Main.rand.Next(17, 24) * 10;
        }

        public enum AICase
        {
            Stunned = -2,
            Hit = -1,
            Idle = 0,
            Angry = 1,
            Dive = 2,
            Grab = 3,
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        Player player => Main.player[NPC.target];

        float flySpeedX = 2;
        float flySpeedY = 0;
        float frameRate = 5;
        float aggroDelay = 60;

        Vector2 idlePosition;
        Vector2 diveStartPosition;
        Vector2 diveTargetPosition;

        float grabOffset;

        public override void AI()
        {
            switch (AIState)
            {
                case (int)AICase.Idle:
                    if (AICounter++ == 0)
                    {
                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else              
                            idlePosition = NPC.Center + new Vector2(-30, 0);  
                    }

                    if (idleDirection == 1)
                    {
                        if (NPC.Center.X <= idlePosition.X && flySpeedX <= 2)
                        {
                            flySpeedX += 0.1f;
                            NPC.direction = 1;
                        }
                    }
                    else
                    {
                        if (NPC.Center.X >= idlePosition.X && flySpeedX >= -2)
                        {
                            flySpeedX -= 0.1f;
                            NPC.direction = -1;
                        }
                    }

                    // This creates the bobbing up/down motion
                    if (NPC.Center.Y >= idlePosition.Y - 2)
                    {
                        flySpeedY -= 0.1f;

                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else
                            idlePosition = NPC.Center + new Vector2(-30, 0);
                    }
                    else if (flySpeedY <= 2) flySpeedY += 0.1f;

                    //if (flySpeedY <= -4 || flySpeedY >= 4) flySpeedY = 0;

                    // Nudge the NPC off the ground if they are too close
                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, idleHeight) < idleHeight || TRay.CastLength(NPC.Bottom, Vector2.UnitY, idleHeight) < idleHeight)
                    {
                        flySpeedY -= 0.1f;

                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else
                            idlePosition = NPC.Center + new Vector2(-30, 0);
                    }
                    
                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, 10000) > 300)
                    {
                        flySpeedY += 0.2f;
                    }

                    // Sometimes the bird decides to just launch upwards towards the sky uncontrollably
                    if (flySpeedY <= -10) flySpeedY += 7;

                    // Force the NPC to fly upwards and away if there is an obstacle in front of it
                    if (TRay.CastLength(NPC.Center, Vector2.UnitX * NPC.direction, 100) < 100 || TRay.CastLength(NPC.Bottom, Vector2.UnitX * NPC.direction, 100) < 100)
                    {
                        flySpeedX -= 0.1f * NPC.direction;
                        flySpeedY -= 1f;

                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else
                            idlePosition = NPC.Center + new Vector2(-30, 0);
                    }
                    break;
                case (int)AICase.Angry:
                    frameRate = 5;
                    NPC.TargetClosest();

                    if (NPC.Center.X >= player.Center.X && flySpeedX >= -2) // flies to players x position
                        flySpeedX -= 0.1f;

                    if (NPC.Center.X <= player.Center.X && flySpeedX <= 2)
                        flySpeedX += 0.1f;

                    if (NPC.Center.Y >= player.Center.Y - 75)
                    {
                        flySpeedY -= 0.1f;
                    }
                    else
                        if (flySpeedY <= 2) flySpeedY += 0.1f;

                    // Nudge the NPC off the ground if they are too close
                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, 25) < 25)
                        flySpeedY -= 0.5f;

                    // Force the NPC to fly upwards and away if there is an obstacle in front of it
                    if (TRay.CastLength(NPC.Center, Vector2.UnitX * NPC.direction, 45) < 45)
                    {
                        flySpeedX -= 0.25f * NPC.direction;
                        flySpeedY -= 0.5f;
                    }

                    float xDistance = Math.Abs(NPC.Center.X - player.Center.X);
                    bool xDistanceCheck = xDistance < 200;
                    bool yDistanceCheck = Math.Abs(NPC.Center.Y - player.Center.Y) < 100;

                    if (xDistanceCheck && yDistanceCheck && Collision.CanHitLine(player.Center, 1, 1, NPC.Center, 1, 1))
                    {
                        aggroDelay--;
                        if (aggroDelay <= 0)
                        {
                            if (xDistance < 32)
                                AIState = (int)AICase.Grab;
                            else
                                AIState = (int)AICase.Dive;
                        }
                    }
                    break;
                case (int)AICase.Dive:
                    frameRate = 3;
                    flySpeedX = 0;
                    flySpeedY = 0;

                    // Initialize anchor positions
                    if (AICounter++ == 0)
                    {
                        float randomOffset = Main.rand.Next(0, 5) * 32;
                        diveStartPosition = NPC.Center;
                        diveTargetPosition = player.Center + new Vector2(randomOffset * NPC.direction, 0);

                        NPC.netUpdate = true;
                    }

                    //Dust target = Dust.NewDustDirect(diveTargetPosition, 1, 1, DustID.Torch);
                    //target.noGravity = true;

                    if (AICounter >= 60 && AICounter <= 120)
                    {
                        Vector2 controlPoint1 = diveTargetPosition + new Vector2(25 * NPC.direction, 0);
                        Vector2 controlPoint2 = diveTargetPosition + new Vector2(75 * NPC.direction, 0);
                        Vector2 diveEndPosition = new Vector2(diveTargetPosition.X + 100 * NPC.direction, diveStartPosition.Y);

                        NPC.Center = ModUtils.Bezier(diveStartPosition, diveEndPosition, controlPoint1, controlPoint2, (AICounter - 60) / 60f);
                    }

                    if (AICounter > 120)
                    {
                        AIState = (int)AICase.Angry;
                        AICounter = 0;
                        aggroDelay = 60;
                    }
                    break;
                case (int)AICase.Grab:
                    if (AICounter++ == 0)
                    {
                        grabOffset = Main.rand.Next(4, 6) * -32;
                        NPC.netUpdate = true;
                    }

                    if (AICounter < 30)
                    {
                        diveStartPosition = NPC.Center;
                        diveTargetPosition = player.Center + new Vector2(0, grabOffset);

                        if (NPC.Center.X >= diveTargetPosition.X && flySpeedX >= -2) // flies to players x position
                            flySpeedX -= 0.1f;

                        if (NPC.Center.X <= diveTargetPosition.X && flySpeedX <= 2)
                            flySpeedX += 0.1f;

                        if (NPC.Center.Y >= diveTargetPosition.Y - 75)
                            flySpeedY -= 0.1f;
                        else
                            if (flySpeedY <= 2) flySpeedY += 0.1f;
                    }
                    else if (AICounter < 45)
                    {
                        flySpeedX = 0;
                        flySpeedY = -2;
                    }
                    else if (AICounter < 60)
                    {
                        flySpeedX = 0;
                        flySpeedY = 8;
                    }

                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, 24) < 24)
                    {
                        AICounter = 60;
                        flySpeedY = -2;
                    }

                    if (AICounter >= 60)
                    {
                        AIState = (int)AICase.Angry;
                        AICounter = 0;
                        aggroDelay = 60;
                    }
                    break;
            }

            if (AIState == (int)AICase.Angry) NPC.rotation = NPC.velocity.X * 0.075f;

            NPC.velocity.X = flySpeedX;
            NPC.velocity.Y = flySpeedY;
        }

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (AIState == (int)AICase.Idle)
            {
                NPC.friendly = false;
                AIState = (int)AICase.Angry;
                AICounter = 0;

                //flySpeedX += knockback;
                //flySpeedY += knockback;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (AIState == (int)AICase.Idle)
            {
                NPC.friendly = false;
                AIState = (int)AICase.Angry;
                AICounter = 0;
            }

            flySpeedX += Utils.Clamp(projectile.velocity.X * (projectile.knockBack * NPC.knockBackResist), -2f, 2f);
            flySpeedY += Utils.Clamp(projectile.velocity.Y * (projectile.knockBack * NPC.knockBackResist), -2f, 2f);
        }

        private int frame = 0;
        private int frameTimer = 0;
        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Y = frameHeight * frame;

            if (Main.gamePaused) return;

            switch (AIState)
            {
                case (int)AICase.Grab:
                    if (AICounter >= 30) frame = 0;
                    else
                    {
                        frameTimer++;
                        if (frameTimer % frameRate == 0)
                        {
                            if (frame < 7)
                                frame++;
                            else
                                frame = 0;
                        }
                    }
                    break;
                default:
                    frameTimer++;
                    if (frameTimer % frameRate == 0)
                    {
                        if (frame < 7)
                            frame++;
                        else
                            frame = 0;
                    }
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.ZonePurity && Main.dayTime ? 0.25f : 0f;
        }
    }
}