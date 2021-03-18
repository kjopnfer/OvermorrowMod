using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Consumable;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Projectiles.Boss;
using System;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs
{
    public class Hemomancer : ModNPC
    {
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hemomancer");
            Main.npcFrameCount[npc.type] = 3;
        }

        public override void SetDefaults()
        {
            //npc.width = 18;
            //npc.height = 40;
            npc.width = 32;
            npc.height = 46;
            npc.damage = 29;
            npc.defense = 10;
            npc.lifeMax = 350;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            npc.knockBackResist = 0.6f;
            npc.value = 140f;
            npc.npcSlots = 2f;
            npc.buffImmune[20] = true;
        }

        public override void AI()
        {
            Lighting.AddLight(npc.Center, 0.75f, 0f, 0f);
            //npc.ai[0] = 400f;
            npc.TargetClosest();
            npc.velocity.X *= 0.93f;
            if (npc.velocity.X > -0.1 && npc.velocity.X < 0.1)
            {
                npc.velocity.X = 0f;
            }
            if (npc.ai[0] == 0f)
            {
                npc.ai[0] = 500f;
            }

            if (npc.ai[2] != 0f && npc.ai[3] != 0f)
            {
                Main.PlaySound(SoundID.Item8, npc.position);
                for (int num175 = 0; num175 < 50; num175++)
                {
                    int num214 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1.5f);
                    Dust dust46 = Main.dust[num214];
                    dust46.velocity *= 3f;
                    Main.dust[num214].noGravity = true;
                }
                npc.position.X = npc.ai[2] * 16f - (npc.width / 2) + 8f;
                npc.position.Y = npc.ai[3] * 16f - npc.height;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Main.PlaySound(SoundID.Item8, npc.position);
                for (int num179 = 0; num179 < 50; num179++)
                {
                    int num180 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 5, 0f, 0f, 100, default, 1.5f);
                    Dust dust46 = Main.dust[num180];
                    dust46.velocity *= 3f;
                    Main.dust[num180].noGravity = true;
                }
            }

            npc.ai[0] += 1f;

            // Shooting code, shoots thrice
            if (npc.ai[0] == 100f || npc.ai[0] == 200f || npc.ai[0] == 300f)
            {
                npc.ai[1] = 30f;
                npc.netUpdate = true;
            }

            // Teleporting code
            if (npc.ai[0] >= 650f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 1f;
                int num234 = (int)Main.player[npc.target].position.X / 16;
                int num235 = (int)Main.player[npc.target].position.Y / 16;
                int num239 = (int)npc.position.X / 16;
                int num241 = (int)npc.position.Y / 16;
                int num242 = 20;
                int num251 = 0;
                bool flag149 = false;
                if (Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
                {
                    num251 = 100;
                    flag149 = true;
                }
                while (!flag149 && num251 < 100)
                {
                    num251++;
                    int num253 = Main.rand.Next(num234 - num242, num234 + num242);
                    int num254 = Main.rand.Next(num235 - num242, num235 + num242);
                    for (int num255 = num254; num255 < num235 + num242; num255++)
                    {
                        if ((num255 < num235 - 4 || num255 > num235 + 4 || num253 < num234 - 4 || num253 > num234 + 4) && (num255 < num241 - 1 || num255 > num241 + 1 || num253 < num239 - 1 || num253 > num239 + 1) && Main.tile[num253, num255].nactive())
                        {
                            bool flag150 = true;
                            if ((!Main.wallDungeon[Main.tile[num253, num255 - 1].wall]))
                            {
                                flag150 = false;
                            }
                            else if (Main.tile[num253, num255 - 1].lava())
                            {
                                flag150 = false;
                            }
                            if (flag150 && Main.tileSolid[Main.tile[num253, num255].type] && !Collision.SolidTiles(num253 - 1, num253 + 1, num255 - 4, num255 - 1))
                            {
                                npc.ai[1] = 20f;
                                npc.ai[2] = num253;
                                npc.ai[3] = num255;
                                flag149 = true;
                                break;
                            }
                        }
                    }
                }
                npc.netUpdate = true;
            }

            // Projectile code
            if (npc.ai[1] > 0f)
            {
                npc.ai[1] -= 1f;
                if (npc.ai[1] == 25f)
                {
                    frame = 1;
                    Main.PlaySound(SoundID.Item8, npc.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //NPC.NewNPC((int)npc.position.X + npc.width / 2, (int)npc.position.Y - 8, 33);

                        Player player = Main.player[npc.target];
                        Vector2 shootPosition = new Vector2(npc.Center.X, npc.Center.Y - 20);

                        float speed = 7;

                        float speedX = player.Center.X - shootPosition.X;
                        float speedY = player.Center.Y - shootPosition.Y;
                        float length = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                        float num12 = speed / length;
                        speedX = speedX * num12;
                        speedY = speedY * num12;

                        Vector2 position = shootPosition;
                        Vector2 targetPosition = Main.player[npc.target].Center;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(npc.Center + new Vector2(0, -20), direction * speed, ModContent.ProjectileType<SplittingBlood_Hemomancer>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                        }
                    }
                }
            }

            // NPC Dust
            if (Main.rand.Next(3) != 0)
            {
                int num327 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 5, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 0.9f);
                Main.dust[num327].noGravity = true;
                Dust expr_448D_cp_0 = Main.dust[num327];
                expr_448D_cp_0.velocity.X = expr_448D_cp_0.velocity.X * 0.3f;
                Dust expr_44AB_cp_0 = Main.dust[num327];
                expr_44AB_cp_0.velocity.Y = expr_44AB_cp_0.velocity.Y * 0.2f;
                Dust expr_44C9_cp_0 = Main.dust[num327];
                expr_44C9_cp_0.velocity.Y = expr_44C9_cp_0.velocity.Y - 1f;
            }

            // 15 ticks of frame 1
            if(frame == 1 && frameTimer < 15)
            {
                frameTimer++;
            }
            else
            {
                frame = 0;
                frameTimer = 0;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dmg = 10;
            if (npc.life > 0)
            {
                for (int num333 = 0; (double)num333 < dmg / (double)npc.lifeMax * 50.0; num333++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 26, hitDirection, -1f);
                }
                return;
            }
            for (int num331 = 0; num331 < 20; num331++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 26, 2.5f * (float)hitDirection, -2.5f);
            }

            Gore.NewGore(npc.position, npc.velocity, 42, npc.scale);
            Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 20f), npc.velocity, 43, npc.scale);
            Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 20f), npc.velocity, 43, npc.scale);
            Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 34f), npc.velocity, 44, npc.scale);
            Gore.NewGore(new Vector2(npc.position.X, npc.position.Y + 34f), npc.velocity, 44, npc.scale);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
        }

        public override void NPCLoot()
        {
            // Drops Bloodburst Staff and Sanguine Beacon (Replace with Bloody Tear when 1.4)

            if (Main.rand.Next(2) == 0) // Beacon Dropchance
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BloodBeacon>());
            }

            if (Main.rand.Next(3) == 0) // Staff Dropchance
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BloodStaff>());
            }

            if (Main.rand.Next(4) == 0) // Blood Gem Dropchance
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BloodGem>());
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.ZoneDungeon && OvermorrowWorld.downedTree ? 0.025f : 0f;
        }
    }
}
