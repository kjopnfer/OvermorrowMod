using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Armor.Marble;
using OvermorrowMod.Content.Items.Consumable.BossBags;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.MarbleBook;
using OvermorrowMod.Content.Items.Weapons.Ranged.MarbleBow;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.NPCs.Bosses.Apollus
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
        int projalt;
        // TODO: This is unused
        int proj;
        //bool direction;
        bool direction = true;
        bool changedPhase2 = false;
        bool changedPhase2Indicator = false;
        bool spawnedShields = false;
        Vector2 playerCenterSnapShot;
        bool dead = false;

        // TODO: This is also unused
        // private int groupAttack;


        public int graknightIdentity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marbella");
            Main.npcFrameCount[NPC.type] = 5;
        }
        public override void SetDefaults()
        {
            NPC.width = 122;
            NPC.height = 126;
            NPC.defense = 12;
            NPC.lifeMax = 1250;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.damage = 45;
            Music = SoundLoader.GetSoundSlot("Sounds/Music/STONEBROS");
        }

        public override void AI()
        {
            OvermorrowWorld.downedLady = false;


            Player player = Main.player[NPC.target];

            // AI [0] = Case Value
            // AI [1] = 
            // AI [2] = 
            // AI [3] = Timer

            if (NPC.ai[3] > 0f && dead == true) // death animation
            {
                NPC.velocity = Vector2.Zero;

                if (NPC.ai[2] > 0)
                {
                    NPC.ai[2]--;
                }
                else
                {
                    NPC.dontTakeDamage = true;
                    NPC.ai[3]++; // Death timer
                    NPC.velocity.X *= 0.95f;

                    if (NPC.velocity.Y < 0.5f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + 0.01f;
                    }

                    if (NPC.velocity.X > 0.5f)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - 0.01f;
                    }

                    if (NPC.ai[3] > 120f)
                    {
                        NPC.Opacity = 1f - (NPC.ai[3] - 120f) / 60f;
                    }

                    if (Main.rand.NextBool(5) && NPC.ai[3] < 120f)
                    {
                        // This dust spawn adapted from the Pillar death code in vanilla.
                        for (int dustNumber = 0; dustNumber < 6; dustNumber++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(NPC.Left, NPC.width, NPC.height / 2, DustID.Enchanted_Gold, 0f, 0f, 0, default(Color), 1f)];
                            dust.position = NPC.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(NPC.width * 1.5f, NPC.height * 1.1f) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                            dust.velocity.X = 0f;
                            dust.velocity.Y = -Math.Abs(dust.velocity.Y - (float)dustNumber + NPC.velocity.Y - 4f) * 3f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.scale = 1f + Main.rand.NextFloat() + (float)dustNumber * 0.3f;
                        }
                    }

                    if (NPC.ai[3] == 90)
                    {
                        SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), NPC.Center); // every half second while dying, play a sound
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                            if (distance <= 600)
                            {
                                //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 60;
                            }
                        }
                    }

                    if (NPC.ai[3] >= 180f)
                    {
                        NPC.life = 0;
                        NPC.HitEffect(0, 0);
                        NPC.checkDead(); // This will trigger ModNPC.CheckDead the second time, causing the real death.
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
                    Vector2 position = NPC.Center + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), position, Vector2.Zero, ProjectileType<HostileReflectile>(), 0, 0f, Main.myPlayer, NPC.whoAmI, 120 * i);
                }
                spawnedShields = true;
            }

            // reset boolean
            if (NPC.life <= NPC.lifeMax * 0.5f || OvermorrowWorld.downedKnight)
            {
                changedPhase2 = true;
            }

            switch (NPC.ai[0])
            {
                case -10: // group attacks
                    {
                        if (!PlayerAlive(player)) { break; }
                        // Unfinished
                        /* switch (groupAttack)
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
                        } */
                    }
                    break;
                case -2: // yell and change phase
                    {
                        if (!PlayerAlive(player)) { break; }
                        NPC.dontTakeDamage = true;
                        NPC.immortal = true;
                        if (NPC.ai[1] == 45)
                        {
                            SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
                        }
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                            if (distance <= 1200)
                            {
                                //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                            }
                        }

                        if (++NPC.ai[1] > 240)
                        {
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;
                            changedPhase2Indicator = true;

                            NPC.ai[1] = 0;
                            NPC.ai[2] = 1;
                            NPC.ai[0] = 4;
                            NPC.ai[3] = 0;
                        }
                    }
                    break;
                case -1: // spawn sequence
                    {
                        if (!PlayerAlive(player)) { break; }

                        // reset boolean
                        OvermorrowWorld.downedLady = false;

                        NPC.velocity = Vector2.UnitY * 0.7f;
                        NPC.dontTakeDamage = true;

                        if (++NPC.ai[1] > 380)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            player.GetModPlayer<OvermorrowModPlayer>().ShowText = false;
                            NPC.dontTakeDamage = false;
                            NPC.velocity = Vector2.Zero;
                            SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 600)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                                }
                            }
                        }
                    }
                    break;
                case 0: // spawn rune circle
                    {
                        if (changedPhase2 && !changedPhase2Indicator)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 1;
                            NPC.ai[0] = -2;
                            NPC.ai[3] = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        if (++NPC.ai[1] == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), player.Center.X, player.Center.Y - 100f, 0f, 0f, ProjectileType<ArrowRuneCircle>(), NPC.damage / 2, 0f, Main.myPlayer, -10, NPC.whoAmI);
                            }
                        }

                        if (NPC.ai[1] == 360)
                        {
                            if (changedPhase2 == true && changedPhase2Indicator == false)
                            {
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 1;
                                NPC.ai[0] = -2;
                                NPC.ai[3] = 0;
                            }
                            else
                            {
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 1;
                                NPC.ai[0] = 4;
                                NPC.ai[3] = 0;
                            }
                        }
                    }
                    break;
                case 1: // fire arrows in all directions
                    {
                        if (changedPhase2 && !changedPhase2Indicator)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 1;
                            NPC.ai[0] = -2;
                            NPC.ai[3] = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        if (++NPC.ai[1] % 30 == 0)
                        {
                            int projectiles = changedPhase2 ? 5 : 3;
                            int random = Main.rand.Next(5);
                            for (int j = 0; j < projectiles; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, new Vector2(0f, 8.5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + (NPC.ai[2] * 30) + random), ProjectileType<ApollusArrow>(), NPC.damage / 2, 10f, Main.myPlayer);
                                }
                            }
                            NPC.ai[2] += 1;
                        }

                        if (NPC.ai[1] == 420)
                        {
                            if (changedPhase2 == true && changedPhase2Indicator == false)
                            {
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 1;
                                NPC.ai[0] = -2;
                                NPC.ai[3] = 0;
                            }
                            else
                            {
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 1;
                                NPC.ai[0] = 4;
                                NPC.ai[3] = 0;
                            }
                        }
                    }
                    break;
                case 2: // arrow circle
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (NPC.ai[1]++ == 0)
                        {
                            playerCenterSnapShot = player.Center;
                        }

                        if (NPC.ai[2]++ > 0 && NPC.ai[3] != 390 / 30 && NPC.ai[1] > 1)
                        {
                            NPC.position = playerCenterSnapShot + new Vector2(-550, 0).RotatedBy(MathHelper.ToRadians(30 * NPC.ai[3]));
                            NPC.position.X -= NPC.width / 2;
                            NPC.position.Y -= NPC.height / 2;
                            if (NPC.ai[3] <= 360 / 30)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectileDirect(NPC.GetSpawnSource_ForProjectile(), NPC.Center, NPC.DirectionTo(playerCenterSnapShot) * 5f, ProjectileType<ApollusArrowTwo>(), NPC.damage / 2, /*12*/ 6, Main.myPlayer, 0, NPC.ai[3]);
                                }
                            }
                            Vector2 origin = NPC.Center;
                            float radius = 20;
                            int numLocations = 30;
                            for (int k = 0; k < 2; k++)
                            {
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                            NPC.ai[3]++;
                            NPC.ai[2] = 0;
                        }

                        if (NPC.ai[3] == 390 / 30 && NPC.ai[1] == 240)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 1;
                            NPC.ai[0] = 4;
                            NPC.ai[3] = 0;
                        }
                    }
                    break;
                case 3: // hold rune circle
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (++NPC.ai[1] == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                projalt = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + NPC.Center + new Vector2(-35 + (566 * NPC.spriteDirection), -35), Vector2.Zero, ProjectileType<ArrowRuneCircle>(), NPC.damage / 2, direction ? -1 : 1, Main.myPlayer, -20, NPC.whoAmI);
                            }
                        }

                        if (NPC.ai[1] > 1 && NPC.ai[1] < 360)
                        {
                            NPC.position = player.Center + new Vector2(-61 + (-600 * (direction ? 1 : -1)), -61);
                        }

                        if (NPC.ai[1] == 360)
                        {
                            Main.projectile[projalt].Kill();

                            NPC.ai[1] = 0;
                            NPC.ai[2] = 1;
                            NPC.ai[0] = 4;
                            NPC.ai[3] = 0;
                        }
                    }
                    break;
                case 4: // teleport
                    {
                        if (!PlayerAlive(player)) { break; }

                        NPC.ai[1]++;

                        if (changedPhase2 == true) { randomCeiling = 4; }
                        else { randomCeiling = 2; }
                        while (randomCase == LastCase)
                        {
                            randomCase = Main.rand.Next(randomCeiling);
                        }
                        if (randomCase == 3 && NPC.ai[1] == 15)
                        {
                            direction = Main.rand.NextBool();
                            teleportPosition = player.Center + new Vector2(-600 * (direction ? 1 : -1), 0);
                        }
                        else if (randomCase != 3 && NPC.ai[1] == 15)
                        {
                            teleportPosition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            while (Main.tile[(int)teleportPosition.X / 16, (int)teleportPosition.Y / 16].HasTile)
                            {
                                teleportPosition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            }
                        }

                        if (NPC.ai[1] > 30)
                        {
                            if (++NPC.ai[2] % 5 == 0)
                            {
                                Vector2 origin = teleportPosition;
                                float radius = 20;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, DustID.Enchanted_Gold, dustvelocity.X, dustvelocity.Y, 0, default, /*1*/ 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (NPC.ai[1] > 90)
                        {
                            NPC.Teleport(teleportPosition + new Vector2(-61, -61), 57);
                        }
                        if (NPC.ai[1] > 100)
                        {
                            LastCase = randomCase;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 1;
                            NPC.ai[3] = 0;
                            NPC.ai[0] = randomCase;
                        }
                    }
                    break;
            }

            NPC.spriteDirection = NPC.direction;
        }

        bool PlayerAlive(Player player)
        {
            if (!player.active || player.dead)
            {
                player = Main.player[NPC.target];
                NPC.TargetClosest();
                if (!player.active || player.dead)
                {
                    if (NPC.timeLeft > 25)
                    {
                        NPC.timeLeft = 25;
                        NPC.velocity = Vector2.UnitY * -7;
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
            NPC.frameCounter++;

            if (NPC.frameCounter % 6f == 5f)
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 10) // 10 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }

            // NPC.frame.Y = frameHeight * frame;

            if (Main.player[NPC.target].Center.X < NPC.Center.X)
            {
                NPC.spriteDirection = -1;
            }
        }

        public override bool CheckDead()
        {
            if (NPC.ai[3] == 0f)
            {
                NPC.ai[2] = 0;
                dead = true;
                NPC.ai[3] = 1f;
                NPC.damage = 0;
                NPC.life = NPC.lifeMax;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                return false;
            }
            return true;
        }

        public override void OnKill()
        {
            OvermorrowWorld.downedLady = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<LadyBag>()));

            var gearRule = new OneFromRulesRule(
                1,
                ItemDropRule.Common(ItemType<MarbleHelm>())
                    .OnSuccess(ItemDropRule.Common(ItemType<MarblePlate>()))
                    .OnSuccess(ItemDropRule.Common(ItemType<MarbleLegs>())),
                ItemDropRule.Common(ItemType<MarbleBow>()),
                ItemDropRule.Common(ItemType<MarbleBook>())
            );

            var nonExpert = new LeadingConditionRule(new Conditions.NotExpert())
                .OnSuccess(ItemDropRule.Common(ItemType<HeartStone>(), 1, 2, 2))
                .OnSuccess(ItemDropRule.Common(ItemType<MarbleTrophy>(), 10))
                .OnSuccess(gearRule);

            npcLoot.Add(nonExpert);
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
