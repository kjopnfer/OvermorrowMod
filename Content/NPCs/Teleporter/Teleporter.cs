using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Teleporter
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
            Main.npcFrameCount[NPC.type] = 20;
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 46;
            NPC.damage = 40;
            NPC.defense = 8;
            NPC.lifeMax = 120;
            AnimationType = NPCID.ChaosElemental;
            NPC.HitSound = SoundID.Item2;
            NPC.DeathSound = SoundID.Item2;
            NPC.value = 5f;
            NPC.knockBackResist = 0.4f;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameHeight * frame;
        }

        public override void AI()
        {

            Player player = Main.player[NPC.target];
            if (NPC.velocity.Y == 0)
            {
                AttTimer++;
            }
            if (AttTimer > 175 && Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 80 && NPC.velocity.Y == 0)
            {
                frameTimer++;

                if (frameTimer > 3)
                {
                    frameTimer = 0;
                }

                NPC.velocity.X *= 0.3f;
                if (NPC.velocity.X < 0.5f && NPC.velocity.X > -0.5f)
                {
                    if (frame > 0 && frameTimer == 3)
                    {
                        frame--;
                    }
                    if (frame > 4)
                    {
                        frame = 0;
                    }
                }


                ProjTimer++;
                if (ProjTimer == 20)
                {
                    Vector2 position = NPC.Center;
                    Vector2 targetPosition = Main.player[NPC.target].Center + new Vector2(0, -120);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, direction * 12f, Mod.Find<ModProjectile>("Teleproj").Type, 0, 0.0f, Main.myPlayer, 0.0f, (float)NPC.whoAmI);
                    SoundEngine.PlaySound(SoundID.Item, NPC.position, 19);
                    frame = 4;
                }
            }
            else
            {
                frameTimer++;
                if (NPC.velocity.Y == 0)
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

            if (AttTimer >= 200)
            {
                ProjTimer = 0;
                AttTimer = 0;
            }

        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return Main.tile[spawnInfo.spawnTileX, spawnInfo.spawnTileY].TileType == 367 ? 0.7f : 0f;
        }
    }
}

