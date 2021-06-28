using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;




namespace OvermorrowMod.Projectiles.Summon
{

    public class BloodSeeker : ModNPC
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
        private int timer3 = 0;
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
            DisplayName.SetDefault("Blood Seeker");
        }
        public override void SetDefaults()
        {
            npc.width = 20;
            npc.height = 20;
            npc.friendly = true;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.defense = 0;
            npc.knockBackResist = 0f;
            npc.lifeMax = 60;
            npc.HitSound = SoundID.NPCHit1;
            npc.buffImmune[BuffID.Burning] = true;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
        }

        bool target = false;




        public override void AI()
        {
            timer3++;
            experttimer++;
            
            if(expert && experttimer == 1)
            {
                npc.life = 120;
            }

            if(expert)
            {
                npc.lifeMax = 120;
                npc.damage = (int)(Main.player[npc.target].maxMinions * 3 + 10);
            }
            else
            {
                npc.damage = (int)(Main.player[npc.target].maxMinions * 3 + 10);
            }

            if (Main.player[npc.target].position.X > npc.position.X)
            {
                npc.spriteDirection = -1;
            }

            float distanceFromTarget = 130f;
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
                        float betweenatt = Vector2.Distance(tar.Center, npc.Center);
                        float between = Vector2.Distance(tar.Center, Main.player[npc.target].Center);
                        bool closest = Vector2.Distance(npc.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;

                        if(betweenatt < 21 && timer3 > 0)
                        {
			                tar.StrikeNPC(npc.damage, 1, 0);
                            timer3 = -25;
                        }

                        if (((closest && inRange) || !foundTarget))
                        {
                            NPCtargetX = tar.Center.X;
                            NPCtargetY = tar.Center.Y;
                            Vector2 Rot = tar.Center;
                            distanceFromTarget = between;
                            targetCenter = tar.Center;
                            foundTarget = true;
                        }
                    }
                }
            }




            if (foundTarget)
            {
                npc.rotation = npc.velocity.X * 0.03f;
                movement = 1;
                movement2++;
            
                if (movement2 == 70)
                {
                    mrand = Main.rand.Next(-470, 471);
                    mrand2 = Main.rand.Next(-470, 471);
                    movement2 = 0;
                }

                if (NPCtargetX > npc.Center.X)
                {
                    npc.velocity.X += 0.5f;
                }

                if (NPCtargetX < npc.Center.X)
                {
                    npc.velocity.X -= 0.5f;
                }

                if (NPCtargetY > npc.Center.Y)
                {
                    npc.velocity.Y += 0.5f;
                }
                if (NPCtargetY < npc.Center.Y)
                {
                    npc.velocity.Y -= 0.5f;
                }


                if (npc.velocity.Y < -6.5f)
                {
                    npc.velocity.Y = -6.5f;
                }

                if (npc.velocity.Y > 6.5f)
                {
                    npc.velocity.Y = 6.5f;
                }


                if (npc.velocity.X < -6.5f)
                {
                    npc.velocity.X = -6.5f;
                }

                if (npc.velocity.X > 6.5f)
                {
                    npc.velocity.X = 6.5f;
                }



            }
            else
            {
                npc.rotation = 0;
                npc.velocity.Y -= 0.9f;
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/Summon/BloodSeeker_Glow");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X, npc.Center.Y - Main.screenPosition.Y), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }
    }
}