using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Misc;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Molemen
{
    public class MoleScout : ModNPC
    {
        private bool RunOnce = true;
        private int NPCStyle;

        private int frame = 0;
        private int frameTimer = 0;

        private const int MAX_FRAMES = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moleman Scout");
            Main.npcFrameCount[NPC.type] = MAX_FRAMES;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 38;
            NPC.damage = 20;
            NPC.defense = 6;
            NPC.lifeMax = 360;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;

            // knockBackResist is the multiplier applied to the knockback the NPC receives when it takes damage
            NPC.knockBackResist = 0.5f;

            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            if (Main.netMode == NetmodeID.Server || Main.dedServ)
            {
                writer.Write(RunOnce);
                writer.Write(NPCStyle);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                RunOnce = reader.ReadBoolean();
                NPCStyle = reader.ReadInt32();
            }
        }

        public enum AIStates
        {
            Walk = 0,
            Swipe = 1
        }

        public ref float AICase => ref NPC.ai[0];
        public ref float AICounter => ref NPC.ai[1];

        public override void AI()
        {
            if (RunOnce)
            {
                NPCStyle = Main.rand.Next(2);
                RunOnce = false;

                NPC.netUpdate = true;
            }

            switch (AICase)
            {
                case (int)AIStates.Walk:
                    if (NPC.collideX && NPC.velocity.Y == 0)
                    {
                        NPC.velocity.Y -= 3f;
                    }

                    #region Frame Animation
                    // Idle animation
                    // Frames 0 - 9
                    if (frame < 9)
                    {
                        frameTimer++;
                        if (frameTimer % 8 == 0)
                        {
                            frame++;
                        }
                    }
                    else
                    {
                        frameTimer++;
                        if (frameTimer % 8 == 0)
                        {
                            frame = 0;
                            frameTimer = 0;
                        }
                    }
                    #endregion

                    if (AICounter++ == 240)
                    {
                        AICase = (int)AIStates.Walk;
                        AICounter = 0;
                    }
                    break;

            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            NPC.frame.Width = TextureAssets.Npc[NPC.type].Value.Width / 2;
            NPC.frame.X = NPC.frame.Width * NPCStyle;
            //npc.frame.X = 0;
            NPC.frame.Y = frameHeight * frame;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, spriteEffects, 0);

            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // TODO: ((MonkeyStone)Main.item[item].modItem).itemFrame = Main.rand.Next(0, 3); can this be done somehow?
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonkeyStone>()));
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.spawnTileY <= WorldGen.lavaLine - 100 && spawnInfo.spawnTileY > Main.rockLayer)
            {
                return 0.5f;
            }

            return 0;
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), tileX * 16, tileY * 16, NPC.type);
        }
    }
}