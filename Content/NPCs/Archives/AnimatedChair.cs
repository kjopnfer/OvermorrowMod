using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using OvermorrowMod.Content.Biomes;
using Terraria.Localization;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;

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
            AIState = (int)AICase.Summon;
        }

        public override void AI()
        {

            switch ((AICase)AIState)
            {
                case AICase.Summon:
                    Vector3 originalColor = new Vector3(0.5f, 0.3765f, 0.3980f);
                    float lerpFactor = MathHelper.Clamp((AICounter - 60f) / 60f, 0f, 1f);
                    Vector3 lerpedColor = Vector3.Lerp(originalColor, Vector3.Zero, lerpFactor);

                    Lighting.AddLight(NPC.Center, lerpedColor);

                    if (AICounter++ >= 120)
                    {
                        AIState = (int)AICase.Idle;
                        AICounter = 0;
                    }
                    break;
                case AICase.Idle:
                    if (AICounter++ >= idleTime)
                    {
                        AIState = (int)AICase.Jump;
                        AICounter = 0;
                    }
                    break;
                case AICase.Jump:
                    NPC.TargetClosest();
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
                    float rectangleHeight = MathHelper.SmoothStep(-NPC.frame.Height, 0, AICounter / 60f);
                    drawRectangle = new Rectangle(0, (int)rectangleHeight, NPC.frame.Width, NPC.frame.Height);
                }
            }

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, drawRectangle, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            if (AIState == (int)AICase.Summon)
            {
                Rectangle glowRectangle = new Rectangle(0, 0, NPC.width + 16, 1);
                drawColor = Color.Pink;

                float scale = 0.1f;
                float particleSpawnRate = MathHelper.Lerp(6, 15, MathHelper.Clamp(AICounter - 60, 0, 60f) / 60f);
                if (AICounter % particleSpawnRate == 0 && !Main.gamePaused && AICounter < 110)
                {
                    int randomIterations = Main.rand.Next(1, 3);
                    for (int i = 0; i < randomIterations; i++)
                    {
                        Vector2 spawnPosition = NPC.Center + new Vector2(Main.rand.Next(-2, 3) * 6, 20);
                        Particle.CreateParticleDirect(Particle.ParticleType<LightOrb>(), spawnPosition, -Vector2.UnitY, Color.Pink, 1f, scale, 0f, 0, scale * 0.5f);
                    }
                }

                float baseAlpha = 0.8f;
                if (AICounter >= 60)
                {
                    baseAlpha = MathHelper.Lerp(0.8f, 0, (AICounter - 60f) / 60f);
                }

                for (int i = 0; i < 30; i++)
                {
                    Vector2 drawOffset = new Vector2(0, 39 - i);
                    float drawAlpha = baseAlpha - (i / 30f);
                    Color auraColor = Color.Lerp(Color.Pink, Color.DeepPink, i / 30f);

                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, NPC.Center + drawOffset - Main.screenPosition, glowRectangle, auraColor * drawAlpha, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
                }
            }

            return false;
        }
    }
}