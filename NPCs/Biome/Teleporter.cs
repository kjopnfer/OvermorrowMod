using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Biome
{
    public class Teleporter : ModNPC
    {


        private int ProjTimer = 0;
        private int AttTimer = 0;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Test 2");
            Main.npcFrameCount[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 46;
            npc.damage = 14;
            npc.defense = 6;
            npc.lifeMax = 45;
            animationType = NPCID.ChaosElemental;
            npc.HitSound = SoundID.Item64;
            npc.DeathSound = SoundID.Item27;
            npc.value = 5f;
            npc.knockBackResist = 0.5f;
            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
        }

        public override void AI()
        {

            Player player = Main.player[npc.target];
            if(npc.velocity.Y == 0)
            {
                AttTimer++;
            }
            if(AttTimer > 250 && Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 600 && npc.velocity.Y == 0)
            {
                frameTimer++;

                if(frameTimer > 3)
                {
                    frameTimer = 0;
                }

                npc.velocity.X *= 0.3f;
                if(npc.velocity.X < 0.5f && npc.velocity.X > -0.5f)
                {
                    if(frame > 0 && frameTimer == 3)
                    {
                        frame--;
                    }
                    if(frame > 4)
                    {
                        frame = 0;
                    }
                }


                ProjTimer++;
                if(ProjTimer == 40)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center + new Vector2(0, -100);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Projectile.NewProjectile(npc.Center, direction * 13, ModContent.ProjectileType<Teleproj>(), 17, 3f, Main.player[npc.target].whoAmI, 0f);
                    Main.PlaySound(2, npc.position, 61);
                    frame = 4;
                }
            }
            else
            {
                frameTimer++;
                if(npc.velocity.Y == 0)
                {
                    if (frameTimer > 3)
                    {
                        frameTimer = 0;
                        frame++;
                    }
                    if(frame > 19)
                    {
                        frame = 6;
                    }
                    if(frame < 6)
                    {
                        frame = 6;
                    }
                }
                else
                {
                    frame = 5;
                }
            }

            if(AttTimer >= 300)
            {
                ProjTimer = 0;
                AttTimer = 0;
            }

        }
    }
}

