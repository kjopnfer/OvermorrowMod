using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.BloodMoon
{
    public class BottleNPC : ModNPC
    {


        private int ProjTimer = 0;
        private int AttTimer = 0;
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angry Bottle");
            Main.npcFrameCount[npc.type] = 16;
        }

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 53;
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
                frameTimer = 0;
                npc.velocity.X *= 0.3f;
                if(npc.velocity.X < 0.5f && npc.velocity.X > -0.5f)
                {
                    frame = 7;
                }


                ProjTimer++;
                if(ProjTimer == 40)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = npc.Center + new Vector2(0, -100);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Projectile.NewProjectile(npc.Center, direction * 9, ModContent.ProjectileType<SkullHoming>(), 17, 3f, Main.player[npc.target].whoAmI, 0f);
                    Main.PlaySound(2, npc.position, 61);
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
                    if(frame > 15)
                    {
                        frame = 2;
                    }
                    if(frame < 2)
                    {
                        frame = 2;
                    }
                }
                else
                {
                    frame = 1;
                }
            }

            if(AttTimer >= 300)
            {
                ProjTimer = 0;
                AttTimer = 0;
            }

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.bloodMoon == true && spawnInfo.player.ZoneOverworldHeight ? SpawnCondition.OverworldNightMonster.Chance * 0.15f : 0f;
        }
    }
}

