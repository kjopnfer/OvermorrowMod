using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Core.Globals;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class AnimatedSofa : ChairSummon
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
        {
            AggroSound = SoundID.Zombie58 with
            {
                Pitch = 0.8f
            };

            NPC.width = 30;
            NPC.height = 42;
            NPC.lifeMax = 110;
            NPC.defense = 8;
            NPC.damage = 23;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, silver: 2, copper: 20);

            SpawnModBiomes = [ModContent.GetInstance<GrandArchives>().Type];
        }

        public override void MovementAI()
        {
            /*NPC.TargetClosest();
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
            }*/
        }

        public override void SetFrame()
        {
            if (NPC.IsABestiaryIconDummy)
            {
                xFrame = 0;
                if (NPC.frameCounter++ % 8 == 0)
                {
                    yFrame++;
                    if (yFrame >= 2) yFrame = 0;
                }

                return;
            }

            switch (AIState)
            {
                case Hop:
                    xFrame = 0;
                    yFrame = 2;
                    break;
                default:
                    xFrame = 0;
                    if (NPC.frameCounter++ % 8 == 0)
                    {
                        yFrame++;
                        if (yFrame >= 2) yFrame = 0;
                    }
                    break;
            }

            /*if (NPC.IsABestiaryIconDummy) AIState = (int)AICase.Idle;

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

            }*/
        }

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 drawOffset = new Vector2(0, -2);
            spriteBatch.Draw(texture, NPC.Center + drawOffset, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }

        protected override int GlowRectangleWidth => NPC.width + 30;
        protected override Vector2 ParticleSpawnOffset => new Vector2(Main.rand.Next(-3, 4) * 6, 20);
        protected override int AuraHeightOffset => 51;
    }
}