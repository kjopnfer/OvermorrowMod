using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class FlameThrower : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        private float expertflames = 0f;
        private int frametimer = 0;
        private int frame = 6;
        private int timer = 0;
        private int movetimer = 0;
        private int jumptimer = 0;
        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 56;
            npc.aiStyle = 0;
            animationType = NPCID.SkeletonArcher;
            npc.damage = 15;
            npc.defense = 4;
            npc.lifeMax = 150;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 25f;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inferno Flame Bower");
            Main.npcFrameCount[npc.type] = 20;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;
            npc.frame.Y = frameHeight * frame;
        }
        public override void AI()
        {

                experttimer++;
                if(expert && experttimer == 1)
                {
                    npc.life = npc.life / 2;
                    npc.lifeMax = npc.lifeMax / 2;
                    npc.damage = npc.damage / 2;
                    expertflames = 0.5f;
                }


            if (Main.player[npc.target].position.X + 200 < npc.position.X && movetimer == 0)
            {
                npc.velocity.X -= 0.5f;
            }

            if (Main.player[npc.target].position.X + 175 > npc.position.X && movetimer == 0 && Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.X += 0.5f;
            }

            if (Main.player[npc.target].position.X - 200 > npc.position.X && movetimer == 0)
            {
                npc.velocity.X += 0.5f;
            }

            if (Main.player[npc.target].position.X - 175 < npc.position.X && movetimer == 0 && Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.X -= 0.5f;
            }

            if (npc.velocity.X > 1.5f)
            {
                npc.velocity.X = 1.5f;
            }

            if (npc.velocity.X < -1.5f)
            {
                npc.velocity.X = -1.5f;
            }

            if (Main.player[npc.target].position.X - 200 > npc.position.X && Main.player[npc.target].position.Y - 10 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 6f;
                jumptimer = 150;
                npc.velocity.X = 1.5f;
            }

            if (Main.player[npc.target].position.X + 200 < npc.position.X && Main.player[npc.target].position.Y - 10 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 6f;
                jumptimer = 150;
                npc.velocity.X = -1.5f;
            }

            if (Main.player[npc.target].position.X - 200 > npc.position.X && Main.player[npc.target].position.Y + 10 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 2f;
                jumptimer = 150;
                npc.velocity.X = 1.5f;
            }

            if (Main.player[npc.target].position.X + 200 < npc.position.X && Main.player[npc.target].position.Y + 10 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 2f;
                jumptimer = 150;
                npc.velocity.X = -1.5f;
            }

            if (jumptimer > 0)
            {
                jumptimer--;
            }

            if (timer < 166)
            {
                frametimer++;
                if(frametimer == 3)
                {
                    frametimer = 0;
                    frame++;
                }
                if(frame > 19)
                {
                    frame = 6;
                }
            }
            timer++;
            if (timer > 165)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 1.5f + expertflames;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("FlameFire");
                int damage = npc.damage + 7;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                movetimer++;
                frame = 3;
            }
            if (timer == 183)
            {
                timer = 0;
                movetimer = 0;
                frame = 6;
            }
        }
    }
}

