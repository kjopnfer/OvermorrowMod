using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Misc;
using OvermorrowMod.Core;
using System.IO;
using Terraria;
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
            Main.npcFrameCount[npc.type] = MAX_FRAMES;
        }

        public override void SetDefaults()
        {
            npc.width = 32;
            npc.height = 38;
            npc.damage = 20;
            npc.defense = 6;
            npc.lifeMax = 360;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.value = 60f;

            // knockBackResist is the multiplier applied to the knockback the NPC receives when it takes damage
            npc.knockBackResist = 0.5f;

            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public enum AIStates
        {
            Walk = 0,
            Swipe = 1
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

        public ref float AICase => ref npc.ai[0];
        public ref float AICounter => ref npc.ai[1];

        public override void AI()
        {
            if (RunOnce)
            {
                NPCStyle = Main.rand.Next(2);
                RunOnce = false;

                npc.netUpdate = true;
            }

            switch (AICase)
            {
                case (int)AIStates.Walk:
                    if (npc.collideX && npc.velocity.Y == 0)
                    {
                        npc.velocity.Y -= 3f;
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
            npc.spriteDirection = npc.direction;
            npc.frame.Width = Main.npcTexture[npc.type].Width / 2;
            npc.frame.X = npc.frame.Width * NPCStyle;
            //npc.frame.X = 0;
            npc.frame.Y = frameHeight * frame;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = Main.npcTexture[npc.type];
            var spriteEffects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override void NPCLoot()
        {
            int item = Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MonkeyStone>());
            ((MonkeyStone)Main.item[item].modItem).itemFrame = Main.rand.Next(0, 3);
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
            return NPC.NewNPC(tileX * 16, tileY * 16, npc.type);
        }
    }
}