using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.InverseKinematics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Core.NPCs;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
            NPC.lifeMax = 200;
            NPC.defense = 18;
            NPC.damage = 23;
            NPC.knockBackResist = 0.2f;
            NPCID.Sets.ImmuneToAllBuffs[Type] = true;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override bool CheckActive() => false;
        public override NPCTargetingConfig TargetingConfig()
        {
            return new NPCTargetingConfig(
                maxAggroTime: ModUtils.SecondsToTicks(7f),
                aggroLossRate: 1f,
                aggroCooldownTime: ModUtils.SecondsToTicks(4f),
                targetRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(25),            // Far right detection
                    left: ModUtils.TilesToPixels(25),             // Close left detection
                    up: ModUtils.TilesToPixels(15),               // Medium up detection
                    down: ModUtils.TilesToPixels(15),             // Far down detection
                    flipWithDirection: true                       // Flip based on NPC direction
                ),
                attackRadius: AggroRadius.Circle(ModUtils.TilesToPixels(35)),
                alertRadius: new AggroRadius(
                    right: ModUtils.TilesToPixels(35),
                    left: ModUtils.TilesToPixels(35),
                    up: ModUtils.TilesToPixels(25),
                    down: ModUtils.TilesToPixels(25),
                    flipWithDirection: true
                ),
                prioritizeAggro: true
            )
            {
                ShowDebugVisualization = true
            };
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new Wander(this)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new GroundDashAttack(this),
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new MeleeWalk(this, 0.2f, 1.4f),
        };

        public override void AI()
        {
            State currentState = AIStateMachine.GetCurrentSubstate();
            AIStateMachine.Update(NPC.ModNPC as OvermorrowNPC);
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
            TargetingModule?.DrawDebugVisualization(spriteBatch);

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
            base.ModifyNPCLoot(npcLoot);
        }
    }
}