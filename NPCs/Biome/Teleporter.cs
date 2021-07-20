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
            npc.damage = 40;
            npc.defense = 8;
            npc.lifeMax = 120;
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

            Player player = Main.player[npc.target];
            if(npc.velocity.Y == 0)
            {
                AttTimer++;
            }
            if(AttTimer > 175 && Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 80 && npc.velocity.Y == 0)
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
                if(ProjTimer == 20)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center + new Vector2(0, -120);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
				    int proj = Projectile.NewProjectile(npc.Center, direction * 12f, mod.ProjectileType("Teleproj"), 0, 0.0f, Main.myPlayer, 0.0f, (float)npc.whoAmI);
                    Main.PlaySound(2, npc.position, 19);
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

            if(AttTimer >= 200)
            {
                ProjTimer = 0;
                AttTimer = 0;
            }

        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].type == 367 ? 0.7f : 0f;
		}
    }
}

