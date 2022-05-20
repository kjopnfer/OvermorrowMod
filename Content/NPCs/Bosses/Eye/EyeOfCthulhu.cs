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

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            switch (npc.type)
            {
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
                    npc.lifeMax = 60;
                    break;
            }
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
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
            Tear = 1,
            Minions = 2,
            Suck = 3
        }

        // ai[0] - AI Case
        // ai[1] - AI Counter
        // ai[2] - Secondary AI Counter
        // ai[3] - Miscellaneous (VFX) Counter
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.ServantofCthulhu)
            {
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

                return true;
            }

            if (npc.type != NPCID.EyeofCthulhu) return true;

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];

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


            if (Main.netMode != NetmodeID.Server)
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
            }

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

            npc.rotation += 0.0125f;

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
                        //npc.ai[0] = (float)AIStates.Minions;

                        npc.ai[1] = 0;
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
                        spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, Color.Orange * (1f - progress), npc.rotation, texture.Size() / 2, scale * npc.scale, SpriteEffects.None, 0f);
                    }

                }

                spriteBatch.Draw(texture, npc.Center - Main.screenPosition, null, drawColor, npc.rotation - MathHelper.PiOver4, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);

                return false;
            }

            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "Eye/EyeOfCthulhu_Glow").Value;
                spriteBatch.Draw(texture, npc.Center - screenPos, null, Color.White, npc.rotation - MathHelper.PiOver4, texture.Size() / 2, npc.scale, SpriteEffects.None, 0);

                spriteBatch.Reload(BlendState.Additive);

                if (npc.ai[1] >= 120 && npc.ai[1] <= 300)
                {
                    float alpha = MathHelper.Lerp(0.65f, 0.8f, (float)Math.Sin(npc.localAI[0]++ / 20f));
                    Texture2D spotlight = ModContent.Request<Texture2D>(AssetDirectory.Textures + "EyeTell").Value;
                    spriteBatch.Draw(spotlight, npc.Center + new Vector2(npc.width * 1.52f, -npc.height * 1.35f).RotatedBy(npc.rotation + MathHelper.PiOver2) - Main.screenPosition, null, new Color(255, 200, 46) * alpha, npc.rotation - MathHelper.PiOver4, spotlight.Size() / 2, 1f, SpriteEffects.None, 0);
                }

                spriteBatch.Reload(BlendState.AlphaBlend);
            }

            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }


    /*public class ShockwaveExplosion : ModProjectile
    {
        private int rippleCount = 3;
        private int rippleSize = 5;
        private int rippleSpeed = 45;
        private float distortStrength = 100f;

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.alpha = 255;
            projectile.friendly = true;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            if (projectile.ai[0]++ == 0)
            {
                if (Main.netMode != NetmodeID.Server && !Filters.Scene["Shockwave"].IsActive())
                {
                    Filters.Scene.Activate("Shockwave", projectile.Center).GetShader().UseColor(rippleCount, rippleSize, rippleSpeed).UseTargetPosition(projectile.Center);
                }
            }

            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                float progress = (180f - projectile.timeLeft) / 60f; // Will range from -3 to 3, 0 being the point where the bomb explodes.
                Filters.Scene["Shockwave"].GetShader().UseProgress(progress).UseOpacity(distortStrength * (1 - progress / 3f));
            }
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode != NetmodeID.Server && Filters.Scene["Shockwave"].IsActive())
            {
                Filters.Scene["Shockwave"].Deactivate();
            }
        }
    }*/

}