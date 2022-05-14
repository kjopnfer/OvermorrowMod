using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class EyeOfCthulhu : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        bool Temp = true;

        private int MoveDirection = 1;

        private float InitialRotation;
        private int TearDirection = 1;

        private int RotateDirection = 1;
        private Vector2 InitialPosition;

        private bool TransitionPhase = false;

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => false;

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                npc.aiStyle = NPCID.DemonEye;
                npc.noTileCollide = true;
            }

            if (npc.type == NPCID.EyeofCthulhu)
            {
                npc.lifeMax = 3200;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            npc.ai[0] = 0;
            npc.ai[1] = 0;
            npc.ai[2] = 0;
            npc.ai[3] = 0;

            base.OnSpawn(npc, source);
        }

        public enum AIStates
        {
            Transition = -2,
            Intro = -1,
            Selector = 0,
            Tear = 1,
            Minions = 2
        }

        // ai[0] - AI Case
        // ai[1] - AI Counter
        // ai[2] - Secondary AI Counter
        // ai[3] - Miscellaneous (VFX) Counter

        public override bool PreAI(NPC npc)
        {
            if (npc.type != NPCID.EyeofCthulhu) return true;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            npc.rotation = npc.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;

            npc.defense = 12;
            foreach (NPC minion in Main.npc)
            {
                if (minion.ModNPC is EyeStalk)
                {
                    npc.defense = 32;
                }
            }

            if (npc.life <= npc.lifeMax * 0.5f && !TransitionPhase)
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
            }

            switch (npc.ai[0])
            {
                case (float)AIStates.Transition:
                    if (TransitionPhase)
                    {
                        if (++npc.ai[1] % 15 == 0 && npc.ai[1] < 540)
                        {

                            Vector2 RandomPosition = player.Center + new Vector2(Main.rand.Next(-9, 7) + 1, Main.rand.Next(-7, 5) + 1) * 75;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), RandomPosition, Vector2.Zero, ModContent.ProjectileType<DarkEye>(), npc.damage, 0f, Main.myPlayer);

                            if (Main.rand.NextBool(4))
                            {
                                Projectile.NewProjectile(npc.GetSource_FromAI(), player.position, Vector2.Zero, ModContent.ProjectileType<DarkEye>(), npc.damage, 0f, Main.myPlayer);
                            }
                        }

                        if (npc.ai[1] >= /*870*/750)
                        {

                            if (npc.ai[3] > 0)
                            {
                                npc.ai[3] -= 0.05f;
                            }
                            else
                            {
                                npc.dontTakeDamage = false;

                                npc.ai[0] = (float)AIStates.Selector;
                                npc.ai[1] = 0;
                                npc.ai[2] = 0;
                                npc.ai[3] = 0;
                            }
                        }
                    }
                    break;
                case (float)AIStates.Intro:
                    //npc.Center = player.Center - new Vector2(0, 50);
                    break;
                case (float)AIStates.Selector:
                    if (npc.ai[1]++ == 0 && Temp)
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            var entitySource = npc.GetSource_FromAI();
                            int index = NPC.NewNPC(entitySource, (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<EyeStalk>(), npc.whoAmI);

                            NPC minionNPC = Main.npc[index];
                            if (minionNPC.ModNPC is EyeStalk minion)
                            {
                                minion.StalkID = i;
                                minion.ParentIndex = npc.whoAmI;
                            }

                            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: index);
                            }
                        }

                        // Long tentacles
                        for (int i = 0; i <= 3; i++)
                        {
                            int projectileIndex = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeTentacle>(), 0, 0f, Main.myPlayer, Main.rand.Next(5, 7) * 15, Main.rand.NextFloat(2.5f, 3.75f));
                            Projectile proj = Main.projectile[projectileIndex];
                            if (proj.ModProjectile is EyeTentacle tentacle)
                            {
                                tentacle.value = Main.rand.Next(0, 3) * 50;
                                tentacle.parentID = npc.whoAmI;
                            }
                        }

                        // Short tentacles
                        for (int i = 0; i <= 3; i++)
                        {
                            int projectileIndex = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeTentacle>(), 0, 0f, Main.myPlayer, Main.rand.Next(2, 4) * 15, Main.rand.NextFloat(4f, 5f));
                            Projectile proj = Main.projectile[projectileIndex];
                            if (proj.ModProjectile is EyeTentacle tentacle)
                            {
                                tentacle.value = Main.rand.Next(0, 3) * 50;
                                tentacle.parentID = npc.whoAmI;
                            }
                        }

                        Temp = false;
                    }

                    if (npc.ai[1] % 360 == 0)
                    {
                        RotateDirection = Main.rand.NextBool() ? 1 : -1;
                    }

                    Dust.NewDust(player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(npc.ai[2] += (0.25f * RotateDirection))) * 500, 1, 1, DustID.Adamantite);
                    npc.Move(player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(npc.ai[2] += (0.25f * RotateDirection))) * 500, 2.5f, 1);

                    if (npc.ai[1] == 360)
                    {
                        //npc.ai[0] = Main.rand.NextBool() ? (float)AIStates.Minions : (float)AIStates.Tear;
                        npc.ai[0] = (float)AIStates.Minions;

                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }

                    break;
                case (float)AIStates.Tear:
                    if (npc.ai[1]++ == 0)
                    {
                        npc.velocity = Vector2.Zero;
                        InitialPosition = npc.Center;
                        TearDirection = Main.rand.NextBool() ? 1 : -1;

                        MoveDirection = TearDirection;
                    }

                    if (npc.ai[1] <= 120)
                    {
                        npc.Center = Vector2.Lerp(InitialPosition, player.Center + new Vector2(500 * TearDirection, -250), npc.ai[1] / 180f);
                    }

                    if (npc.ai[1] >= 120)
                    {
                        float ToRotation = npc.DirectionTo(npc.Center + Vector2.UnitY * 50).ToRotation() - MathHelper.PiOver2;

                        if (npc.ai[2] == 0)
                        {
                            InitialRotation = npc.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2;
                        }

                        npc.rotation = MathHelper.Lerp(InitialRotation, ToRotation, Utils.Clamp(npc.ai[2]++, 0, 60) / 60f);

                        // Each second, swap the direction that the NPC is moving if the player somehow gets through the barrage
                        if (npc.ai[1] % 60 == 0) MoveDirection = player.Center.X > npc.Center.X ? -1 : 1;

                        npc.Move(npc.Center - (Vector2.UnitX * 2 * MoveDirection), 4, 10);
                    }

                    if (npc.ai[1] == 420)
                    {
                        npc.ai[0] = (float)AIStates.Selector;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
                case (float)AIStates.Minions:
                    if (npc.ai[1]++ == 0)
                    {
                        npc.velocity = Vector2.Zero;
                    }

                    if (npc.ai[1] % 15 == 0)
                    {
                        Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-8, -4)) * 25;

                        Vector2 velocity = Vector2.UnitY /** Main.rand.NextFloat(0.25f, 1f)*/;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), RandomPosition, velocity, ModContent.ProjectileType<DemonEye>(), npc.damage, 0f, Main.myPlayer, 0, Main.rand.NextFloat(-1f, 1f));
                    }

                    /*if (npc.ai[1] == 180)
                    {
                        for (int i = 0; i < Main.rand.Next(4, 7); i++)
                        {
                            Vector2 RandomPosition = npc.Center + new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-8, -4)) * 25;
                            var entitySource = npc.GetSource_FromAI();
                            int index = NPC.NewNPC(entitySource, (int)RandomPosition.X, (int)RandomPosition.Y, NPCID.ServantofCthulhu);

                            if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: index);
                            }
                        }
                    }*/

                    if (npc.ai[1] == 360)
                    {
                        npc.ai[0] = (float)AIStates.Selector;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
            }


            return false;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu").Value;

                if (npc.ai[0] == (float)AIStates.Minions && npc.ai[1] > 120 && npc.ai[1] < 180)
                {
                    int amount = 5;
                    //float progress = (float)Math.Sin(npc.localAI[1]++ / 30f);
                    float progress = Utils.Clamp(npc.ai[1] - 120f, 0, 60) / 60f;

                    for (int i = 0; i < amount; i++)
                    {
                        float scaleAmount = i / (float)amount;
                        float scale = 1f + progress * scaleAmount;
                        spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Cyan * (1f - progress), npc.rotation, texture.Size() / 2, scale * npc.scale, SpriteEffects.None, 0f);
                    }

                }

                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, drawColor, npc.rotation, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);
                return false;
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu_Glow").Value;
                spriteBatch.Draw(texture, npc.Center - screenPos, null, Color.White, npc.rotation, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}