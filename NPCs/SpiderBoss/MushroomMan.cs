using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class MushroomMan : ModNPC
    {
        private readonly int timer = 0;
        private readonly int movetimer = 0;
        private int jumptimer = 0;
        private int frame = 1;
        int spiderspritetimer = 0;
        private int lasertimer = 0;
        int diret = 1;

        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 72;
            npc.damage = 0;
            npc.aiStyle = 3;
            aiType = NPCID.CorruptPenguin;
            npc.noTileCollide = false;
            npc.defense = 0;
            npc.lifeMax = 200;
            npc.alpha = 20;
            npc.value = 25f;
            npc.noGravity = false;
            npc.lavaImmune = true;
            npc.buffImmune[BuffID.OnFire] = true;
        }

        int spiderexp = 0;


        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = diret;
            npc.frame.Y = frameHeight * frame;
        }

        public override void AI()
        {

            if(lasertimer > 150)
            {
                int dust = Dust.NewDust(npc.Center, npc.width, npc.height, DustID.Fire, 0.0f, 0.0f, 400, new Color(), 1.2f);
                Main.dust[dust].noGravity = false;
            }

            if (Main.player[npc.target].Center.X < npc.Center.X)
            {
                diret = -1;
            }
            if (Main.player[npc.target].Center.X > npc.Center.X)
            {
                diret = 1;
            }

            if (Main.player[npc.target].Center.X + 95 < npc.Center.X && movetimer == 0)
            {
                npc.velocity.X -= 0.3f;
            }

            if (Main.player[npc.target].Center.X + 70 > npc.Center.X && movetimer == 0 && Main.player[npc.target].Center.X < npc.Center.X)
            {
                npc.velocity.X += 0.3f;
            }

            if (Main.player[npc.target].Center.X - 95 > npc.Center.X && movetimer == 0)
            {
                npc.velocity.X += 0.3f;
            }

            if (Main.player[npc.target].Center.X - 70 < npc.Center.X && movetimer == 0 && Main.player[npc.target].Center.X > npc.Center.X)
            {
                npc.velocity.X -= 0.3f;
            }

            if (npc.velocity.X > 1.2f)
            {
                npc.velocity.X = 1.2f;
            }

            if (npc.velocity.X < -1.2f)
            {
                npc.velocity.X = -1.2f;
            }


            if (jumptimer > 0)
            {
                jumptimer--;
            }


            lasertimer++;
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
            if(lasertimer == 200 && Main.player[npc.target].Center.X + 220 > npc.Center.X && Main.player[npc.target].Center.X < npc.Center.X || lasertimer == 100 && Main.player[npc.target].Center.X - 220 < npc.Center.X && Main.player[npc.target].Center.X > npc.Center.X)
            {
                Vector2 position = npc.Center;
                Vector2 targetPosition = Main.player[npc.target].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 0.7f;
                int type = mod.ProjectileType("SpiderBolt");
                int damage = 29;
                Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                Main.PlaySound(SoundID.Item12, (int)position.X, (int)position.Y);
            }

            if(lasertimer == 201)
            {
                lasertimer = 0;
            }

        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[npc.type] = 4;
            DisplayName.SetDefault("Mushroom Spider");
        }
    }
}

