using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.Event.Goblin
{
    public class GoblinWarden : ModNPC
    {

        private int ProjTimer = 0;
        private int AttTimer = 0;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goblin Warden");
            Main.npcFrameCount[npc.type] = 20;
        }

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 46;
            npc.damage = 15;
            npc.defense = 8;
            npc.lifeMax = 65;
            animationType = NPCID.ChaosElemental;
            npc.HitSound = SoundID.Item2;
            npc.DeathSound = SoundID.Item2;
            npc.value = 5f;
            npc.knockBackResist = 0.4f;
            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
        }

        public override void AI()
        {


            if (npc.velocity.X > 1)
            {
                npc.velocity.X = 1;
            }

            if (npc.velocity.X < -1)
            {
                npc.velocity.X = -1;
            }

            Player player = Main.player[npc.target];
            if (npc.velocity.Y == 0)
            {
                AttTimer++;
            }
            if (AttTimer > 50 && Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 175 && npc.velocity.Y == 0)
            {

                frameTimer = 0;


                npc.velocity.X *= 0.3f;



                if (ProjTimer > 33)
                {
                    ProjTimer = 0;
                }
                if (Main.player[npc.target].ownedProjectileCounts[ModContent.ProjectileType<GoblinKnife>()] > 0)
                {
                    frame = 3;
                }
                else if (npc.velocity.X < 0.5f && npc.velocity.X > -0.5f)
                {
                    frame = 0;
                }

                ProjTimer++;
                if (ProjTimer > 30 && Main.player[npc.target].ownedProjectileCounts[ModContent.ProjectileType<GoblinKnife>()] < 1)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center + new Vector2(0, -120);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    int proj = Projectile.NewProjectile(npc.Center, direction * 0, mod.ProjectileType("GoblinKnife"), 15, 0.0f, Main.myPlayer, 0.0f, (float)npc.whoAmI);
                    npc.netUpdate = true;
                    Main.PlaySound(SoundID.Item, npc.position, 19);
                    ProjTimer = 0;
                }
            }
            else
            {
                frameTimer++;
                if (npc.velocity.Y == 0)
                {
                    if (frameTimer > 3)
                    {
                        frameTimer = 0;
                        frame++;
                    }
                    if (frame > 19)
                    {
                        frame = 6;
                    }
                    if (frame < 6)
                    {
                        frame = 6;
                    }
                }
                else
                {
                    frame = 5;
                }
            }

            if (AttTimer >= 100)
            {
                ProjTimer = 0;
                AttTimer = 0;
            }

        }
    }
}

