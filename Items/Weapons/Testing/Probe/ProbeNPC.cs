using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;



namespace OvermorrowMod.Items.Weapons.Testing.Probe
{

    public class ProbeNPC : ModNPC
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        int timertodie = 0;
        readonly bool expert = Main.expertMode;
        Vector2 RotNPC;
        Vector2 Rot;
        Vector2 newMove;
        private int PosCheck = 0;
        private readonly int timer2 = 0;
        private int timer = 0;
        private int movement = 0;
        private int movement2 = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        private int NumNPC = 0;
        int mrand = Main.rand.Next(-500, 501);
        int mrand2 = Main.rand.Next(-500, 501);
        private int experttimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prime Laser");
        }
        private const string ChainTexturePath = "Juvenation/Items/Projectiles/Probe/ProbeBone";
        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 40;
            npc.friendly = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.defense = 45;
            npc.lifeMax = 1000;
            npc.HitSound = SoundID.NPCHit4;
            npc.buffImmune[BuffID.Burning] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
        }

        bool target = false;




        public override void AI()
        {

            if (Main.player[npc.target].Center.X > npc.position.X + 1000)
            {
                npc.position.X = Main.player[npc.target].Center.X;
            }
            if (Main.player[npc.target].Center.X < npc.position.X - 1000)
            {
                npc.position.X = Main.player[npc.target].Center.X;
            }



            if (Main.player[npc.target].Center.Y > npc.position.Y + 1000)
            {
                npc.position.Y = Main.player[npc.target].Center.Y;
            }
            if (Main.player[npc.target].Center.Y < npc.position.Y - 1000)
            {
                npc.position.Y = Main.player[npc.target].Center.Y;
            }

            npc.damage = (int)(Main.player[npc.target].minionDamage * 20);
            experttimer++;
            if (expert && experttimer == 1)
            {
                npc.life = 1000;
                npc.lifeMax = 1000;
                npc.damage = npc.damage / 2;
            }

            if (Main.player[npc.target].position.X > npc.position.X)
            {
                npc.spriteDirection = -1;
            }

            float distanceFromTarget = 600f;
            Vector2 targetCenter = npc.position;
            bool foundTarget = false;
            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC tar = Main.npc[i];
                    if (tar.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(tar.Center, Main.player[npc.target].Center);
                        bool closest = Vector2.Distance(npc.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = tar.Center.X;
                            NPCtargetY = tar.Center.Y;
                            Vector2 Rot = tar.Center;
                            distanceFromTarget = between;
                            targetCenter = tar.Center;
                            foundTarget = true;
                            npc.rotation = (Rot - npc.Center).ToRotation();
                        }
                    }
                }
            }




            if (foundTarget)
            {
                npc.velocity.Y = 0f;
                movement = 1;
                movement2++;

                if (movement2 == 70)
                {
                    mrand = Main.rand.Next(-470, 471);
                    mrand2 = Main.rand.Next(-470, 471);
                    movement2 = 0;
                }

                if (NPCtargetX + mrand > npc.Center.X)
                {
                    npc.velocity.X += 0.9f;
                }

                if (NPCtargetX + mrand < npc.Center.X)
                {
                    npc.velocity.X -= 0.9f;
                }

                if (NPCtargetY + mrand2 > npc.Center.Y)
                {
                    npc.velocity.Y += 2f;
                }
                if (NPCtargetY + mrand2 < npc.Center.Y)
                {
                    npc.velocity.Y -= 2f;
                }


                if (npc.velocity.Y < -18f)
                {
                    npc.velocity.Y = -18f;
                }

                if (npc.velocity.Y > 18f)
                {
                    npc.velocity.Y = 18f;
                }


                if (npc.velocity.X < -9f)
                {
                    npc.velocity.X = -9f;
                }

                if (npc.velocity.X > 9f)
                {
                    npc.velocity.X = 9f;
                }



                timer++;
                if (timer == 45 + Random2)
                {
                    Random2 = Main.rand.Next(-15, 12);
                    Random = Main.rand.Next(-2, 3);
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = targetCenter;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(1.5f));
                    float speed = Random + 20f;
                    Projectile.NewProjectile(npc.Center.X, npc.Center.Y, direction.X * speed, direction.Y * speed, mod.ProjectileType("PrimeLaser"), 85, 0f, Main.myPlayer);
                    timer = 0;
                }


            }
            else
            {
                npc.life = -125;
            }
        }

        public override void NPCLoot()
        {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(npc.Center.X, npc.Center.Y, value1.X, value1.Y, mod.ProjectileType("ProbeBomb"), npc.damage + 20, 1f);
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 10f)
            {
                vector *= 10f / magnitude;
            }
        }
    }
}
