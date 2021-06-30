using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    public class ApollusBoss : ModNPC
    {
        public override string Texture => "OvermorrowMod/NPCs/Bosses/Apollus/Apollus";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apollus");
            Main.npcFrameCount[npc.type] = 5;
        }
        public override void SetDefaults()
        {
            npc.width = 123;
            npc.height = 123;
            npc.defense = 12;
            npc.lifeMax = 2800;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(gold: 5);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
        }

        public int maxRuneCircle = 3;
        public int timer = 0;
        int RandomCase = 0;
        int LastCase = 0;
        int RandomCeiling;
        Vector2 teleportposition = Vector2.Zero;
        int spritetimer = 0;
        int frame = 1;
        int proj;
        int projalt;
        //bool direction;
        bool direction = true;
        bool changedPhase2 = false;
        bool changedPhase3 = false;
        Vector2 playercentersnapshot;
        Vector2 spawnpos;

        public override void AI()
        {
            Player player = Main.player[npc.target];
            bool expertMode = Main.expertMode;
            if (npc.life <= npc.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }
            if (npc.life <= npc.lifeMax * 0.25f)
            {
                changedPhase3 = true;
            }
            switch (npc.ai[0])
            {
                case 0:
                    {
                        if (!AliveCheck(player)) { break; }

                        if (++npc.ai[1] == 1)
                        {
                            proj = Projectile.NewProjectile(player.Center.X, player.Center.Y - 100f, 0f, 0f, ProjectileType<ArrowRuneCircle>(), 10, 0f, Main.myPlayer, -10, npc.whoAmI);
                        }

                        if (npc.ai[1] == 360)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = 4;
                            npc.ai[3] = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        if (!AliveCheck(player)) { break; }

                        if (++npc.ai[1] % 30 == 0)
                        {
                            int projectiles = changedPhase2 ? 5 : 3;
                            int random = Main.rand.Next(5);
                            for (int j = 0; j < projectiles; j++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(0f, 8.5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + (npc.ai[2] * 30) + random), ProjectileType<ApollusArrow>(), 12, 10f, Main.myPlayer);
                            }
                            npc.ai[2] += 1;
                        }

                        if (npc.ai[1] == 420)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = 4;
                            npc.ai[3] = 0;
                        }
                    }
                    break;
                case 2:
                    {
                        if (!AliveCheck(player)) { break; }

                        if (npc.ai[1]++ == 0)
                        {
                            playercentersnapshot = player.Center;
                        }

                        if (npc.ai[2]++ > 0 && npc.ai[3] != 390/ 30 && npc.ai[1] > 1)
                        {
                            npc.position = playercentersnapshot + new Vector2(-550, 0).RotatedBy(MathHelper.ToRadians(30 * npc.ai[3]));
                            npc.position.X -= npc.width / 2;
                            npc.position.Y -= npc.height / 2;
                            if (npc.ai[3] <= 360 / 30)
                            {
                                Projectile.NewProjectileDirect(npc.Center, npc.DirectionTo(playercentersnapshot) * 5f, ProjectileType<ApollusArrowTwo>(), 15, 12, Main.myPlayer, 0, npc.ai[3]);
                            }
                            Vector2 origin = npc.Center;
                            float radius = 20;
                            int numLocations = 30;
                            for (int k = 0; k < 2; k++)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 236, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                            npc.ai[3]++;
                            npc.ai[2] = 0;
                        }

                        /*if(npc.ai[2]++ > 0 && npc.ai[3] != 390 / 30 && npc.ai[1] > 1)
                        {
                            npc.position = playercentersnapshot + new Vector2(-550, 0).RotatedBy(MathHelper.ToRadians(30 * npc.ai[3]));
                            npc.position.X -= npc.width / 2;
                            npc.position.Y -= npc.height / 2;
                            Vector2 origin = npc.Center;
                            float radius = 20;
                            int numLocations = 30;
                            for (int k = 0; k < 2; k++)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 236, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                            npc.ai[3]++;
                            npc.ai[2] = 0;
                        }*/

                        if (npc.ai[3] == 390 / 30 && npc.ai[1] == 240)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = 4;
                            npc.ai[3] = 0;
                        }
                    }
                    break;
                case 3:
                    {
                        if (!AliveCheck(player)) { break; }

                        if (++npc.ai[1] == 1)
                        {
                            projalt = Projectile.NewProjectile(npc.Center + (Vector2.UnitX * - 50 * (direction ? -1 : 1)), Vector2.Zero, ProjectileType<ArrowRuneCircle>(), 15, direction ? -1 : 1, Main.myPlayer, -20, npc.whoAmI);
                        }

                        if (npc.ai[1] > 1 && npc.ai[1] < 360)
                        {
                            npc.position = player.Center + new Vector2(-61 + (-600 * (direction ? 1 : -1)), -61);
                        }

                        if (npc.ai[1] == 360)
                        {
                            Main.projectile[projalt].Kill();
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[3] = 0;
                            npc.ai[0] = 4;
                        }
                    }
                    break;
                case 4:
                    {
                        if (!AliveCheck(player)) { break; }

                        npc.ai[1]++;

                        if (changedPhase2 == true) { RandomCeiling = 4; }
                        else { RandomCeiling = 2; }
                        while (RandomCase == LastCase)
                        {
                            RandomCase = Main.rand.Next(RandomCeiling);
                        }
                        if (RandomCase == 3 && npc.ai[1] == 15)
                        {
                            direction = Main.rand.NextBool();
                            teleportposition = player.Center + new Vector2(-61 + (-600 * (direction ? 1 : -1)), -61);
                        }
                        else if (RandomCase != 3 && npc.ai[1] == 15)
                        {
                            teleportposition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            while (Main.tile[(int)teleportposition.X / 16, (int)teleportposition.Y / 16].active())
                            {
                                teleportposition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            }
                        }

                        if (npc.ai[1] > 30)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = teleportposition;
                                float radius = 20;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 236, dustvelocity.X, dustvelocity.Y, 0, default, 1);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (npc.ai[1] > 90)
                        {
                            npc.Teleport(teleportposition + new Vector2(-61, -61), 236);
                        }
                        if (npc.ai[1] > 100)
                        {
                            LastCase = RandomCase;
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[3] = 0;
                            npc.ai[0] = RandomCase;
                        }
                    }
                    break;
            }

            npc.spriteDirection = npc.direction;

            spritetimer++;
            if (spritetimer > 8)
            {
                frame++;
                spritetimer = 0;
            }
            if (frame > 3)
            {
                frame = 0;
            }
        }

        private bool AliveCheck(Player player)
        {
            if (!player.active || player.dead)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    if (npc.timeLeft > 25)
                    {
                        npc.timeLeft = 25;
                        npc.velocity = Vector2.UnitY * -7;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            if (Main.player[npc.target].Center.X < npc.Center.X)
            {
                npc.spriteDirection = -1;
            }
        }
    }
}

/*case 2:
                    {
                        if (!AliveCheck(player)) { break; }

                        if (++npc.ai[1] % 45 == 0)
                        {
                            int projectiles = 6;
                            for (int j = 0; j < projectiles; j++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(0f, 5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + (npc.ai[2] * 15)), ProjectileType<ApollusArrowNormal>(), 2, 10f, Main.myPlayer);
                            }
                            npc.ai[2] += 1;
                        }

                        if (npc.ai[1] == 180)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = 3;
                        }
                    }
break;*/