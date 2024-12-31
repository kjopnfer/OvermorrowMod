using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.UI;
using Terraria.DataStructures;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Biomes;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ArchiveRat : ModNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return AIState == (int)AICase.Attack && NPC.velocity.X != 0;
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

        public override void SetDefaults()
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

            SpawnModBiomes = [ModContent.GetInstance<Library>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.OvermorrowMod.Bestiary.ArchiveRat")),
            });
        }

        private bool canAttack = false;
        private int frame = 0;
        private int frameTimer = 0;
        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];
        public ref Player player => ref Main.player[NPC.target];

        public enum AICase
        {
            Idle = 0,
            Walk = 1,
            Attack = 2,
            Stealth = 3
        }

        private float maxSpeed = 1.8f;
        public bool isStealthed => NPC.HasBuff<Stealth>();
        public override void OnSpawn(IEntitySource source)
        {
            maxSpeed = Main.rand.NextFloat(1.8f, 3f);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage.Base *= 2;
        }

        public override void AI()
        {
            NPC.knockBackResist = isStealthed ? 0f : 0.5f;
            switch ((AICase)AIState)
            {
                case AICase.Idle:
                    if (NPC.velocity.X != 0)
                    {
                        NPC.velocity.X *= 0.75f;
                    }

                    if (AICounter++ >= 30)
                    {
                        AIState = canAttack ? (int)AICase.Attack : (int)AICase.Walk;
                        AICounter = 0;
                    }
                    break;
                case AICase.Walk:
                    NPC.TargetClosest();

                    Vector2 distance = NPC.Move(player.Center, 0.6f, maxSpeed, 8f);
                    bool isWithinAttackRange = distance.X < 18 * 10 && distance.Y < 5;

                    if (isWithinAttackRange)
                    {
                        canAttack = true;
                        SetAIState(AICase.Idle);
                    }
                    else if (!isStealthed && NPC.GetGlobalNPC<BuffNPC>().StealthDelay <= 0)
                    {
                        SetAIState(AICase.Stealth);
                    }

                    if (AICounter++ >= 54 * 5)
                    {
                        SetAIState(AICase.Idle);
                    }
                    break;
                case AICase.Attack:
                    if (isStealthed)
                    {
                        int buff = NPC.FindBuffIndex(ModContent.BuffType<Stealth>());
                        NPC.DelBuff(buff);
                    }

                    NPC.velocity.X = Main.rand.Next(14, 17) * NPC.direction;

                    if (AICounter++ >= 10)
                    {
                        canAttack = false;
                        AIState = (int)AICase.Idle;
                        AICounter = 0;
                    }
                    break;
                case AICase.Stealth:
                    NPC.velocity.X = 0;
                    NPC.AddBuff(ModContent.BuffType<Stealth>(), 18000);

                    if (AICounter++ >= 60)
                    {
                        NPC.GetGlobalNPC<BuffNPC>().StealthDelay = 300;
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
                case AICase.Idle:
                    xFrame = 1;
                    yFrame = 1;
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
                    break;
            }
        }

        int xFrame = 0;
        int yFrame = 0;
        int FRAME_WIDTH = 56;
        int FRAME_HEIGHT = 48;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 10;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 9;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (NPC.IsABestiaryIconDummy)
            {
                spriteBatch.Draw(texture, NPC.Center, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
                return true;
            }
      
            if (AIState == (int)AICase.Attack && NPC.velocity != Vector2.Zero)
            {
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = NPC.oldPos[k] + texture.Size() / 2f - screenPos;
                    Color afterImageColor = NPC.GetAlpha(new Color(56, 40, 26)) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                    spriteBatch.Draw(texture, drawPos + new Vector2(0, 0), NPC.frame, afterImageColor * NPC.Opacity, NPC.rotation, texture.Size() / 2f, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawOffset = new Vector2(0, 2);
            spriteBatch.Draw(texture, NPC.Center + drawOffset - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
            

            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Rat, chanceDenominator: 10));
        }
    }
}