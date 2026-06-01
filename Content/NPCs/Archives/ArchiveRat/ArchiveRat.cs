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
using OvermorrowMod.Content.Items.Archives.Weapons;

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
            AggroSound = SoundID.NPCDeath4 with
            {
                Pitch = 0.5f
            };

            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;

            NPC.width = 30;
            NPC.height = 44;
            NPC.lifeMax = 40;
            NPC.defense = 8;
            NPC.damage = 13;
            NPC.knockBackResist = 0.5f;
            NPC.HitSound = SoundID.NPCDeath4 with
            {
                Pitch = 0.2f,
                PitchVariance = 0.2f
            };
            
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        private const bool DebugAI = true;
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

        protected override PersonalityProfile PersonalityRanges => new PersonalityProfile
        {
            Aggression = (0.2f, 0.9f),
            Caution = (0.1f, 0.8f),
            Reactivity = (0.2f, 0.7f)
        };

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new Wander(this, minRange: 30, maxRange: 50)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new GroundDashAttack(this),
            new GainStealth(this)
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new MeleeWalk(this),
            new MoveBack(this),
        };

        public override List<BaseDefenseState> InitializeDefenseStates() => new List<BaseDefenseState> {
            new RecoilLeap(this),
            new BreakOff(this)
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
                case MovementState moveState when moveState.currentSubstate is MoveBack:
                    xFrame = 0;
                    if (NPC.frameCounter++ % 6 == 0)
                    {
                        yFrame = (yFrame + 8) % 9;
                    }
                    break;

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

            if (DebugAI && Personality != null)
            {
                string sub = AIStateMachine.GetCurrentSubstate()?.GetType().Name ?? AIStateMachine.GetCurrentState()?.GetType().Name ?? "none";
                string info = $"{sub}\nAgg {Personality.Aggression:0.00}  Cau {Personality.Caution:0.00}  Rea {Personality.Reactivity:0.00}\nDmg {RecentDamageFraction():0.00}";
                Vector2 textPos = NPC.Top - Main.screenPosition - new Vector2(0, 48);
                Utils.DrawBorderString(spriteBatch, info, textPos, Color.White, 0.7f, 0.5f, 0.5f);
            }

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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ArchiveKey>(), chanceDenominator: 50));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CarvingKnife>(), chanceDenominator: 50));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cheese>(), chanceDenominator: 20));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonkeyStoneBlue>(), chanceDenominator: 5));
        }
    }
}