using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class InfernoMage : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        int frame = 1;
        private int look = 0;
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inferno Mage");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void SetDefaults()
        {
            npc.aiStyle = 0;//-1;
            npc.width = 30;
            npc.height = 46;
            npc.aiStyle = 3;
            npc.damage = 9;
            npc.defense = 3;
            npc.lifeMax = 140;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath24;
            npc.value = 25f;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            npc.spriteDirection = look;
        }
        public override void AI()
        {

                experttimer++;
                if(expert && experttimer == 1)
                {
                    npc.life = npc.life / 2;
                    npc.lifeMax = npc.lifeMax / 2;
                }


            if (Main.player[npc.target].position.X + 275 > npc.position.X && Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.X += 0.5f;
            }
            if (Main.player[npc.target].position.X - 275 < npc.position.X && Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.X -= 0.5f;
            }
            if (npc.velocity.X > 4f)
            {
                npc.velocity.X = 4f;
            }
            if (npc.velocity.X < -4f)
            {
                npc.velocity.X = -4f;
            }

            if (Main.player[npc.target].position.X < npc.position.X)
            {
                look = -1;
            }

            if (Main.player[npc.target].position.X > npc.position.X)
            {
                look = 1;
            }

            npc.TargetClosest(true);
            npc.netUpdate = true;
            timer++;
            if (timer == 150)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 10, mod.NPCType("InfernoRoller"), 0, npc.whoAmI, npc.Center.X, npc.Center.Y, 0.0f, byte.MaxValue);
                frame = 2;
            }
            if (timer == 160)
            {
                frame = 0;
            }
            if (timer == 170)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 10, mod.NPCType("InfernoRoller"), 0, npc.whoAmI, npc.Center.X, npc.Center.Y, 0.0f, byte.MaxValue);
                frame = 2;
            }
            if (timer == 180)
            {
                frame = 0;
            }
            if (timer == 190)
            {
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y + 10, mod.NPCType("InfernoRoller"), 0, npc.whoAmI, npc.Center.X, npc.Center.Y, 0.0f, byte.MaxValue);
                frame = 2;
            }
            if (timer == 200)
            {
                frame = 0;
            }
            if (timer == 450)
            {
                timer = 0;
            }
        }
    }
}

