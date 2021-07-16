using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Town
{
    public class Twin1NPC : ModNPC
    {
        private int ProjTimer = 0;
        private int AttTimer = 0;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spazmamini");
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.width = 22;
            npc.height = 36;
            npc.damage = 35;
            npc.defense = 9;
            npc.lifeMax = 75;
            animationType = NPCID.CursedSkull;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath4;
            npc.value = 5f;
            npc.knockBackResist = 0.4f;
            npc.aiStyle = 5;
            aiType = NPCID.EaterofSouls;
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            AttTimer++;
            if(AttTimer == 100)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                npc.velocity = direction * 7.5f;
                AttTimer = 0;
            }
            if(AttTimer < 50)
            {
                if (Main.player[npc.target].position.X < npc.position.X)
                {
                    npc.velocity.X -=  0.05f;
                }

                if (Main.player[npc.target].position.Y < npc.position.Y )
                {
                    npc.velocity.Y -= 0.05f;
                }

                if (Main.player[npc.target].position.X > npc.position.X)
                {
                    npc.velocity.X += 0.05f;
                }

                if (Main.player[npc.target].position.Y > npc.position.Y)
                {
                    npc.velocity.Y += 0.05f;
                }
            }
        }
    }
}

