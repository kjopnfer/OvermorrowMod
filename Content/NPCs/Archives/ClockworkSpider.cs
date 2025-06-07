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
using static Terraria.GameContent.PlayerEyeHelper;
using System.Xml.Linq;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ClockworkSpider : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

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

            NPC.width = 44;
            NPC.height = 44;
            NPC.lifeMax = 200;
            NPC.defense = 18;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
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
                maxAggroTime: 300f,
                aggroLossRate: 1f,
                aggroCooldownTime: 180f,
                maxTargetRange: ModUtils.TilesToPixels(35),
                maxAttackRange: ModUtils.TilesToPixels(35),
                alertRange: ModUtils.TilesToPixels(40),
                prioritizeAggro: true
            );
        }

        public override List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
            new Wander(this)
        };

        public override List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
            new GroundDashAttack(this),
        };

        public override List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
            new CeilingWalk(this),
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
                xFrame = 0;

                if (NPC.frameCounter++ % 6 == 0)
                {
                    yFrame++;
                    if (yFrame >= 8) yFrame = 0;
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
                        yFrame = (yFrame + 1) % 8;
                    }
                    break;
                default:
                    xFrame = 2;
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
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 3;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 8;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center + new Vector2(0, -8), NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawOvermorrowNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            State currentState = AIStateMachine.GetCurrentSubstate();
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOffset = new Vector2(0, -2);
            if (currentState is CeilingWalk)
            {
                spriteEffects |= SpriteEffects.FlipVertically; // Add vertical flip
                drawOffset = new Vector2(0, 2);
            }

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