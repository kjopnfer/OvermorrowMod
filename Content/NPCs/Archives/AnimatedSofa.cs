using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using OvermorrowMod.Core.Biomes;
using Terraria.Localization;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class AnimatedSofa : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 38;
            NPC.lifeMax = 48;
            NPC.defense = 8;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue(LocalizationPath.Bestiary + Name)),
            });
        }

        public ref float AIState => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        public enum AICase
        {
            Summon = -1,
            Idle = 0,
            Jump = 1
        }

        private int idleTime = 30;
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public override void AI()
        {
            switch ((AICase)AIState)
            {
                case AICase.Idle:
                    NPC.TargetClosest();

                    if (AICounter++ >= idleTime)
                    {
                        AIState = (int)AICase.Jump;
                        AICounter = 0;
                    }
                    break;
                case AICase.Jump:
                    if (AICounter++ == 0)
                    {
                        int jumpDirection = Main.rand.Next(2, 6) * NPC.direction;
                        int jumpHeight = Main.rand.Next(-10, -4);
                        NPC.velocity = new Vector2(jumpDirection, jumpHeight);
                    }

                    if (((NPC.collideY && NPC.velocity.Y == 0) || (NPC.collideX && NPC.velocity.X == 0)) && AICounter > 24)
                    {
                        NPC.velocity.X = 0;
                        idleTime = Main.rand.Next(3, 4) * 10;

                        AIState = (int)AICase.Idle;
                        AICounter = 0;
                    }
                    break;
            }
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy) AIState = (int)AICase.Idle;

            switch ((AICase)AIState)
            {
                case AICase.Idle:
                    xFrame = 0;
                    if (NPC.frameCounter++ % 8 == 0)
                    {
                        yFrame++;
                        if (yFrame >= 2) yFrame = 0;
                    }
                    break;
                case AICase.Jump:
                    xFrame = 0;
                    yFrame = 2;
                    break;

            }
        }

        int xFrame = 0;
        int yFrame = 0;
        public override void FindFrame(int frameHeight)
        {
            SetFrame();

            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 4;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 3;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }
        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawOffset = new Vector2(0, -2);
            spriteBatch.Draw(texture, NPC.Center + drawOffset, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }
    }
}