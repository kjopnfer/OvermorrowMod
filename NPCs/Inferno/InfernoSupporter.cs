using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class InfernoSupporter : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        int frame = 0;
        private int timer = 0;
        private readonly int movetimer = 0;

        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 56;
            npc.aiStyle = 0;
            animationType = NPCID.SkeletonArcher;
            npc.damage = 1;
            npc.defense = 4;
            npc.lifeMax = 100;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 25f;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inferno Supporter");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void AI()
        {


                experttimer++;
                if(expert && experttimer == 1)
                {
                    npc.life = npc.life / 2;
                    npc.lifeMax = npc.lifeMax / 2;
                    npc.damage = 1;
                }

            if (Main.player[npc.target].position.X + 365 < npc.position.X && movetimer == 0)
            {
                npc.velocity.X -= 0.6f;
            }

            if (Main.player[npc.target].position.X + 355 > npc.position.X && movetimer == 0 && Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.X += 0.6f;
            }

            if (Main.player[npc.target].position.X - 365 > npc.position.X && movetimer == 0)
            {
                npc.velocity.X += 0.6f;
            }

            if (Main.player[npc.target].position.X - 355 < npc.position.X && movetimer == 0 && Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.X -= 0.6f;
            }

            if (Main.player[npc.target].position.Y - 70 < npc.position.Y)
            {
                npc.velocity.Y -= 0.6f;
            }

            if (npc.velocity.Y > 9f)
            {
                npc.velocity.Y -= 0.6f;
            }

            if (npc.velocity.X > 2.5f)
            {
                npc.velocity.X = 2.5f;
            }

            if (npc.velocity.X < -2.5f)
            {
                npc.velocity.X = -2.5f;
            }

            if(timer < 10 || timer > 190)
            {
                frame = 1;
            }
            else
            {
                frame = 0;
            }

            timer++;
            if (timer > 200)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 0f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("Pentagram");
                int damage = 1;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                timer = 0;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            npc.spriteDirection = npc.direction;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = npc.lifeMax * 1;
            npc.damage = npc.damage * 1;
        }
    }
}

