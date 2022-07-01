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

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class ServantOfCthulhu : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private bool EntranceFade = false;
        private bool ExitFade = false;
        private Vector2 TrailOffset;

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
            }

            base.OnSpawn(npc, source);
        }

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                if (npc.alpha >= 255) npc.alpha = 255;
                if (npc.alpha <= 0) npc.alpha = 0;

                if (npc.ai[1] == 420)
                {
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
                        //if (npc.ai[0] < 480)
                        if (npc.ai[3] < 239 || (npc.ai[3] > 269 && npc.ai[3] < 509))
                        {
                            npc.Center = parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[(int)npc.ai[0]] + TrailOffset;

                            if (npc.ai[0] != 240) npc.rotation = npc.DirectionTo(parent.GetGlobalNPC<EyeOfCthulhu>().TrailPositions[(int)npc.ai[0] + 1] + TrailOffset).ToRotation() - MathHelper.PiOver2;
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

                        /*else
                        {
                            // NPC has moved through all indices and launches themselves out a random direction
                            // This doesn't occur until 60 seconds later, after the boss has roared
                            if (npc.ai[3]++ >= 60)
                            {
                                if (npc.alpha > 0) npc.alpha -= 10;

                                // Start moving after the NPC has completely faded in
                                if (npc.alpha == 0 && npc.ai[0] == 60)
                                {
                                    npc.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * 5;
                                    npc.rotation = npc.velocity.ToRotation() - MathHelper.PiOver2;
                                    npc.ai[1] = 0;
                                }
                            }
                        }*/
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
                                //if (npc.alpha >= 255) npc.alpha = 255;
                                //if (npc.alpha <= 0) npc.alpha = 0;

                                // Entrance portal makes them fade in, if they are going out the final we don't want them to fade in yet
                                if ((projectile.ai[0] == 240 || projectile.ai[0] == 360) && parent.GetGlobalNPC<EyeOfCthulhu>().PortalRuns < 2)
                                {
                                    EntranceFade = true;
                                }

                                // The exit portal should make them fade out
                                if (projectile.ai[0] == 450)
                                {
                                    ExitFade = true;
                                    //if (npc.alpha < 255) npc.alpha += 10;
                                }
                            }
                        }
                    }

                    // These lines of code make it so that contacting other portals on route dont cause the opacity to change
                    // Also makes it so that all the NPCs can fade in and out completely instead of being partial due to offset
                    if (EntranceFade)
                    {
                        if (npc.alpha > 0)
                        {
                            npc.alpha -= 12;
                        }
                        else
                        {
                            EntranceFade = false;
                        }
                    }

                    if (ExitFade)
                    {
                        if (npc.alpha < 255)
                        {
                            npc.alpha += 12;
                        }
                        else
                        {
                            ExitFade = false;
                        }
                    }
                    #endregion

                    // During the following state, we don't want the AI to run
                    return false;
                }
                else
                {
                    // When the AI is set to -1, the NPC will not deal damage for 1.5 seconds
                    // This is set whenever they are spawned from the world portals
                    if (npc.ai[0] == -1)
                    {
                        if (npc.ai[1]++ > 90) npc.ai[0] = 0;
                    }

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

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
                spriteBatch.Reload(SpriteSortMode.Immediate);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/ServantOfCthulhu").Value;

                if (npc.ai[1] < 90)
                {
                    //Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
                    //progress = Utils.Clamp(npc.ai[1], 0, 90) / 90f;
                    //effect.Parameters["WhiteoutColor"].SetValue(Color.Black.ToVector3());
                    //effect.Parameters["WhiteoutProgress"].SetValue(1 - progress);
                    //effect.CurrentTechnique.Passes["Whiteout"].Apply();
                }

                Color color = Color.Lerp(drawColor, Color.Transparent, npc.alpha / 255f);

                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, color, npc.rotation + MathHelper.PiOver2, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 0);

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
                spriteBatch.Draw(texture, npc.Center - screenPos, null, color, npc.rotation + MathHelper.PiOver2, npc.frame.Size() / 2, npc.scale, SpriteEffects.None, 0);
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
}