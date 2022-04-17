using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Content.Items.Consumable.BossBags;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Placeable.Boss;
using OvermorrowMod.Common.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using OvermorrowMod.Common;
using OvermorrowMod.Content.Items.Weapons.Summoner.DrakeStaff;
using OvermorrowMod.Content.Items.Weapons.Ranged.TempestGreatbow;
using OvermorrowMod.Content.Items.Weapons.Melee.StormTalon;
using OvermorrowMod.Content.Items.Weapons.Magic.BoltStream;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent;

using static Terraria.ModLoader.ModContent;
using Terraria.GameContent.ItemDropRules;

namespace OvermorrowMod.Content.NPCs.Bosses.StormDrake
{
    [AutoloadBossHead]
    public class StormDrake : ModNPC
    {
        private bool halfHealth = false;
        private bool Phase2Switched = false;
        private bool canPulse = false;
        private bool createAfterimage = false;
        private bool Dashing = false;
        private int spriteDirectionStore = 1;
        private int myProjectileStore;
        private int myProjectileStore2;
        public Vector2 targetPos;
        public float targetFloat;
        public bool p2 { get { return NPC.life < NPC.lifeMax / 2; } }
        public float AICase { get { return NPC.ai[0]; } set { NPC.ai[0] = value; } }
        public float AICounter { get { return NPC.ai[1]; } set { NPC.ai[1] = value; } }
        public float AICounter2 { get { return NPC.ai[2]; } set { NPC.ai[2] = value; } }
        public float ai3 { get { return NPC.ai[3]; } set { NPC.ai[3] = value; } }
        public float ai4 { get { return NPC.localAI[0]; } set { NPC.localAI[0] = value; } }
        private int RandomCeiling;
        private int RandomCase;
        private int LastCase;
        private int SecondToLastCase;
        private Vector2 PlayerCenterStore;
        private int relativeX = -15;
        private bool firstRunThru;
        private bool hitBoxOff;

        public Type TrailType()
        {
            return typeof(LightningTrail);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Drake");
            //Main.npcFrameCount[npc.type] = 6;
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = 296;
            NPC.height = 232;
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.defense = 12;
            NPC.lifeMax = 7600;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.DD2_BetsyDeath;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 3);
            NPC.npcSlots = 10f;
            Music = SoundLoader.GetSoundSlot("Sounds/Music/StormDrake");
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossLifeScale);
            NPC.defense = 15;
            NPC.damage /= 2;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            if (Phase2Switched && !Dashing)
            {
                if (NPC.direction == 1) // Facing right
                {
                    for (int num1202 = 0; num1202 < 6; num1202++)
                    {
                        Vector2 vector304 = NPC.position - new Vector2(-165, 74).RotatedBy(MathHelper.ToRadians(NPC.rotation));
                        vector304 -= NPC.velocity * ((float)num1202 * 0.25f);
                        Dust.NewDustDirect(vector304 + new Vector2(NPC.width / 2, NPC.height / 2), 1, 1, DustID.UnusedWhiteBluePurple, 0, 0, 0, default, 1.5f);
                    }
                }
                else // Facing left
                {
                    for (int num1202 = 0; num1202 < 6; num1202++)
                    {
                        Vector2 vector304 = NPC.position + new Vector2(-165, -74).RotatedBy(MathHelper.ToRadians(NPC.rotation));
                        vector304 -= NPC.velocity * ((float)num1202 * 0.25f);
                        Dust.NewDustDirect(vector304 + new Vector2(NPC.width / 2, NPC.height / 2), 1, 1, DustID.UnusedWhiteBluePurple, 0, 0, 0, default, 1.5f);
                    }
                }
            }

            if (halfHealth && !Phase2Switched && !(AICase == -2))
            {
                AICase = -2;
                AICounter = 0;
                AICounter2 = 0;
                NPC.ai[3] = 0;
                NPC.velocity = Vector2.Zero;
                Dashing = false;
            }

            switch (AICase)
            {
                case -3: // Spawn animation set to this
                    {
                        if (!PlayerAlive(player)) { break; }

                        NPC.dontTakeDamage = true;
                        if (AICounter >= 0 && AICounter <= 330)
                        {
                            NPC.velocity = Vector2.Zero;
                        }

                        if (NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75 && AICounter > 330)
                        {
                            NPC.Move(player.Center + new Vector2(250 * (NPC.spriteDirection * -1), -250), 4, 2);
                        }

                        if (AICounter++ > 390)
                        {
                            NPC.dontTakeDamage = false;
                            AICounter = 0;
                            AICase = -1;
                            NPC.velocity = Vector2.Zero;
                        }
                    }
                    break;
                case -2: // Phase Change
                    {
                        if (!PlayerAlive(player)) { break; }

                        NPC.rotation = 0;
                        createAfterimage = false;

                        if (++AICounter > 0 && AICounter < 100)
                        {
                            NPC.velocity = Vector2.Zero;
                            canPulse = true;
                            NPC.immortal = true;
                            NPC.dontTakeDamage = true;
                        }
                        else if (AICounter == 100)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (int i = -2; i < 9; i++)
                                {
                                    Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(25 * (i * NPC.spriteDirection), -60), Vector2.UnitY * -1, ModContent.ProjectileType<TestLightning4>(), 0, 2, Main.myPlayer, 0, NPC.whoAmI);
                                }
                            }

