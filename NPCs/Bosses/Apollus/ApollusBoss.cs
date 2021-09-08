using System;
using Microsoft.Xna.Framework;
using OvermorrowMod.Items.Armor.Marble;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Projectiles.NPCs.Hostile;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.Apollus
{
    [AutoloadBossHead]
    public class ApollusBoss : ModNPC
    {
        public int maxRuneCircle = 3;
        public int timer; // = 0;
        int randomCase; // = 0;
        int LastCase; // = 0;
        int randomCeiling;
        Vector2 teleportPosition; //= Vector2.Zero;
        int spriteTimer = 0;
        int frame = 1;
        int proj;
        int projalt;
        //bool direction;
        bool direction = true;
        bool changedPhase2 = false;
        bool changedPhase2Indicator = false;
        bool spawnedShields = false;
        Vector2 playerCenterSnapShot;
        Vector2 spawnpos;
        bool dead = false;


        private int groupAttack;


        public int graknightIdentity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marbella");
            Main.npcFrameCount[npc.type] = 5;
        }
        public override void SetDefaults()
        {
            npc.width = 122;
            npc.height = 126;
            npc.defense = 12;
            npc.lifeMax = 1250;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(gold: 5);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.damage = 45;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/STONEBROS");
            bossBag = ModContent.ItemType<LadyBag>();
        }

        public override void AI()
        {
            OvermorrowWorld.downedLady = false;

            
            Player player = Main.player[npc.target];

            // AI [0] = Case Value
            // AI [1] = 
            // AI [2] = 
            // AI [3] = Timer

            if (npc.ai[3] > 0f && dead == true) // death animation
            {
                npc.velocity = Vector2.Zero;

                if (npc.ai[2] > 0)
                {
                    npc.ai[2]--;
                }
                else
                {
                    npc.dontTakeDamage = true;
                    npc.ai[3]++; // Death timer
                    npc.velocity.X *= 0.95f;

                    if (npc.velocity.Y < 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                    }

                    if (npc.velocity.X > 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                    }

                    if (npc.ai[3] > 120f)
                    {
                        npc.Opacity = 1f - (npc.ai[3] - 120f) / 60f;
                    }

                    if (Main.rand.NextBool(5) && npc.ai[3] < 120f)
                    {
                        // This dust spawn adapted from the Pillar death code in vanilla.
                        for (int dustNumber = 0; dustNumber < 6; dustNumber++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(npc.Left, npc.width, npc.height / 2, 57, 0f, 0f, 0, default(Color), 1f)];
                            dust.position = npc.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(npc.width * 1.5f, npc.height * 1.1f) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                            dust.velocity.X = 0f;
                            dust.velocity.Y = -Math.Abs(dust.velocity.Y - (float)dustNumber + npc.velocity.Y - 4f) * 3f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.scale = 1f + Main.rand.NextFloat() + (float)dustNumber * 0.3f;
                        }
                    }

                    if (npc.ai[3] == 90)
                    {
                        Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), npc.Center); // every half second while dying, play a sound
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                            if (distance <= 600)
                            {
                                Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                            }
                        }
                    }

                    if (npc.ai[3] >= 180f)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 0);
                        npc.checkDead(); // This will trigger ModNPC.CheckDead the second time, causing the real death.
                    }
                }
                return;
            }

            // spawn shields
            if (!spawnedShields && OvermorrowWorld.downedKnight)
            {
                float radius = 100;
                int numLocations = 3;
                for (int i = 0; i < numLocations; i++)
                {
                    Vector2 position = npc.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Projectile.NewProjectile(position, Vector2.Zero, ProjectileType<HostileReflectile>(), 0, 0f, Main.myPlayer, npc.whoAmI, 120 * i);
                }
                spawnedShields = true;
            }

            // reset boolean
            if (npc.life <= npc.lifeMax * 0.5f || OvermorrowWorld.downedKnight)
            {
                changedPhase2 = true;
            }

            switch (npc.ai[0])
            {
                case -10: // group attacks
                    {
                        if (!PlayerAlive(player)) { break; }

                        switch (groupAttack)
                        {

                            case 0:
                                {

                                }
                                break;
                            case 1:
                                {

                                }
                                break;
                            case 2:
                                {

                                }
                                break;
                            case 3:
                                {
                                    
                                }
                                break;
                        }
                    }
                    break;
                case -2: // yell and change phase
                    {
                        if (!PlayerAlive(player)) { break; }
                        npc.dontTakeDamage = true;
                        npc.immortal = true;
                        if (npc.ai[1] == 45)
                        {
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                        }
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                            if (distance <= 1200)
                            {
                                Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                            }
                        }

                        if (++npc.ai[1] > 240)
                        {
                            npc.immortal = false;
                            npc.dontTakeDamage = false;
                            changedPhase2Indicator = true;

                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = 4;
                            npc.ai[3] = 0;
                        }
                    }
                    break;
                case -1: // spawn sequence
                    {
                        if (!PlayerAlive(player)) { break; }

                        // reset boolean
                        OvermorrowWorld.downedLady = false;

                        npc.velocity = Vector2.UnitY * 0.7f;
                        npc.dontTakeDamage = true;

                        if (++npc.ai[1] > 380)
                        {
                            npc.ai[0] = 0;
                            npc.ai[1] = 0;
                            player.GetModPlayer<OvermorrowModPlayer>().ShowText = false;
                            npc.dontTakeDamage = false;
                            npc.velocity = Vector2.Zero;
                            Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)npc.Center.X, (int)npc.Center.Y);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(npc.Center, Main.player[i].Center);
                                if (distance <= 600)
                                {
                                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                                }
                            }
                        }
                    }
                    break;
                case 0: // spawn rune circle
                    {
                        if (changedPhase2 && !changedPhase2Indicator)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = -2;
                            npc.ai[3] = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        if (++npc.ai[1] == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                proj = Projectile.NewProjectile(player.Center.X, player.Center.Y - 100f, 0f, 0f, ProjectileType<ArrowRuneCircle>(), npc.damage / 2, 0f, Main.myPlayer, -10, npc.whoAmI);
                            }
                        }

                        if (npc.ai[1] == 360)
                        {
                            if (changedPhase2 == true && changedPhase2Indicator == false)
                            {
                                npc.ai[1] = 0;
                                npc.ai[2] = 1;
                                npc.ai[0] = -2;
                                npc.ai[3] = 0;
                            }
                            else
                            {
                                npc.ai[1] = 0;
                                npc.ai[2] = 1;
                                npc.ai[0] = 4;
                                npc.ai[3] = 0;
                            }
                        }
                    }
                    break;
                case 1: // fire arrows in all directions
                    {
                        if (changedPhase2 && !changedPhase2Indicator)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = -2;
                            npc.ai[3] = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        if (++npc.ai[1] % 30 == 0)
                        {
                            int projectiles = changedPhase2 ? 5 : 3;
                            int random = Main.rand.Next(5);
                            for (int j = 0; j < projectiles; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(npc.Center, new Vector2(0f, 8.5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + (npc.ai[2] * 30) + random), ProjectileType<ApollusArrow>(), npc.damage / 2, 10f, Main.myPlayer);
                                }
                            }
                            npc.ai[2] += 1;
                        }

                        if (npc.ai[1] == 420)
                        {
                            if (changedPhase2 == true && changedPhase2Indicator == false)
                            {
                                npc.ai[1] = 0;
                                npc.ai[2] = 1;
                                npc.ai[0] = -2;
                                npc.ai[3] = 0;
                            }
                            else
                            {
                                npc.ai[1] = 0;
                                npc.ai[2] = 1;
                                npc.ai[0] = 4;
                                npc.ai[3] = 0;
                            }
                        }
                    }
                    break;
                case 2: // arrow circle
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (npc.ai[1]++ == 0)
                        {
                            playerCenterSnapShot = player.Center;
                        }

                        if (npc.ai[2]++ > 0 && npc.ai[3] != 390 / 30 && npc.ai[1] > 1)
                        {
                            npc.position = playerCenterSnapShot + new Vector2(-550, 0).RotatedBy(MathHelper.ToRadians(30 * npc.ai[3]));
                            npc.position.X -= npc.width / 2;
                            npc.position.Y -= npc.height / 2;
                            if (npc.ai[3] <= 360 / 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectileDirect(npc.Center, npc.DirectionTo(playerCenterSnapShot) * 5f, ProjectileType<ApollusArrowTwo>(), npc.damage / 2, /*12*/ 6, Main.myPlayer, 0, npc.ai[3]);
                                }
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
                                    int dust = Dust.NewDust(position, 2, 2, 57, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                            npc.ai[3]++;
                            npc.ai[2] = 0;
                        }

                        if (npc.ai[3] == 390 / 30 && npc.ai[1] == 240)
                        {
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[0] = 4;
                            npc.ai[3] = 0;
                        }
                    }
                    break;
                case 3: // hold rune circle
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (++npc.ai[1] == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                projalt = Projectile.NewProjectile(npc.Center + npc.Center + new Vector2(-35 + (566 * npc.spriteDirection), -35), Vector2.Zero, ProjectileType<ArrowRuneCircle>(), npc.damage / 2, direction ? -1 : 1, Main.myPlayer, -20, npc.whoAmI);
                            }
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
                            npc.ai[0] = 4;
                            npc.ai[3] = 0;
                        }
                    }
                    break;
                case 4: // teleport
                    {
                        if (!PlayerAlive(player)) { break; }

                        npc.ai[1]++;

                        if (changedPhase2 == true) { randomCeiling = 4; }
                        else { randomCeiling = 2; }
                        while (randomCase == LastCase)
                        {
                            randomCase = Main.rand.Next(randomCeiling);
                        }
                        if (randomCase == 3 && npc.ai[1] == 15)
                        {
                            direction = Main.rand.NextBool();
                            teleportPosition = player.Center + new Vector2(-600 * (direction ? 1 : -1), 0);
                        }
                        else if (randomCase != 3 && npc.ai[1] == 15)
                        {
                            teleportPosition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            while (Main.tile[(int)teleportPosition.X / 16, (int)teleportPosition.Y / 16].active())
                            {
                                teleportPosition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            }
                        }

                        if (npc.ai[1] > 30)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = teleportPosition;
                                float radius = 20;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 57, dustvelocity.X, dustvelocity.Y, 0, default, /*1*/ 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (npc.ai[1] > 90)
                        {
                            npc.Teleport(teleportPosition + new Vector2(-61, -61), 57);
                        }
                        if (npc.ai[1] > 100)
                        {
                            LastCase = randomCase;
                            npc.ai[1] = 0;
                            npc.ai[2] = 1;
                            npc.ai[3] = 0;
                            npc.ai[0] = randomCase;
                        }
                    }
                    break;
            }

            npc.spriteDirection = npc.direction;
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

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f)
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 10) // 10 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }

            // npc.frame.Y = frameHeight * frame;

            if (Main.player[npc.target].Center.X < npc.Center.X)
            {
                npc.spriteDirection = -1;
            }
        }

        public override bool CheckDead()
        {
            if (npc.ai[3] == 0f)
            {
                npc.ai[2] = 0;
                dead = true;
                npc.ai[3] = 1f;
                npc.damage = 0;
                npc.life = npc.lifeMax;
                npc.dontTakeDamage = true;
                npc.netUpdate = true;
                return false;
            }
            return true;
        }

        public override void NPCLoot()
        {
            OvermorrowWorld.downedLady = true;

            if (Main.expertMode)
            {
                npc.DropBossBags();
                return;
            }

            int choice = Main.rand.Next(3);
            if (choice == 0) // Armor
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MarbleHelm>());
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MarblePlate>());
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MarbleLegs>());
            }
            else if (choice == 1) // Ranger
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MarbleBow>());
            }
            else if (choice == 2) // Mage
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MarbleBook>());
            }

            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<HeartStone>(), 2);

            if (Main.rand.Next(10) == 0) // Trophy Dropchance
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MarbleTrophy>());
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
    }
}
