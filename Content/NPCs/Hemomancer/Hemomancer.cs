using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Weapons.Magic.BloodStaff;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Hemomancer
{
    public class Hemomancer : ModNPC
    {
        private int frame = 0;
        private int frameTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hemomancer");
            Main.npcFrameCount[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            //npc.width = 18;
            //npc.height = 40;
            NPC.width = 32;
            NPC.height = 46;
            NPC.damage = 29;
            NPC.defense = 10;
            NPC.lifeMax = 350;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.6f;
            NPC.value = 140f;
            NPC.npcSlots = 2f;
            NPC.buffImmune[20] = true;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0.75f, 0f, 0f);
            //npc.ai[0] = 400f;
            NPC.TargetClosest();
            NPC.velocity.X *= 0.93f;
            if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
            {
                NPC.velocity.X = 0f;
            }
            if (NPC.ai[0] == 0f)
            {
                NPC.ai[0] = 500f;
            }

            if (NPC.ai[2] != 0f && NPC.ai[3] != 0f)
            {
                SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                for (int num175 = 0; num175 < 50; num175++)
                {
                    int num214 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.5f);
                    Dust dust46 = Main.dust[num214];
                    dust46.velocity *= 3f;
                    Main.dust[num214].noGravity = true;
                }
                NPC.position.X = NPC.ai[2] * 16f - (NPC.width / 2) + 8f;
                NPC.position.Y = NPC.ai[3] * 16f - NPC.height;
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                for (int num179 = 0; num179 < 50; num179++)
                {
                    int num180 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 1.5f);
                    Dust dust46 = Main.dust[num180];
                    dust46.velocity *= 3f;
                    Main.dust[num180].noGravity = true;
                }
            }

            NPC.ai[0] += 1f;

            // Shooting code, shoots thrice
            if (NPC.ai[0] == 100f || NPC.ai[0] == 200f || NPC.ai[0] == 300f)
            {
                NPC.ai[1] = 30f;
                NPC.netUpdate = true;
            }

            // Teleporting code
            if (NPC.ai[0] >= 650f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.ai[0] = 1f;
                int num234 = (int)Main.player[NPC.target].position.X / 16;
                int num235 = (int)Main.player[NPC.target].position.Y / 16;
                int num239 = (int)NPC.position.X / 16;
                int num241 = (int)NPC.position.Y / 16;
                int num242 = 20;
                int num251 = 0;
                bool flag149 = false;
                if (Math.Abs(NPC.position.X - Main.player[NPC.target].position.X) + Math.Abs(NPC.position.Y - Main.player[NPC.target].position.Y) > 2000f)
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
                        if ((num255 < num235 - 4 || num255 > num235 + 4 || num253 < num234 - 4 || num253 > num234 + 4) && (num255 < num241 - 1 || num255 > num241 + 1 || num253 < num239 - 1 || num253 > num239 + 1) && Main.tile[num253, num255].HasUnactuatedTile)
                        {
                            bool flag150 = true;
                            if ((!Main.wallDungeon[Main.tile[num253, num255 - 1].WallType]))
                            {
                                flag150 = false;
                            }
                            else if (Main.tile[num253, num255 - 1].LiquidType == LiquidID.Lava)
                            {
                                flag150 = false;
                            }
                            if (flag150 && Main.tileSolid[Main.tile[num253, num255].TileType] && !Collision.SolidTiles(num253 - 1, num253 + 1, num255 - 4, num255 - 1))
                            {
                                NPC.ai[1] = 20f;
                                NPC.ai[2] = num253;
                                NPC.ai[3] = num255;
                                flag149 = true;
                                break;
                            }
                        }
                    }
                }
                NPC.netUpdate = true;
            }

            // Projectile code
            if (NPC.ai[1] > 0f)
            {
                NPC.ai[1] -= 1f;
                if (NPC.ai[1] == 25f)
                {
                    frame = 1;
                    SoundEngine.PlaySound(SoundID.Item8, NPC.position);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        //NPC.NewNPC((int)npc.position.X + npc.width / 2, (int)npc.position.Y - 8, 33);

                        Player player = Main.player[NPC.target];
                        Vector2 shootPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 20);

                        float speed = 7;

                        float speedX = player.Center.X - shootPosition.X;
                        float speedY = player.Center.Y - shootPosition.Y;
                        float length = (float)Math.Sqrt(speedX * speedX + speedY * speedY);
                        float num12 = speed / length;
                        speedX = speedX * num12;
                        speedY = speedY * num12;

                        Vector2 position = shootPosition;
                        Vector2 targetPosition = Main.player[NPC.target].Center;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -20), direction * speed, ModContent.ProjectileType<SplittingBlood_Hemomancer>(), NPC.damage / 2, 3f, Main.myPlayer, 0, 0);
                        }
                    }
                }
            }

            // NPC Dust
            if (!Main.rand.NextBool(3))
            {
                int num327 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + 2f), NPC.width, NPC.height, DustID.Blood, NPC.velocity.X * 0.2f, NPC.velocity.Y * 0.2f, 100, default(Color), 0.9f);
                Main.dust[num327].noGravity = true;
                Dust expr_448D_cp_0 = Main.dust[num327];
                expr_448D_cp_0.velocity.X = expr_448D_cp_0.velocity.X * 0.3f;
                Dust expr_44AB_cp_0 = Main.dust[num327];
                expr_44AB_cp_0.velocity.Y = expr_44AB_cp_0.velocity.Y * 0.2f;
                Dust expr_44C9_cp_0 = Main.dust[num327];
                expr_44C9_cp_0.velocity.Y = expr_44C9_cp_0.velocity.Y - 1f;
            }

            // 15 ticks of frame 1
            if (frame == 1 && frameTimer < 15)
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
            if (NPC.life > 0)
            {
                for (int num333 = 0; (double)num333 < dmg / (double)NPC.lifeMax * 50.0; num333++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, hitDirection, -1f);
                }
                return;
            }
            for (int num331 = 0; num331 < 20; num331++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Bone, 2.5f * (float)hitDirection, -2.5f);
            }

            var source = NPC.GetSource_OnHurt(null);
            Gore.NewGore(source, NPC.position, NPC.velocity, 42, NPC.scale);
            Gore.NewGore(source, new Vector2(NPC.position.X, NPC.position.Y + 20f), NPC.velocity, 43, NPC.scale);
            Gore.NewGore(source, new Vector2(NPC.position.X, NPC.position.Y + 20f), NPC.velocity, 43, NPC.scale);
            Gore.NewGore(source, new Vector2(NPC.position.X, NPC.position.Y + 34f), NPC.velocity, 44, NPC.scale);
            Gore.NewGore(source, new Vector2(NPC.position.X, NPC.position.Y + 34f), NPC.velocity, 44, NPC.scale);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = frameHeight * frame;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Drops Bloodburst Staff and Sanguine Beacon (Replace with Bloody Tear when 1.4) TODO somebody who knows what this means please
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodBeacon>(), 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodStaff>(), 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodGem>(), 4));

            base.ModifyNPCLoot(npcLoot);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].type == ModContent.NPCType<Hemomancer>() && Main.npc[i].active)
                {
                    return 0f;
                }
            }
            return spawnInfo.Player.ZoneDungeon ? 0.025f : 0f;
        }
    }
}
