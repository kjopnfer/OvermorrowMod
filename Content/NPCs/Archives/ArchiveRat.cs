using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Content.Biomes;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using System.Linq;
using OvermorrowMod.Common.CustomCollision;
using System.Collections.Generic;
using OvermorrowMod.Content.Misc;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ArchiveRat : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return afterimageLinger > 0;
        }

        public override bool CanHitNPC(NPC target)
        {
            return afterimageLinger > 0;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPC.width = 30;
            NPC.height = 44;
            NPC.lifeMax = 100;
            NPC.defense = 8;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        private bool canAttack = false;
        private int afterimageLinger = 0;

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        public enum AICase
        {
            Decelerate = -1,
            Pause = -2,
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Stealth = 3
        }


        private float maxSpeed = 1.8f;
        private int idleTime = 30;
        private int attackRange = 18 * 10;
        private int stealthDelay = 300;
        public override void OnSpawn(IEntitySource source)
        {
            maxSpeed = Main.rand.NextFloat(1.8f, 3f);
            idleTime = Main.rand.Next(24, 36);
            attackRange = Main.rand.Next(16, 19) * 10;
            stealthDelay = Main.rand.Next(24, 32) * 10;

            NPC.GetGlobalNPC<BuffNPC>().StealthDelay = stealthDelay;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // This is because the NPC immediately remove stealth on attack so checking the buff isn't viable
            if (NPC.GetGlobalNPC<BuffNPC>().StealthCounter > 0) modifiers.SourceDamage *= 2;
        }

        #region Test
        bool IsRectangleIntersectingSlope(Vector2 start, Vector2 end, float width, Rectangle npcBottomHitbox)
        {
            // Calculate the direction vector of the slope
            Vector2 direction = Vector2.Normalize(end - start);

            // Calculate the perpendicular vector for width
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * width * 0.5f;

            // Define the four corners of the rotated rectangle
            Vector2 p1 = start - perpendicular; // Bottom-left
            Vector2 p2 = start + perpendicular; // Top-left
            Vector2 p3 = end + perpendicular;   // Top-right
            Vector2 p4 = end - perpendicular;   // Bottom-right

            // Define the slope as a polygon
            Vector2[] slopePolygon = new[] { p1, p2, p3, p4 };

            // Check for intersection between the NPC's bottom hitbox and the polygon
            return PolygonIntersectsRectangle(slopePolygon, npcBottomHitbox);
        }

        // Function to check if a polygon intersects with a rectangle
        bool PolygonIntersectsRectangle(Vector2[] polygon, Rectangle rectangle)
        {
            // Convert the rectangle into a polygon (clockwise points)
            Vector2[] rectPolygon = new Vector2[]
            {
                new Vector2(rectangle.Left, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Top),
                new Vector2(rectangle.Right, rectangle.Bottom),
                new Vector2(rectangle.Left, rectangle.Bottom)
            };

            // Perform SAT-based intersection test between the two polygons
            return PolygonsIntersect(polygon, rectPolygon);
        }

        // SAT-based polygon intersection test
        bool PolygonsIntersect(Vector2[] poly1, Vector2[] poly2)
        {
            // Helper function to project a polygon onto an axis
            void ProjectPolygon(Vector2 axis, Vector2[] polygon, out float min, out float max)
            {
                min = Vector2.Dot(axis, polygon[0]);
                max = min;
                for (int i = 1; i < polygon.Length; i++)
                {
                    float projection = Vector2.Dot(axis, polygon[i]);
                    if (projection < min) min = projection;
                    if (projection > max) max = projection;
                }
            }

            // Helper function to check overlap between two projections
            bool Overlaps(float minA, float maxA, float minB, float maxB) => maxA >= minB && maxB >= minA;

            // Get the axes (normals of edges) for both polygons
            List<Vector2> axes = new List<Vector2>();
            for (int i = 0; i < poly1.Length; i++)
                axes.Add(Vector2.Normalize(new Vector2(-(poly1[(i + 1) % poly1.Length] - poly1[i]).Y, (poly1[(i + 1) % poly1.Length] - poly1[i]).X)));

            for (int i = 0; i < poly2.Length; i++)
                axes.Add(Vector2.Normalize(new Vector2(-(poly2[(i + 1) % poly2.Length] - poly2[i]).Y, (poly2[(i + 1) % poly2.Length] - poly2[i]).X)));

            // Check projections on all axes
            foreach (var axis in axes)
            {
                ProjectPolygon(axis, poly1, out float min1, out float max1);
                ProjectPolygon(axis, poly2, out float min2, out float max2);
                if (!Overlaps(min1, max1, min2, max2)) return false; // No overlap means no intersection
            }

            return true; // All axes overlapped, polygons intersect
        }
        #endregion

        public override bool CheckActive() => false;
        public override void AI()
        {
            NPC.noGravity = false;
            NPC.knockBackResist = NPC.IsStealthed() ? 0f : 0.5f;
            if (afterimageLinger > 0) afterimageLinger--;

            #region Impact Collision
            var activeTileCollisionNPCs = Main.npc
            .Where(npc => npc.active && npc.ModNPC is TileCollisionNPC)
            .Select(npc => (TileCollisionNPC)npc.ModNPC)
            .ToList();

            Vector2 bottomLeft = NPC.Hitbox.BottomLeft();
            Vector2 bottomRight = NPC.Hitbox.BottomRight();

            foreach (var tileCollisionNPC in activeTileCollisionNPCs)
            {
                if (tileCollisionNPC.colliders == null) continue;

                foreach (var colliders in tileCollisionNPC.colliders)
                {
                    var start = colliders.endPoints[0];
                    var end = colliders.endPoints[1];
                    //float colliderWidth = (end - start).Length(); // Distance between start and end points
                    float colliderWidth = 5; // Distance between start and end points

                    // Check if the NPC's bottom hitbox intersects with the slope
                    Rectangle npcBottomHitbox = new Rectangle((int)bottomLeft.X, (int)bottomLeft.Y, (int)(bottomRight.X - bottomLeft.X), 1); // 1-pixel tall rectangle
                    if (IsRectangleIntersectingSlope(start, end, colliderWidth, npcBottomHitbox))
                    {
                        // Perform desired action for the collision
                        Main.NewText($"Collision detected with {tileCollisionNPC}");
                        NPC.collideY = true;
                        NPC.velocity.Y = 0;
                        NPC.noGravity = true;

                        // Adjust the NPC upwards until the collision no longer occurs
                        while (IsRectangleIntersectingSlope(start, end, colliderWidth, npcBottomHitbox))
                        {
                            NPC.position.Y -= 0.05f; // Move the NPC upwards by a small increment
                            bottomLeft = NPC.Hitbox.BottomLeft();
                            bottomRight = NPC.Hitbox.BottomRight();
                            npcBottomHitbox = new Rectangle((int)bottomLeft.X, (int)bottomLeft.Y, (int)(bottomRight.X - bottomLeft.X), 1); // Update hitbox
                        }
                    }
                }
            }
            #endregion

            switch ((AICase)AIState)
            {
                case AICase.Decelerate:
                    if (NPC.velocity.X != 0)
                    {
                        NPC.velocity.X *= 0.9f;
                    }

                    if (NPC.collideX)
                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    if (AICounter++ >= 42)
                    {
                        AIState = (int)AICase.Walk;
                        AICounter = 0;
                    }
                    break;
                case AICase.Idle:
                    NPC.velocity.X = 0;

                    // Each second, look for a valid target.
                    if (AICounter++ >= 60)
                    {
                        Entity target = FindNearestTarget(NPC.Center, 16 * 8, true);

                        if (target != null)
                        {
                            Target = target;
                            AIState = (int)AICase.Walk;
                        }
                        else
                        {

                            // Continue idling but change the NPC's direction
                            if (Main.rand.NextBool())
                            {
                                AIState = (int)AICase.Idle;
                                NPC.direction = Main.rand.NextBool() ? 1 : -1;
                            }
                            else
                            {
                                AIState = (int)AICase.Walk;
                                if (SpawnPoint != null)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AggroIndicator>(), 1, 1f, Main.myPlayer, ai0: NPC.whoAmI);

                                    Main.NewText("Get a random position to walk to");

                                    Vector2 spawnPosition = SpawnPoint.Position.ToWorldCoordinates(); // Convert tile position to world position
                                    Vector2 newPosition;

                                    do
                                    {
                                        float offsetX = Main.rand.NextFloat(-10f, 10f) * 16; // Random X within 10 tiles
                                        float offsetY = Main.rand.NextFloat(-10f, 10f) * 16; // Random Y within 10 tiles
                                        newPosition = spawnPosition + new Vector2(offsetX, offsetY);

                                        // Find the nearest ground position for the generated newPosition
                                        newPosition = TileUtils.FindNearestGround(newPosition);

                                    } while (Vector2.Distance(NPC.Center, newPosition) < 4 * 16); // Ensure at least 4 tiles away

                                    TargetPosition = newPosition; // Assign the valid position on the ground
                                    Dust.NewDust(TargetPosition.Value, 1, 1, DustID.BlueTorch);
                                }
                            }
                        }

                        AICounter = 0;
                    }
                    break;
                case AICase.Pause:
                    NPC.velocity.X = 0;

                    if (AICounter++ >= idleTime)
                    {
                        AIState = canAttack ? (int)AICase.Attack : (int)AICase.Walk;
                        AICounter = 0;
                    }
                    break;
                case AICase.Walk:
                    // If there's no valid target, move toward the target position or return to idle
                    if (Target == null)
                    {
                        if (TargetPosition.HasValue)
                        {
                            NPC.direction = NPC.GetDirection(TargetPosition.Value);
                            Vector2 distance = NPC.Move(TargetPosition.Value, 0.2f, maxSpeed, 8f);

                            if (distance.X <= 4)
                                SetAIState(AICase.Idle);
                        }
                        else
                        {
                            SetAIState(AICase.Idle);
                        }
                        return;
                    }

                    // Move towards the target
                    NPC.direction = NPC.GetDirection(Target);
                    Vector2 targetDistance = NPC.Move(Target.Center, 0.2f, maxSpeed, 8f);

                    bool isWithinAttackRange = targetDistance.X < attackRange && targetDistance.Y <= 31;

                    if (isWithinAttackRange)
                    {
                        canAttack = true;
                        SetAIState(AICase.Pause);
                    }
                    else if (!NPC.IsStealthOnCooldown())
                    {
                        SetAIState(AICase.Stealth);
                    }

                    // After a set duration, transition to Pause state
                    if (AICounter++ >= 54 * 5)
                        SetAIState(AICase.Pause);

                    break;
                case AICase.Attack:
                    afterimageLinger = 30;

                    NPC.RemoveStealth();
                    NPC.velocity.X = Main.rand.Next(14, 17) * NPC.direction;

                    if (NPC.collideX)
                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    if (AICounter++ >= 10)
                    {
                        canAttack = false;
                        AIState = (int)AICase.Decelerate;
                        AICounter = 0;
                    }
                    break;
                case AICase.Stealth:
                    NPC.velocity.X = 0;
                    NPC.SetStealth(stealthTime: 18000, stealthDelay);

                    if (AICounter++ >= 60)
                    {
                        SetAIState(AICase.Walk);
                    }
                    break;
            }
        }

        private void SetAIState(AICase state)
        {
            AIState = (int)state;
            AICounter = 0;
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy) AIState = (int)AICase.Walk;

            switch ((AICase)AIState)
            {
                case AICase.Decelerate:
                    if (NPC.velocity.X != 0)
                    {
                        xFrame = 0;

                        if (NPC.frameCounter++ % 6 == 0)
                        {
                            yFrame++;
                            if (yFrame >= 9) yFrame = 0;
                        }
                    }
                    else
                    {
                        xFrame = 1;
                        yFrame = 1;
                    }
                    break;
                case AICase.Idle:
                    xFrame = 1;
                    yFrame = 1;
                    break;
                case AICase.Pause:
                    xFrame = 1;
                    yFrame = canAttack && AICounter >= idleTime - 6 ? 0 : 1;
                    break;
                case AICase.Walk:
                    xFrame = 0;

                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        yFrame++;
                        if (yFrame >= 9) yFrame = 0;
                    }
                    break;
                case AICase.Attack:
                    xFrame = 1;
                    yFrame = 0;
                    break;
                case AICase.Stealth:
                    xFrame = 1;
                    if (AICounter == 0)
                    {
                        yFrame = 2;
                        NPC.frameCounter = 0;
                    }

                    if (AICounter <= 30)
                    {
                        if (NPC.frameCounter++ % 6 == 0)
                        {
                            yFrame++;
                            if (yFrame >= 5) yFrame = 5;
                        }
                    }
                    else
                    {
                        if (NPC.frameCounter++ % 6 == 0)
                        {
                            yFrame--;
                            if (yFrame <= 2) yFrame = 2;
                        }
                    }
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 10;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 9;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }


        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //if (AIState == (int)AICase.Attack && NPC.velocity != Vector2.Zero)
            if (afterimageLinger > 0)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Vector2 offset = new Vector2(-14, 0);
                    Vector2 drawPos = offset + NPC.oldPos[k] + texture.Size() / 2f - screenPos;
                    Color afterImageColor = (drawColor * 0.5f) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos + new Vector2(0, 0), NPC.frame, afterImageColor * NPC.Opacity, NPC.rotation, texture.Size() / 2f, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawOffset = new Vector2(0, 2);
            var lightAverage = (drawColor.R / 255f + drawColor.G / 255f + drawColor.B / 255f) / 3;
            if (Main.LocalPlayer.HasBuff(BuffID.Hunter))
            {
                drawColor = Color.Lerp(new Color(255, 50, 50), drawColor, lightAverage);
            }

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Blood, 2 * hit.HitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        dust.noGravity = true;
                        dust.scale = 1.2f * NPC.scale;
                    }
                    else
                    {
                        dust.scale = 0.7f * NPC.scale;
                    }
                }

                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}Hand").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}Leg").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>($"{Name}HeadYellow").Type, NPC.scale);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Rat, chanceDenominator: 10));
        }
    }
}