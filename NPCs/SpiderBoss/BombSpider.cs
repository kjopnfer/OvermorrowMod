using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class BombSpider : ModNPC
    {
        readonly bool expert = Main.expertMode;
        int frame = 1;
        int spiderspritetimer = 0;
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 34;
            npc.damage = 25;
            npc.aiStyle = 3;
            aiType = NPCID.CorruptPenguin;
            npc.noTileCollide = false;
            npc.defense = 0;
            npc.lifeMax = 40;
            npc.alpha = 20;
            npc.value = 25f;
            npc.noGravity = false;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        int spiderexp = 0;
        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = npc.direction;
            npc.frame.Y = frameHeight * frame;
        }
        public override void AI()
        {

            spiderexp++;
            spiderspritetimer++;
            if(spiderspritetimer > 3)
            {
                frame++;
                spiderspritetimer = 0;
            }
            if(frame > 3)
            {
                frame = 0;
            }

                if (Main.player[npc.target].position.X < npc.position.X)
                {
                   npc.velocity.X -= 0.4f; // accelerate to the left
                }
                if (Main.player[npc.target].position.X > npc.position.X)
                {
                    npc.velocity.X += 0.4f; // accelerate to the right
                }
                if(npc.velocity.X > 5.5f)
                {
                    npc.velocity.X = 5.5f;
                }
                if(npc.velocity.X < -5.5f)
                {
                 npc.velocity.X = -5.5f;
                }

            if(expert)
            {
                if (Main.player[npc.target].position.X < npc.position.X)
                {
                   npc.velocity.X -= 0.65f; // accelerate to the left
                }
                if (Main.player[npc.target].position.X > npc.position.X)
                {
                    npc.velocity.X += 0.65f; // accelerate to the right
                }
                if(npc.velocity.X > 7.5f)
                {
                    npc.velocity.X = 7.5f;
                }
                if(npc.velocity.X < -7.5f)
                {
                 npc.velocity.X = -7.5f;
                }
            }

            if (Main.player[npc.target].Center.X + 28 > npc.Center.X && Main.player[npc.target].Center.X < npc.Center.X && Main.player[npc.target].Center.Y > npc.Center.Y - 28 || Main.player[npc.target].Center.X + 28 > npc.Center.X && Main.player[npc.target].Center.X < npc.Center.X && Main.player[npc.target].Center.Y > npc.Center.Y + 28)
            {
                spiderexp = 900;
            }

            if (Main.player[npc.target].Center.X - 28 < npc.Center.X && Main.player[npc.target].Center.X > npc.Center.X && Main.player[npc.target].Center.Y > npc.Center.Y - 28 || Main.player[npc.target].Center.X - 28 < npc.Center.X && Main.player[npc.target].Center.X > npc.Center.X && Main.player[npc.target].Center.Y > npc.Center.Y + 28) 
            {
                spiderexp = 900;
            }

            if(spiderexp > 750)
            {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, value1.X, value1.Y, mod.ProjectileType("SpiderBomb"), npc.damage + 7, 1f);
            npc.life -= 1000;
            }
        }
        public override void NPCLoot()
        {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, value1.X, value1.Y, mod.ProjectileType("SpiderBomb"), npc.damage + 7, 1f);
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
            DisplayName.SetDefault("Bomb Spider");
        }
    }
}

