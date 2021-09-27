using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class InfernoWarrior : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        private int movetimer = 0;
        private int jumptimer = 0;
        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 56;
            npc.aiStyle = 0;
            aiType = NPCID.CorruptPenguin;
            animationType = NPCID.ArmoredSkeleton;
            npc.damage = 15;
            npc.defense = 6;
            npc.lifeMax = 210;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath54;
            npc.value = 25f;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Inferno Warrior");
            Main.npcFrameCount[npc.type] = 15;
        }
        public override void AI()
        {
            experttimer++;
            if (expert && experttimer == 1)
            {
                npc.life = npc.life / 2;
                npc.lifeMax = npc.lifeMax / 2;
            }


            movetimer--;
            if (movetimer > 37)
            {
                jumptimer = 100;
                npc.velocity.X = 0f;
            }

            if (Main.player[npc.target].position.X + 50 > npc.position.X && Main.player[npc.target].position.X < npc.position.X)
            {
                npc.velocity.X = 0f;
            }

            if (Main.player[npc.target].position.X - 50 < npc.position.X && Main.player[npc.target].position.X > npc.position.X)
            {
                npc.velocity.X = 0f;
            }

            if (Main.player[npc.target].position.X - 50 < npc.position.X)
            {
                npc.velocity.X -= 0.5f;
            }

            if (Main.player[npc.target].position.X + 50 > npc.position.X)
            {
                npc.velocity.X += 0.5f;
            }

            if (npc.velocity.X > 2.1f)
            {
                npc.velocity.X = 2.1f;
            }

            if (npc.velocity.X < -2.1f)
            {
                npc.velocity.X = -2.1f;
            }

            if (Main.player[npc.target].position.X + 50 > npc.position.X && movetimer < 1 && Main.player[npc.target].position.X < npc.position.X)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 2f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoSpear");
                int damage = npc.damage + 7;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                movetimer = 37;
            }

            if (Main.player[npc.target].position.X - 50 < npc.position.X && movetimer < 1 && Main.player[npc.target].position.X > npc.position.X)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 2f;
                npc.velocity.X = 0f;
                int type = mod.ProjectileType("InfernoSpear");
                int damage = npc.damage + 7;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                movetimer = 37;
            }

            if (Main.player[npc.target].position.X - 20 > npc.position.X && Main.player[npc.target].position.Y + 10 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 2f;
                jumptimer = 150;
                npc.velocity.X = 1.5f;
            }

            if (Main.player[npc.target].position.X + 20 < npc.position.X && Main.player[npc.target].position.Y + 10 < npc.position.Y && jumptimer == 0 && movetimer == 0)
            {
                npc.velocity.Y -= 2f;
                jumptimer = 150;
                npc.velocity.X = -1.5f;
            }

            if (jumptimer > 0)
            {
                jumptimer--;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = npc.lifeMax * 1;
            npc.damage = npc.damage * 1;
        }
    }
}

