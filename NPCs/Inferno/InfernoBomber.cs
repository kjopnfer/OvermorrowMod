using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{

    public class InfernoBomber : ModNPC
    {

        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        int frame = 6;
        private int timer = 0;
        private int othertimer = 0;
        private readonly int movetimer = 0;
        private int jumptimer = 0;
        private int look = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inferno Bomber");
            Main.npcFrameCount[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 0;//-1;
            npc.width = 30;
            npc.height = 55;
            npc.aiStyle = 0;
            npc.damage = 15;
            npc.defense = 3;
            npc.lifeMax = 150;
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
            if (expert && experttimer == 1)
            {
                npc.life = npc.life / 2;
                npc.lifeMax = npc.lifeMax / 2;
                npc.damage = npc.damage / 2;
            }

            if (Main.player[npc.target].position.X + 512 < npc.position.X && movetimer == 0)
            {
                npc.velocity.X -= 0.3f;
                look = -1;
            }

            if (Main.player[npc.target].position.X + 499 > npc.position.X && movetimer == 0 && Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.X += 0.3f;
                look = 1;
            }


            if (Main.player[npc.target].position.X - 512 > npc.position.X && movetimer == 0)
            {
                npc.velocity.X += 0.3f;
                look = 1;
            }

            if (Main.player[npc.target].position.X - 499 < npc.position.X && movetimer == 0 && Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.X -= 0.3f;
                look = -1;
            }

            if (Main.player[npc.target].position.X + 490 > npc.position.X && Main.player[npc.target].position.X < npc.position.X)
            {
                look = -1;
            }

            if (Main.player[npc.target].position.X + 490 > npc.position.X && Main.player[npc.target].position.X < npc.position.X)
            {
                look = 1;
            }


            if (npc.velocity.X > 1.13f)
            {
                npc.velocity.X = 1.13f;
            }

            if (npc.velocity.X < -1.13f)
            {
                npc.velocity.X = -1.13f;
            }

            if (Main.player[npc.target].position.X - 475 > npc.position.X && Main.player[npc.target].position.Y + 15 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 6f;
                jumptimer = 250;
            }

            if (Main.player[npc.target].position.X + 475 < npc.position.X && Main.player[npc.target].position.Y + 15 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 6f;
                jumptimer = 250;
            }

            if (jumptimer > 0)
            {
                jumptimer--;
            }

            npc.TargetClosest(true);
            npc.netUpdate = true;
            timer++;


            if (timer > 150 && timer < 230)
            {
                frame = 5;
            }
            else
            {
                othertimer++;
                if (npc.velocity.X > 0.3f || npc.velocity.X < -0.3f)
                {
                    frame++;
                    othertimer = 0;
                }
                else
                {
                    frame = 4;
                }


                if (frame > 19)
                {
                    frame = 6;
                }

            }

            if (timer == 150 && Main.player[npc.target].position.X < npc.position.X)
            {
                look = -1;
            }

            if (timer == 150 && Main.player[npc.target].position.X > npc.position.X)
            {
                look = 1;
            }

            if (timer == 150)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 5f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoBomb");
                int damage = npc.damage + 5;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }

            if (timer == 160)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 5f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoBomb");
                int damage = npc.damage + 5;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }

            if (timer == 170)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 5f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoBomb");
                int damage = npc.damage + 5;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
            if (timer == 180)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 5f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoBomb");
                int damage = npc.damage + 5;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
            if (timer == 190)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 5f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoBomb");
                int damage = npc.damage + 5;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
            }
            if (timer == 550)
            {
                timer = 0;
            }
        }
    }
}

