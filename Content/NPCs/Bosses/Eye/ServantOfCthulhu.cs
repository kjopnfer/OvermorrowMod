using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class ServantOfCthulhu : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private bool EntranceFade = false;
        private bool ExitFade = false;
        private Vector2 TrailOffset;

        private bool BossPulse = false;
        private int PulseCounter = 0;


        Vector2 InitialVelocity;
        float InitialRotation;
        Vector2 ChargePosition;

        float RandomAngle;
        float RandomSpeed;
        float ChargeAngle = 0;

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                NPC parent = Main.npc[(int)npc.ai[2]];
                if (parent.active && parent.type == NPCID.EyeofCthulhu)
                {
                    if (parent.GetGlobalNPC<EyeOfCthulhu>().IntroPortal) return false;
                }

                return npc.ai[0] != -1;
            }

            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                NPCID.Sets.TrailCacheLength[npc.type] = 100;
                NPCID.Sets.TrailingMode[npc.type] = 3;

                npc.lifeMax = 12;
            }

            base.SetDefaults(npc);
        }

        public override void DrawBehind(NPC npc, int index)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                npc.hide = true;
                Main.instance.DrawCacheNPCProjectiles.Add(index);
            }
            base.DrawBehind(npc, index);
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                if (npc.ai[1] == 420)
                {
                    npc.alpha = 255;
                }

                BossPulse = false;
                PulseCounter = 0;

                npc.localAI[1] = -30;
                npc.localAI[2] = -60;

                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<ServantTrail>(), 0, 0f, Main.myPlayer, npc.whoAmI);
            }

            base.OnSpawn(npc, source);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                if (npc.alpha >= 255) npc.alpha = 255;
                if (npc.alpha <= 0) npc.alpha = 0;

                if (npc.ai[1] == 42069)
                {
                    // AI[0] is the indexer
                    // AI[1] is the state of moving through the portal
                    // AI[2] is the ID of the EoC
                    // AI[3] is the AI counter
                    if (npc.ai[0] == 0)
                    {
                        TrailOffset = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)) * 15;
                    }

                    NPC parent = Main.npc[(int)npc.ai[2]];
                    if (parent.active && parent.type == NPCID.EyeofCthulhu)
                    {
                        npc.dontTakeDamage = parent.GetGlobalNPC<EyeOfCthulhu>().IntroPortal ? true : false;

                        // For each AI tick, move through an index of the array
                        // This shit is grossly hard-coded to account for the delays
                        //if (npc.ai[3] < 239 || (npc.ai[3] > 269 && npc.ai[3] < 509))
                        if (npc.ai[3] < 59 || (npc.ai[3] > 89 && npc.ai[3] < 329))
                        {
                            npc.Center = parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[(int)npc.ai[0]] + TrailOffset;

                            if (npc.ai[0] != 60) npc.rotation = npc.DirectionTo(parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[(int)npc.ai[0] + 1] + TrailOffset).ToRotation() - MathHelper.PiOver2;
                            //if (npc.ai[0] != 480) npc.rotation = npc.DirectionTo(parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[(int)npc.ai[0] + 1] + TrailOffset).ToRotation() - MathHelper.PiOver2;

                            npc.ai[0]++;
                        }

                        /*if (npc.ai[3] < 630)
                        {
                            npc.ai[3]++;
                        }*/
                        if (npc.ai[3]++ == 660)
                        {
                            npc.Center = parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[480] + TrailOffset;

                            // Reset the ai0 counter so we can reuse, since we are not going through the array anymore
                            npc.ai[0] = 0;
                        }
                        else if (npc.ai[3] >= 660)
                        {

                            if (npc.ai[0]++ == 160)
                            {
                                npc.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * Main.rand.Next(3, 6);
                                npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                            }

                            if (npc.ai[0] >= 160)
                            {
                                if (npc.alpha > 0 && !ExitFade && !EntranceFade) npc.alpha -= 10;
                            }

                            if (npc.alpha == 0 /*&& npc.ai[0] == 120*/)
                            {
                                npc.ai[1] = 0;
                            }
                        }
                    }

                    #region Fade
                    // TODO: Uncringe this code
                    if (!ExitFade && !EntranceFade)
                    {
                        foreach (Projectile projectile in Main.projectile)
                        {
                            if (projectile.type != ModContent.ProjectileType<EyePortal>() || !projectile.active) continue;

                            if (npc.Hitbox.Intersects(projectile.Hitbox))
                            {
                                // Entrance portal makes them fade in, if they are going out the final we don't want them to fade in yet
                                if ((projectile.ai[0] == 240 || projectile.ai[0] == 360) && parent.GetGlobalNPC<EyeOfCthulhu>().PortalRuns < 2)
                                    EntranceFade = true;


                                // The exit portal should make them fade out
                                if (projectile.ai[0] == 210) ExitFade = true;
                            }
                        }
                    }

                    // These lines of code make it so that contacting other portals on route dont cause the opacity to change
                    // Also makes it so that all the NPCs can fade in and out completely instead of being partial due to offset
                    if (EntranceFade)
                    {
                        if (npc.alpha > 0) npc.alpha -= 20;
                        else EntranceFade = false;
                    }

                    if (ExitFade)
                    {
                        if (npc.alpha < 255) npc.alpha += 20;
                        else ExitFade = false;
                    }
                    #endregion

                    // During the following state, we don't want the AI to run
                    return false;
                }
                else
                {
                    // AI[0] is something idk
                    // AI[1] is the AI counter

                    // When the AI is set to -1, the NPC will not deal damage for 1.5 seconds
                    // This is set whenever they are spawned from the world portals
                    if (npc.ai[0] == -1)
                    {
                        if (npc.ai[1] > 90f) npc.ai[0] = 0;
                    }

                    npc.TargetClosest(true);
                    Player player = Main.player[npc.target];


                    /*foreach (NPC boss in Main.npc)
                    {
                        if (!boss.active || boss.type != NPCID.EyeofCthulhu) continue;

                        if (npc.Hitbox.Intersects(boss.Hitbox))
                        {
                            boss.HealEffect(npc.life);
                            boss.life += npc.life;

                            npc.HitEffect(0, npc.damage);
                            npc.Kill();
                        }
                    }*/

                    npc.ai[1]++;
                    if (npc.ai[1] >= 360 && npc.ai[1] <= 600)
                    {
                        if (npc.ai[1] < 480) // NPC slows down
                        {
                            if (npc.ai[1] == 360)
                            {
                                InitialVelocity = npc.velocity;
                                InitialRotation = npc.rotation;
                            }

                            float progress = Utils.Clamp(npc.ai[1] - 360, 0, 60) / 60f;
                            npc.velocity = Vector2.Lerp(InitialVelocity, Vector2.Zero, progress);
                            npc.rotation = MathHelper.Lerp(InitialRotation, npc.DirectionTo(player.Center).ToRotation() - MathHelper.PiOver2, progress);


                            // These are generated so the NPC can simulate all the variables for the charge within PreDraw
                            if (npc.ai[1] == 420)
                            {
                                ChargePosition = npc.DirectionTo(player.Center + Vector2.UnitY * (Main.rand.Next(-5, 5) * 10)) * 15;
                                RandomAngle = Main.rand.NextFloat(1.6f, 1.8f);
                                RandomSpeed = Main.rand.Next(15, 18);

                                InitialVelocity = npc.DirectionTo(ChargePosition) * RandomSpeed;
                            }
                        }
                        else // NPC dashes forward
                        {
                            if (npc.ai[1] == 480) npc.velocity = InitialVelocity;

                            ChargeAngle = MathHelper.Lerp(0, RandomAngle, Utils.Clamp(npc.ai[1] - 420, 0, 60) / 60f);
                            npc.velocity = Vector2.Lerp(InitialVelocity, Vector2.Zero, Utils.Clamp(npc.ai[1] - 480, 0, 120) / 120f).RotatedBy(MathHelper.ToRadians(ChargeAngle));
                            npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                        }

                        if (npc.ai[1] == 600) npc.ai[1] = 0;

                        return false;
                    }
                }

                return true;
            }

            return base.PreAI(npc);
        }

        public override bool CheckActive(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                return false;
            }

            return base.CheckActive(npc);
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                //if (!BossPulse) BossPulse = true;
            }

            base.OnHitPlayer(npc, target, damage, crit);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                spriteBatch.Reload(BlendState.AlphaBlend);
                Texture2D afterImage = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Glow").Value;
                for (int k = 0; k < npc.oldPos.Length; k++)
                {
                    Vector2 drawPos = npc.oldPos[k] + npc.Size / 2 - Main.screenPosition;
                    var trailLength = ProjectileID.Sets.TrailCacheLength[npc.type];
                    var fadeMult = 1f / trailLength;
                    Color afterImageColor = Color.Black * (1f - fadeMult * k);

                    spriteBatch.Draw(afterImage, drawPos, null, afterImageColor, npc.oldRot[k] + MathHelper.PiOver2, afterImage.Size() / 2f, npc.scale * (trailLength - k) / trailLength, SpriteEffects.None, 0f);
                }

                spriteBatch.Reload(BlendState.Additive);

                if (npc.ai[1] >= 420 && npc.ai[1] <= 480)
                {
                    Texture2D trail = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/LineIndicator").Value;
                    Vector2 simulatedPosition = npc.Center;
                    Vector2 simulatedVelocity = InitialVelocity;

                    // Trajectory simulation for 120 ticks ahead
                    for (int i = 0; i < 120; i++)
                    {
                        Color trailColor = Color.Orange * ((120 - i) / 120f);

                        float simulatedAngle = MathHelper.Lerp(0, RandomAngle, Utils.Clamp(i, 0, 60) / 60f);
                        simulatedVelocity = Vector2.Lerp(InitialVelocity, Vector2.Zero, i / 120f).RotatedBy(MathHelper.ToRadians(simulatedAngle));
                        //simulatedVelocity = simulatedVelocity.RotatedBy(MathHelper.ToRadians(ChargeAngle));
                        simulatedPosition += simulatedVelocity;
                        spriteBatch.Draw(trail, simulatedPosition - screenPos, null, trailColor, 0f, trail.Size() / 2, 1f, SpriteEffects.None, 1f);
                    }
                }

                spriteBatch.Reload(BlendState.AlphaBlend);

                spriteBatch.Reload(SpriteSortMode.Immediate);

                if (npc.ai[1] < 90f)
                {
                    //Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                    //progress = Utils.Clamp(npc.ai[1], 0, 90f) / 90f;
                    //effect.Parameters["WhiteoutColor"].SetValue(Color.Black.ToVector3());
                    //effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
                    //effect.CurrentTechnique.Passes["Whiteout"].Apply();
                }

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/ServantOfCthulhu").Value;

                Rectangle drawRectangle = new Rectangle(0, (int)(texture.Height / 2f) * 1, texture.Width, (int)(texture.Height / 2f));

                Color color = Color.Lerp(drawColor, Color.Transparent, npc.alpha / 255f);
                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, drawRectangle, color, npc.rotation + MathHelper.PiOver2, drawRectangle.Size() / 2, npc.scale, SpriteEffects.None, 0);

                spriteBatch.Reload(SpriteSortMode.Deferred);

                return false;
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/ServantOfCthulhu_Glow").Value;
                Color color = Color.Lerp(Color.White, Color.Transparent, npc.alpha / 255f);

                if (npc.ai[1] < 360)
                    spriteBatch.Draw(texture, npc.Center - screenPos, null, color, npc.rotation + MathHelper.PiOver2, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);

                #region Pulse
                spriteBatch.Reload(BlendState.Additive);

                if (!Main.gamePaused) PulseCounter++;
                if (BossPulse && PulseCounter <= 360)
                {
                    texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;

                    if (npc.localAI[0]++ == 90f) npc.localAI[0] = 0;
                    if (npc.localAI[1]++ == 90f) npc.localAI[1] = 0;
                    if (npc.localAI[2]++ == 90f) npc.localAI[2] = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        float progress = ModUtils.EaseOutQuad(Utils.Clamp(npc.localAI[i], 0, 90f) / 90f);
                        color = Color.Lerp(Color.Orange, Color.Transparent, progress);
                        float scale = MathHelper.Lerp(0, 0.25f, progress);

                        // The original is kinda faded out this just makes it thicker
                        for (int j = 0; j < 4; j++)
                            spriteBatch.Draw(texture, npc.Center - screenPos, null, color, 0f, texture.Size() / 2, scale, SpriteEffects.None, 1f);
                    }
                }

                if (PulseCounter >= 360)
                {
                    npc.localAI[0] = 0;
                    npc.localAI[1] = -30;
                    npc.localAI[2] = -60;

                    BossPulse = false;
                    PulseCounter = 0;
                }

                spriteBatch.Reload(BlendState.AlphaBlend);
                #endregion
            }


            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}