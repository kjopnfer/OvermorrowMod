using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.DemonEye
{
    public class EyeSummon : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 24;
            projectile.minionSlots = 1f;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 200000;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Eye");
            Main.projFrames[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<DemEyeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<DemEyeBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion

            // 5 ticks per frame
            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            float num540 = 2000f;
            float num541 = 800f;
            float num542 = 1200f;
            float num543 = 150f;

            float num544 = 0.05f;
            for (int num545 = 0; num545 < 1000; num545++)
            {
                bool flag22 = true;

                if (num545 != projectile.whoAmI && Main.projectile[num545].active && Main.projectile[num545].owner == projectile.owner && flag22 && Math.Abs(projectile.position.X - Main.projectile[num545].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num545].position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < Main.projectile[num545].position.X)
                    {
                        projectile.velocity.X -= num544;
                    }
                    else
                    {
                        projectile.velocity.X += num544;
                    }
                    if (projectile.position.Y < Main.projectile[num545].position.Y)
                    {
                        projectile.velocity.Y -= num544;
                    }
                    else
                    {
                        projectile.velocity.Y += num544;
                    }
                }
            }

            bool flag23 = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1]++;
                projectile.extraUpdates = 1;
                projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI;

                if (projectile.ai[1] > 40f)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = 0;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    flag23 = true;
                }
            }

            if (flag23)
            {
                return;
            }
            Vector2 center4 = projectile.position;
            bool flag24 = false;
            if (projectile.ai[0] != 1f)
            {
                projectile.tileCollide = true;
            }

            if (projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16)))
            {
                projectile.tileCollide = false;
            }
            NPC ownerMinionAttackTargetNPC3 = projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC3 != null && ownerMinionAttackTargetNPC3.CanBeChasedBy(this))
            {
                float num552 = Vector2.Distance(ownerMinionAttackTargetNPC3.Center, projectile.Center);
                float num553 = num540 * 3f;
                if (num552 < num553 && !flag24 && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, ownerMinionAttackTargetNPC3.position, ownerMinionAttackTargetNPC3.width, ownerMinionAttackTargetNPC3.height))
                {
                    num540 = num552;
                    center4 = ownerMinionAttackTargetNPC3.Center;
                    flag24 = true;
                }
            }
            if (!flag24)
            {
                for (int num554 = 0; num554 < 200; num554++)
                {
                    NPC nPC2 = Main.npc[num554];
                    if (nPC2.CanBeChasedBy(this))
                    {
                        float num555 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!(num555 >= num540) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, nPC2.position, nPC2.width, nPC2.height))
                        {
                            num540 = num555;
                            center4 = nPC2.Center;
                            flag24 = true;
                        }
                    }
                }
            }
            float num556 = num541;
            if (flag24)
            {
                num556 = num542;
            }
            Player player4 = Main.player[projectile.owner];
            if (Vector2.Distance(player4.Center, projectile.Center) > num556)
            {
                projectile.ai[0] = 1f;
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }
            if (flag24 && projectile.ai[0] == 0f)
            {
                Vector2 vector44 = center4 - projectile.Center;
                float num557 = vector44.Length();
                vector44.Normalize();
                if (num557 > 200f)
                {
                    float num558 = 6f;

                    num558 = 14f;

                    vector44 *= num558;
                    projectile.velocity = (projectile.velocity * 40f + vector44) / 41f;
                }
                else
                {
                    float num559 = 4f;
                    vector44 *= 0f - num559;
                    projectile.velocity = (projectile.velocity * 40f + vector44) / 41f;
                }
            }
            else
            {
                bool flag25 = false;
                if (!flag25)
                {
                    flag25 = projectile.ai[0] == 1f;
                }
                if (!flag25)
                {
                    flag25 = false;
                }
                float num560 = 6f;

                if (flag25)
                {
                    num560 = 15f;
                }
                Vector2 center5 = projectile.Center;
                Vector2 vector45 = player4.Center - center5 + new Vector2(0f, -60f);
                float num561 = vector45.Length();
                float num562 = num561;
                if (num561 > 200f && num560 < 8f)
                {
                    num560 = 8f;
                }
                if (num561 < num543 && flag25 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {

                    projectile.ai[0] = 0f;


                    projectile.netUpdate = true;
                }
                if (num561 > 2000f)
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num561 > 70f)
                {
                    Vector2 vector46 = vector45;
                    vector45.Normalize();
                    vector45 *= num560;
                    projectile.velocity = (projectile.velocity * 40f + vector45) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI;


            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += Main.rand.Next(1, 4);
            }

            if (projectile.ai[1] > 40f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }

            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f && flag24 && num540 < 500f)
                {
                    projectile.ai[1]++;
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.ai[0] = 2f;
                        Vector2 vector49 = center4 - projectile.Center;
                        vector49.Normalize();
                        projectile.velocity = vector49 * 8f;
                        projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (!(projectile.ai[0] < 3f))
                {
                    return;
                }
                int num571 = 0;
                switch ((int)projectile.ai[0])
                {
                    case 0:
                    case 3:
                    case 6:
                        num571 = 400;
                        break;
                    case 1:
                    case 4:
                    case 7:
                        num571 = 400;
                        break;
                    case 2:
                    case 5:
                    case 8:
                        num571 = 600;
                        break;
                }
                if (!(projectile.ai[1] == 0f && flag24) || !(num540 < (float)num571))
                {
                    return;
                }
                projectile.ai[1]++;
                if (Main.myPlayer != projectile.owner)
                {
                    return;
                }
                if (projectile.localAI[0] >= 3f)
                {
                    projectile.ai[0] += 4f;
                    if (projectile.ai[0] == 6f)
                    {
                        projectile.ai[0] = 3f;
                    }
                    projectile.localAI[0] = 0f;
                }
                else
                {
                    projectile.ai[0] += 6f;
                    Vector2 vector50 = center4 - projectile.Center;
                    vector50.Normalize();
                    float num572 = ((projectile.ai[0] == 8f) ? 12f : 10f);
                    projectile.velocity = vector50 * num572;
                    projectile.netUpdate = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
    }
}
