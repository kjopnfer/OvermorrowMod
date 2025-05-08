using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace OvermorrowMod.Content.NPCs
{
    public class ATestStateNPC : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + "ArchiveRat";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            NPC.width = 30;
            NPC.height = 44;
            NPC.lifeMax = 100;
            NPC.defense = 8;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new GrimoireHidden()
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new GrimoireSpellCast()
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new BasicFly(),
        };

        public override void OnSpawn(IEntitySource source)
        {
            Main.NewText("spawned and forced to hidden", Color.Green);
            AIStateMachine.SetSubstate<GrimoireHidden>(AIStateType.Idle, NPC.ModNPC as OvermorrowNPC);
        }

        public override void AI()
        {
            // TEMP:
            NPC.noGravity = true;

            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);

            if (AIStateMachine.GetPreviousSubstates().FirstOrDefault() is GrimoireHidden)
            {
                // Do something
                //Main.NewText("remove the hidden state and add new idle", Color.Cyan);

                //AIStateMachine.RemoveSubstate<GrimoireHidden>(AIStateType.Idle, new GrimoireHidden());
                //Main.NewText("wtf");
                //var newIdle = new GrimoireIdle();
                //AIStateMachine.AddSubstate(AIStateType.Idle, newIdle);
                //AIStateMachine.SetSubstate<GrimoireIdle>(AIStateType.Idle, NPC.ModNPC as OvermorrowNPC);
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
            }

            State currentState = AIStateMachine.GetCurrentState();
            if (currentState is MovementState moveState)
            {
                xFrame = 0;

                if (NPC.frameCounter++ % 6 == 0)
                {
                    yFrame++;
                    if (yFrame >= 9) yFrame = 0;
                }
            }
            else if (currentState is AttackState attackState)
            {
                if (attackState.currentSubstate is GroundDashAttack)
                {
                    if (AICounter < 30)
                    {
                        xFrame = 1;
                        yFrame = AICounter >= 30 - 6 ? 0 : 1;
                    }
                    else
                    {
                        if (AICounter < 40)
                        {
                            xFrame = 1;
                            yFrame = 0;
                        }
                        else
                        {

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
                        }
                    }
                }

                if (attackState.currentSubstate is GainStealth)
                {
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
                }
            }
            else
            {
                xFrame = 1;
                yFrame = 1;
            }

            /*switch ((AICase)AIState)
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
            }*/
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

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawOffset = new Vector2(0, 2);
            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}