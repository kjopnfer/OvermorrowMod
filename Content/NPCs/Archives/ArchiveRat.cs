using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.DataStructures;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Content.Biomes;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using OvermorrowMod.Common.CustomCollision;
using System.Collections.Generic;
using OvermorrowMod.Core.NPCs;
using System;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Items.Archives;

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
            ItemID.Sets.KillsToBanner[Type] = 10;

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
            NPC.lifeMax = 160;
            NPC.defense = 12;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        private int afterimageLinger = 0;
        public override void OnSpawn(IEntitySource source)
        {
            var stealthDelay = ModUtils.SecondsToTicks(Main.rand.NextFloat(4, 5.5f));

            if (Main.rand.NextBool() && Main.expertMode)
                NPC.SetStealth(stealthTime: ModUtils.SecondsToTicks(300), stealthDelay);
            else
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
        public override NPCTargetingConfig TargetingConfig()
        {
            //return new NPCTargetingConfig(
            //    maxAggroTime: 300f,
            //    aggroLossRate: 1f,
            //    aggroCooldownTime: 180f,
            //    maxTargetRange: ModUtils.TilesToPixels(35),
            //    maxAttackRange: ModUtils.TilesToPixels(35),
            //    alertRange: ModUtils.TilesToPixels(40),
            //    prioritizeAggro: true
            //);
            return new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(7f),
                aggroLossRate: 1f,
                aggroCooldownTime: ModUtils.SecondsToTicks(4f),
                aggroRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(25),            // Far right detection
                    left: ModUtils.TilesToPixels(0),             // Close left detection
                    up: ModUtils.TilesToPixels(5),               // Medium up detection
                    down: ModUtils.TilesToPixels(5),             // Far down detection
                    flipWithDirection: true                       // Flip based on NPC direction
                ),
                attackRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(35),
                    left: ModUtils.TilesToPixels(0),
                    up: ModUtils.TilesToPixels(15),
                    down: ModUtils.TilesToPixels(10),
                    flipWithDirection: true
                ),
                alertRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(35),
                    left: ModUtils.TilesToPixels(0),
                    up: ModUtils.TilesToPixels(10),
                    down: ModUtils.TilesToPixels(10),
                    flipWithDirection: true
                ),
                prioritizeAggro: true
            )
            {
                ShowDebugVisualization = true
            };
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new Wander(this, minRange: 30, maxRange: 50)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new GroundDashAttack(this),
            new GainStealth(this)
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new MeleeWalk(this),
        };

        public override void AI()
        {
            State currentState = AIStateMachine.GetCurrentSubstate();

            NPC.noGravity = false;
            NPC.knockBackResist = 0.5f;
            if (NPC.IsStealthed() || currentState is GroundDashAttack || currentState is GainStealth)
            {
                NPC.knockBackResist = 0f;
            }

            if (afterimageLinger > 0) afterimageLinger--;

            // TODO: Generalize this into a collision module
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
                        //Main.NewText($"Collision detected with {tileCollisionNPC}");
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
            /*if (!TargetingModule.HasTarget())
            {
                NPC.RemoveStealth();
            }*/

            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);

            if (currentState is GroundDashAttack)
            {
                afterimageLinger = 1;
            }
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy)
            {
                xFrame = 0;

                if (NPC.frameCounter++ % 6 == 0)
                {
                    yFrame++;
                    if (yFrame >= 9) yFrame = 0;
                }

                return;
            }

            State currentState = AIStateMachine.GetCurrentState();
            switch (currentState)
            {
                case MovementState:
                    xFrame = 0;
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        yFrame = (yFrame + 1) % 9;
                    }
                    break;

                case AttackState attackState:
                    switch (attackState.currentSubstate)
                    {
                        case GroundDashAttack:
                            if (AICounter < 30)
                            {
                                xFrame = 1;
                                yFrame = AICounter >= 24 ? 0 : 1;
                            }
                            else if (AICounter < 40)
                            {
                                xFrame = 1;
                                yFrame = 0;
                            }
                            else if (NPC.velocity.X != 0 && NPC.collideY)
                            {
                                xFrame = 0;
                                if (NPC.frameCounter++ % 6 == 0)
                                {
                                    yFrame = (yFrame + 1) % 9;
                                }
                            }
                            break;

                        case GainStealth:
                            xFrame = 1;
                            if (AICounter == 0)
                            {
                                yFrame = 2;
                                NPC.frameCounter = 0;
                            }
                            if (AICounter <= 30 && NPC.frameCounter++ % 6 == 0)
                            {
                                yFrame = Math.Min(yFrame + 1, 5);
                            }
                            break;
                    }
                    break;

                case IdleState idleState when idleState.currentSubstate is Wander:
                    xFrame = 0;
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        yFrame = (yFrame + 1) % 9;
                    }
                    break;
                default:
                    xFrame = 1;
                    yFrame = 1;
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
            //TargetingModule?.DrawDebugVisualization(spriteBatch);

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

            var lightAverage = (drawColor.R / 255f + drawColor.G / 255f + drawColor.B / 255f) / 3;
            if (Main.LocalPlayer.HasBuff(BuffID.Hunter))
            {
                drawColor = Color.Lerp(new Color(255, 50, 50), drawColor, lightAverage);
            }

            Vector2 drawOffset = new Vector2(0, 2);
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArchiveKey>(), chanceDenominator: 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cheese>(), chanceDenominator: 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonkeyStoneBlue>(), chanceDenominator: 5));
        }
    }
}