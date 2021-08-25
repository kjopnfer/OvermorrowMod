//TODO
//safe zones workin pretty well
//alright so next step
//is to work with these even further
//id reduce the radius by maybe half
//we should also do an attack where if ur not in a safe zone u get damaged
//but thered be a delay
//and like a telegraph
//ie darude glows red
//id also decrease the delay for his charges
//acutally no
//dont reduce the radius
//i have an idea
//the safe zones have a value that stores the radius
//the longer the player is in there
//the more the circle shrinks
//that way its an expendable resource
//dharuud will reset the circles occasionally
//basically the boss changes will be
//- shrinking safe zones while inside them
//- telegraphing and then making safe zones hostile (being inside damages you and the dust turns red)
//-telegraphing and then making the sandstorm damage the player (darude has a red aura and not being in a safe zone hits you every few seconds)
//-increase safe zone dust scale to be more obvious
//- reduce the delay for his charges as his health goes down
//and i think after like 2 runs through his moveset
//he'll reset the safe zones
//we might also keep two active at the same time idk
//also the center of the safe zone glows red which i guess is code from the blood moon artifact
//also not testing chat sadge
//overall this means u have to manage your safe zones
//using it too much or using it all up means when he does the sandstorm attack you wont have a safe zone to hide in
//so youll have to occasionally choose to go outside and contend with dodging while inside the sandstorm
//any feedback?


//shrinking safe zones while inside them
//-telegraphing and then making safe zones hostile (being inside damages you and the dust turns red)
//-telegraphing and then making the sandstorm damage the player (darude has a red aura and not being in a safe zone hits you every few seconds)
//-increase safe zone dust scale to be more obvious
//- reduce the delay for his charges as his health goes down
//after two run thrus he'll reset
//in non expert the safezones will regen when you aren't in them
//make glow sandy


