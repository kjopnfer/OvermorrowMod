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

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public partial class EyeOfCthulhu : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        bool Temp = true;

        private int MoveDirection = 1;

        private float InitialRotation;
        private int TearDirection = 1;

        private int RotateDirection = 1;
        private Vector2 InitialPosition;

        private bool TransitionPhase = false;

        private List<Projectile> TentacleList;
        public List<Vector2> TrailPositions;

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            switch (npc.type)
            {
                case NPCID.ServantofCthulhu:
                    return npc.ai[0] != -1;
                case NPCID.EyeofCthulhu:
                    return false;
            }

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        //public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => false;

        public override void SetDefaults(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    npc.lifeMax = 3200;
                    break;
                case NPCID.ServantofCthulhu:
                    npc.lifeMax = 12;
                    break;
            }
        }

        public override void DrawBehind(NPC npc, int index)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                npc.hide = true;
                Main.instance.DrawCacheNPCProjectiles.Add(index);
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                TentacleList = new List<Projectile>();
                TrailPositions = new List<Vector2>();

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

        Vector2 TrailOffset;
        // ai[0] - AI Case
        // ai[1] - AI Counter
        // ai[2] - Secondary AI Counter
        // ai[3] - Miscellaneous (VFX) Counter
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                if (npc.ai[1] == 420)
                {
                    if (npc.ai[0] == 0)
                    {
                        TrailOffset = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 15;
                    }

                    NPC parent = Main.npc[(int)npc.ai[2]];
                    if (parent.active && parent.type == NPCID.EyeofCthulhu)
                    {
                        if (npc.ai[0]++ < parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions.Count - 1)
                        {
                            npc.Center = parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[(int)npc.ai[0]] + TrailOffset;
                        }
                    }
                }
                else
                {
                    // When the AI is set to -1, the NPC will not deal damage for 1.5 seconds
                    // This is set whenever they are spawned from the portals
                    if (npc.ai[0] == -1)
                    {
                        if (npc.ai[1]++ > 90) npc.ai[0] = 0;
                    }

                    foreach (NPC boss in Main.npc)
                    {
                        if (!boss.active || boss.type != NPCID.EyeofCthulhu) continue;

                        if (npc.Hitbox.Intersects(boss.Hitbox))
                        {
                            boss.HealEffect(npc.life);
                            boss.life += npc.life;

                            npc.HitEffect(0, npc.damage);
                            npc.Kill();
                        }
                    }
                }

                return true;
            }

            if (npc.type != NPCID.EyeofCthulhu) return true;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

            if (npc.ai[0] != (float)AIStates.Portal)
                npc.rotation = npc.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver4;

            npc.defense = 12;
            foreach (NPC minion in Main.npc)
            {
                if (minion.ModNPC is VortexEye)
                {
                    npc.defense = 32;
                }
            }

            //float progress = MathHelper.Lerp(0, 1 / 3f, (float)Math.Sin(npc.localAI[2]++ / 30f));
            //float progress = -0.106f;

            float darkIncrease = MathHelper.Lerp(-0.01f, -0.08f, Utils.Clamp(npc.localAI[2]++, 0, 14400f) / 14400f);
            float progress = MathHelper.Lerp(darkIncrease, -0.06f - darkIncrease, (float)Math.Sin(npc.localAI[2]++ / 60f));
            //float progress = MathHelper.Lerp(-0.066f, -0.106f, (float)Math.Sin(npc.localAI[2]++ / 30f));


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

            if (npc.life > npc.lifeMax) npc.life = npc.lifeMax;

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
                        targetPlayer.AddBuff(ModContent.BuffType<Paralyzed>(), 360);
                    }
                }
            }

            switch (npc.ai[0])
            {
                case (float)AIStates.Transition:
                    Transition(npc, player);
                    break;
                case (float)AIStates.Intro:
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

                        TentacleList.Add(proj);
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

                        TentacleList.Add(proj);
                    }

                    npc.ai[0] = (float)AIStates.Selector;
                    break;
                case (float)AIStates.Selector:
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

                    if (npc.ai[1] % 360 == 0)
                    {
                        RotateDirection = Main.rand.NextBool() ? 1 : -1;
                    }

                    Dust.NewDust(player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(npc.ai[2] += (0.25f * RotateDirection))) * 500, 1, 1, DustID.Adamantite);
                    npc.Move(player.Center + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(npc.ai[2] += (0.25f * RotateDirection))) * 370, 2.5f, 1);

                    if (npc.ai[1] == 360)
                    {
                        //npc.ai[0] = Main.rand.NextBool() ? (float)AIStates.Minions : (float)AIStates.Tear;
                        npc.ai[0] = (float)AIStates.Portal;

                        npc.ai[1] = 0;
                    }

                    break;
                case (float)AIStates.Portal:
                    if (npc.ai[1] == 0)
                    {
                        npc.velocity = Vector2.Zero;
                        npc.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 3f;
                    }
                    // recording time
                    if (npc.ai[1]++ < 480)
                    {
                        if (npc.ai[1] % 60 == 0)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 3f;
                        }

                        TrailPositions.Add(npc.Center);
                    }

                    if (npc.ai[1] == 480)
                    {
                        npc.velocity = Vector2.Zero;
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)TrailPositions[0].X, (int)TrailPositions[0].Y, NPCID.ServantofCthulhu, 0, 0, 420, npc.whoAmI);
                    }

                    if (npc.ai[1] > 480 && npc.ai[1] % 5 == 0)
                    {
                        int RandomOffset = Main.rand.Next(-5, 5) * 10;
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)TrailPositions[0].X, (int)TrailPositions[0].Y, NPCID.ServantofCthulhu, 0, 0, 420, npc.whoAmI, RandomOffset);
                    }
                    /*if (npc.ai[1]++ == 0)
                    {
                        npc.velocity = Vector2.Zero;
                    }

                    if (npc.ai[1] > 45)
                    {
                        foreach (Projectile projectile in TentacleList)
                        {
                            if (projectile.active && projectile.ModProjectile is EyeTentacle tentacle)
                            {
                                tentacle.lockGrow = true;
                                if (tentacle.length > 0 && npc.localAI[3]++ % 3 == 0) tentacle.length--;
                            }
                        }
                    }

                    if (npc.ai[1] > 300 && npc.alpha < 255)
                    {
                        npc.alpha += 5;
                    }

                    if (npc.ai[1] >= 450)
                    {
                        npc.rotation += MathHelper.Lerp(0.12f, 0.03f, Utils.Clamp(npc.ai[1] - 450, 0, 120) / 120f);
                    }
                    else
                    {
                        npc.rotation += MathHelper.Lerp(0.03f, 0.12f, Utils.Clamp(npc.ai[1], 0, 120) / 120f);
                    }

                    // From 240 to 420 is when the portal fully opens
                    // Hold for half a second
                    if (npc.ai[1] >= 420 + 30)
                    {
                        npc.dontTakeDamage = true;
                        npc.ShowNameOnHover = false;

                        // Close after a second
                        if (npc.ai[1] == 630)
                        {
                            npc.ShowNameOnHover = true;
                            npc.dontTakeDamage = false;

                            npc.ai[0] = (float)AIStates.Selector;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                        }
                    }*/
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

                    if (npc.ai[1] == 360)
                    {
                        npc.ai[0] = (float)AIStates.Selector;
                        npc.ai[1] = 0;
                        npc.ai[2] = 0;
                    }
                    break;
                case (float)AIStates.Suck:
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
            }


            return false;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture;
            float progress;
            float scale;

            switch (npc.type)
            {
                case NPCID.ServantofCthulhu:
                    spriteBatch.Reload(SpriteSortMode.Immediate);

                    texture = TextureAssets.Npc[npc.type].Value;

                    if (npc.ai[1] < 90)
                    {
                        //Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                        //progress = Utils.Clamp(npc.ai[1], 0, 90) / 90f;
                        //effect.Parameters["WhiteoutColor"].SetValue(Color.Black.ToVector3());
                        //effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
                        //effect.CurrentTechnique.Passes["Whiteout"].Apply();
                    }

                    spriteBatch.Draw(texture, npc.Center - Main.screenPosition, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 0);

                    spriteBatch.Reload(SpriteSortMode.Deferred);

                    return false;
                case NPCID.EyeofCthulhu:
                    #region Portal
                    if (npc.ai[0] == (float)AIStates.Portal && npc.ai[1] > 200)
                    {
                        scale = MathHelper.Lerp(0, 2.25f, Utils.Clamp(npc.ai[1] - 200, 0, 180) / 180f);

                        if (npc.ai[1] >= 450) scale = MathHelper.Lerp(2.25f, 0f, Utils.Clamp(npc.ai[1] - 450, 0, 180) / 180f);

                        npc.localAI[1] += 0.065f;

                        texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Vortex2").Value;
                        spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, new Color(60, 3, 79), npc.localAI[1] * 0.5f, texture.Size() / 2, scale, SpriteEffects.None, 0);

                        texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "VortexCenter").Value;
                        scale = MathHelper.Lerp(0, 2f, Utils.Clamp(npc.ai[1] - 240, 0, 180) / 180f);
                        if (npc.ai[1] >= 450) scale = MathHelper.Lerp(2.2f, 0f, Utils.Clamp(npc.ai[1] - 450, 0, 180) / 180f);

                        spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Black, npc.localAI[1], texture.Size() / 2, scale, SpriteEffects.None, 0);
                    }
                    #endregion

                    texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu").Value;

                    if (npc.ai[0] == (float)AIStates.Minions && npc.ai[1] > 120 && npc.ai[1] < 180)
                    {
                        int amount = 5;
                        progress = Utils.Clamp(npc.ai[1] - 120f, 0, 60) / 60f;

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