                            if (!Main.raining)
                            {
                                ModUtils.StartRain();
                                Main.raining = true;
                                //Main.rainTime = 99999;
                            }

                            if (AICounter2 == 0) // Print phase 2 notifier
                            {
                                if (Main.netMode == NetmodeID.SinglePlayer) // Singleplayer
                                {
                                    Main.NewText("The air crackles with electricity...", Color.Teal);
                                }
                                else if (Main.netMode == NetmodeID.Server) // Server
                                {
                                    ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("The air crackles with electricity..."), Color.Teal);
                                }
                                AICounter2++;
                            }
                        }
                        else if (AICounter == 280)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), NPC.Center, Vector2.Zero, Color.LightCyan, 1, 4, 0, 1f);
                            SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)NPC.position.X, (int)NPC.position.Y);
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 600)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 45;
                                }
                            }
                        }
                        else if (AICounter == 480)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectiles = 12;
                                for (int j = 0; j < projectiles; j++)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(0f, 75f).RotatedBy(j * MathHelper.TwoPi / projectiles), new Vector2(0f, 5f).RotatedBy(j * MathHelper.TwoPi / projectiles), ModContent.ProjectileType<LightningPhaseChangeWarning>(), NPC.damage / 2, 10f, Main.myPlayer);
                                    ((LightningPhaseChangeWarning)Main.projectile[proj].ModProjectile).RotateBy = 0.375f;
                                }
                            }
                        }
                        else if (AICounter == 780)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int projectiles = 12;
                                for (int j = 0; j < projectiles; j++)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(0f, 75f).RotatedBy((j * MathHelper.TwoPi / projectiles) + MathHelper.ToRadians(25)), new Vector2(0f, 5f).RotatedBy((j * MathHelper.TwoPi / projectiles) + 45), ModContent.ProjectileType<LightningPhaseChangeWarning>(), NPC.damage / 2, 10f, Main.myPlayer);
                                    ((LightningPhaseChangeWarning)Main.projectile[proj].ModProjectile).RotateBy = -0.375f;
                                }
                            }
                        }
                        else if (AICounter == 1080)
                        {
                            canPulse = false;
                            AICase = 0;
                            AICounter = 0;
                            AICounter2 = 0;
                            Phase2Switched = true;
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;
                        }
                        if (canPulse)
                        {
                            if (NPC.direction == 1) // Facing right
                            {
                                for (int num1202 = 0; num1202 < 6; num1202++)
                                {
                                    Vector2 vector304 = NPC.position - new Vector2(-165, 74);
                                    vector304 -= NPC.velocity * ((float)num1202 * 0.25f);
                                    Dust.NewDustPerfect(vector304 + new Vector2((NPC.width / 2), (NPC.height / 2)), 206, null, 0, default, 1.5f);
                                }
                            }
                            else // Facing left
                            {
                                for (int num1202 = 0; num1202 < 6; num1202++)
                                {
                                    Vector2 vector304 = NPC.position + new Vector2(-165, -74);
                                    vector304 -= NPC.velocity * ((float)num1202 * 0.25f);
                                    Dust.NewDustPerfect(vector304 + new Vector2((NPC.width / 2), (NPC.height / 2)), 206, null, 0, default, 1.5f);
                                }
                            }
                        }
                    }
                    break;
                case -1: // Random Attack Case Selector
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (halfHealth == true && Phase2Switched == false)
                        {
                            AICase = -2;
                            AICounter = 0;
                            AICounter2 = 0;
                            break;
                        }

                        if (Phase2Switched == true) { RandomCeiling = 8; }
                        else { RandomCeiling = 3; }
                        while (RandomCase == LastCase || RandomCase == SecondToLastCase)
                        {
                            RandomCase = Main.rand.Next(RandomCeiling);
                        }
                        if (RandomCase != LastCase && RandomCase != SecondToLastCase)
                        {
                            SecondToLastCase = LastCase;
                            LastCase = RandomCase;
                            AICase = RandomCase;
                        }
                    }
                    break;
                case 3: // Lightning Deathray Attack
                    {
                        if (!PlayerAlive(player)) { break; }

                        // Dust animation
                        if (AICounter > 0 && AICounter <= 180)
                        {
                            NPC.velocity = Vector2.Zero;
                            Vector2 spawnPos = NPC.Center + new Vector2((180 + Main.rand.NextFloat(75, 125)) * NPC.spriteDirection, -40 + Main.rand.NextFloat(-60, 60));
                            Vector2 Direction = NPC.Center + new Vector2(160 * NPC.spriteDirection, -40) - spawnPos;
                            Direction.Normalize();
                            int mydust = Dust.NewDust(spawnPos, 0, 0, DustID.Vortex, 0f, 0f, 100);
                            Main.dust[mydust].noLight = true;
                            Main.dust[mydust].noGravity = true;
                            Main.dust[mydust].velocity = NPC.velocity + (Direction * Main.rand.NextFloat(12, 14));
                        }

                        // Lightning Indicator
                        if (AICounter++ == 1)
                        {
                            AICounter2 = 0;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                myProjectileStore = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(160 * NPC.spriteDirection, -40), (Vector2.UnitX * NPC.spriteDirection).RotatedBy(MathHelper.ToRadians((NPC.spriteDirection == 1) ? 315 : 45)), ModContent.ProjectileType<LaserBreathWarning>(), NPC.damage, 2, Main.myPlayer, 0, NPC.whoAmI);
                            }
                            ((LaserBreathWarning)Main.projectile[myProjectileStore].ModProjectile).Direction = NPC.spriteDirection;
                        }

                        // Lightning Indicator Rotation
                        if (AICounter > 1 && AICounter < 180)
                        {
                            AICounter2 += 0.3f * 2;
                            ((LaserBreathWarning)Main.projectile[myProjectileStore].ModProjectile).Direction = NPC.spriteDirection;
                            ((LaserBreathWarning)Main.projectile[myProjectileStore].ModProjectile).RotateBy = AICounter2;
                            if (NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75)
                            {
                                NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0), 20, 2);
                            }
                        }

                        // Spawn Lightning Laser Beam
                        if (AICounter == 180)
                        {
                            NPC.velocity = Vector2.Zero;
                            AICounter2 = 0;
                            spriteDirectionStore = NPC.spriteDirection;
                            Dashing = true;

                            // Spawn lightning
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                myProjectileStore = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(187 * NPC.spriteDirection, -49), NPC.velocity, ModContent.ProjectileType<TestLightning2>(), NPC.damage * 2, 2, Main.myPlayer, 0, NPC.whoAmI);
                            }
                            ((TestLightning2)Main.projectile[myProjectileStore].ModProjectile).Direction = NPC.spriteDirection;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                myProjectileStore2 = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(187 * NPC.spriteDirection, -49), NPC.velocity, ModContent.ProjectileType<TestLightning2>(), NPC.damage * 2, 2, Main.myPlayer, 0, NPC.whoAmI);
                            }
                            ((TestLightning2)Main.projectile[myProjectileStore2].ModProjectile).Direction = NPC.spriteDirection;

                        }

                        // Screenshake & Lightning Rotation
                        if (AICounter > 180 && AICounter < 539)
                        {
                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 1000)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 10;
                                }
                            }
                            AICounter2 += 0.3f;
                            ((TestLightning2)Main.projectile[myProjectileStore].ModProjectile).Direction = NPC.spriteDirection;
                            ((TestLightning2)Main.projectile[myProjectileStore2].ModProjectile).Direction = NPC.spriteDirection;
                            ((TestLightning2)Main.projectile[myProjectileStore].ModProjectile).RotateBy = AICounter2;
                            ((TestLightning2)Main.projectile[myProjectileStore2].ModProjectile).RotateBy = AICounter2;
                            Main.projectile[myProjectileStore].position = NPC.Center + new Vector2(187 * NPC.spriteDirection, -49);
                            Main.projectile[myProjectileStore2].position = NPC.Center + new Vector2(187 * NPC.spriteDirection, -49);
                        }

                        if (AICounter >= 600)
                        {
                            if (!firstRunThru)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            AICounter2 = 0;
                            Dashing = false;
                            NPC.velocity = Vector2.Zero;
                            AICounter = 0;
                            myProjectileStore = 0;
                        }
                    }
                    break;
                case 4: // Radial Lightning Ball (shoots lightning in all directions)
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (((AICounter > 0 && AICounter <= 180)))//|| (AICounter > 440 && AICounter <= 580)))
                        {
                            Vector2 spawnPos = NPC.Center + new Vector2((180 + Main.rand.NextFloat(75, 125)) * NPC.spriteDirection, -40 + Main.rand.NextFloat(-60, 60));
                            Vector2 Direction = NPC.Center + new Vector2(160 * NPC.spriteDirection, -40) - spawnPos;
                            Direction.Normalize();
                            int mydust = Dust.NewDust(spawnPos, 0, 0, DustID.Vortex, 0f, 0f, 100);
                            Main.dust[mydust].noLight = true;
                            Main.dust[mydust].noGravity = true;
                            Main.dust[mydust].velocity = NPC.velocity + (Direction * Main.rand.NextFloat(12, 14));
                        }

                        if (AICounter++ == 180)// || AICounter == 580)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(170 * NPC.spriteDirection, -45), NPC.DirectionTo(player.Center) * 6f, ModContent.ProjectileType<ElectricBallRadialLightning>(), NPC.damage / 2, 2, Main.myPlayer);
                            }
                        }
                        if (AICounter > 1 && AICounter < 900 && NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75)
                        {
                            NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0), 5, 2);
                        }
                        if (AICounter > 540)//940)  //900)
                        {
                            if (!firstRunThru)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            AICounter = 0;
                            NPC.velocity = Vector2.Zero;
                        }
                    }
                    break;
                case 2: // Spinning Electric Balls
                    {
                        // Break out phase change
                        if (NPC.life <= (Main.expertMode ? NPC.lifeMax * 0.75f : NPC.lifeMax / 2) && !Phase2Switched)
                        {
                            halfHealth = true;
                            AICase = -2;
                            AICounter = 0;
                            AICounter2 = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        if (halfHealth)
                        {
                            if (!firstRunThru)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            NPC.velocity = Vector2.Zero;
                            AICounter = 0;
                        }

                        if (AICounter++ == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(160 * NPC.spriteDirection, -40), Vector2.UnitX * NPC.spriteDirection, ModContent.ProjectileType<ElectricBallCenter>(), NPC.damage, 2, Main.myPlayer, 0, NPC.whoAmI);
                            }
                        }
                        if (AICounter > 1 && AICounter < 600 && NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75)
                        {
                            NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0), 10, 2);
                        }

                        if (AICounter >= 600) // End Attack Case
                        {
                            if (!firstRunThru && halfHealth)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            NPC.velocity = Vector2.Zero;
                            AICounter = 0;
                        }
                    }
                    break;
                case 0: // Multi-directional Dash
                    {
                        // Break out phase change
                        if (NPC.life <= (Main.expertMode ? NPC.lifeMax * 0.75f : NPC.lifeMax / 2) && !Phase2Switched)
                        {
                            halfHealth = true;
                            AICase = -2;
                            AICounter = 0;
                            AICounter2 = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        if (++AICounter > 0 && AICounter < 180 && NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), targetFloat)) > 75)
                        {
                            if (AICounter == 1)
                            {
                                NPC.rotation = 0;
                                targetFloat = Main.rand.NextFloat(-350, 350);
                            }
                            else
                            {
                                NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), targetFloat), 15, 2);
                            }
                        }
                        if (++AICounter == 180)
                        {
                            if (Main.player[NPC.target].Center.X < NPC.Center.X && Dashing == false)
                            {
                                PlayerCenterStore = player.Center;
                                NPC.rotation = NPC.DirectionTo(PlayerCenterStore).RotatedBy(MathHelper.ToRadians(180)).ToRotation();
                                NPC.spriteDirection = -1;
                            }
                            else
                            {
                                PlayerCenterStore = player.Center;
                                NPC.rotation = NPC.DirectionTo(PlayerCenterStore).ToRotation();
                            }
                            NPC.velocity = Vector2.Zero;
                            spriteDirectionStore = NPC.spriteDirection;
                            Dashing = true;
                        }
                        // not only is the multi rotational dashes broken with afterimage, but pulse too
                        if (AICounter > 180 && AICounter < 240)
                        {
                            NPC.velocity = Vector2.Zero;
                            if (AICounter > 180 && AICounter < 230)
                            {
                                canPulse = true;

                                if (AICounter == 181)
                                {
                                    SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)NPC.position.X, (int)NPC.position.Y);
                                }
                            }
                            else
                            {
                                canPulse = false;
                            }
                        }
                        if (AICounter == 300)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), NPC.Center, Vector2.Zero, Color.LightCyan, 1, 4, 0, 1f);

                            int projectiles = 12;
                            for (int j = 0; j < projectiles; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ElectricBall2>(), NPC.damage / 3, 2, Main.myPlayer, 0, MathHelper.ToDegrees(j * MathHelper.TwoPi / projectiles));
                                    ((ElectricBall2)Main.projectile[proj].ModProjectile).direction = NPC.spriteDirection;
                                }
                            }

                            if (Main.expertMode)
                            {
                                for (int j = 0; j < projectiles; j++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ElectricBall2>(), NPC.damage / 3, 2, Main.myPlayer, 0, MathHelper.ToDegrees(j * MathHelper.TwoPi / projectiles));
                                        ((ElectricBall2)Main.projectile[proj].ModProjectile).direction = NPC.spriteDirection * -1;
                                        ((ElectricBall2)Main.projectile[proj].ModProjectile).Multiplier = 0.5f;
                                    }
                                }
                            }

                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                if (Main.rand.Next(2) == 0)
                                {
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("Sounds/NPC/woosh"), Main.player[i].position);
                                }
                                else
                                {
                                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("Sounds/NPC/flep"), Main.player[i].position);
                                }

                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 1600)
                                {
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 25;
                                }
                            }


                            //Main.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)npc.position.X, (int)npc.position.Y);
                            createAfterimage = true;

                            // Dashing speed
                            NPC.velocity = (35 * 4) * NPC.DirectionTo(PlayerCenterStore);
                        }
                        else if (AICounter > 310 && AICounter < 400)
                        {
                            NPC.velocity = Vector2.SmoothStep(NPC.velocity, Vector2.Zero, 0.065f * 3);
                        }
                        else if (AICounter > 400 && AICounter2 <= 0)
                        {
                            AICounter = 0;
                            AICounter2++;
                            Dashing = false;
                            createAfterimage = false;
                        }
                        else if (AICounter > 400 && AICounter2 > 0)
                        {
                            if (!firstRunThru && halfHealth)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            AICounter = 0;
                            AICounter2 = 0;
                            Dashing = false;
                            createAfterimage = false;
                            NPC.velocity = Vector2.Zero;
                            NPC.rotation = 0;
                        }
                    }
                    break;
                case 1: // Horizontal Dash
                    {
                        // Break out phase change
                        if (NPC.life <= (Main.expertMode ? NPC.lifeMax * 0.75f : NPC.lifeMax / 2) && !Phase2Switched)
                        {
                            halfHealth = true;
                            AICase = -2;
                            AICounter = 0;
                            AICounter2 = 0;
                            break;
                        }

                        if (!PlayerAlive(player)) { break; }

                        // Create sparks while dashing
                        //if (Dashing == true && AICounter % 3 == 0 && AICounter < 275)
                        //{
                        //    if (Main.netMode != NetmodeID.MultiplayerClient)
                        //    {
                        //        Projectile.NewProjectile(npc.Center.X, npc.Center.Y - 60 + Main.rand.Next(-5, 5), 0, 0, ModContent.ProjectileType<LightningSparkHitbox>(), npc.damage / 3, 1, Main.myPlayer, 0, 0);
                        //        Projectile.NewProjectile(npc.Center.X, npc.Center.Y + 60 + Main.rand.Next(-5, 5), 0, 0, ModContent.ProjectileType<LightningSparkHitbox>(), npc.damage / 3, 1, Main.myPlayer, 0, 0);
                        //    }
                        //}

                        // Glue self to side of player
                        if (++AICounter > 0 && AICounter < 200 && NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75)
                        {
                            NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0), 15, 2);
                        }
                        else if (AICounter > 200 && AICounter < 260) // Charging Telegraph Preparation
                        {
                            NPC.velocity = Vector2.Zero;
                            if (AICounter > 200 && AICounter < 250)
                            {
                                canPulse = true;

                                if (AICounter == 201)
                                {
                                    SoundEngine.PlaySound(new Terraria.Audio.LegacySoundStyle(SoundID.Roar, 0), (int)NPC.position.X, (int)NPC.position.Y);
                                }
                            }
                            else
                            {
                                canPulse = false;
                            }
                        }
                        else if (AICounter == 260) // Charge at the player
                        {
                            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), NPC.Center, Vector2.Zero, Color.LightCyan, 1, 4, 0, 1f);

                            int projectiles = 12;
                            for (int j = 0; j < projectiles; j++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ElectricBall2>(), NPC.damage / 3, 2, Main.myPlayer, 0, MathHelper.ToDegrees(j * MathHelper.TwoPi / projectiles));
                                    ((ElectricBall2)Main.projectile[proj].ModProjectile).direction = NPC.spriteDirection;
                                }
                            }

                            if (Main.expertMode)
                            {
                                for (int j = 0; j < projectiles; j++)
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        int proj = Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ElectricBall2>(), NPC.damage / 3, 2, Main.myPlayer, 0, MathHelper.ToDegrees(j * MathHelper.TwoPi / projectiles));
                                        ((ElectricBall2)Main.projectile[proj].ModProjectile).direction = NPC.spriteDirection;
                                        ((ElectricBall2)Main.projectile[proj].ModProjectile).Multiplier = 0.5f * -1;
                                    }
                                }
                            }

                            for (int i = 0; i < Main.maxPlayers; i++)
                            {
                                float distance = Vector2.Distance(NPC.Center, Main.player[i].Center);
                                if (distance <= 6400)
                                {
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("Sounds/NPC/woosh"), Main.player[i].position);
                                    }
                                    else
                                    {
                                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot("Sounds/NPC/flep"), Main.player[i].position);
                                    }
                                    //Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 25;
                                }
                            }

                            // Dashing speed
                            NPC.velocity = (Vector2.UnitX * (35f * 4)) * NPC.spriteDirection;
                            spriteDirectionStore = NPC.spriteDirection;
                            Dashing = true;
                            createAfterimage = true;
                        }
                        else if (AICounter > 270 && AICounter < 360) // Slow down to stop
                        {
                            NPC.velocity = Vector2.SmoothStep(NPC.velocity, Vector2.Zero, 0.065f * 3);
                        }
                        else if (AICounter > 360 && AICounter2 <= 0) // Flag dash end
                        {
                            AICounter = 0;
                            AICounter2++;
                            Dashing = false;
                            NPC.velocity = Vector2.Zero;
                            createAfterimage = false;
                        }
                        else if (AICounter > 360 && AICounter2 > 0) // End attack case, dashing must have occurred twice indicated by ai[2]
                        {
                            // Run through next case for second phase
                            if (!firstRunThru && halfHealth)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            AICounter = 0;
                            AICounter2 = 0;
                            Dashing = false;
                            createAfterimage = false;
                            NPC.velocity = Vector2.Zero;
                        }
                    }
                    break;
                case 5: // Lightning Strikes From Above
                    {
                        if (!PlayerAlive(player)) { break; }

                        canPulse = true;
                        hitBoxOff = true;
                        NPC.velocity = Vector2.Zero;


                        AICounter++;

                        if (AICounter % (10 - (5 - ((NPC.life / NPC.lifeMax) * 5))) == 0 && relativeX < 15 && NPC.ai[3] <= 0)
                        {
                            if (relativeX == -15)
                            {
                                Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), NPC.Center, Vector2.Zero, Color.LightCyan, 1, 4, 0, 1f);
                                PlayerCenterStore = player.Center;
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), new Vector2(PlayerCenterStore.X + (200 * relativeX), PlayerCenterStore.Y - 650), Vector2.UnitY * 5, ModContent.ProjectileType<LaserWarning2>(), 17, 1f, Main.myPlayer, 0, 1);
                            }
                            relativeX += 1;
                        }
                        if (relativeX > 14 && NPC.ai[3] <= 0)
                        {
                            if (NPC.life < NPC.lifeMax / 3)
                            {
                                NPC.ai[3] = 1;
                                AICounter = 0;
                            }
                            else
                            {
                                AICounter2 += 1;
                            }

                        }

                        if (AICounter % (10 - (5 - ((NPC.life / NPC.lifeMax) * 5))) == 0 && relativeX > -15 && NPC.ai[3] > 0)
                        {
                            if (relativeX == 15)
                            {
                                Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), NPC.Center, Vector2.Zero, Color.LightCyan, 1, 4, 0, 1f);
                                PlayerCenterStore = player.Center + (Vector2.UnitX * 33);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), new Vector2(PlayerCenterStore.X + (200 * relativeX), PlayerCenterStore.Y - 650), Vector2.UnitY * 5, ModContent.ProjectileType<LaserWarning2>(), 17, 1f, Main.myPlayer, 0, 1);
                            }
                            relativeX -= 1;
                        }

                        if (relativeX < -14 && NPC.ai[3] > 0)
                        {
                            AICounter2 += 1;
                        }

                        if (AICounter2 >= 100) // End attack case
                        {
                            if (!firstRunThru)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            hitBoxOff = false;
                            canPulse = false;
                            PlayerCenterStore = Vector2.Zero;
                            relativeX = -15;
                            AICounter = 0;
                            AICounter2 = 0;
                            NPC.ai[3] = 0;
                        }
                    }
                    break;
                case 6: // Electric Shots
                    {
                        if (!PlayerAlive(player)) { break; }

                        if (AICounter2++ > 15 && AICounter2 <= 50)
                        {
                            Vector2 spawnPos = NPC.Center + new Vector2((180 + Main.rand.NextFloat(75, 125)) * NPC.spriteDirection, -40 + Main.rand.NextFloat(-60, 60));
                            Vector2 Direction = NPC.Center + new Vector2(160 * NPC.spriteDirection, -40) - spawnPos;
                            Direction.Normalize();
                            int mydust = Dust.NewDust(spawnPos, 0, 0, DustID.Vortex, 0f, 0f, 100);
                            Main.dust[mydust].noLight = true;
                            Main.dust[mydust].noGravity = true;
                            Main.dust[mydust].velocity = NPC.velocity + (Direction * Main.rand.NextFloat(12, 14));
                        }

                        if (AICounter++ % (50 - (15 - ((NPC.life / NPC.lifeMax) * 15))) == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center + new Vector2(160 * NPC.spriteDirection, -40), NPC.DirectionTo(player.Center) * 7.5f, ModContent.ProjectileType<LaserWarning2>(), NPC.damage, 2, Main.myPlayer, 0, NPC.whoAmI);
                            }
                            AICounter2 = 0;
                        }
                        if (AICounter > 1 && AICounter < 300 && NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75)
                        {
                            NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0), 10, 2);
                        }
                        if (AICounter >= 240)//300)
                        {
                            if (!firstRunThru)
                            {
                                AICase++;
                            }
                            else
                            {
                                AICase = -1;
                            }
                            AICounter = 0;
                            AICounter2 = 0;
                            NPC.velocity = Vector2.Zero;
                        }
                    }
                    break;
                case 7: // Electric Shots Vertical (at player position)
                    {
                        if (!PlayerAlive(player)) { break; }

                        hitBoxOff = true;

                        if (AICounter++ % (50 - (15 - ((NPC.life / NPC.lifeMax) * 15))) == 0)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), player.Center + new Vector2(Main.rand.NextFloat(-200, 200), -500), Vector2.UnitY, ModContent.ProjectileType<LaserWarning2>(), NPC.damage, 2, Main.myPlayer, 0, NPC.whoAmI);
                            }
                            AICounter2 = 0;
                        }
                        if (AICounter > 1 && AICounter < 300 && NPC.Distance(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0)) > 75)
                        {
                            NPC.Move(player.Center + new Vector2(450 * (NPC.spriteDirection * -1), 0), 10, 2);
                        }
                        if (AICounter >= 240)
                        {
                            hitBoxOff = false;
                            AICase = -1;
                            AICounter = 0;
                            AICounter2 = 0;
                            NPC.velocity = Vector2.Zero;
                            firstRunThru = true;
                        }
                    }
                    break;

            }

            if (AICase != -3)
            {
                NPC.direction = NPC.spriteDirection;
                NPC.spriteDirection = 1;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple, 2 * hitDirection, -2f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].scale = 1.2f * NPC.scale;
                    }
                    else
                    {
                        Main.dust[dust].scale = 0.7f * NPC.scale;
                    }
                }

                Gore.NewGore(NPC.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, Mod.Find<ModGore>("Assets/Gores/DrakeHead").Type, NPC.scale);
                Gore.NewGore(NPC.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, Mod.Find<ModGore>("Assets/Gores/DrakeWing").Type, NPC.scale);
                Gore.NewGore(NPC.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, Mod.Find<ModGore>("Assets/Gores/DrakeLeg").Type, NPC.scale);
                Gore.NewGore(NPC.Center + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), Vector2.Zero, Mod.Find<ModGore>("Assets/Gores/DrakeTail").Type, NPC.scale);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            // Phase 2 Debuff
            if (AICounter2 == 2 && p2)
            {
                target.AddBuff(BuffID.Electrified, Main.expertMode ? 240 : 120);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            /*npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f)
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 6) // 6 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }*/
            if (Main.player[NPC.target].Center.X < NPC.Center.X && Dashing == false)
            {
                NPC.spriteDirection = -1;
            }
            while (Dashing == true && NPC.spriteDirection != spriteDirectionStore)
            {
                NPC.spriteDirection = spriteDirectionStore;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 drawOrigin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width * 0.5f, NPC.height * 0.5f);
            Vector2 drawPos2 = Main.screenPosition + drawOrigin - new Vector2(60f, 290);

            Texture2D texture2D16 = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/StormDrake/StormDrake2_Afterimage").Value;

            if (createAfterimage)
            {
                //Vector2 drawOrigin = new Vector2(Main.npcTexture[npc.type].Width * 0.5f, npc.height * 0.5f);
                //Vector2 drawOrigin = new Vector2(416, 522);
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    // Adjust drawPos if the hitbox does not match sprite dimension
                    Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin - new Vector2(60f, 290);
                    //Color afterImageColor = npc.life <= npc.lifeMax * 0.5 ? Color.LightCyan : drawColor;
                    Color afterImageColor = Color.Lerp(Color.DarkCyan, Color.DarkBlue, k / (float)NPC.oldPos.Length);
                    //Main.NewText(k / (float)npc.oldPos.Length);
                    //Color color = npc.GetAlpha(afterImageColor) * ((float)(npc.oldPos.Length - k) / (float)npc.oldPos.Length);
                    //spriteBatch.Draw(texture2D16, drawPos, npc.frame, afterImageColor, npc.rotation, drawOrigin, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }


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
            //    float num176 = 1f - (float)Math.Cos((AICounter - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
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
            /*
            Texture2D texture = Main.npcTexture[npc.type];
            //Texture2D texture2 = mod.GetTexture("Overmorrow/NPCs/StormDrake/StormDrake_Glowmask");
            Texture2D texture2 = mod.GetTexture("Overmorrow/NPCs/StormDrake/StormDrake_Glowmask");
            SpriteEffects effects = npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = npc.frame.Size() / 2;
            spriteBatch.Reload(BlendState.Additive);
            if ((npc.aiAction == 0 && AICase > 300))
            {
                for (int i = 0; i < NPCID.Sets.TrailCacheLength[npc.type]; i++)
                {
                    float alpha = (float)i / (float)NPCID.Sets.TrailCacheLength[npc.type];
                    Vector2 pos = npc.oldPos[i] - Main.screenPosition + npc.Size / 2;
                    spriteBatch.Draw(texture, pos, npc.frame, Color.White * ((float)i / (float)npc.oldPos.Length), npc.rotation, origin, npc.scale, effects, 0f);
                }
            }
            if (AICounter2 == 1 && AICase > 70 && AICase < 170)
            {
                for (int i = 0; i < 4; i++)
                {
                    float rot = MathHelper.ToRadians(i * 360 / 4);
                    float progress = (float)Math.Sin((AICase - 70) / 100 * Math.PI);
                    Vector2 offset = rot.ToRotationVector2() * 30 * progress;
                    spriteBatch.Draw(texture, npc.Center + offset - Main.screenPosition, npc.frame, npc.GetAlpha(drawColor) * 0.6f, npc.rotation, origin, npc.scale, effects, 0f);
                }
            }
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, npc.GetAlpha(drawColor), npc.rotation, origin, npc.scale, effects, 0f);
            spriteBatch.Reload(BlendState.AlphaBlend);
            spriteBatch.Draw(texture2, npc.Center, npc.frame, Color.White, npc.rotation, origin, npc.scale, effects, 0.01f);
            */
            //return false;
            if (canPulse)
            {
                // this gets the npc's frame
                Vector2 vector472 = drawOrigin;
                Color color552 = Color.White; // This is just white lol
                float amount102 = 0f; // I think this controls amount of color
                int num1782 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
                int num1792 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


                // default value
                int num1772 = 6; // ok i think this controls the number of afterimage frames
                float num1762 = 1f - (float)Math.Cos((AICounter - (float)num1782) / (float)num1792 * ((float)Math.PI * 2f));  // this controls pulsing effect
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
                    color452 = ((ModNPC)this).NPC.GetAlpha(color452);
                    color452 *= 1f - num1762; // num176 is put in here to effect the pulsing

                    // num176 is used here too
                    Vector2 vector452 = ((Entity)((ModNPC)this).NPC).Center + Utils.ToRotationVector2((float)num164 / (float)num1772 * ((float)Math.PI * 2f) + ((ModNPC)this).NPC.rotation) * scaleFactor102 * num1762 - Main.screenPosition;
                    vector452 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[((ModNPC)this).NPC.type]) * ((ModNPC)this).NPC.scale / 2f;
                    vector452 += vector472 * ((ModNPC)this).NPC.scale + new Vector2(0f, 4f + ((ModNPC)this).NPC.gfxOffY);

                    // the actual drawing of the pulsing effect
                    spriteBatch.Draw(texture2D16, vector452 - new Vector2(0, 290 / 2), ((ModNPC)this).NPC.frame, color452, ((ModNPC)this).NPC.rotation, vector472, ((ModNPC)this).NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
                }
            }
            return true;
            //return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/StormDrake/StormDrake_Glowmask").Value;
            //spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y- 141), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void OnKill()
        {
            OvermorrowWorld.downedDrake = true;
            if (Main.raining)
            {
                Main.raining = false;
                Main.rainTime = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<DrakeBag>()));
            var nonExpert = new LeadingConditionRule(new Conditions.NotExpert())
                .OnSuccess(ItemDropRule.OneFromOptions(1,
                    ItemType<BoltStream>(),
                    ItemType<StormTalon>(),
                    ItemType<TempestGreatbow>(),
                    ItemType<DrakeStaff>()))
                .OnSuccess(ItemDropRule.Common(ItemType<DrakeTrophy>(), 10))
                .OnSuccess(ItemDropRule.Common(ItemType<StormCore>(), 1, 10, 16));
            npcLoot.Add(nonExpert);

        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.LesserHealingPotion;
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

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (hitBoxOff)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(BuffID.BrokenArmor, 3600);
            }

            target.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), damage * 2, 0, false, false, crit);
        }
    }
}