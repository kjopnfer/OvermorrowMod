using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Armor.Granite;
using OvermorrowMod.Content.Items.Consumable.BossBags;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Content.Items.Weapons.Magic.GraniteBook;
using OvermorrowMod.Content.Items.Weapons.Melee.GraniteSpear;
using OvermorrowMod.Content.Items.Weapons.Summoner.GraniteStaff;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Content.NPCs.Bosses.GraniteMini
{
    [AutoloadBossHead]
    public class AngryStone : ModNPC
    {
        int spritetimer = 0;
        int frame = 1;
        int attackCounter = 0;
        Vector2 teleportPosition = Vector2.Zero;
        bool changedPhase2 = false;
        bool changedPhase2Indicator = false;

        int Direction = -1;
        bool direction = false;

        int RandomCase = 0;
        int LastCase = 1;
        int RandomCeiling;
        bool movement = true;

        bool npcDashing = false;
        int spriteDirectionStore = 0;

        bool dead = false;


        public int apollusIdentity;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gra-Knight");
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SetDefaults()
        {
            NPC.width = 102;
            NPC.height = 102;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
            NPC.damage = 45;//15;
            NPC.defense = 4;
            NPC.lifeMax = 4000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.boss = true;
            Music = MusicLoader.GetMusicSlot("OvermorrowMod/Sounds/Music/STONEBROS");
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(36, 600);
        }

        public override void AI()
        {
            // reset boolean
            OvermorrowWorld.downedKnight = false;

            Player player = Main.player[NPC.target];

            // Reset counters
            int countMinions = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == NPCType<GraniteMinibossMinion>())
                {
                    countMinions++;
                }
            }

            // AI [0] = Case Value
            // AI [1] = Timer
            // AI [2] = 
            // AI [3] = 

            if (NPC.ai[3] > 0f && dead == true)
            {
                NPC.velocity = Vector2.Zero;
                NPC.rotation = 0;

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
                            Dust dust = Main.dust[Dust.NewDust(NPC.Left, NPC.width, NPC.height / 2, DustID.UnusedWhiteBluePurple, 0f, 0f, 0, default(Color), 1f)];
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
                                //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
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



            if (NPC.life <= NPC.lifeMax * 0.5f || OvermorrowWorld.downedLady)
            {
                changedPhase2 = true;
            }

            switch (NPC.ai[0])
            {
                case -4: // half hp roar
                    if (!PlayerAlive(player)) { break; }

                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;
                    if (NPC.ai[1] == 45)
                    {
                        SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.NPCKilled, 10), (int)NPC.Center.X, (int)NPC.Center.Y);
                    }

                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                        if (distance <= 600)
                        {
                            //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
                        }
                    }

                    if (++NPC.ai[1] > 240)
                    {
                        npcDashing = false;
                        NPC.dontTakeDamage = false;
                        NPC.immortal = false;
                        changedPhase2Indicator = true;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 1;
                        NPC.ai[0] = -1;
                        NPC.ai[3] = 0;
                    }
                    break;
                case -3: // spawn sequence
                    if (!PlayerAlive(player)) { break; }

                    // reset boolean
                    OvermorrowWorld.downedKnight = false;

                    NPC.velocity = Vector2.UnitY * 0.7f;
                    NPC.dontTakeDamage = true;

                    if (++NPC.ai[1] > 380)
                    {
                        // check if no minions
                        if (countMinions <= 0)
                        {
                            NPC.ai[0] = 1;
                            NPC.ai[1] = 0;
                        }
                        else
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                        }

                        NPC.dontTakeDamage = false;
                        player.GetModPlayer<OvermorrowModPlayer>().ShowText = false;
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
                        npcDashing = false;
                    }
                    break;
                case -2: // slow movement towards player

                    if (!PlayerAlive(player)) { break; }

                    Vector2 moveTo = player.Center;
                    var move = moveTo - NPC.Center;
                    var speed = 5;

                    float length = move.Length();
                    if (length > speed)
                    {
                        move *= speed / length;
                    }
                    var turnResistance = 45;
                    move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
                    length = move.Length();
                    if (length > 10)
                    {
                        move *= speed / length;
                    }
                    NPC.velocity.X = move.X;
                    NPC.velocity.Y = move.Y * .98f;

                    if (++NPC.ai[1] > (changedPhase2 ? 150 : 180))
                    {
                        if (changedPhase2 == true && changedPhase2Indicator == false)
                        {
                            NPC.ai[0] = -4;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                        else
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                    }

                    break;
                case -1: // case switching
                    {
                        if (movement == true)
                        {
                            if (changedPhase2 == true) { RandomCeiling = 4; }
                            else { RandomCeiling = 2; }
                            while (RandomCase == LastCase)
                            {
                                RandomCase = Main.rand.Next(RandomCeiling);
                            }
                            LastCase = RandomCase;
                            movement = false;
                            NPC.ai[0] = RandomCase;
                        }
                        else
                        {
                            movement = true;
                            NPC.ai[0] = -2;
                        }
                    }
                    break;
                case 0: // dash towards player
                    if (changedPhase2 && !changedPhase2Indicator)
                    {
                        NPC.ai[0] = -4;
                        NPC.ai[1] = 0;
                        break;
                    }

                    if (!PlayerAlive(player)) { break; }

                    if (NPC.ai[1] > 5 && NPC.ai[1] < 30)
                    {
                        if (++NPC.ai[2] % 5 == 0)
                        {
                            Vector2 origin = NPC.Center;
                            float radius = 45;
                            int numLocations = 30;
                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                int dust = Dust.NewDust(position, 2, 2, DustID.UnusedWhiteBluePurple, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                Main.dust[dust].noGravity = true;
                            }
                        }
                    }

                    if (NPC.ai[1] == 30)
                    {
                        if (Main.player[NPC.target].Center.X < NPC.Center.X && npcDashing == false)
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(180)).ToRotation();
                            NPC.spriteDirection = -1;
                        }
                        else
                        {
                            NPC.rotation = NPC.DirectionTo(player.Center).ToRotation();
                        }
                        spriteDirectionStore = NPC.spriteDirection;
                        npcDashing = true;
                    }

                    if (NPC.ai[1] > 30 && NPC.ai[1] < /*90*/ 60 && NPC.ai[1] % /*10*/ 11 == 0 && changedPhase2 == true)
                    {
                        for (int i = -1; i < 1; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 5 + (10 * i)).RotatedBy(NPC.rotation), ProjectileType<GranLaser>(), NPC.damage / 3, 5f, Main.myPlayer);
                            }
                        }
                    }

                    if (++NPC.ai[1] == 30)
                    {
                        NPC.velocity = (changedPhase2 ? 12 : 8) * NPC.DirectionTo(new Vector2(Main.rand.NextFloat(player.Center.X - 25, player.Center.X + 25), Main.rand.NextFloat(player.Center.Y - 25, player.Center.Y + 25)));
                    }
                    else if (NPC.ai[1] > 60 && NPC.ai[1] < 120)
                    {
                        NPC.velocity = new Vector2(MathHelper.Lerp(NPC.velocity.X, 0, changedPhase2 ? 0.05f : 0.025f), MathHelper.Lerp(NPC.velocity.Y, 0, changedPhase2 ? 0.05f : 0.025f));
                    }
                    else if (NPC.ai[1] > 120)
                    {
                        NPC.ai[1] = 0;
                        attackCounter++;
                        npcDashing = false;
                    }
                    if (attackCounter == (changedPhase2 ? 7 : 5))
                    {
                        if (changedPhase2 == true && changedPhase2Indicator == false)
                        {
                            NPC.ai[0] = -4;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                        else
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                    }
                    break;
                case 3: // telebombs
                    if (!PlayerAlive(player)) { break; }

                    if (NPC.ai[1] == 15)
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
                                int dust = Dust.NewDust(position, 2, 2, DustID.UnusedWhiteBluePurple, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                Main.dust[dust].noGravity = true;
                            }
                        }
                    }
                    if (++NPC.ai[1] > 90)
                    {
                        NPC.Teleport(teleportPosition + new Vector2(-51, -51), 206);
                        int projectiles = 4 + (attackCounter * (changedPhase2 ? 3 : 2));
                        for (int j = 0; j < projectiles; j++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0f, 5f).RotatedBy(j * MathHelper.TwoPi / projectiles), ProjectileType<GranLaser>(), NPC.damage / 2, 10f, Main.myPlayer);
                            }
                        }
                        attackCounter++;
                        NPC.ai[1] = 0;
                    }
                    if (attackCounter == 4)
                    {
                        if (changedPhase2 == true && changedPhase2Indicator == false)
                        {
                            NPC.ai[0] = -4;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                        else
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                    }
                    break;
                case 2: // throwing lasers in patterns
                    if (!PlayerAlive(player)) { break; }

                    if (NPC.ai[2] == 0)
                    {
                        direction = Main.rand.NextBool();
                        Direction = direction ? -1 : 1;
                        NPC.ai[2]++;
                    }

                    if (NPC.ai[1] > 5 && NPC.ai[1] < 40)
                    {
                        if (++NPC.ai[2] % 5 == 0)
                        {
                            Vector2 origin = new Vector2(player.Center.X + (-600 * Direction), player.Center.Y);
                            float radius = 15;
                            int numLocations = 30;
                            for (int i = 0; i < 30; i++)
                            {
                                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                int dust = Dust.NewDust(position, 2, 2, DustID.UnusedWhiteBluePurple, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                Main.dust[dust].noGravity = true;
                            }
                        }
                    }

                    if (NPC.ai[1] > 40)
                    {
                        NPC.position = new Vector2(player.Center.X + (-600 * Direction) - 51, player.Center.Y - 51);
                    }

                    if (++NPC.ai[1] % (changedPhase2 ? 45 : 60) == 0)
                    {
                        Vector2 direction = player.Center - NPC.Center;
                        direction.Normalize();
                        int projectiles = Main.rand.Next(6, 12);
                        for (int i = projectiles * -1 / 2; i < projectiles / 2; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction.RotatedBy(i * 3) * (changedPhase2 ? 7f : 5f), ProjectileType<GranLaser>(), NPC.damage / 2, 10f, Main.myPlayer);
                            }
                        }
                        attackCounter++;
                    }
                    if (attackCounter == 8)
                    {
                        if (changedPhase2 == true && changedPhase2Indicator == false)
                        {
                            NPC.ai[0] = -4;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                        else
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                    }
                    break;
                case 1: // minions
                    if (changedPhase2 && !changedPhase2Indicator)
                    {
                        NPC.ai[0] = -4;
                        NPC.ai[1] = 0;
                        break;
                    }

                    if (!PlayerAlive(player)) { break; }

                    if (NPC.ai[2] == 0)
                    {
                        direction = Main.rand.NextBool();
                        Direction = direction ? -1 : 1;
                        NPC.ai[2]++;
                    }

                    if (NPC.ai[1] > 5 && NPC.ai[1] < 240) //90)
                    {
                        if (++NPC.ai[2] % 5 == 0)
                        {
                            Vector2 origin1 = new Vector2(player.Center.X + 300 * Direction, player.Center.Y);
                            float radius1 = 7.5f; //15;
                            int numLocations1 = 25; //30;
                            for (int i = 0; i < 25; i++)
                            {
                                Vector2 position = origin1 + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations1 * i)) * radius1;
                                Vector2 dustvelocity = new Vector2(0f, /*10*/ 7.5f).RotatedBy(MathHelper.ToRadians(360f / numLocations1 * i));
                                int dust = Dust.NewDust(position, 2, 2, DustID.UnusedWhiteBluePurple, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f); //2);
                                Main.dust[dust].noGravity = true;
                            }

                            if (NPC.ai[1] > 5 && NPC.ai[1] < 90)
                            {
                                Vector2 origin = new Vector2(player.Center.X + -300 * Direction, player.Center.Y);
                                float radius = 15;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, DustID.UnusedWhiteBluePurple, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }
                    }

                    if (NPC.ai[1] > 90)
                    {
                        NPC.position = new Vector2(player.Center.X + -300 * Direction - 51, player.Center.Y - 51);
                    }

                    if (++NPC.ai[1] % 120 == 0 && NPC.ai[1] < 360)
                    {
                        for (int i = -1; i < 1; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)((player.Center.X + 300 * Direction - 51) + (/*150*/ 75 + (/*300*/ 150 * i))), (int)NPC.Center.Y, NPCType<GraniteMinibossMinion>(), 0, 0, 0, changedPhase2 ? 1 : 0);
                            }
                        }
                        int count = 0;
                        for (int k = 0; k < 200; k++)
                        {
                            if (Main.npc[k].active && Main.npc[k].type == Mod.Find<ModNPC>("GraniteMinibossMinion").Type)
                            {
                                if (count < 4)
                                {
                                    count++;
                                }
                                else
                                {
                                    ((GraniteMinibossMinion)Main.npc[k].ModNPC).kill = true;
                                }
                            }
                        }
                    }

                    if (NPC.ai[1] == 420)
                    {
                        if (changedPhase2 == true && changedPhase2Indicator == false)
                        {
                            NPC.ai[0] = -4;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                        else
                        {
                            NPC.ai[0] = -1;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            attackCounter = 0;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                            npcDashing = false;
                            teleportPosition = Vector2.Zero;
                        }
                    }
                    break;

            }
            NPC.spriteDirection = NPC.direction;
            NPC.spriteDirection = 1;

            spritetimer++;
            if (spritetimer > 4)
            {
                frame++;
                spritetimer = 0;
            }
            if (frame > 7)
            {
                frame = 0;
            }
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/GraniteMini/AngryStone_Glow").Value;
            spriteBatch.Draw(texture, new Vector2(NPC.Center.X - screenPos.X - 1, NPC.Center.Y - screenPos.Y), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 6f == 5f)
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 8) // 8 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }

            if (Main.player[NPC.target].Center.X < NPC.Center.X && npcDashing == false)
            {
                NPC.spriteDirection = -1;
            }
            while (npcDashing == true && NPC.spriteDirection != spriteDirectionStore)
            {
                NPC.spriteDirection = spriteDirectionStore;
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
            OvermorrowWorld.downedKnight = true;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<KnightBag>()));

            var gearRule = new OneFromRulesRule(
                1,
                ItemDropRule.Common(ItemType<GraniteHelmet>())
                    .OnSuccess(ItemDropRule.Common(ItemType<GraniteBreastplate>()))
                    .OnSuccess(ItemDropRule.Common(ItemType<GraniteLeggings>())),
                ItemDropRule.Common(ItemType<GraniteSpear>()),
                ItemDropRule.Common(ItemType<GraniteBook>()),
                ItemDropRule.Common(ItemType<GraniteStaff>())
            );

            var nonExpert = new LeadingConditionRule(new Conditions.NotExpert())
                .OnSuccess(ItemDropRule.Common(ItemType<HeartStone>(), 1, 2, 2))
                .OnSuccess(ItemDropRule.Common(ItemType<GraniteTrophy>(), 10))
                .OnSuccess(gearRule);

            npcLoot.Add(nonExpert);

            base.ModifyNPCLoot(npcLoot);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
        }
    }
}