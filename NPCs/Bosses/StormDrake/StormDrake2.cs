using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using Terraria.Localization;
using OvermorrowMod.Projectiles.Boss;

namespace OvermorrowMod.NPCs.Bosses.StormDrake
{
    [AutoloadBossHead]
    public class StormDrake2 : ModNPC
    {
        private bool twothirdshealth = false;
        private bool phase2switched = false;
        private bool canPulse = false;
        private bool createAfterimage = false;
        private bool dashing = false;
        private int spritedirectionstore = 1;
        private int myprojectilestore;
        private int myprojectilestore2;
        public Vector2 targetPos;
        public float targetFloat;
        public bool p2 {get {return npc.life < npc.lifeMax / 2;}}
        public float ai0 {get {return npc.ai[0];} set{npc.ai[0] = value;}}
        public float ai1 {get {return npc.ai[0];} set{npc.ai[1] = value;}}
        public float ai2 {get {return npc.ai[0];} set{npc.ai[2] = value;}}
        public float ai3 {get {return npc.ai[0];} set{npc.ai[3] = value;}}
        public float ai4 {get {return npc.ai[0];} set{npc.localAI[0] = value;}}
        private int RandomCeiling;
        private int RandomCase;
        private int LastCase;
        private int SecondToLastCase;
        private Vector2 PlayerCenterStore;
        private int relativeX = -15;//-7;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Drake");
            Main.npcFrameCount[npc.type] = 6;
            NPCID.Sets.TrailCacheLength[npc.type] = 7;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.width = 296;
            npc.height = 232;
            npc.aiStyle = -1;
            npc.damage = 37;
            npc.defense = 14;
            npc.lifeMax = 7600;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.DD2_BetsyDeath;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.value = Item.buyPrice(gold: 3);
            npc.npcSlots = 10f;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/StormDrake");
            //bossBag = ModContent.ItemType<DrakeBag>();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.localAI[1]);
            writer.Write(npc.localAI[2]);
            writer.Write(npc.localAI[3]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.localAI[1] = reader.ReadSingle();
            npc.localAI[2] = reader.ReadSingle();
            npc.localAI[3] = reader.ReadSingle();
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.defense = 19;
            npc.damage = (int)(npc.damage * 0.25f);
        }
        public override void AI()
        {
            npc.ai[0] = 2;
            Player player = Main.player[npc.target];

            if (npc.lifeMax * 2 / 3 >= npc.life && twothirdshealth != true)
            {
                twothirdshealth = true;
            }
            if (phase2switched && !dashing)
            {
                if (npc.direction == 1) // Facing right
                {
                    for (int num1202 = 0; num1202 < 6; num1202++)
                    {
                        Vector2 vector304 = npc.position - new Vector2(-165, 74).RotatedBy(MathHelper.ToRadians(npc.rotation));
                        vector304 -= npc.velocity * ((float)num1202 * 0.25f);
                        Dust.NewDustDirect(vector304 + new Vector2(npc.width / 2, npc.height / 2), 1, 1, 206, 0, 0, 0, default, 1.5f);
                    }
                }
                else // Facing left
                {
                    for (int num1202 = 0; num1202 < 6; num1202++)
                    {
                        Vector2 vector304 = npc.position + new Vector2(-165, -74).RotatedBy(MathHelper.ToRadians(npc.rotation));
                        vector304 -= npc.velocity * ((float)num1202 * 0.25f);
                        Dust.NewDustDirect(vector304 + new Vector2(npc.width / 2, npc.height / 2), 1, 1, 206, 0, 0, 0, default, 1.5f);
                    }
                }
            }

            switch (npc.ai[0])
            {
                case -3:
                    {
                        if (!PlayerAlive(player)) { break; }

                        npc.dontTakeDamage = true;
                        if (npc.ai[1] >= 0 && npc.ai[1] <= 330)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                        if (npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75 && npc.ai[1] > 330)
                        {
                            npc.Move(player.Center + new Vector2(250 * (npc.spriteDirection * -1), -250), 4, 2);
                        }
                        if (npc.ai[1]++ > 480)//600)
                        {
                            npc.dontTakeDamage = false;
                            npc.ai[1] = 0;
                            npc.ai[0] = -1;
                            npc.velocity = Vector2.Zero;
                        }
                    }
                    break;
                case -2:
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (++npc.ai[1] > 0 && npc.ai[1] < 100)
                        {
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), 5, 2);
                        }
                        else if (npc.ai[1] == 100)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -2; i < 9; i++)
                                {
                                    Projectile.NewProjectile(npc.Center + new Vector2(25 * (i * npc.spriteDirection), -60), Vector2.UnitY * -1, ModContent.ProjectileType<TestLightning4>(), npc.damage, 2, Main.myPlayer, 0, npc.whoAmI);
                                }
                            }
                            npc.velocity = Vector2.Zero;
                            canPulse = true;
                            npc.immortal = true;
                            npc.dontTakeDamage = true;
                            if (!Main.raining)
                            {
                                Main.raining = true;
                                Main.rainTime = 180;
                            }
                            else
                            {
                                Main.rainTime += 120;
                            }

                            if (npc.ai[2] == 0) // Print phase 2 notifier
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
                                {
                                    Main.NewText("The air crackles with electricity...", Color.Teal);
                                }
                                else if (Main.netMode == NetmodeID.Server) // Server
                                {
                                    NetMessage.BroadcastChatMessage(NetworkText.FromLiteral("The air crackles with electricity..."), Color.Teal);
                                }
                                npc.ai[2]++;
                            }
                        }
                        else if (npc.ai[1] == 280)
                        {
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                                if (distance <= 600)
                                {
                                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 45;
                                }
                            }
                        }
                        else if (npc.ai[1] == 480)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectiles = 12;
                                for (int j = 0; j < projectiles; j++)
                                {
                                    Projectile.NewProjectile(npc.Center + new Vector2(0f, 75f).RotatedBy(j * MathHelper.TwoPi / projectiles), new Vector2(0f, 5f).RotatedBy(j * MathHelper.TwoPi / projectiles), ModContent.ProjectileType<LaserWarning2>(), npc.damage, 10f, Main.myPlayer);
                                }
                            }
                        }
                        else if (npc.ai[1] == 600)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectiles = 24;
                                for (int j = 0; j < projectiles; j++)
                                {
                                    Projectile.NewProjectile(npc.Center + new Vector2(0f, 75f).RotatedBy((j * MathHelper.TwoPi / projectiles) + MathHelper.ToRadians(25)), new Vector2(0f, 5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + 45), ModContent.ProjectileType<LaserWarning2>(), npc.damage, 10f, Main.myPlayer);
                                }
                            }
                        }
                        else if (npc.ai[1] == 700)
                        {
                            canPulse = false;
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            phase2switched = true;
                            npc.immortal = false;
                            npc.dontTakeDamage = false;
                        }
                        if (canPulse)
                        {
                            if (npc.direction == 1) // Facing right
                            {
                                for (int num1202 = 0; num1202 < 6; num1202++)
                                {
                                    Vector2 vector304 = npc.position - new Vector2(-165, 74);
                                    vector304 -= npc.velocity * ((float)num1202 * 0.25f);
                                    Dust.NewDustPerfect(vector304 + new Vector2((npc.width / 2), (npc.height / 2)), 206, null, 0, default, 1.5f);
                                }
                            }
                            else // Facing left
                            {
                                for (int num1202 = 0; num1202 < 6; num1202++)
                                {
                                    Vector2 vector304 = npc.position + new Vector2(-165, -74);
                                    vector304 -= npc.velocity * ((float)num1202 * 0.25f);
                                    Dust.NewDustPerfect(vector304 + new Vector2((npc.width / 2), (npc.height / 2)), 206, null, 0, default, 1.5f);
                                }
                            }
                        }
                    }
                    break;
                case -1:
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (twothirdshealth == true) { RandomCeiling = 7; }
                        else { RandomCeiling = 3; }
                        while (RandomCase == LastCase || RandomCase == SecondToLastCase)
                        {
                            RandomCase = Main.rand.Next(RandomCeiling);
                        }
                        if (RandomCase != LastCase && RandomCase != SecondToLastCase)
                        {
                            SecondToLastCase = LastCase;
                            LastCase = RandomCase;
                            npc.ai[0] = RandomCase;
                        }
                    }
                    break;
                case 3://0: // lightning breath
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (npc.ai[1] > 0 && npc.ai[1] <= 180 )
                        {
                            Vector2 spawnpos = npc.Center + new Vector2((/*180*/ 180 + Main.rand.NextFloat(75, 125)) * npc.spriteDirection, -40 + Main.rand.NextFloat(/*-10, 10*/ /*-25, 25*/ /*-50, 50*/ /*-75, 75*/ -60, 60));
                            Vector2 direction = npc.Center + new Vector2(160 * npc.spriteDirection, -40) - spawnpos;
                            direction.Normalize();
                            int mydust = Dust.NewDust(spawnpos, 0, 0, 229, 0f, 0f, 100);
                            Main.dust[mydust].noLight = true;
                            Main.dust[mydust].noGravity = true;
                            Main.dust[mydust].velocity = npc.velocity + (direction * Main.rand.NextFloat(12, 14));
                        }

                        if (npc.ai[1]++ == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                myprojectilestore = Projectile.NewProjectile(npc.Center + new Vector2(160 * npc.spriteDirection, -40), Vector2.UnitX * npc.spriteDirection, ModContent.ProjectileType<LaserBreathWarning>(), npc.damage, 2, Main.myPlayer, 0, npc.whoAmI);
                            }
                            ((LaserBreathWarning)Main.projectile[myprojectilestore].modProjectile).direction = npc.spriteDirection;
                        }
                        if (npc.ai[1] > 1 && npc.ai[1] < 180 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            ((LaserBreathWarning)Main.projectile[myprojectilestore].modProjectile).direction = npc.spriteDirection;
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), /*5*/ /*3*/ 4, 2);
                        }
                        if (npc.ai[1] == 180)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                myprojectilestore = Projectile.NewProjectile(npc.Center, npc.velocity, ModContent.ProjectileType<TestLightning2>(), npc.damage, 2, Main.myPlayer, 0, npc.whoAmI);
                            }
                            ((TestLightning2)Main.projectile[myprojectilestore].modProjectile).direction = npc.spriteDirection;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                myprojectilestore2 = Projectile.NewProjectile(npc.Center, npc.velocity, ModContent.ProjectileType<TestLightning2>(), npc.damage, 2, Main.myPlayer, 0, npc.whoAmI);
                            }
                            ((TestLightning2)Main.projectile[myprojectilestore2].modProjectile).direction = npc.spriteDirection;
                        }
                        if (npc.ai[1] > 180 && npc.ai[1] < 539 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            ((TestLightning2)Main.projectile[myprojectilestore].modProjectile).direction = npc.spriteDirection;
                            ((TestLightning2)Main.projectile[myprojectilestore2].modProjectile).direction = npc.spriteDirection;
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), /*5*/ /*3*/ 4, 2);
                        }
                        if (npc.ai[1] >= 600)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            myprojectilestore = 0;
                        }
                    }
                    break;
                case 4://1: // radial lightning ball
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (((npc.ai[1] > 0 && npc.ai[1] <= 180) || (npc.ai[1] > 440 && npc.ai[1] <= 580)))
                        {
                            Vector2 spawnpos = npc.Center + new Vector2((180 + Main.rand.NextFloat(75, 125)) * npc.spriteDirection, -40 + Main.rand.NextFloat(-60, 60));
                            Vector2 direction = npc.Center + new Vector2(160 * npc.spriteDirection, -40) - spawnpos;
                            direction.Normalize();
                            int mydust = Dust.NewDust(spawnpos, 0, 0, 229, 0f, 0f, 100);
                            Main.dust[mydust].noLight = true;
                            Main.dust[mydust].noGravity = true;
                            Main.dust[mydust].velocity = npc.velocity + (direction * Main.rand.NextFloat(12, 14));
                        }

                        if (npc.ai[1]++ == 180 || npc.ai[1] == 580)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center + new Vector2(170 * npc.spriteDirection, -45), npc.DirectionTo(player.Center) * 6f, ModContent.ProjectileType<ElectricBallRadialLightning>(), npc.damage / 2, 2, Main.myPlayer);
                            }
                        }
                        if (npc.ai[1] > 1 && npc.ai[1] < 900 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), 5, 2);
                        }
                        if (npc.ai[1] > 900)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            npc.velocity = Vector2.Zero;
                           
                        }
                    }
                    break;
                case 2: // now spinning electric balls  //lightning breath boring
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (twothirdshealth)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                        }

                        if (npc.ai[1]++ == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center + new Vector2(160 * npc.spriteDirection, -40), Vector2.UnitX * npc.spriteDirection, ModContent.ProjectileType<ElectricBallCenter>(), npc.damage, 2, Main.myPlayer, 0, npc.whoAmI);
                            }
                        }
                        if (npc.ai[1] > 1 && npc.ai[1] < 600 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), 10, 2);
                        }
                        if (npc.ai[1] >= 600)
                        {
                            if (twothirdshealth == true && phase2switched == false)
                            {
                                npc.velocity = Vector2.Zero;
                                npc.ai[0] = -2;
                                npc.ai[1] = 0;
                            }
                            else
                            {
                                npc.velocity = Vector2.Zero;
                                npc.ai[0] = -1;
                                npc.ai[1] = 0;
                            }
                        }
                    }
                    break;
                case 0://3: // multi directional dash
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (++npc.ai[1] > 0 && npc.ai[1] < 180 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), targetFloat)) > 75)
                        {
                            if (npc.ai[1] == 1)
                            {
                                npc.rotation = 0;
                                targetFloat = Main.rand.NextFloat(-350, 350);
                            }
                            else
                            {
                                npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), targetFloat), /*10*/ 15, 2);
                            }
                        }
                        if (++npc.ai[1] == 180)
                        {
                            if (Main.player[npc.target].Center.X < npc.Center.X && dashing == false)
                            {
                                PlayerCenterStore = player.Center;
                                npc.rotation = npc.DirectionTo(PlayerCenterStore).RotatedBy(MathHelper.ToRadians(180)).ToRotation();
                                npc.spriteDirection = -1;
                            }
                            else
                            {
                                PlayerCenterStore = player.Center;
                                npc.rotation = npc.DirectionTo(PlayerCenterStore).ToRotation();
                            }
                            npc.velocity = Vector2.Zero;
                            spritedirectionstore = npc.spriteDirection;
                            dashing = true;
                            //createAfterimage = true;
                        }
                        // not only is the multi rotational dashes broken with afterimage, but pulse too
                        if (npc.ai[1] > 180 && npc.ai[1] < /*240*/ /*180*/ 240)
                        {
                            //npc.velocity = Vector2.Zero;
                            if (npc.ai[1] > 180 && npc.ai[1] < 230)
                            {
                                canPulse = true;
                            }
                            else
                            {
                                canPulse = false;
                            }
                        }
                        if (npc.ai[1] == /*180*/ 240)
                        {
                            npc.velocity =25 * npc.DirectionTo(PlayerCenterStore);
                        }
                        else if (npc.ai[1] >  300 && npc.ai[1] < 420)
                        {
                            npc.velocity = Vector2.SmoothStep(npc.velocity, Vector2.Zero, 0.065f);
                        }
                        else if (npc.ai[1] > 420 && npc.ai[2] <= 5)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2]++;
                            dashing = false;
                            //createAfterimage = false;
                        }
                        else if (npc.ai[1] > 420 && npc.ai[2] > 5)
                        {
                            if (twothirdshealth == true && phase2switched == false)
                            {
                                npc.ai[0] = -2;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                dashing = false;
                                //createAfterimage = false;
                                npc.velocity = Vector2.Zero;
                                npc.rotation = 0;
                            }
                            else
                            {
                                npc.ai[0] = -1;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                dashing = false;
                                createAfterimage = false;
                                npc.velocity = Vector2.Zero;
                                npc.rotation = 0;
                            }
                        }
                    }
                    break;
                case 1://4: // horizontal dash
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (dashing == true && npc.ai[1] % 10 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 60 + Main.rand.Next(-5, 5), 4 * npc.spriteDirection, Main.rand.Next(-3, -1), ModContent.ProjectileType<TestLightning3>(), npc.damage / 3, 1, Main.myPlayer, 0, 0);
                                Projectile.NewProjectile(npc.Center.X, npc.Center.Y + 60 + Main.rand.Next(-5, 5), 4 * npc.spriteDirection, Main.rand.Next(1, 3), ModContent.ProjectileType<TestLightning3>(), npc.damage / 3, 1, Main.myPlayer, 0, 0);
                            }
                        }
                        if (++npc.ai[1] > 0 && npc.ai[1] < 200 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), /*10*/ 15, 2);
                        }
                        else if (npc.ai[1] > 200 && npc.ai[1] < 260)
                        {
                            npc.velocity = Vector2.Zero;
                            if (npc.ai[1] > 200 && npc.ai[1] < 250)
                            {
                                canPulse = true;
                            }
                            else
                            {
                                canPulse = false;
                            }
                        }
                        else if (npc.ai[1] == 260)
                        {
                            npc.velocity = (Vector2.UnitX * 25f) * npc.spriteDirection;
                            spritedirectionstore = npc.spriteDirection;
                            dashing = true;
                            createAfterimage = true;
                        }
                        else if (npc.ai[1] > 270 && npc.ai[1] < 430)
                        {
                            npc.velocity = Vector2.SmoothStep(npc.velocity, Vector2.Zero, 0.065f);
                        }
                        else if (npc.ai[1] > 430 && npc.ai[2] <= 2)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2]++;
                            dashing = false;
                            npc.velocity = Vector2.Zero;
                            createAfterimage = false;
                        }
                        else if (npc.ai[1] > 430 && npc.ai[2] > 2)
                        {
                            if (twothirdshealth == true && phase2switched == false)
                            {
                                npc.ai[0] = -2;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                dashing = false;
                                createAfterimage = false;
                                npc.velocity = Vector2.Zero;
                            }
                            else
                            {
                                npc.ai[0] = -1;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                dashing = false;
                                createAfterimage = false;
                                npc.velocity = Vector2.Zero;
                            }
                        }
                    }
                    break;
                case 5: // now       //spinning electro balls
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (npc.ai[1]++ % 10 == 0 && relativeX < 15)
                        {
                            if (npc.ai[1] < 15)
                            {
                                PlayerCenterStore = player.Center;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(new Vector2(PlayerCenterStore.X + (200 * relativeX), PlayerCenterStore.Y - 600), Vector2.UnitY * 5, ModContent.ProjectileType<LaserWarning2>(), 17, 1f, Main.myPlayer, 0, 1);
                            }
                            relativeX += 1;
                        }
                        if (npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), 10, 2);
                        }
                        if (relativeX > 14)
                        {
                            npc.ai[2]++;
                        }
                        if (npc.ai[2] >= 240)
                        {
                            PlayerCenterStore = Vector2.Zero;
                            relativeX = -15;
                            npc.velocity = Vector2.Zero;
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                        }
                    }
                    break;
                case 6: // electric shots
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (npc.ai[2]++ > 15 && npc.ai[2] <= 50 )
                        {
                            Vector2 spawnpos = npc.Center + new Vector2((180 + Main.rand.NextFloat(75, 125)) * npc.spriteDirection, -40 + Main.rand.NextFloat(-60, 60));
                            Vector2 direction = npc.Center + new Vector2(160 * npc.spriteDirection, -40) - spawnpos;
                            direction.Normalize();
                            int mydust = Dust.NewDust(spawnpos, 0, 0, 229, 0f, 0f, 100);
                            Main.dust[mydust].noLight = true;
                            Main.dust[mydust].noGravity = true;
                            Main.dust[mydust].velocity = npc.velocity + (direction * Main.rand.NextFloat(12, 14));
                        }

                        if (npc.ai[1]++ % 50 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(npc.Center + new Vector2(160 * npc.spriteDirection, -40), npc.DirectionTo(player.Center) * 7.5f, ModContent.ProjectileType<LaserWarning2>(), npc.damage, 2, Main.myPlayer, 0, npc.whoAmI);
                            }
                            npc.ai[2] = 0;
                        }
                        if (npc.ai[1] > 1 && npc.ai[1] < 300 && npc.Distance(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0)) > 75)
                        {
                            npc.Move(player.Center + new Vector2(450 * (npc.spriteDirection * -1), 0), 10, 2);
                        }
                        if (npc.ai[1] >= 300)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            npc.velocity = Vector2.Zero;
                        }
                    }
                    break;

            }
            npc.direction = npc.spriteDirection;
            npc.spriteDirection = 1;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if(npc.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, 206, 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * npc.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * npc.scale;
                    }
                }

                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeHead"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeWing"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeLeg"), npc.scale);
                Gore.NewGore(npc.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, mod.GetGoreSlot("Gores/DrakeTail"), npc.scale);
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            // Phase 2 Debuff
            if (npc.ai[2] == 2 && p2)
            {
                target.AddBuff(BuffID.Electrified, Main.expertMode ? 240 : 120);
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f)
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 6) // 6 is max # of frames
            { 
                npc.frame.Y = 0; // Reset back to default
            }
            if (Main.player[npc.target].Center.X < npc.Center.X && dashing == false)
            {
                npc.spriteDirection = -1;
            }
            while (dashing == true && npc.spriteDirection != spritedirectionstore)
            {
                npc.spriteDirection = spritedirectionstore;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
            Vector2 drawPos2 = Main.screenPosition + drawOrigin - new Vector2(60f, 290);

            if (createAfterimage)
            {
                //Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                //Vector2 drawOrigin = new Vector2(416, 522);
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = npc.oldPos[k] - Main.screenPosition + drawOrigin - new Vector2(60f, 290);
                    //Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.LightCyan : drawColor;
                    Color afterImageColor = Color.Lerp(Color.DarkBlue, Color.LightCyan, ((float)k / npc.oldPos.Length));
                    Color color = npc.GetAlpha(afterImageColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            Texture2D texture2D16 = mod.GetTexture("NPCs/Bosses/StormDrake/StormDrake2");

            //// this controls the passive pulsing effect
            //if (canPulse)
            //{
            //    // this gets the npc's frame
            //    Vector2 vector47 = drawOrigin;
            //    Color color55 = Color.White; // This is just white lol
            //    float amount10 = 0f; // I think this controls amount of color
            //    int num178 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
            //    int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


            //    // default value
            //    int num177 = 12; // ok i think this controls the number of afterimage frames
            //    float num176 = 1f - (float)Math.Cos((npc.ai[1] - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
            //    num176 /= 3f;
            //    float scaleFactor10 = 10f; // Change scale factor of the pulsing effect and how far it draws outwards

            //    Color color47 = Color.Lerp(Color.White, Color.Blue, 0.5f);
            //    color55 = Color.Cyan;
            //    amount10 = 1f;

            //    // ok this is the pulsing effect drawing
            //    for (int num164 = 1; num164 < num177; num164++)
            //    {
            //        // these assign the color of the pulsing
            //        Color color45 = Color.Cyan;
            //        //color45 = Color.Lerp(Color.DarkBlue, Color.Cyan, (float)num164 / num177);
            //        color45 = ((ModNPC)this).npc.GetAlpha(color45);
            //        color45 *= 1f - num176; // num176 is put in here to effect the pulsing

            //        // num176 is used here too
            //        Vector2 vector45 = ((Entity)((ModNPC)this).npc).Center + Terraria.Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + ((ModNPC)this).npc.rotation) * scaleFactor10 * num176 - Main.screenPosition;
            //        vector45 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[((ModNPC)this).npc.type]) * ((ModNPC)this).npc.scale / 2f;
            //        vector45 += vector47 * ((ModNPC)this).npc.scale + new Vector2(0f, 4f + ((ModNPC)this).npc.gfxOffY);

            //        // the actual drawing of the pulsing effect
            //        spriteBatch.Draw(texture2D16, vector45 - new Vector2(0, 290 / 2), ((ModNPC)this).npc.frame, color45, ((ModNPC)this).npc.rotation, vector47, ((ModNPC)this).npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            //    }
            //}
            //Texture2D texture = Main.npcTexture[npc.type];
            ////Texture2D texture2 = mod.GetTexture("Overmorrow/NPCs/StormDrake/StormDrake_Glowmask");
            //Texture2D texture2 = mod.GetTexture("Overmorrow/NPCs/StormDrake/StormDrake_Glowmask");
            //SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            //Vector2 origin = npc.frame.Size() / 2;
            /*spriteBatch.Reload(BlendState.Additive);
            if ((npc.aiAction == 0 && npc.ai[0] > 300))
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    float alpha = (float)i / (float)NPCID.Sets.TrailCacheLength[npc.type];
                    Vector2 pos = npc.oldPos[i] - Main.screenPosition + npc.Size / 2;
                    spriteBatch.Draw(texture, pos, npc.frame, Color.White * ((float)i / (float)npc.oldPos.Length), npc.rotation, origin, npc.scale, effects, 0f);
                }
            }
            if (npc.ai[2] == 1 && npc.ai[0] > 70 && npc.ai[0] < 170)
            {
                for (int i = 0; i < 4; i++)
                {
                    float rot = MathHelper.ToRadians(i * 360 / 4);
                    float progress = (float)Math.Sin((npc.ai[0] - 70) / 100 * Math.PI);
                    Vector2 offset = rot.ToRotationVector2() * 30 * progress;
                    spriteBatch.Draw(texture, npc.Center + offset - Main.screenPosition, npc.frame, npc.GetAlpha(drawColor) * 0.6f, npc.rotation, origin, npc.scale, effects, 0f);
                }
            }
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, effects, 0f);
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(texture2, npc.Center, npc.frame, Color.White, npc.rotation, origin, npc.scale, effects, 0.01f);*/
            //return false;
            if (canPulse)//(phaseAnimation)
            {
                // this gets the npc's frame
                Vector2 vector472 = drawOrigin;
                Color color552 = Color.White; // This is just white lol
                float amount102 = 0f; // I think this controls amount of color
                int num1782 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num1792 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


                // default value
                int num1772 = 6; // ok i think this controls the number of afterimage frames
                float num1762 = 1f - (float)Math.Cos((npc.ai[1] - (float)num1782) / (float)num1792 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num1762 /= 3f;
                float scaleFactor102 = 40f; // Change scale factor of the pulsing effect and how far it draws outwards

                Color color472 = Color.Lerp(Color.White, Color.Blue, 0.5f);
                color552 = Color.Cyan;
                amount102 = 1f;

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num1772; num164++)
                {
                    // these assign the color of the pulsing
                    Color color452 = color472;
                    color452 = Color.Lerp(color452, color552, amount102);
                    color452 = ((ModNPC)this).npc.GetAlpha(color452);
                    color452 *= 1f - num1762; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector452 = ((Entity)((ModNPC)this).npc).Center + Utils.ToRotationVector2((float)num164 / (float)num1772 * ((float)Math.PI * 2f) + ((ModNPC)this).npc.rotation) * scaleFactor102 * num1762 - Main.screenPosition;
                    vector452 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[((ModNPC)this).npc.type]) * ((ModNPC)this).npc.scale / 2f;
                    vector452 += vector472 * ((ModNPC)this).npc.scale + new Vector2(0f, 4f + ((ModNPC)this).npc.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture2D16, vector452 - new Vector2(0, 290 / 2), ((ModNPC)this).npc.frame, color452, ((ModNPC)this).npc.rotation, vector472, ((ModNPC)this).npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }
            return true;
        }
        public override void NPCLoot()
        {
            OvermorrowWorld.downedDrake = true;
            if (Main.raining)
            {
                Main.raining = false;
                Main.rainTime = 0;
            }

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(5);
                // Always drops one of:
                if (choice == 0) // Warden
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<LightningPiercer>());
                }
                else if (choice == 1) // Mage
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<BoltStream>());
                }
                else if (choice == 2) // Warrior
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<StormTalon>());
                }
                else if (choice == 3) // Ranger
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<TempestGreatbow>());
                }
                else if (choice == 4) // Summoner
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrakeStaff>());
                }

                if (Main.rand.Next(10) == 0) // Trophy Dropchance
                {
                    //Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrakeTrophy>());
                }
            }
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
        bool PlayerAlive(Player player)
        {
            if (!player.active || player.dead)
            {
                player = Main.player[npc.target];
                npc.TargetClosest();
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
    }
}