using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Forest
{
    public class StrykeBeak : OvermorrowNPC
    {
        private const int MAX_FRAMES = 8;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = MAX_FRAMES;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                // The small one
                Position = new Vector2(16, 8),

                // The big image
                PortraitPositionXOverride = 16,
                PortraitPositionYOverride = 12,
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPC.width = 64;
            NPC.height = 38;
            NPC.damage = 20;
            NPC.defense = 6;
            NPC.lifeMax = 200;
            //NPC.friendly = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit28;
            NPC.DeathSound = SoundID.NPCDeath31;
            NPC.value = Item.buyPrice(silver: 1, copper: 30);

            // knockBackResist is the multiplier applied to the knockback the NPC receives when it takes damage
            NPC.knockBackResist = 0.5f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

                new FlavorTextBestiaryInfoElement("Strykebeaks are large social birds that live in colonies under a Matriarch. Preying on large forest insects, their dive bombs are particularly effective against hard shells.")
            });
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
            Dash = 2,
            Dive = 3,
        }

        float flySpeedX = 2;
        float flySpeedY = 0;
        float frameRate = 5;
        float aggroDelay = 60;

        Vector2 idlePosition;
        Vector2 diveStartPosition;
        Vector2 diveTargetPosition;

        float diveOffset;

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
                        if (NPC.velocity.X <= 2) NPC.velocity.X += 0.05f;
                        if (NPC.Center.X <= idlePosition.X && flySpeedX <= 2)
                        {
                            //NPC.velocity.X += 0.05f;
                            flySpeedX += 0.1f;
                            NPC.direction = 1;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.X >= -2) NPC.velocity.X -= 0.05f;

                        if (NPC.Center.X >= idlePosition.X && flySpeedX >= -2)
                        {
                            //NPC.velocity.X -= 0.05f;
                            flySpeedX -= 0.1f;
                            NPC.direction = -1;
                        }
                    }

                    // This creates the bobbing up/down motion
                    if (NPC.Center.Y >= idlePosition.Y - 2)
                    {
                        NPC.velocity.Y -= 0.1f;
                        flySpeedY -= 0.1f;

                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else
                            idlePosition = NPC.Center + new Vector2(-30, 0);
                    }
                    else if (flySpeedY <= 2)
                    {
                        flySpeedY += 0.1f;
                        NPC.velocity.Y += 0.1f;
                    }


                    //if (flySpeedY <= -4 || flySpeedY >= 4) flySpeedY = 0;

                    // Nudge the NPC off the ground if they are too close
                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, idleHeight) < idleHeight || TRay.CastLength(NPC.Bottom, Vector2.UnitY, idleHeight) < idleHeight)
                    {
                        flySpeedY -= 0.1f;
                        NPC.velocity.Y -= 0.1f;

                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else
                            idlePosition = NPC.Center + new Vector2(-30, 0);
                    }

                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, 10000) > 300)
                    {
                        flySpeedY += 0.2f;
                        NPC.velocity.Y += 0.2f;
                    }

                    // Sometimes the bird decides to just launch upwards towards the sky uncontrollably
                    if (flySpeedY <= -10) flySpeedY += 7;

                    // Force the NPC to fly upwards and away if there is an obstacle in front of it
                    if (TRay.CastLength(NPC.Center, Vector2.UnitX * NPC.direction, 100) < 100 || TRay.CastLength(NPC.Bottom, Vector2.UnitX * NPC.direction, 100) < 100)
                    {
                        flySpeedX -= 0.1f * NPC.direction;
                        NPC.velocity.X -= 0.1f * NPC.direction;

                        flySpeedY -= 1f;
                        NPC.velocity.Y -= 1f;

                        if (idleDirection == 1)
                            idlePosition = NPC.Center + new Vector2(30, 0);
                        else
                            idlePosition = NPC.Center + new Vector2(-30, 0);
                    }
                    break;
                case (int)AICase.Angry:
                    if (activeHitboxCount > 0) activeHitboxCount--;

                    frameRate = 5;
                    //NPC.TargetClosest();
                    FaceTarget();

                    if (NPC.Center.X >= target.Center.X)
                    {
                        if (NPC.velocity.X >= -2) NPC.velocity.X -= 0.05f;
                        if (flySpeedX >= -2) flySpeedX -= 0.1f;
                    }

                    if (NPC.Center.X <= target.Center.X)
                    {
                        if (NPC.velocity.X <= 2) NPC.velocity.X += 0.05f;
                        if (flySpeedX <= 2) flySpeedX += 0.1f;
                    }

                    // NPC is too high up from the target, move downwards
                    if (NPC.Center.Y <= target.Center.Y - (16 * 5))
                    {
                        if (NPC.velocity.Y <= 2f) NPC.velocity.Y += 0.1f;

                        // Prevent the NPC from moving in a straight line when the y velocity stays the same
                        if (Main.rand.NextBool(3)) NPC.velocity.Y += 0.05f;

                        flySpeedY += 0.1f;
                    }
                    else
                    {

                        /*if (flySpeedY <= -2)
                        {
                            NPC.velocity.Y += 0.1f;
                            flySpeedY += 0.1f;
                        }*/
                    }

                    // Nudge the NPC off the ground if they are too close
                    /*if (TRay.CastLength(NPC.Center, Vector2.UnitY, 25) < 25)
                    {
                        NPC.velocity.Y -= 0.5f;
                        flySpeedY -= 0.5f;
                    }*/
                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, 128) < 128)
                    {
                        NPC.velocity.Y -= 0.1f;
                        flySpeedY -= 0.1f;
                    }

                    // Force the NPC to fly upwards and away if there is an obstacle in front of it
                    if (TRay.CastLength(NPC.Center, Vector2.UnitX * NPC.direction, 45) < 45)
                    {
                        NPC.velocity.X -= 0.25f * NPC.direction;
                        flySpeedX -= 0.25f * NPC.direction;

                        NPC.velocity.Y -= 0.5f;
                        flySpeedY -= 0.5f;
                    }

                    float xDistance = Math.Abs(NPC.Center.X - target.Center.X);
                    bool xDistanceCheck = xDistance < 200;
                    bool yDistanceCheck = Math.Abs(NPC.Center.Y - target.Center.Y) < 100;

                    if (xDistanceCheck && yDistanceCheck && Collision.CanHitLine(target.Center, 1, 1, NPC.Center, 1, 1))
                    {
                        aggroDelay--;
                        if (aggroDelay <= 0)
                        {
                            if (xDistance < 32)
                                AIState = (int)AICase.Dive;
                            else
                                AIState = (int)AICase.Dash;
                        }
                    }
                    break;
                case (int)AICase.Dash:
                    activeHitboxCount = 20;
                    frameRate = 3;

                    NPC.velocity = Vector2.Zero;

                    flySpeedX = 0;
                    flySpeedY = 0;

                    // Initialize anchor positions
                    if (AICounter++ == 0)
                    {
                        float randomOffset = Main.rand.Next(0, 5) * 32;
                        diveStartPosition = NPC.Center;
                        diveTargetPosition = target.Center + new Vector2(randomOffset * NPC.direction, 0);

                        NPC.netUpdate = true;
                    }

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
                case (int)AICase.Dive:
                    activeHitboxCount = 60;

                    if (AICounter++ == 0)
                    {
                        diveOffset = Main.rand.Next(4, 6) * -32;
                        NPC.netUpdate = true;
                    }

                    if (AICounter < 30)
                    {
                        diveStartPosition = NPC.Center;
                        diveTargetPosition = target.Center + new Vector2(0, diveOffset);

                        if (NPC.Center.X >= diveTargetPosition.X)
                        {
                            if (flySpeedX >= -2) flySpeedX -= 0.1f;
                            if (NPC.velocity.X >= -2) NPC.velocity.X -= 0.05f;
                        }

                        if (NPC.Center.X <= diveTargetPosition.X)
                        {
                            if (flySpeedX <= 2) flySpeedX += 0.1f;
                            if (NPC.velocity.X <= 2) NPC.velocity.X += 0.05f;
                        }

                        if (NPC.Center.Y >= diveTargetPosition.Y - 75)
                        {
                            flySpeedY -= 0.1f;
                            NPC.velocity.Y -= 0.1f;
                        }
                        else
                        {
                            if (flySpeedY <= 2)
                            {
                                NPC.velocity.Y += 0.1f;
                                flySpeedY += 0.1f;
                            }
                        }
                    }
                    else if (AICounter < 45)
                    {
                        flySpeedX = 0;
                        flySpeedY = -2;

                        NPC.velocity.X = 0;
                        NPC.velocity.Y = -2;
                    }
                    else if (AICounter < 60)
                    {
                        flySpeedX = 0;
                        flySpeedY = 8;

                        NPC.velocity.X = 0;
                        NPC.velocity.Y = 8;
                    }

                    if (TRay.CastLength(NPC.Center, Vector2.UnitY, 24) < 24)
                    {
                        AICounter = 60;
                        flySpeedY = -2;
                        NPC.velocity.Y = -2;
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

            //NPC.velocity.X = flySpeedX;
            //NPC.velocity.Y = flySpeedY;

            //Main.NewText("flyspeed: [" + flySpeedX + ", " + flySpeedY + "] vs velocity: [" + NPC.velocity.X + ", " + NPC.velocity.Y + "]");
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (AIState == (int)AICase.Idle)
            {
                NPC.friendly = false;
                AIState = (int)AICase.Angry;
                AICounter = 0;

                target = player;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (target == null)
            {
                Entity ownerEntity = projectile.GlobalProjectile().ownerEntity;
                target = ownerEntity;
            }

            if (AIState == (int)AICase.Idle)
            {
                NPC.friendly = false;
                AIState = (int)AICase.Angry;
                AICounter = 0;

                // For some fucking reason if the target has no value then get the nearest player
                if (target == null)
                {
                    NPC.TargetClosest();
                    target = Main.player[NPC.target];
                }
            }

            //flySpeedX += Utils.Clamp(projectile.velocity.X * (projectile.knockBack * NPC.knockBackResist), -2f, 2f);
            //flySpeedY += Utils.Clamp(projectile.velocity.Y * (projectile.knockBack * NPC.knockBackResist), -2f, 2f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            switch (AIState)
            {
                case (int)AICase.Dive:
                    if (AICounter >= 30) frame = 0;
                    else
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter % frameRate == 0)
                        {
                            if (NPC.frame.Y < 7 * frameHeight)
                                NPC.frame.Y += frameHeight;
                            else
                                NPC.frame.Y = 0;
                        }
                    }
                    break;
                default:
                    NPC.frameCounter++;
                    if (NPC.frameCounter % frameRate == 0)
                    {
                        if (NPC.frame.Y < 5 * frameHeight)
                            NPC.frame.Y += frameHeight;
                        else
                            NPC.frame.Y = 0;
                    }
                    break;
            }
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            bool isUnderground = Framing.GetTileSafely(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY).WallType > 0 || spawnInfo.SpawnTileY > Main.worldSurface;
            return spawnInfo.Player.ZonePurity && !isUnderground && Main.dayTime ? 0.05f : 0f;
        }
    }
}