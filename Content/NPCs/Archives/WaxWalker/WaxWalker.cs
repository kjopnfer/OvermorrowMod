using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.NPCs;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Content.NPCs.Archives
{
    public class WaxWalker : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 19;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            NPC.width = 30;
            NPC.height = 100;
            NPC.lifeMax = 450;
            NPC.defense = 18;
            NPC.damage = 23;
            NPC.knockBackResist = 0.2f;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void OnSpawn(IEntitySource source)
        {
            CurrentAttack = Main.rand.NextBool() ? AttackType.FlameLob : AttackType.FlameRing;
        }

        public override bool CheckActive() => false;
        public override NPCTargetingConfig TargetingConfig()
        {
            return new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(10f),
                aggroLossRate: 0.5f,
                aggroCooldownTime: ModUtils.SecondsToTicks(4f),
                aggroRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(25),            // Far right detection
                    left: ModUtils.TilesToPixels(25),             // Close left detection
                    up: ModUtils.TilesToPixels(15),               // Medium up detection
                    down: ModUtils.TilesToPixels(5),             // Far down detection
                    flipWithDirection: true                       // Flip based on NPC direction
                ),
                attackRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(50),
                    left: ModUtils.TilesToPixels(50),
                    up: ModUtils.TilesToPixels(25),
                    down: ModUtils.TilesToPixels(10),
                    flipWithDirection: true
                ),
                alertRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(35),
                    left: ModUtils.TilesToPixels(35),
                    up: ModUtils.TilesToPixels(25),
                    down: ModUtils.TilesToPixels(5),
                    flipWithDirection: true
                ),
                prioritizeAggro: true
            )
            {
                ShowDebugVisualization = true
            };
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new Wander(this, minRange: 40, maxRange: 60)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState>
        {
            //new GroundDashAttack(this),
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new MeleeWalk(this, 0.2f, 1.4f),
        };


        public enum WalkerMode
        {
            Red,
            Blue
        }
        public WalkerMode Mode { get; private set; } = WalkerMode.Red;

        public enum AttackType
        {
            FlameLob,
            FlameRing,
            HomingFlame
        }
        public AttackType CurrentAttack { get; private set; } = AttackType.FlameLob;

        public ref float FlameCounter => ref NPC.ai[1];
        public float AttackDelay = ModUtils.SecondsToTicks(10); // default delay in ticks (1 second)
        public override void AI()
        {
            State currentState = AIStateMachine.GetCurrentSubstate();
            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);

            Vector2 flamePosition = NPC.Center + new Vector2(-4 * NPC.direction, -64);

            if (TargetingModule.HasTarget())
            {
                FlameCounter++;

                // Choose new attack based on a set delay
                if (FlameCounter % AttackDelay == 0)
                {
                    ChooseNewAttack();
                }

                // Execute current attack
                ExecuteAttack(flamePosition);
            }

            // Spawn particles (existing code)
            SpawnFlameParticles(flamePosition);
        }

        private void ChooseNewAttack()
        {
            // Switch mode 70% of the time
            if (Main.rand.NextFloat() < 0.7f)
            {
                Mode = Mode == WalkerMode.Red ? WalkerMode.Blue : WalkerMode.Red;
            }

            // Choose attack based on mode
            if (Mode == WalkerMode.Red)
            {
                CurrentAttack = Main.rand.NextBool() ? AttackType.FlameLob : AttackType.FlameRing;
            }
            else
            {
                CurrentAttack = AttackType.HomingFlame;
            }

            FlameCounter = 0;
        }

        private void ExecuteAttack(Vector2 flamePosition)
        {
            switch (CurrentAttack)
            {
                case AttackType.FlameLob:
                    if (FlameCounter % 100 == 0 && FlameCounter < AttackDelay + ModUtils.SecondsToTicks(3))
                    {
                        int randomSpeed = Main.rand.Next(6, 13);
                        Projectile.NewProjectile(
                            NPC.GetSource_FromThis(),
                            flamePosition,
                            new Vector2(randomSpeed * NPC.direction, -randomSpeed),
                            ModContent.ProjectileType<Flame>(),
                            NPC.damage,
                            1f,
                            Main.myPlayer
                        );
                    }
                    break;

                case AttackType.FlameRing:
                    // Fire once per attack cycle (every 3 seconds)
                    if (FlameCounter % 200 == 0 && FlameCounter < AttackDelay + ModUtils.SecondsToTicks(3))
                    {
                        Vector2 directionToTarget = (TargetingModule.Target.Center - flamePosition).SafeNormalize(Vector2.UnitY);
                        int randomSpeed = Main.rand.Next(3, 5);
                        Vector2 shootVelocity = directionToTarget * randomSpeed;

                        Projectile.NewProjectile(
                            NPC.GetSource_FromThis(),
                            flamePosition,
                            shootVelocity,
                            ModContent.ProjectileType<FlameRing>(),
                            NPC.damage,
                            1f,
                            Main.myPlayer
                        );
                    }
                    break;

                case AttackType.HomingFlame:
                    // Fire once per attack cycle (every 3 seconds)
                    if (FlameCounter % AttackDelay == 60)
                    {
                        Vector2 directionToTarget = (TargetingModule.Target.Center - flamePosition).SafeNormalize(Vector2.UnitY);
                        int randomSpeed = Main.rand.Next(6, 13);
                        Vector2 shootVelocity = directionToTarget * randomSpeed;

                        Projectile.NewProjectile(
                            NPC.GetSource_FromThis(),
                            flamePosition,
                            shootVelocity,
                            ModContent.ProjectileType<HomingFlame>(),
                            NPC.damage,
                            1f,
                            Main.myPlayer
                        );
                    }
                    break;
            }
        }

        private void SpawnFlameParticles(Vector2 flamePosition)
        {
            if (NPC.localAI[0]++ % 2 != 0) return;

            var (particleColor, endColor, lightColor) = GetModeColors();

            // Particle spawning
            int version = Main.rand.Next(1, 4);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_0" + version, AssetRequestMode.ImmediateLoad).Value;
            float scale = MathHelper.Lerp(0.035f, 0.075f, MathHelper.Clamp(FlameCounter / AttackDelay, 0, 1f));

            var flameParticle = new Circle(texture, 0f, useSineFade: false);
            flameParticle.endColor = endColor;

            // Add lighting based on mode
            Lighting.AddLight(NPC.Center + new Vector2(-4 * NPC.direction, -72), lightColor);

            ParticleManager.CreateParticleDirect(
                flameParticle,
                flamePosition,
                -Vector2.UnitY,
                particleColor,
                1f,
                scale,
                Main.rand.NextFloat(0f, MathHelper.TwoPi),
                ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true
            );

            var glowParticle = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value, 0f, useSineFade: false);
            ParticleManager.CreateParticleDirect(
                glowParticle,
                flamePosition,
                -Vector2.UnitY,
                particleColor * 0.2f,
                1f,
                scale: scale + 0.15f,
                Main.rand.NextFloat(0f, MathHelper.TwoPi),
                ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true
            );
        }

        private (Color particleColor, Color endColor, Vector3 lightColor) GetModeColors()
        {
            switch (Mode)
            {
                case WalkerMode.Red:
                    return (Color.DarkOrange, Color.Red, new Vector3(0.7f, 0.4f, 0.1f));
                case WalkerMode.Blue:
                    return (new Color(108, 108, 224), new Color(202, 188, 255), new Vector3(0.1f, 0.4f, 0.7f));
                default:
                    return (Color.DarkOrange, Color.Red, new Vector3(0.7f, 0.4f, 0.1f));
            }
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy)
            {
                xFrame = 1;

                if (NPC.frameCounter++ % 6 == 0)
                {
                    yFrame++;
                    if (yFrame >= 18) yFrame = 0;
                }

                return;
            }

            State currentState = AIStateMachine.GetCurrentState();
            switch (currentState)
            {
                case IdleState:
                    xFrame = 1;
                    if (Math.Abs(NPC.velocity.X) > 0)
                    {
                        NPC.frameCounter++;
                        if (NPC.frameCounter % 6 == 0)
                        {
                            yFrame = (yFrame + 1) % 18;
                        }
                    }
                    else
                    {
                        NPC.frameCounter = 0;

                        xFrame = 0;
                        yFrame = 0;
                    }
                    break;
                case MovementState:
                    xFrame = 1;
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        yFrame = (yFrame + 1) % 18;
                    }
                    break;
                default:
                    xFrame = 0;
                    yFrame = 0;
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 2;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 18;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center + new Vector2(0, 0), NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //TargetingModule?.DrawDebugVisualization(spriteBatch);

            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            State currentState = AIStateMachine.GetCurrentSubstate();
            State currentSuperState = AIStateMachine.GetCurrentState();
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOffset = new Vector2(0, -4);

            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArchiveKey>(), chanceDenominator: 10));
            base.ModifyNPCLoot(npcLoot);
        }
    }
}