//hmm
//id say after 2 run throughs
//actually
//the hostile safe zones should happen once during the 2 run throughs
//and the sandstorm damaging will be at the end of the 2 run throughs
//his telegraph will also be a thing in chat
//ie. "The desert winds become more violent" or something prior to making the sandstorm damage u outside of safe zones


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    [AutoloadBossHead]
    public class SandstormBoss : ModNPC
    {
        public bool safetyCircleSwitch = false;
        private bool doDustAttack = false;
        private Vector2 rotationCenter;
        private int enrageTimer;
        private bool enraged;
        bool movingUp = true;
        bool leftOfPlayer = true;
        bool halfLife = false;
        bool fourthLife = false;
        bool circleActive = false;
        Vector2 playerCenterSnapShot;
        private int storedDamage;
        private bool clockWise;
        private bool moved = false;
        public bool secondRunThru;// = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dharuud, the Sandstorm");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 168;
            npc.height = 130;
            npc.aiStyle = -1;
            npc.damage = 21;
            npc.defense = 12;
            npc.lifeMax = 4100;
            npc.HitSound = SoundID.NPCHit23;
            npc.DeathSound = SoundID.NPCDeath39;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.boss = true;
            npc.npcSlots = 10f;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/SandstormBoss");
            bossBag = ModContent.ItemType<SandstormBag>();
        }

        public static void SandstormStuff()
        {
            Sandstorm.IntendedSeverity = 20; //0.4f;
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0.0f, 0.0f, 0.0f, 0, 0, 0);
        }

        public override void AI()
        {
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.spriteDirection = npc.direction;

            if (npc.life <= npc.lifeMax * 0.5f) { halfLife = true; }
            if (npc.life <= npc.lifeMax * 0.25f) { fourthLife = true; }
            if (npc.ai[0] == 1 && fourthLife) { circleActive = true; }
            else { circleActive = false; }

            // Handles Despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (Sandstorm.Happening)
                {
                    Sandstorm.Happening = false;
                    Sandstorm.TimeLeft = 18000;
                    SandstormStuff();
                }

                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }

            }

            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                npc.velocity.Y = 2000;
            }

            if (!Sandstorm.Happening)
            {
                Sandstorm.Happening = true;
                Sandstorm.TimeLeft = (int)(3600.0 * (8.0 + (double)Main.rand.NextFloat() * 16.0));
                SandstormStuff();
            }

            if (!player.ZoneDesert)
            {
                if (enrageTimer == 120)
                {
                    enraged = true;
                }
                else
                {
                    enraged = false;
                    enrageTimer++;
                }
            }
            else
            {
                enraged = false;
                enrageTimer = 0;
            }

            if (npc.ai[0] == 3)
            {
                npc.ai[1] += 10f;
            }
            else
            {
                npc.ai[1]++;
            }

            if (npc.ai[0] == 2 || npc.ai[0] == 3 || circleActive == true)
            {
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }

            switch (npc.ai[0])
            {
                case 0: // General case: Move towards player
                    if (!player.ZoneDesert && enraged)
                    {
                        npc.ai[0] = 1.5f;
                        npc.ai[1] = 0;
                        npc.ai[2] = 900;
                        break;
                    }

                    if (!doDustAttack && (npc.ai[1] == 90 /*|| npc.ai[1] == 91*/))
                    {
                        //if (npc.ai[1] == 90)
                        //{
                        secondRunThru = !secondRunThru;
                        //}
                        //safetyCircleSwitch = !safetyCircleSwitch;
                    }

                    if (!doDustAttack && npc.ai[1] <= 160 && secondRunThru)
                    {
                        npc.velocity = Vector2.Zero;
                        if (npc.ai[1] == 90)
                        {
                            Vector2 origin = npc.Center;
                            float radius = 90;
                            int numLocations = 90;
                            for (int i = 0; i < 90; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                Vector2 dustvelocity = new Vector2(0f, -15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                int dust = Dust.NewDust(position, 2, 2, 32, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                                Main.dust[dust].noGravity = false;
                            }
                        }
                    }

                    if (doDustAttack || (!doDustAttack && npc.ai[1] > 160))
                    {
                        Vector2 moveTo = player.Center;
                        var move = moveTo - npc.Center;
                        var speed = 5;

                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 45;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        npc.velocity.X = move.X;
                        npc.velocity.Y = move.Y * .98f;
                    }

                    if (npc.ai[1] == 360)
                    {
                        if (doDustAttack && !secondRunThru)
                        {
                            npc.ai[0] = 1.5f;
                            npc.ai[1] = 0;
                            npc.ai[2] = 900;
                            doDustAttack = false;
                        }
                        else if (doDustAttack && secondRunThru)
                        {
                            npc.ai[0] = 0.5f;
                            npc.ai[1] = 0;
                        }
                        else
                        {
                            storedDamage = npc.damage;
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 0.5f: // attack if not inside 
                    {
                        if (npc.ai[1] == 1)
                        {
                            Main.NewText("The desert winds become more violent");
                            npc.velocity = Vector2.Zero;
                        }
                        if (npc.ai[1] > 120 && npc.ai[1] % 60 == 0)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                                if (distance <= 2500)
                                {
                                    if (Main.player[i].HasBuff(194) == true)
                                    {
                                        Main.player[i].Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), 50, 0, false, false, false, 60);
                                    }
                                }
                            }
                        }
                        if (npc.ai[1] == 360)
                        {
                            Main.NewText("The desert winds calm");
                            storedDamage = npc.damage;
                            npc.ai[0] = 1f;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 1: // Spawn sand from off screen
                    if (!player.ZoneDesert && enraged)
                    {
                        npc.ai[0] = 1.5f;
                        npc.ai[1] = 0;
                        npc.ai[2] = 900;
                        break;
                    }

                    npc.damage = 0;

                    if (fourthLife == false)
                    {
                        npc.velocity = Vector2.Zero;

                        if (npc.ai[1] % 60 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = 0; i < (halfLife ? Main.rand.Next(1, 3) : Main.rand.Next(2, 4)); i++)
                                {
                                    npc.netUpdate = true;
                                    Projectile.NewProjectile(new Vector2(player.Center.X + Main.rand.Next(1200, 1500), npc.Center.Y + Main.rand.Next(-360, 360)), new Vector2(Main.rand.Next(-11, -6), 0), ModContent.ProjectileType<SandBall>(), 21 / (Main.expertMode ? 4 : 2), 0f, Main.myPlayer);
                                }

                                if (halfLife)
                                {
                                    for (int i = 0; i < Main.rand.Next(1, 3); i++)
                                    {
                                        npc.netUpdate = true;
                                        Projectile.NewProjectile(new Vector2(player.Center.X - Main.rand.Next(1200, 1500), npc.Center.Y + Main.rand.Next(-360, 360)), new Vector2(Main.rand.Next(6, 11), 0), ModContent.ProjectileType<SandBall>(), 21 / (Main.expertMode ? 4 : 2), 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (npc.ai[1] == 1)
                        {
                            npc.hide = true;
                            npc.width = 68;
                            npc.height = 56;
                        }

                        if (npc.ai[2]++ > 0 && npc.ai[1] > 2)
                        {
                            npc.position = player.Center + new Vector2(-475, 0).RotatedBy(MathHelper.ToRadians(4 * npc.ai[3]));
                            npc.position.X -= npc.width / 2;
                            npc.position.Y -= npc.height / 2;
                            npc.ai[3] += clockWise ? 1 : -1;
                            npc.ai[2] = 0;
                        }

                        if (npc.ai[1] % 15 == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectileDirect(npc.Center, npc.DirectionTo(player.Center) * 7.5f, ModContent.ProjectileType<SandBall>(), 21 / (Main.expertMode ? 4 : 2), 0, Main.myPlayer, 0, npc.ai[3]);
                                int proj = Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 7.5f, ModContent.ProjectileType<SandyIndicator>(), 0, 0, Main.myPlayer);
                                ((SandyIndicator)Main.projectile[proj].modProjectile).waittime = 49;
                                ((SandyIndicator)Main.projectile[proj].modProjectile).length = 495f;
                            }
                        }
                    }

                    if (npc.ai[1] == 600)
                    {
                        if (!doDustAttack)
                        {
                            doDustAttack = true;
                        }

                        npc.damage = storedDamage;
                        npc.hide = false;
                        npc.width = 168;
                        npc.height = 130;
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                        npc.ai[3] = 0;
                    }
                    break;
                case 1.5f: // Do dust swirl animation
                    npc.velocity = Vector2.Zero;

                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 dustPos = npc.Center + new Vector2(npc.ai[2], 0).RotatedBy(MathHelper.ToRadians(i * 20 + npc.ai[1]));
                        Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 32, 0f, 0f, 0, default, 2.04f)];
                        dust.noGravity = true;
                    }

                    npc.ai[2] -= 15;

                    if (npc.ai[2] == 0)
                    {
                        npc.ai[0] = 2;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
                case 2: // Turn into dust cloud
                    npc.hide = true;

                    if (npc.ai[2] == (Main.expertMode ? 50 : 66) && npc.ai[1] < 600 || (npc.ai[1] < 80 && npc.ai[2] == 0))
                    {
                        npc.velocity = Vector2.Zero;
                        playerCenterSnapShot = player.Center;
                        int proj = Projectile.NewProjectile(npc.Center, npc.DirectionTo(playerCenterSnapShot), ModContent.ProjectileType<SandyIndicator>(), 0, 0, Main.myPlayer);
                        ((SandyIndicator)Main.projectile[proj].modProjectile).waittime = (Main.expertMode ? 49 : 65);
                        ((SandyIndicator)Main.projectile[proj].modProjectile).length = 850f;
                    }
                    else if (npc.ai[2] == 50)
                    {
                        npc.velocity = Vector2.Zero;
                    }

                    npc.width = 68;
                    npc.height = 56;
                    if (npc.ai[1] < 600)
                    {
                        if (npc.ai[2] <= 0 && npc.ai[1] > 80)
                        {
                            float chargeSpeed = !player.ZoneDesert ? 25 : 18;
                            npc.velocity = npc.DirectionTo(playerCenterSnapShot) * chargeSpeed;
                            npc.netUpdate = true;
                            if (!player.ZoneDesert && npc.ai[1] < 598)
                            {
                                npc.ai[2] = 40;
                            }
                            else if (npc.ai[1] < 598)
                            {
                                if (Main.expertMode)
                                {
                                    npc.ai[2] = 80; // charging delay
                                }
                                else
                                {
                                    npc.ai[2] = 100;
                                }
                            }
                        }
                        if (npc.ai[2] >= 0 && npc.ai[2] <= (Main.expertMode ? 50 : 66))
                        {
                            for (int i = 0; i < 18; i++)
                            {
                                Vector2 dustPos = npc.Center + new Vector2(npc.ai[2], 0).RotatedBy(MathHelper.ToRadians(i * 20 + (npc.ai[1] * /*7.5*/ /*10f*/ 12f)));
                                Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 32, 0f, 0f, 0, default, 2.04f)];
                                dust.noGravity = true;
                            }
                        }
                        npc.ai[2] -= 1f;
                        if (npc.ai[1] > 598)
                        {
                            npc.ai[2] = -100;
                        }
                    }
                    else if (halfLife == true && npc.ai[1] > 600 && npc.ai[1] < 900)
                    {
                        if (npc.ai[2] < 0 && npc.ai[2] > -150)
                        {
                            leftOfPlayer = Main.rand.NextBool();
                            npc.ai[2] = 0;
                        }
                        if (leftOfPlayer == true)
                        {
                            if (npc.ai[2] >= 0 && npc.ai[2] < 30)
                            {
                                npc.Teleport(player.Center + new Vector2(-700, (npc.height / 2) - 90), 32);
                                npc.velocity = Vector2.Zero;
                                movingUp = Main.rand.NextBool();
                            }
                            if (npc.ai[2] % 30 == 0 && npc.ai[2] < 150 && npc.ai[2] > 0)
                            {
                                if (movingUp == true && moved == false)
                                {
                                    npc.velocity = new Vector2(Main.rand.Next(10, 13), Main.rand.Next(-13, -10));
                                    int proj = Projectile.NewProjectile(npc.Center, npc.velocity, ModContent.ProjectileType<SandyIndicator>(), 0, 0, Main.myPlayer);
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).waittime = 49;
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).length = 495f;
                                    movingUp = false;
                                    moved = true;
                                }
                                else if (moved == false)
                                {
                                    npc.velocity = new Vector2(Main.rand.Next(10, 13), Main.rand.Next(10, 13));
                                    int proj = Projectile.NewProjectile(npc.Center, npc.velocity, ModContent.ProjectileType<SandyIndicator>(), 0, 0, Main.myPlayer);
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).waittime = 49;
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).length = 495f;
                                    movingUp = true;
                                    moved = true;
                                }
                            }
                            if (npc.ai[2] == 150)
                            {
                                npc.ai[2] = 0;
                                leftOfPlayer = false;
                                moved = false;
                            }
                        }
                        else
                        {
                            if (npc.ai[2] >= 0 && npc.ai[2] < 30 && npc.ai[2] > 0)
                            {
                                npc.Teleport(player.Center + new Vector2(700, (npc.height / 2) - 90), 32);
                                npc.velocity = Vector2.Zero;
                                movingUp = Main.rand.NextBool();
                            }
                            if (npc.ai[2] % 30 == 0 && npc.ai[2] < 150)
                            {
                                if (movingUp == true && moved == false)
                                {
                                    npc.velocity = new Vector2(Main.rand.Next(-13, -10), Main.rand.Next(-13, -10));
                                    int proj = Projectile.NewProjectile(npc.Center, npc.velocity, ModContent.ProjectileType<SandyIndicator>(), 0, 0, Main.myPlayer);
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).waittime = 49;
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).length = 495f;
                                    movingUp = false;
                                    moved = true;
                                }
                                else if (moved == false)
                                {
                                    npc.velocity = new Vector2(Main.rand.Next(-13, -10), Main.rand.Next(10, 13));
                                    int proj = Projectile.NewProjectile(npc.Center, npc.velocity, ModContent.ProjectileType<SandyIndicator>(), 0, 0, Main.myPlayer);
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).waittime = 49;
                                    ((SandyIndicator)Main.projectile[proj].modProjectile).length = 495f;
                                    movingUp = true;
                                    moved = true;
                                }
                            }
                            if (npc.ai[2] == 150)
                            {
                                npc.ai[2] = 0;
                                leftOfPlayer = true;
                                moved = false;
                            }
                        }
                        npc.ai[2] += 1f;
                    }

                    if (npc.ai[1] == (halfLife ? 900 : 600))
                    {
                        moved = false;
                        if (player.ZoneDesert)
                        {
                            npc.width = 136;
                            npc.height = 112;

                            npc.velocity = Vector2.Zero;
                            npc.hide = false;

                            npc.ai[2] = 0;

                            if (Main.expertMode)
                            {
                                npc.ai[0] = 3;
                                npc.ai[1] = 0;
                            }
                            else
                            {
                                npc.ai[0] = 0;
                                npc.ai[1] = 0;
                            }
                        }
                        else
                        {
                            npc.ai[0] = 2;
                            npc.ai[1] = 0;
                        }
                    }
                    break;
                case 3: // Spin in a circle

                    // Get center to spin around
                    if (npc.ai[1] == 10)
                    {
                        rotationCenter = new Vector2(npc.Center.X + 50, npc.Center.Y);
                    }

                    double deg = (double)npc.ai[1];
                    double rad = deg * (Math.PI / 180); //Convert degrees to radians
                    double dist = 50; //Distance away from the player

                    npc.position.X = rotationCenter.X - (int)(Math.Cos(rad) * dist) - npc.width / 2;
                    npc.position.Y = rotationCenter.Y - (int)(Math.Sin(rad) * dist) - npc.height / 2;

                    if (npc.ai[1] % 50 == 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Projectile.NewProjectile(npc.Center, Vector2.One.RotatedByRandom(Math.PI) * 6, ModContent.ProjectileType<SandBall2>(), 21 / 4, 0f, Main.myPlayer);
                            }
                        }
                    }
                    // Move the NPC around the center
                    if (npc.ai[1] == 2400)
                    {
                        if (clockWise)
                        {
                            clockWise = false;
                        }
                        else
                        {
                            clockWise = true;
                        }

                        npc.ai[0] = 0;
                        npc.ai[1] = 0;
                    }
                    break;
            }

            if (npc.ai[0] != 2 && npc.ai[0] != 3 && circleActive != true)
            {
                if (Main.rand.NextFloat() < 0.5526316f)
                {
                    for (int num1202 = 0; num1202 < 4; num1202++)
                    {
                        Dust.NewDust(npc.Center - new Vector2(npc.width / 4, -35), npc.width / 3, npc.height, 32, 0, 2.63f, default, default, 1.45f);
                    }
                }
            }
            else
            {
                for (int num1101 = 0; num1101 < 6; num1101++)
                {
                    int num1110 = Dust.NewDust(new Vector2(npc.Center.X, npc.Center.Y), npc.width, npc.height, 32, npc.velocity.X, npc.velocity.Y, 50, default(Color), 3f);
                    Main.dust[num1110].position = (Main.dust[num1110].position + npc.Center) / 2f;
                    Main.dust[num1110].noGravity = true;
                    Dust dust81 = Main.dust[num1110];
                    dust81.velocity *= 0.5f;
                }
            }
            moved = false;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 12f == 11f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }

            if (npc.frame.Y >= frameHeight * 4) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[0] != 2 && npc.ai[0] != 3 && circleActive != true) //|| (!doDustAttack && npc.ai[0] == 0 && npc.ai[1] <= 120))
            {
                Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                Texture2D texture2D16 = mod.GetTexture("NPCs/Bosses/SandstormBoss/SandstormBoss_Afterimage");


                // this gets the npc's frame
                float amount10 = 0f; // I think this controls amount of color
                int num178 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


                // default value
                int num177 = 6; // ok i think this controls the number of afterimage frames
                float num176 = 1f - (float)Math.Cos((npc.ai[1] - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
                num176 /= 3f;
                float scaleFactor10 = 7f; // Change scale factor of the pulsing effect and how far it draws outwards

                amount10 = 1f;

                // ok this is the pulsing effect drawing
                for (int num164 = 1; num164 < num177; num164++)
                {
                    // these assign the color of the pulsing
                    Color spriteColor = (npc.ai[0] == 0.5) ? Color.OrangeRed : Color.Yellow;
                    spriteColor = npc.GetAlpha(spriteColor);
                    spriteColor *= 1f - num176; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector45 = ((Entity)((ModNPC)this).npc).Center + Terraria.Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + ((ModNPC)this).npc.rotation) * scaleFactor10 * num176 - Main.screenPosition;
                    vector45 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[((ModNPC)this).npc.type]) * ((ModNPC)this).npc.scale / 2f;
                    vector45 += drawOrigin * npc.scale + new Vector2(0f, 4f + ((ModNPC)this).npc.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture2D16, vector45, npc.frame, spriteColor, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }

            if (npc.ai[0] == 2 || npc.ai[0] == 3 || circleActive == true)
            {
                return false;
            }

            return base.PreDraw(spriteBatch, drawColor);
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.downedDarude = true;

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else
            {
                int choice = Main.rand.Next(4);
                // Always drops one of:
                if (choice == 0) // Warrior
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandstormSpinner>());
                }
                else if (choice == 1) // Mage
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandStaff>());
                }
                else if (choice == 2) // Ranger
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SandThrower>());
                }
                else if (choice == 3) // Summoner
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DustStaff>());
                }
            }

            if (Sandstorm.Happening)
            {
                Sandstorm.Happening = false;
                Sandstorm.TimeLeft = 18000;
                SandstormStuff();
            }

            for (int num785 = 0; num785 < 4; num785++)
            {
                Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 100, default(Color), 1.5f);
            }

            for (int num788 = 0; num788 < 40; num788++)
            {
                int num797 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num797].noGravity = true;
                Dust dust24 = Main.dust[num797];
                dust24.velocity *= 3f;
                num797 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 32, 0f, 0f, 100, default(Color), 1.5f);
                dust24 = Main.dust[num797];
                dust24.velocity *= 2f;
                Main.dust[num797].noGravity = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}