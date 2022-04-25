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
            Projectile.width = 38;
            Projectile.height = 24;
            Projectile.minionSlots = 1f;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 200000;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Eye");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<DemEyeBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<DemEyeBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            #endregion

            // 5 ticks per frame
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
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

                if (num545 != Projectile.whoAmI && Main.projectile[num545].active && Main.projectile[num545].owner == Projectile.owner && flag22 && Math.Abs(Projectile.position.X - Main.projectile[num545].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[num545].position.Y) < (float)Projectile.width)
                {
                    if (Projectile.position.X < Main.projectile[num545].position.X)
                    {
                        Projectile.velocity.X -= num544;
                    }
                    else
                    {
                        Projectile.velocity.X += num544;
                    }
                    if (Projectile.position.Y < Main.projectile[num545].position.Y)
                    {
                        Projectile.velocity.Y -= num544;
                    }
                    else
                    {
                        Projectile.velocity.Y += num544;
                    }
                }
            }

            bool flag23 = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1]++;
                Projectile.extraUpdates = 1;
                Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI;

                if (Projectile.ai[1] > 40f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 0;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
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
            Vector2 center4 = Projectile.position;
            bool flag24 = false;
            if (Projectile.ai[0] != 1f)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16)))
            {
                Projectile.tileCollide = false;
            }
            NPC ownerMinionAttackTargetNPC3 = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC3 != null && ownerMinionAttackTargetNPC3.CanBeChasedBy(this))
            {
                float num552 = Vector2.Distance(ownerMinionAttackTargetNPC3.Center, Projectile.Center);
                float num553 = num540 * 3f;
                if (num552 < num553 && !flag24 && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, ownerMinionAttackTargetNPC3.position, ownerMinionAttackTargetNPC3.width, ownerMinionAttackTargetNPC3.height))
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
                        float num555 = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!(num555 >= num540) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, nPC2.position, nPC2.width, nPC2.height))
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
            Player player4 = Main.player[Projectile.owner];
            if (Vector2.Distance(player4.Center, Projectile.Center) > num556)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            if (flag24 && Projectile.ai[0] == 0f)
            {
                Vector2 vector44 = center4 - Projectile.Center;
                float num557 = vector44.Length();
                vector44.Normalize();
                if (num557 > 200f)
                {
                    float num558 = 6f;

                    num558 = 14f;

                    vector44 *= num558;
                    Projectile.velocity = (Projectile.velocity * 40f + vector44) / 41f;
                }
                else
                {
                    float num559 = 4f;
                    vector44 *= 0f - num559;
                    Projectile.velocity = (Projectile.velocity * 40f + vector44) / 41f;
                }
            }
            else
            {
                bool flag25 = false;
                if (!flag25)
                {
                    flag25 = Projectile.ai[0] == 1f;
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
                Vector2 center5 = Projectile.Center;
                Vector2 vector45 = player4.Center - center5 + new Vector2(0f, -60f);
                float num561 = vector45.Length();
                float num562 = num561;
                if (num561 > 200f && num560 < 8f)
                {
                    num560 = 8f;
                }
                if (num561 < num543 && flag25 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {

                    Projectile.ai[0] = 0f;


                    Projectile.netUpdate = true;
                }
                if (num561 > 2000f)
                {
                    Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (num561 > 70f)
                {
                    Vector2 vector46 = vector45;
                    vector45.Normalize();
                    vector45 *= num560;
                    Projectile.velocity = (Projectile.velocity * 40f + vector45) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI;


            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += Main.rand.Next(1, 4);
            }

            if (Projectile.ai[1] > 40f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && flag24 && num540 < 500f)
                {
                    Projectile.ai[1]++;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.ai[0] = 2f;
                        Vector2 vector49 = center4 - Projectile.Center;
                        vector49.Normalize();
                        Projectile.velocity = vector49 * 8f;
                        Projectile.netUpdate = true;
                    }
                }
            }
            else
            {
                if (!(Projectile.ai[0] < 3f))
                {
                    return;
                }
                int num571 = 0;
                switch ((int)Projectile.ai[0])
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
                if (!(Projectile.ai[1] == 0f && flag24) || !(num540 < (float)num571))
                {
                    return;
                }
                Projectile.ai[1]++;
                if (Main.myPlayer != Projectile.owner)
                {
                    return;
                }
                if (Projectile.localAI[0] >= 3f)
                {
                    Projectile.ai[0] += 4f;
                    if (Projectile.ai[0] == 6f)
                    {
                        Projectile.ai[0] = 3f;
                    }
                    Projectile.localAI[0] = 0f;
                }
                else
                {
                    Projectile.ai[0] += 6f;
                    Vector2 vector50 = center4 - Projectile.Center;
                    vector50.Normalize();
                    float num572 = ((Projectile.ai[0] == 8f) ? 12f : 10f);
                    Projectile.velocity = vector50 * num572;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
    }
}
