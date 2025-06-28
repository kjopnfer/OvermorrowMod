using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    /// <summary>
    /// This NPC was made before the AI State machine was implemented.
    /// It is too complicated (and I am too lazy) to convert it to the new system, so it will remain as is.
    /// The NPC does not benefit from any of the modular AI features since it is a unique NPC anyways.
    /// </summary>
    public class InkWormBody : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;
        public override bool? CanBeHitByProjectile(Projectile projectile) => false;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => (AICase)AIState != AICase.Hidden;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPC.width = NPC.height = 32;
            NPC.lifeMax = 30;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.GetGlobalNPC<BarrierNPC>().CanGainBarrier = false;
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

        public override NPCTargetingConfig TargetingConfig()
        {
            NPCTargetingConfig config = new NPCTargetingConfig() {
                DisplayAggroIndicator = false,
                AlertRange = null
            };

            return config;
        }

        public ref float AIState => ref NPC.ai[0];
        public new ref float AICounter => ref NPC.ai[1];

        protected List<NPC> inkTentacles = new List<NPC>();
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
                NPC.life = 0;
                NPC.HitEffect(0, 0);
                NPC.checkDead();
            }

            // Allow the NPC to take damage if their tentacles are damaged.
            NPC.dontTakeDamage = false;
            NPC.chaseable = false;

            switch ((AICase)AIState)
            {
                case AICase.Death:
                    if (AICounter == 200)
                    {
                        // Need it so the bestiary actually counts the deaths
                        foreach (NPC tentacle in inkTentacles)
                        {
                            tentacle.life = 0;
                            tentacle.HitEffect(0, 0);
                            tentacle.checkDead();
                        }
                    }

                    if (AICounter++ >= 240)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 0);
                        NPC.checkDead();
                    }
                    break;
                case AICase.Panic:
                    if (AICounter++ >= 150)
                    {
                        AICounter = 0;
                        AIState = (float)AICase.Death;
                    }
                    break;
                case AICase.Hidden:
                    Player nearestPlayer = null;
                    float nearestDistance = float.MaxValue;

                    // Find the nearest active player
                    foreach (Player player in Main.player)
                    {
                        if (player.active && !player.dead)
                        {
                            float distanceToPlayer = Vector2.Distance(NPC.Center, player.Center);
                            if (distanceToPlayer < nearestDistance)
                            {
                                nearestDistance = distanceToPlayer;
                                nearestPlayer = player;
                            }
                        }
                    }

                    // Ensure there's a nearby player before proceeding
                    if (nearestPlayer != null)
                    {
                        Vector2 directionToPlayer = (nearestPlayer.Center - NPC.Center).SafeNormalize(Vector2.Zero);

                        // Offset calculation based on active tentacles
                        float startAngle = MathHelper.ToRadians(-70);
                        float endAngle = MathHelper.ToRadians(70);
                        float arcStep = (startAngle - endAngle) / (activeTentacles - 1);

                        int tentacleIndex = 0; // Keep track of which tentacle we're processing

                        foreach (NPC tentacle in inkTentacles)
                        {
                            if (!tentacle.active) continue;

                            InkWorm worm = tentacle.ModNPC as InkWorm;
                            if ((AICase)worm.AIState == AICase.Panic || (AICase)worm.AIState == AICase.Death) continue;

                            // Calculate the angle offset for this tentacle
                            float angleOffset = startAngle - (arcStep * tentacleIndex);

                            // Rotate the direction vector by the angle offset
                            Vector2 tentacleDirection = directionToPlayer.RotatedBy(angleOffset);
                            worm.direction = tentacleDirection;

                            tentacleIndex++;
                        }
                    }

                    float detectionRadius = 150f;
                    bool playerNearby = nearestPlayer != null && nearestDistance <= detectionRadius;

                    if (playerNearby)
                    {
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
                    if (AICounter++ == 180)
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
            //return false;
        }
    }

    public class InkWorm : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool CheckActive() => false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => (AICase)AIState != AICase.Hidden && (AICase)AIState != AICase.Panic && (AICase)AIState != AICase.Death;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override void SafeSetDefaults()
        {
            NPC.width = NPC.height = 20;
            NPC.lifeMax = 140;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.damage = 42;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.GetGlobalNPC<BarrierNPC>().CanGainBarrier = false;

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type, ModContent.GetInstance<Inkwell>().Type];
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
        public new ref float AICounter => ref NPC.ai[2];
        NPC Parent => Main.npc[(int)ParentID];


        float randomAnimationOffset = 0;
        float initialDistance = 0;
        private float persistentTime = 0;
        public Vector2 direction = Vector2.Zero;
        private List<Rectangle> handHitboxes = new List<Rectangle>();

        public override void OnSpawn(IEntitySource source)
        {
            randomAnimationOffset = Main.rand.Next(0, 9) * 10;
            initialDistance = (NPC.Center - Parent.Center).Length();
            persistentTime = randomAnimationOffset / 20f;
            direction = NPC.Center - Parent.Center;

            AIState = (int)AICase.Idle;
        }

        public override NPCTargetingConfig TargetingConfig()
        {
            NPCTargetingConfig config = new NPCTargetingConfig
            {
                DisplayAggroIndicator = false,
                AlertRange = null
            };

            return config;
        }

        public override void AI()
        {
            if (!Parent.active)
            {
                NPC.active = false;
                return;
            }

            InkWormBody parentState = Parent.ModNPC as InkWormBody;
            float distance = initialDistance;

            NPC.Opacity = 1f;
            NPC.dontTakeDamage = true;

            // Timer for sine wave animation
            if ((AICase)AIState != AICase.Death)
                persistentTime = (float)(Main.GameUpdateCount + randomAnimationOffset) / 20f;

            AIState = parentState.AIState;
            AICounter = parentState.AICounter;
            float amplitude = 20;
            switch ((AICase)AIState)
            {
                case AICase.Death:
                    // Doesn't like zero for some reason
                    amplitude = MathHelper.Lerp(20, 0.01f, Utils.Clamp(AICounter, 0, 120) / 120f);
                    distance = initialDistance - MathHelper.Lerp(0, initialDistance, Utils.Clamp(AICounter - 90, 0, 120) / 120f);

                    NPC.scale = MathHelper.Lerp(1, 0, Utils.Clamp(AICounter - 90, 0, 120) / 120f);
                    if (AICounter >= 240)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 0);
                        NPC.checkDead();
                    }
                    break;
                case AICase.Panic:
                    // the npc was supposed to freak out and writhe around but i can't get it to work
                    // whatever

                    //amplitude = MathHelper.Lerp(20, 50, AICounter / 150f);
                    /*if (AICounter++ >= 150)
                    {
                        AICounter = 0;
                        AIState = (float)AICase.Death;
                    }*/
                    break;
                case AICase.Hidden:
                    NPC.Opacity = 0f;
                    break;
                case AICase.Extend:
                    distance = initialDistance - MathHelper.Lerp(initialDistance, 0, AICounter / 15f);
                    break;
                case AICase.Idle:
                    // Calculate direction and distance from the parent
                    distance = initialDistance;
                    NPC.dontTakeDamage = false;
                    break;
                case AICase.Retract:
                    distance = initialDistance - MathHelper.Lerp(0, initialDistance, AICounter / 120f);
                    NPC.Opacity = MathHelper.Lerp(1f, 0f, (AICounter - 100) / 20f);
                    break;
            }

            direction.Normalize();

            // Calculate sine wave offset for the head
            float waveOffset = (float)Math.Sin(persistentTime) * amplitude;

            // Perpendicular vector for the sine wave motion
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X) * waveOffset;

            NPC.rotation = direction.ToRotation();
            NPC.Center = Parent.Center + direction * distance + perpendicular;

            CheckHandCollisions();
        }

        public override bool CheckDead()
        {
            // NPC should only be dying when the parent is dead.
            if ((AICase)AIState != AICase.Death)
            {
                NPC.life = NPC.lifeMax;
                return false;
            }

            return true;
        }

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            modifiers.HideCombatText();
            modifiers.ModifyHitInfo += (ref NPC.HitInfo n) => DamageParent(NPC, ref n);
        }

        private void DamageParent(NPC npc, ref NPC.HitInfo info)
        {
            Parent.StrikeNPC(info);
            CombatText.NewText(Parent.Hitbox, Color.Orange, info.Damage);
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Bestiary + Name).Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center, null, drawColor * NPC.Opacity, 0f, texture.Size() / 2, 1f, spriteEffects, 0);
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
            int bodyHeight = (int)((body.Height - 6) * NPC.scale); // Slight overlap adjustment
            int maxIterations = 1000; // Define a reasonable maximum for iterations
            int iterations = Math.Min((int)(distance / bodyHeight), maxIterations); // Limit iterations to avoid overflow
            if (iterations < 0 || iterations > maxIterations)
            {
                iterations = maxIterations;
            }

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

            Main.spriteBatch.Reload(SpriteSortMode.Immediate);


            if ((AICase)AIState == AICase.Death)
            {
                Effect effect = OvermorrowModFile.Instance.ColorFill.Value;
                float progress = 0;
                progress = Utils.Clamp(AICounter, 0, 150f) / 150f;

                effect.Parameters["ColorFillColor"].SetValue(Color.Black.ToVector3());
                effect.Parameters["ColorFillProgress"].SetValue(progress);
                effect.CurrentTechnique.Passes["ColorFill"].Apply();
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

            Main.spriteBatch.Reload(SpriteSortMode.Deferred);

            return base.DrawOvermorrowNPC(spriteBatch, screenPos, drawColor);
        }

        private void DrawWigglingHand(SpriteBatch spriteBatch, Color drawColor, float time, float baseRotation, float offsetY, SpriteEffects spriteEffects)
        {
            float wiggleAmount = MathHelper.ToRadians(25);
            float handRotation = baseRotation + (float)Math.Sin(time) * wiggleAmount;
            Vector2 handOffset = new Vector2(0, offsetY * NPC.scale).RotatedBy(handRotation);

            spriteBatch.Draw(hand, NPC.Center + handOffset - Main.screenPosition, null, drawColor * NPC.Opacity, handRotation, hand.Size() / 2f, NPC.scale, spriteEffects, 0);
            UpdateHandHitbox(NPC.Center + handOffset, hand.Size(), handRotation);
        }

        private void UpdateHandHitbox(Vector2 handPosition, Vector2 handSize, float handRotation)
        {
            // Define the hand's hitbox as a rectangle
            Rectangle hitbox = Utils.CenteredRectangle(handPosition, handSize);

            // Add to the list for collision checks later in the frame
            handHitboxes.Add(hitbox);
        }

        private void CheckHandCollisions()
        {
            if ((AICase)AIState != AICase.Idle)
            {
                handHitboxes.Clear();
                return;
            }

            foreach (Player player in Main.player)
            {
                if (IsPlayerValidForCollision(player))
                {
                    HandlePlayerCollisions(player);
                }
            }

            handHitboxes.Clear();
        }

        private bool IsPlayerValidForCollision(Player player)
        {
            return player.active && !player.dead;
        }

        private void HandlePlayerCollisions(Player player)
        {
            foreach (Rectangle hitbox in handHitboxes)
            {
                if (player.Hitbox.Intersects(hitbox))
                {
                    ApplyDamageToPlayer(player);
                }
            }
        }

        private void ApplyDamageToPlayer(Player player)
        {
            player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), NPC.damage, NPC.direction);
        }
    }
}