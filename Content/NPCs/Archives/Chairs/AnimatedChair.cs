using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Common.Utilities;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class AnimatedChair : ChairSummon
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SafeSetDefaults()
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

        public override void MovementAI()
        {
            NPC.TargetClosest();
            NPC.Move(Player.Center, 0.1f, maxSpeed, 8f);
        }

        public override void SetFrame()
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

        protected override void DrawNPCBestiary(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);
        }
       
        protected override int GlowRectangleWidth => NPC.width + 16;
        protected override Vector2 ParticleSpawnOffset => new Vector2(Main.rand.Next(-2, 3) * 6, 20);
        protected override int AuraHeightOffset => 39;
    }
}