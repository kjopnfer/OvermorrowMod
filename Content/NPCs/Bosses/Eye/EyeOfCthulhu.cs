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

        public List<int> MinionList = new List<int>();

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
                NPCID.Sets.TrailCacheLength[npc.type] = 7;
                NPCID.Sets.TrailingMode[npc.type] = 3;

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
                npc.ai[0] = (float)AIStates.Selector;
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

            //float progress = -0.106f;

            //float darkIncrease = MathHelper.Lerp(-0.01f, -0.08f, Utils.Clamp(npc.localAI[2]++, 0, 14400f) / 14400f);
            //float progress = MathHelper.Lerp(darkIncrease, -0.06f - darkIncrease, (float)Math.Sin(npc.localAI[2]++ / 60f) / 2 + 0.5f);
            //float progress = MathHelper.Lerp(0, 0.1f, (float)Math.Sin(npc.localAI[2]++ / 60f) / 2 + 0.5f);
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


            if (Main.netMode != NetmodeID.Server)
            {
                if (npc.ai[1] >= 340 && npc.ai[1] <= 540)
                {
                    if (!Filters.Scene["ContainedFlash"].IsActive())
                    {
                        Filters.Scene.Activate("ContainedFlash");
                    }

                    float value = ModUtils.EaseOutQuad(Utils.Clamp(npc.localAI[2], 0, 60) / 60f);
                    float progress = MathHelper.Lerp(0, 12f, value);
                    float size = MathHelper.Lerp(0, 30, value);
                    if (npc.ai[1] >= 420)
                    {
                        value = Utils.Clamp(npc.localAI[2] - 60, 0, 30) / 30f;
                        progress = MathHelper.Lerp(12, 0f, value);
                        size = MathHelper.Lerp(30, 0, value);
                    }

                    if (Filters.Scene["ContainedFlash"].IsActive())
                    {
                        Filters.Scene["ContainedFlash"].GetShader().UseTargetPosition(npc.Center + new Vector2(24, -20).RotatedBy(npc.rotation + MathHelper.PiOver2));
                        Filters.Scene["ContainedFlash"].GetShader().UseIntensity(progress);
                        Filters.Scene["ContainedFlash"].GetShader().Shader.Parameters["rotation"].SetValue(npc.rotation + MathHelper.Pi + MathHelper.PiOver4);
                        Filters.Scene["ContainedFlash"].GetShader().Shader.Parameters["rotationArea"].SetValue(MathHelper.ToRadians(size));
                    }

                    npc.localAI[2]++;

                    if (npc.ai[1] == 540)
                    {
                        npc.localAI[2] = 0;
                        if (Filters.Scene["ContainedFlash"].IsActive()) Filters.Scene.Deactivate("ContainedFlash");
                    }
                }
            }
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
                        targetPlayer.AddBuff(ModContent.BuffType<Watched>(), 60);
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
                        if (Main.rand.NextBool(5))
                        {
                            Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-3, 3) + 5, Main.rand.Next(-8, -4)) * 50;

                            var entitySource = npc.GetSource_FromAI();

                            int index = NPC.NewNPC(entitySource, (int)RandomPosition.X, (int)RandomPosition.Y, ModContent.NPCType<MiniServant>(), 0, 0, 0, 0, npc.whoAmI);
                            if (Main.rand.NextBool())
                            {
                                index = NPC.NewNPC(entitySource, (int)RandomPosition.X, (int)RandomPosition.Y, NPCID.ServantofCthulhu);
                            }

                            MinionList.Add(index);

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

                    if (npc.ai[1] == 600)
                    {
                        foreach (int id in MinionList)
                        {
                            NPC minion = Main.npc[id];
                            if (minion.active)
                            {
                                ServantOfCthulhu servant = minion.GetGlobalNPC<ServantOfCthulhu>();
                                // Forces all servants to dash at the player
                                if (!servant.BossDash)
                                {
                                    servant.BossDash = true;
                                    servant.BossDelay = Main.rand.Next(0, 7) * 10;
                                }
                            }
                            else
                            {
                                if (!((MiniServant)minion.ModNPC).shadowForm && minion.ai[0] != 1)
                                {
                                    ((MiniServant)minion.ModNPC).shadowForm = true;
                                    ((MiniServant)minion.ModNPC).shadowCounter = Main.rand.Next(5, 8) * 60;
                                }
                            }
                        }
                        

                        /*foreach (NPC minions in Main.npc)
                        {
                            if (minions.type != NPCID.ServantofCthulhu && minions.type != ModContent.NPCType<MiniServant>() && !minions.active) continue;

                            if (minions.type == NPCID.ServantofCthulhu && minions.active)
                            {
                                ServantOfCthulhu servant = minions.GetGlobalNPC<ServantOfCthulhu>();
                                // Forces all servants to dash at the player
                                if (!servant.BossDash)
                                {
                                    servant.BossDash = true;
                                    servant.BossDelay = Main.rand.Next(0, 7) * 10;
                                }
                            }
                            else
                            {
                                if (!((MiniServant)minions.ModNPC).shadowForm && minions.ai[0] != 1)
                                {
                                    ((MiniServant)minions.ModNPC).shadowForm = true;
                                    ((MiniServant)minions.ModNPC).shadowCounter = Main.rand.Next(5, 8) * 60;
                                }
                            }
                        }*/
                        //npc.ai[0] = Main.rand.NextBool() ? (float)AIStates.Minions : (float)AIStates.Tear;
                        npc.ai[0] = (float)AIStates.Selector;
                        npc.ai[1] = 0;

                    }
                    #endregion
                    break;
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

                #region Pulse
                texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu").Value;

                if (npc.ai[0] == (float)AIStates.Selector && npc.ai[1] > 300 && npc.ai[1] < 360)
                {
                    int amount = 5;
                    float progress = Utils.Clamp(npc.ai[1] - 300, 0, 60) / 60f;

                    for (int i = 0; i < amount; i++)
                    {
                        float scaleAmount = i / (float)amount;
                        scale = 1f + progress * scaleAmount;
                        spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Orange * (1f - progress), npc.rotation - MathHelper.PiOver4, texture.Size() / 2, scale * npc.scale, SpriteEffects.None, 0f);
                    }
                }
                #endregion

                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    Vector2 drawPos = npc.oldPos[k] + npc.Size / 2 - Main.screenPosition;
                    Color afterImageColor = Color.Orange * ((float)npc.oldPos.Length - k / (float)npc.oldPos.Length);
                    //spriteBatch.Draw(texture, drawPos, null, afterImageColor, npc.oldRot[k] - MathHelper.PiOver4, texture.Size() / 2f, npc.scale, SpriteEffects.None, 0f);
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


                if (npc.ai[1] >= 120 && npc.ai[1] <= 360)
                {
                    if (!Main.gamePaused) npc.localAI[0]++;

                    Texture2D spotlight = ModContent.Request<Texture2D>(AssetDirectory.Textures + "EyeTell").Value;
                    //float lightAlpha = MathHelper.Lerp(0.65f, 0.8f, (float)Math.Sin(npc.localAI[0]++ / 20f));
                    float lightAlpha = MathHelper.Lerp(0.8f, 0f, Utils.Clamp(npc.localAI[0] - 120, 0, 60) / 60f);
                    float heightLerp = MathHelper.Lerp(1f, 0, ModUtils.EaseOutQuad(Utils.Clamp(npc.localAI[0] - 60f, 0, 180) / 180f));
                    spriteBatch.Draw(spotlight, npc.Center + new Vector2(npc.width * 1.52f, -npc.height * 1.35f).RotatedBy(npc.rotation + MathHelper.PiOver2) - Main.screenPosition, null, new Color(255, 200, 46) * lightAlpha, npc.rotation - MathHelper.PiOver4, spotlight.Size() / 2, new Vector2(heightLerp, 1), SpriteEffects.None, 0);
                }
                else
                {
                    npc.localAI[0] = 0;
                }

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}