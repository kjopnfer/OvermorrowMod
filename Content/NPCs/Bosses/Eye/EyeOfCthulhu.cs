using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using OvermorrowMod.Common;
using System.Collections.Generic;
using Terraria.Audio;
using OvermorrowMod.Common.Particles;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public partial class EyeOfCthulhu : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        bool Temp = true;

        private int MoveDirection = 1;
        private float turnResistance;

        private float InitialRotation;
        private int TearDirection = 1;

        private int RotateDirection = 1;
        private Vector2 InitialPosition;

        private bool IntroDarkness = true;
        public bool IntroPortal = true;
        public int PortalRuns = 0;

        private bool TransitionPhase = false;
        private bool SpawnServants = true;

        private List<Projectile> TentacleList;
        public List<Vector2> TrailPositions;

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                return false;
            }

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                npc.lifeMax = 3200;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                //Main.hideUI = true;

                TentacleList = new List<Projectile>();
                TrailPositions = new List<Vector2>();

                for (int i = 0; i < 2; i++)
                {
                    int projectileIndex = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeTentacle>(), 0, 0f, Main.myPlayer, 105, Main.rand.NextFloat(2.5f, 3.75f));
                    Projectile proj = Main.projectile[projectileIndex];
                    if (proj.ModProjectile is EyeTentacle tentacle)
                    {
                        tentacle.parentID = npc.whoAmI;
                    }

                    TentacleList.Add(proj);
                }

                for (int i = 0; i < 2; i++)
                {
                    int projectileIndex = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeTentacle>(), 0, 0f, Main.myPlayer, 105, -Main.rand.NextFloat(2.5f, 3.75f));
                    Projectile proj = Main.projectile[projectileIndex];
                    if (proj.ModProjectile is EyeTentacle tentacle)
                    {
                        tentacle.parentID = npc.whoAmI;
                    }

                    TentacleList.Add(proj);
                }

                npc.rotation = -MathHelper.PiOver2;
                npc.ai[0] = (float)AIStates.Intro;
                npc.ai[1] = 0;
                npc.ai[2] = 0;
                npc.ai[3] = 0;

                npc.localAI[2] = 0;
            }

            base.OnSpawn(npc, source);
        }

        public enum AIStates
        {
            Transition = -2,
            Intro = -1,
            Selector = 0,
            Portal = 1,
            Minions = 2,
            Suck = 3
        }

        // ai[0] - AI Case
        // ai[1] - AI Counter
        // ai[2] - Secondary AI Counter
        // ai[3] - Miscellaneous (VFX) Counter
        public override bool PreAI(NPC npc)
        {
            if (npc.type != NPCID.EyeofCthulhu) return true;

            if (npc.alpha >= 255) npc.alpha = 255;
            if (npc.alpha <= 0) npc.alpha = 0;
            if (npc.life > npc.lifeMax) npc.life = npc.lifeMax;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            if (npc.ai[0] != (float)AIStates.Portal)
                npc.rotation = npc.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver4;

            //float progress = MathHelper.Lerp(0, 1 / 3f, (float)Math.Sin(npc.localAI[2]++ / 30f));
            //float progress = -0.106f;

            float darkIncrease = MathHelper.Lerp(-0.01f, -0.08f, Utils.Clamp(npc.localAI[2]++, 0, 14400f) / 14400f);
            float progress = MathHelper.Lerp(darkIncrease, -0.06f - darkIncrease, (float)Math.Sin(npc.localAI[2]++ / 60f));
            //float progress = MathHelper.Lerp(-0.066f, -0.106f, (float)Math.Sin(npc.localAI[2]++ / 30f));

            #region commented out shit
            /*if (Main.netMode != NetmodeID.Server)
            {
                if (!Filters.Scene["Flash"].IsActive())
                {
                    Filters.Scene.Activate("Flash");
                }

                if (Filters.Scene["Flash"].IsActive())
                {
                    Filters.Scene["Flash"].GetShader().UseTargetPosition(npc.Center);
                    Filters.Scene["Flash"].GetShader().UseIntensity(progress);
                }
            }*/

            /*if (npc.life <= npc.lifeMax * 0.5f && !TransitionPhase)
            {
                npc.dontTakeDamage = true;
                npc.velocity = Vector2.Zero;
                npc.ai[0] = (float)AIStates.Transition;

                if (npc.ai[3] < 1f)
                {
                    npc.ai[3] += 0.05f;
                }
                else
                {
                    TransitionPhase = true;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                }
            }*/
            #endregion

            var normalizedRotation = npc.rotation % MathHelper.TwoPi;

            float lowerAngle = normalizedRotation - MathHelper.PiOver4;
            float upperAngle = normalizedRotation + MathHelper.PiOver4;

            foreach (Player targetPlayer in Main.player)
            {
                if (!targetPlayer.active || targetPlayer.dead || npc.Distance(targetPlayer.Center) > 400) continue;

                var playerAngle = npc.DirectionTo(targetPlayer.Center).ToRotation() - MathHelper.PiOver4;

                // Stupid code that checks if the player is in a specific area because the calculations get all stupid
                // I can't explain this properly, just know that once playerAngle reaches a certain area the values all get completely swapped
                // This code effectively takes into account that specific area and reconverts it back to the correct values
                if (npc.rotation > 0)
                {
                    if (playerAngle <= 0)
                    {
                        playerAngle += MathHelper.TwoPi;
                    }
                }
                else
                {
                    if (playerAngle >= 0)
                    {
                        playerAngle -= MathHelper.TwoPi;
                    }
                }

                if (npc.ai[1] >= 120 && npc.ai[1] <= 300)
                {
                    if (playerAngle >= lowerAngle && playerAngle <= upperAngle && player.direction == -npc.direction)
                    {
                        //targetPlayer.AddBuff(ModContent.BuffType<Paralyzed>(), 360);
                    }
                }
            }

            switch (npc.ai[0])
            {
                case (float)AIStates.Transition:
                    Transition(npc, player);
                    break;
                case (float)AIStates.Intro:
                    // Increase the darkness of the screen
                    if (npc.ai[3] < 1f && !TransitionPhase)
                    {
                        npc.ai[3] += 0.05f;
                    }

                    // Screen has darkened and flagged the Intro bool for the Portal
                    if (npc.ai[3] >= 1f)
                    {
                        TransitionPhase = true;

                        npc.ai[0] = (float)AIStates.Portal;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
                case (float)AIStates.Selector:
                    #region temp
                    if (++npc.ai[1] % 120 == 0)
                    {
                        if (Main.rand.NextBool())
                        {
                            Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-3, 3) + 5, Main.rand.Next(-8, -4)) * 50;

                            var entitySource = npc.GetSource_FromAI();
                            int index = NPC.NewNPC(entitySource, (int)RandomPosition.X, (int)RandomPosition.Y, NPCID.ServantofCthulhu);

                            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: index);
                            }
                        }
                    }

                    if (npc.ai[1] % 60 == 0)
                    {
                        RotateDirection = Main.rand.NextBool() ? 1 : -1;
                    }

                    Dust.NewDust(player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(npc.ai[2] += (0.25f * RotateDirection))) * 500, 1, 1, DustID.Adamantite);
                    npc.Move(player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(npc.ai[2] += (0.25f * RotateDirection))) * 370, 2.5f, 1);

                    if (npc.ai[1] == 360)
                    {
                        //npc.ai[0] = Main.rand.NextBool() ? (float)AIStates.Minions : (float)AIStates.Tear;
                        npc.ai[0] = (float)AIStates.Selector;

                        npc.ai[1] = 0;
                    }
                    #endregion
                    break;
                case (float)AIStates.Portal:
                    // Launches towards the player before curving in a random direction. Will spawn a portal at the initial position it teleports
                    // Then simulates the entire path to place an end portal. Records the position of the eye as it travels in order for the minions
                    // to read and follow. At the end of each cycle, it will reset the AI Timer (npc.ai[1]) to zero.
                    #region Portal Spin
                    if (npc.ai[1] < 330)
                    {
                        if (npc.ai[1] == 0) npc.velocity = Vector2.Zero;

                        if (PortalRuns <= 2 && IntroPortal)
                        {
                            npc.ai[1]++;
                        }

                        // FOR SOME APPARENT REASON WHEN I TRIED TO MAKE THE TENTACLES SHRINK AND GROW
                        // IN THE INTRO AI CASE THEY KEPT GETTING INDEX OUT OF BOUNDS ERRORS
                        // THIS IS LITERALLY THE ONLY PLACE WHERE THEY WORK AND SO THE ENTIRE INTRO CASE IS HANDLED IN PORTAL AS WELL
                        if (npc.ai[1] > 45)
                        {
                            foreach (Projectile projectile in TentacleList)
                            {
                                if (projectile.active && projectile.ModProjectile is EyeTentacle tentacle)
                                {
                                    tentacle.lockGrow = true;
                                    if (tentacle.length > 0)
                                    {
                                        if (IntroDarkness)
                                        {
                                            tentacle.length--;
                                        }
                                        else
                                        {
                                            if (npc.localAI[3]++ % 3 == 0) tentacle.length--;
                                        }

                                    }
                                }
                            }
                        }

                        // Darkness eye attack
                        if (npc.ai[1] % 15 == 0 && npc.ai[1] < 240 && TransitionPhase)
                        {
                            Vector2 RandomPosition = player.Center + new Vector2(Main.rand.Next(-9, 7) + 1, Main.rand.Next(-7, 5) + 1) * 75;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<DarkEye>(), npc.damage, 0f, Main.myPlayer);

                            if (Main.rand.NextBool(4))
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), player.position, Vector2.Zero, ModContent.ProjectileType<DarkEye>(), npc.damage, 0f, Main.myPlayer);
                            }
                        }

                        if (IntroDarkness)
                        {
                            if (npc.alpha < 255)
                            {
                                npc.alpha += 5;
                            }
                        }
                        else
                        {
                            if (npc.ai[1] > 300 && npc.alpha < 255)
                            {
                                npc.alpha += 5;
                            }
                        }

                        if (npc.ai[1] >= 150)
                        {
                            npc.rotation += MathHelper.Lerp(0.12f, 0.03f, Utils.Clamp(npc.ai[1] - 450, 0, 120) / 120f);
                        }
                        else
                        {
                            npc.rotation += MathHelper.Lerp(0.03f, 0.12f, Utils.Clamp(npc.ai[1], 0, 120) / 120f);
                        }

                        // From 240 to 420 is when the portal fully opens
                        // Hold for half a second
                        if (npc.ai[1] >= 120 + 30)
                        {
                            npc.dontTakeDamage = true;
                            npc.ShowNameOnHover = false;

                            // Close after a second
                            if (npc.ai[1] == 330)
                            {

                                npc.ShowNameOnHover = true;
                                npc.dontTakeDamage = false;
                            }
                        }
                    }

                    if (npc.ai[1] == 330 && IntroDarkness)
                    {
                        if (npc.ai[3] > 0)
                        {
                            npc.alpha = 255;
                            npc.ai[3] -= 0.05f;
                        }

                        if (npc.ai[3] <= 0)
                        {
                            IntroDarkness = false;

                            foreach (Player cameraPlayer in Main.player)
                            {
                                if (npc.Distance(cameraPlayer.Center) < 1800)
                                {
                                    cameraPlayer.GetModPlayer<OvermorrowModPlayer>().PlayerLockCamera(npc, 600, 120, 60);
                                    cameraPlayer.AddBuff(ModContent.BuffType<Cutscene>(), 720);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Portal Delay
                    // This is the delay counter inbetween portal movements
                    if (npc.ai[2] > 0)
                    {
                        // NPC movement inbetween portal jumps, mainly for the camera to follow
                        switch (PortalRuns)
                        {
                            case 1:
                                npc.Center = Vector2.Lerp(player.Center + new Vector2(6, -2) * 75, InitialPosition, npc.ai[2] / 30f);
                                break;
                            case 2:
                                npc.Center = Vector2.Lerp(player.Center + new Vector2(0, -8) * 75, InitialPosition, npc.ai[2] / 30f);
                                break;
                        }

                        npc.ai[2]--;
                    }
                    #endregion

                    #region Portal Trail
                    if (npc.ai[1] >= 330 && npc.ai[3] <= 0 && npc.ai[2] == 0) // Timer is greater than 330 (end of spin), darkness is gone, and delay is 0
                    {
                        // Flying outside the portal and spawning the end-portal
                        if (npc.ai[1]++ == 330 && PortalRuns < 3)
                        {
                            npc.alpha = 255;

                            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/NPC/EyePortalOpen")
                            {
                                Volume = 0.9f,
                                PitchVariance = 0.2f,
                                MaxInstances = 3,
                            });

                            #region Position
                            if (IntroPortal)
                            {
                                // npc.Center = player.Center + new Vector2(Main.rand.Next(-6, 5) + 1, Main.rand.Next(-9, -7)) * 75;
                                switch (PortalRuns)
                                {
                                    case 0: // Left side
                                        npc.Center = player.Center + new Vector2(-6, -4) * 75;
                                        break;
                                    case 1: // Right side
                                        npc.Center = player.Center + new Vector2(6, -2) * 75;
                                        break;
                                    case 2:
                                        npc.Center = player.Center + new Vector2(0, -8) * 75;
                                        break;
                                }
                            }
                            else
                            {
                                npc.Center = player.Center + new Vector2(Main.rand.Next(-6, 5) + 1, Main.rand.Next(-6, 5) + 1) * 75;
                            }
                            #endregion

                            #region Velocity
                            npc.velocity = Vector2.Zero;
                            if (IntroPortal)
                            {
                                switch (PortalRuns)
                                {
                                    case 0: // Go right
                                        npc.velocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(20)) * 6f;
                                        RotateDirection = -1;
                                        break;
                                    case 1: // Go right
                                        npc.velocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(25)) * 6f;
                                        RotateDirection = -1;
                                        break;
                                    case 2: // Go down
                                        npc.velocity = Vector2.Normalize(npc.DirectionTo(player.Center)) * 3;
                                        break;
                                }
                                //npc.velocity = Vector2.Normalize(npc.DirectionTo(player.Center)).RotatedBy(MathHelper.ToRadians(25) * (Main.rand.NextBool() ? 1 : -1)) * 6f;
                            }
                            else
                            {
                                npc.velocity = Vector2.Normalize(npc.DirectionTo(player.Center)) * 6f;
                            }
                            #endregion

                            if (PortalRuns == 2)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<EyePortal>(), 0, 0f, Main.myPlayer, 360);
                            }
                            else
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity, ModContent.ProjectileType<EyePortal>(), 0, 0f, Main.myPlayer, 240);
                            }

                            if (!IntroPortal) RotateDirection = Main.rand.NextBool() ? 1 : -1;

                            if (PortalRuns < 2)
                            {
                                // Predict where the boss will end up a second before it does
                                Vector2 simulatedPosition = npc.Center;
                                Vector2 simulatedVelocity = npc.velocity;

                                // i am 60 parallel universes ahead of you
                                turnResistance = Main.rand.NextFloat(0.02f, 0.026f);
                                for (int i = 0; i < 240; i++)
                                {
                                    if (i > 60) simulatedVelocity = simulatedVelocity.RotatedBy(turnResistance * RotateDirection);

                                    simulatedPosition += simulatedVelocity.RotatedBy(0.012f * RotateDirection);
                                }

                                Projectile.NewProjectile(npc.GetSource_FromAI(), simulatedPosition, simulatedVelocity, ModContent.ProjectileType<EyePortal>(), 0, 0f, Main.myPlayer, 450);
                            }
                        }

                        #region Tentacle Growth
                        if (npc.ai[1] < 330 + 45)
                        {
                            foreach (Projectile projectile in TentacleList)
                            {
                                if (projectile.active && projectile.ModProjectile is EyeTentacle tentacle)
                                {
                                    if (npc.localAI[3]++ % 3 == 0) tentacle.length += 5;
                                }
                            }
                        }

                        if (PortalRuns < 2)
                        {
                            if (npc.ai[1] > 330 + 185)
                            {
                                foreach (Projectile projectile in TentacleList)
                                {
                                    if (projectile.active && projectile.ModProjectile is EyeTentacle tentacle)
                                    {
                                        if (tentacle.length > 0 && npc.localAI[3]++ % 3 == 0) tentacle.length -= 5;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Movement and Alpha
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver4;
                        if (npc.ai[1] > 330 + 60)
                        {
                            if (PortalRuns < 2)
                            {
                                if (npc.ai[1] > 330 + 225) npc.alpha += 17;

                                npc.velocity = npc.velocity.RotatedBy(turnResistance * RotateDirection);
                            }
                            else if (PortalRuns == 2) // The NPC has exited the final portal, and is slowing down
                            {
                                Texture2D badge = TextureAssets.NpcHeadBoss[0].Value;
                                OvermorrowModSystem.Instance.TitleCard.SetTitle(badge, "eye of cthulhu", "the gamer", 300);

                                if (npc.ai[1] == 360 + 120f)
                                {
                                    foreach (Player cameraPlayer in Main.player)
                                    {
                                        if (npc.Distance(cameraPlayer.Center) < 1800)
                                        {
                                            cameraPlayer.GetModPlayer<OvermorrowModPlayer>().ShowTitleCard(OvermorrowModPlayer.BossID.Eye, 480);
                                            cameraPlayer.GetModPlayer<OvermorrowModPlayer>().AddScreenShake(20, 4);
                                        }
                                    }

                                    SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/NPC/EyeScreech3")
                                    {
                                        Volume = 0.9f,
                                        PitchVariance = 0.2f,
                                        MaxInstances = 3,
                                    });
                                }

                                if (npc.ai[1] >= 360 + 120f && npc.ai[1] <= 360 + 130f && npc.ai[1] % 2 == 0)
                                {
                                    float scale = Main.rand.NextFloat(3f, 5f);
                                    Particle.CreateParticle(Particle.ParticleType<Pulse>(), npc.Center, Vector2.Zero, Color.Purple, 1, scale, 0, scale, Main.rand.Next(40, 50) * 10);
                                }

                                npc.velocity = Vector2.Lerp(Vector2.UnitY * 3, Vector2.UnitY * 0.1f, Utils.Clamp(npc.ai[1] - 390, 0, 60f) / 60f);
                            }
                        }
                        else
                        {
                            npc.alpha -= 15;
                        }

                        // Record the positions for two instances
                        if (PortalRuns < 2)
                        {
                            TrailPositions.Add(npc.Center);
                        }
                        else
                        {
                            // On the 3rd instance, record only the first position the eye teleports to, offset by 1 because it incremented earlier
                            if (npc.ai[1] == 331) { TrailPositions.Add(npc.Center); }
                        }
                        #endregion

                        // Spawn NPCs after a delay
                        if (npc.ai[1] > 330 + 60 && npc.ai[1] < 330 + 185 && npc.ai[1] % 5 == 0 && SpawnServants)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)TrailPositions[0].X, (int)TrailPositions[0].Y, NPCID.ServantofCthulhu, 0, 0, 420, npc.whoAmI);
                        }

                        #region Counter
                        // Reset the counter after 240 ticks
                        if (npc.ai[1] == 330 + 240)
                        {
                            // Set it to false so it doesn't run through multiple attack cycles
                            SpawnServants = false;

                            if (PortalRuns++ < 2)
                            {
                                npc.velocity = Vector2.Zero;
                                InitialPosition = npc.Center;

                                npc.ai[1] = 330;
                                npc.ai[2] = 30; // Set the delay inbetween portal instances
                            }
                            else
                            {
                                IntroPortal = false;
                                npc.ai[0] = (float)AIStates.Selector;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                            }
                        }
                        #endregion
                    }
                    #endregion
                    break;
                case (float)AIStates.Suck:
                    #region Suck
                    if (npc.ai[1] == 360)
                    {
                        foreach (NPC servant in Main.npc)
                        {
                            if (!servant.active || servant.type != NPCID.ServantofCthulhu && npc.Distance(servant.Center) < 400) continue;

                            var npcAngle = npc.DirectionTo(servant.Center).ToRotation() - MathHelper.PiOver4;

                            if (npc.rotation > 0)
                            {
                                if (npcAngle <= 0)
                                {
                                    npcAngle += MathHelper.TwoPi;
                                }
                            }
                            else
                            {
                                if (npcAngle >= 0)
                                {
                                    npcAngle -= MathHelper.TwoPi;
                                }
                            }

                            if (npcAngle >= lowerAngle && npcAngle <= upperAngle)
                            {
                                float PullStrength = MathHelper.Lerp(.65f, .25f, npc.Distance(servant.Center) / 400f);
                                float Direction = (npc.Center - servant.Center).ToRotation();
                                float HorizontalPull = (float)Math.Cos(Direction) * PullStrength;
                                float VerticalPull = (float)Math.Sin(Direction) * PullStrength;

                                servant.velocity += new Vector2(HorizontalPull, VerticalPull);
                            }
                        }

                        // dust for mouth suck
                        /*for (int _ = 0; _ < 2; _++)
                        {
                            var rot = npc.rotation + Main.rand.NextFloat(-MathHelper.PiOver4 + 0.2f, MathHelper.PiOver4 - 0.2f);

                            Vector2 RandomPosition = npc.Center + Vector2.One.RotatedBy(rot + MathHelper.Pi) * -Main.rand.Next(200, 250);
                            Vector2 Direction = Vector2.Normalize(npc.Center - RandomPosition);

                            int DustSpeed = 30;

                            int dust = Dust.NewDust(RandomPosition, 1, 1, DustID.Cloud, Direction.X * DustSpeed, Direction.Y * DustSpeed, 0, default, Main.rand.NextFloat(1, 1.5f));
                            Main.dust[dust].noGravity = true;
                        }*/
                    }
                    break;
                    #endregion
            }

            return false;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture;
            float scale;

            if (npc.type == NPCID.EyeofCthulhu)
            {
                #region Portal
                if (npc.ai[0] == (float)AIStates.Portal && npc.ai[1] > 200)
                {
                    scale = MathHelper.Lerp(0, 2.25f, Utils.Clamp(npc.ai[1] - 200, 0, 180) / 180f);

                    if (npc.ai[1] >= 450) scale = MathHelper.Lerp(2.25f, 0f, Utils.Clamp(npc.ai[1] - 450, 0, 180) / 180f);

                    npc.localAI[1] += 0.065f;

                    texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Vortex2").Value;
                    //spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, new Color(60, 3, 79), npc.localAI[1] * 0.5f, texture.Size() / 2, scale, SpriteEffects.None, 0);

                    texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "VortexCenter").Value;
                    scale = MathHelper.Lerp(0, 2f, Utils.Clamp(npc.ai[1] - 240, 0, 180) / 180f);
                    if (npc.ai[1] >= 450) scale = MathHelper.Lerp(2.2f, 0f, Utils.Clamp(npc.ai[1] - 450, 0, 180) / 180f);

                    //spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Black, npc.localAI[1], texture.Size() / 2, scale, SpriteEffects.None, 0);
                }
                #endregion

                texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu").Value;

                if (npc.ai[0] == (float)AIStates.Minions && npc.ai[1] > 120 && npc.ai[1] < 180)
                {
                    int amount = 5;
                    float progress = Utils.Clamp(npc.ai[1] - 120f, 0, 60) / 60f;

                    for (int i = 0; i < amount; i++)
                    {
                        float scaleAmount = i / (float)amount;
                        scale = 1f + progress * scaleAmount;
                        spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Orange * (1f - progress), npc.rotation, texture.Size() / 2, scale * npc.scale, SpriteEffects.None, 0f);
                    }
                }

                Color color = Color.Lerp(drawColor, Color.Transparent, npc.alpha / 255f);
                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, color, npc.rotation - MathHelper.PiOver4, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);

                return false;
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                #region Glowmask
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu_Glow").Value;
                Color color = Color.Lerp(Color.White, Color.Transparent, npc.alpha / 255f);
                spriteBatch.Draw(texture, npc.Center - screenPos, null, color, npc.rotation - MathHelper.PiOver4, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);
                #endregion

                spriteBatch.Reload(BlendState.Additive);

                if (npc.ai[1] >= 120 && npc.ai[1] <= 300)
                {
                    Texture2D spotlight = ModContent.Request<Texture2D>(AssetDirectory.Textures + "EyeTell").Value;
                    float lightAlpha = MathHelper.Lerp(0.65f, 0.8f, (float)Math.Sin(npc.localAI[0]++ / 20f));

                    spriteBatch.Draw(spotlight, npc.Center + new Vector2(npc.width * 1.52f, -npc.height * 1.35f).RotatedBy(npc.rotation + MathHelper.PiOver2) - Main.screenPosition, null, new Color(255, 200, 46) * lightAlpha, npc.rotation - MathHelper.PiOver4, spotlight.Size() / 2, 1f, SpriteEffects.None, 0);
                }

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}