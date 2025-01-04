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
using OvermorrowMod.Common.Utilities;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class AnimatedChair : OvermorrowNPC
    {
        public override string Texture => AssetDirectory.ArchiveNPCs + Name;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 32;
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
            NPC.TargetClosest();
            AIState = (int)AICase.Summon;

            switch ((AICase)AIState)
            {
                case AICase.Summon:
                    Lighting.AddLight(NPC.Center, new Vector3(0.5f, 0.3765f, 0.3980f));
                    if (AICounter++ >= 120)
                    {
                        AIState = (int)AICase.Summon;
                        AICounter = 0;
                    }
                    break;
                case AICase.Idle:
                    if (AICounter++ >= idleTime)
                    {
                        AIState = (int)AICase.Summon;
                        AICounter = 0;
                    }
                    break;
                case AICase.Jump:
                    float maxSpeed = 2f;
                    NPC.Move(player.Center, 0.1f, maxSpeed, 8f);
                    break;
            }
        }

        private void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy) AIState = (int)AICase.Jump;

            switch ((AICase)AIState)
            {
                case AICase.Idle:
                    xFrame = 0;
                    yFrame = 0;
                    break;
                case AICase.Jump:
                    xFrame = 0;
                    if (NPC.velocity.Y == 0) NPC.frameCounter++;

                    if (NPC.frameCounter % 6 == 0)
                    {
                        yFrame++;
                        if (yFrame >= 6) yFrame = 0;
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
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 4;
            NPC.frame.Height = TextureAssets.Npc[NPC.type].Value.Height / 6;

            NPC.frame.X = NPC.frame.Width * xFrame;
            NPC.frame.Y = NPC.frame.Height * yFrame;
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        public override bool DrawNPC(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle drawRectangle = NPC.frame;
            if (AIState == (int)AICase.Summon)
            {
                if (AICounter < 60)
                {
                    float rectangleHeight = MathHelper.Lerp(-NPC.frame.Height, 0, AICounter / 60f);
                    drawRectangle = new Rectangle(0, (int)rectangleHeight, NPC.frame.Width, NPC.frame.Height);
                }
            }

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, drawRectangle, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            if (AIState == (int)AICase.Summon)
            {
                Rectangle glowRectangle = new Rectangle(0, 0, NPC.width + 16, 1);
                drawColor = Color.Pink;

                float baseAlpha = 0.8f;
                if (AICounter >= 60)
                {
                    baseAlpha = MathHelper.Lerp(0.8f, 0, (AICounter - 60f) / 60f);
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 drawOffset = new Vector2(0, 39 - i);
                    float drawAlpha = baseAlpha - (i / 30f);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, NPC.Center + drawOffset - Main.screenPosition, glowRectangle, drawColor * drawAlpha, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
                }
            }

            return false;
        }
    }
}