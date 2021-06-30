using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.NPCs.Bosses.GraniteMini
{
    public class AngryStone : ModNPC
    {
        int spritetimer = 0;
        int frame = 1;
        int attackcounter = 0;
        Vector2 teleportposition = Vector2.Zero;
        bool changedPhase2 = false;
        bool changedPhase3 = false;

        int Direction = -1;
        bool direction = false;

        int RandomCase = 0;
        int LastCase = 0;
        int RandomCeiling;
        bool movement = true;

        bool dashing = false;
        int spritedirectionstore = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gra-Knight");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.width = 102;
            npc.height = 102;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.knockBackResist = 0f;
            npc.damage = 15;
            npc.defense = 4;
            npc.lifeMax = 2000;
            npc.HitSound = SoundID.NPCHit4;
            npc.value = Item.buyPrice(gold: 5);
            //animationType = NPCID.Zombie;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(36, MethodHelper.SecondsToTicks(10));
        }

        public override void AI()
        {
            Player player = Main.player[npc.target];
            if (npc.life <= npc.lifeMax * 0.5f)
            {
                changedPhase2 = true;
            }
            if (npc.life <= npc.lifeMax * 0.25f)
            {
                changedPhase3 = true;
            }
            switch (npc.ai[0])
            {
                /*case -3:
                    {
                        if (!AliveCheck(player)) { break; }

                        if (npc.ai[2] == 0)
                        {
                            //direction = Main.rand.NextBool();
                            //Direction = direction ? -1 : 1;
                            Direction = -1;
                            npc.ai[2]++;
                        }

                        if (npc.ai[1] > 5 && npc.ai[1] < 40)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = new Vector2(player.Center.X + (-600 * Direction), player.Center.Y);
                                float radius = 15;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 206, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (npc.ai[1] > 40)
                        {
                            npc.position = new Vector2(player.Center.X + (-600 * Direction) - 51, player.Center.Y - 51);
                        }

                        if (++npc.ai[1] == 40)
                        {
                            Projectile.NewProjectile(npc.Center + (Vector2.UnitY * npc.width), Vector2.UnitY, ProjectileType<GraniteLaserEnd>(), (int)(npc.damage / 2), 1f, Main.myPlayer, 0f, npc.whoAmI);
                        }
                    }
                    break;*/
                case -2: // slow movement
                    {
                        if (!AliveCheck(player)) { break; }

                        Vector2 moveTo = player.Center;
                        var move = moveTo - npc.Center;
                        var speed = 5;

                        float length = move.Length();
                        if (length > speed)
                        {
                            move *= speed / length;
                        }
                        var turnResistance = 45;
                        move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                        length = move.Length();
                        if (length > 10)
                        {
                            move *= speed / length;
                        }
                        npc.velocity.X = move.X;
                        npc.velocity.Y = move.Y * .98f;

                        if (++npc.ai[1] > (changedPhase2 ? 90 : 120))
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            attackcounter = 0;
                            npc.velocity = Vector2.Zero;
                            npc.rotation = 0;
                        }
                    }
                    break;
                case -1: // case switching
                    {
                        if (movement == true)
                        {
                            if (changedPhase2 == true) { RandomCeiling = 4; }
                            else { RandomCeiling = 2; }
                            while (RandomCase == LastCase)
                            {
                                RandomCase = Main.rand.Next(RandomCeiling);
                            }
                            LastCase = RandomCase;
                            movement = false;
                            npc.ai[0] = RandomCase;
                        }
                        else
                        {
                            movement = true;
                            npc.ai[0] = -2;
                        }
                    }
                    break;
                case 0: //Dash
                    {
                        if (!AliveCheck(player)) { break; }

                        if (npc.ai[1] > 5 && npc.ai[1] < 30)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = npc.Center;
                                float radius = 45;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 206, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (npc.ai[1] == 30)
                        {
                            if (Main.player[npc.target].Center.X < npc.Center.X && dashing == false)
                            {
                                npc.rotation = npc.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(180 /*- 90*/)).ToRotation();
                                npc.spriteDirection = -1;
                            }
                            else
                            {
                                npc.rotation = npc.DirectionTo(player.Center)./*RotatedBy(MathHelper.ToRadians(90)).*/ToRotation();
                            }
                            spritedirectionstore = npc.spriteDirection;
                            dashing = true;
                        }

                        if (npc.ai[1] > 30 && npc.ai[1] < 90 && npc.ai[1] % 10 == 0 && changedPhase2 == true)
                        {
                            for (int i = -1; i < 1; i++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(0 /*5 + (10 * i)*/, /*0*/ 5 + (10 * i)/*).RotatedBy(npc.rotation)*/).RotatedBy(npc.rotation), ProjectileType<GranLaser>(), 2, 10f, Main.myPlayer);
                            }
                        }

                        if (++npc.ai[1] == 30)
                        {
                            npc.velocity = (changedPhase2 ? 15 : 10) * npc.DirectionTo(new Vector2(Main.rand.NextFloat(player.Center.X - 25, player.Center.X + 25), Main.rand.NextFloat(player.Center.Y - 25, player.Center.Y + 25)));
                        }
                        else if (npc.ai[1] > 60 && npc.ai[1] < 120)
                        {
                            npc.velocity = new Vector2(MathHelper.Lerp(npc.velocity.X, 0, changedPhase2 ? 0.05f : 0.025f), MathHelper.Lerp(npc.velocity.Y, 0, changedPhase2 ? 0.05f : 0.025f));
                        }
                        else if (npc.ai[1] > 120)
                        {
                            npc.ai[1] = 0;
                            attackcounter++;
                            dashing = false;
                        }
                        if (attackcounter == (changedPhase2 ? 7 : 5))
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            attackcounter = 0;
                            npc.velocity = Vector2.Zero;
                            npc.rotation = 0;
                            dashing = false;
                        }
                    }

                    break;
                case 3: // telebombs
                    {
                        if (!AliveCheck(player)) { break; }

                        if (npc.ai[1] == 15)
                        {
                            teleportposition = player.Center + Main.rand.NextVector2Circular(333, 333);

                            while (Main.tile[(int)teleportposition.X / 16, (int)teleportposition.Y / 16].active())
                            {
                                teleportposition = player.Center + Main.rand.NextVector2Circular(333, 333);
                            }
                        }

                        if (npc.ai[1] > 30)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = teleportposition;
                                float radius = 20;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 206, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }
                        if (++npc.ai[1] > 90)
                        {
                            npc.Teleport(teleportposition + new Vector2(-51, -51), 206);
                            int projectiles = 4 + (attackcounter * (changedPhase2 ? 3 : 2));
                            for (int j = 0; j < projectiles; j++)
                            {
                                Projectile.NewProjectile(npc.Center, new Vector2(0f, 5f).RotatedBy(j * MathHelper.TwoPi / projectiles), ProjectileType<GranLaser>(), 2, 10f, Main.myPlayer);
                            }
                            attackcounter++;
                            npc.ai[1] = 0;
                        }
                        if (attackcounter == 4)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            attackcounter = 0;
                            npc.velocity = Vector2.Zero;
                            teleportposition = Vector2.Zero;
                        }
                    }
                    break;
                case 2: // throwing lasers in patterns
                    {
                        if (!AliveCheck(player)) { break; }

                        if (npc.ai[2] == 0)
                        {
                            direction = Main.rand.NextBool();
                            Direction = direction ? -1 : 1;
                            npc.ai[2]++;
                        }

                        if (npc.ai[1] > 5 && npc.ai[1] < 40)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = new Vector2(player.Center.X + (-600 * Direction), player.Center.Y);
                                float radius = 15;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 206, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (npc.ai[1] > 40)
                        {
                            npc.position = new Vector2(player.Center.X + (-600 * Direction) - 51, player.Center.Y -51);
                        }

                        if (++npc.ai[1] % (changedPhase2 ? 45 : 60) == 0)
                        {
                            Vector2 direction = player.Center - npc.Center;
                            direction.Normalize();
                            int projectiles = /*changedPhase2 ? Main.rand.Next(9, 16) :*/ Main.rand.Next(6, 12);
                            for (int i = projectiles * -1 / 2; i < projectiles / 2; i++)
                            {
                                Projectile.NewProjectile(npc.Center, direction.RotatedBy(i * 3) * (changedPhase2 ? 7f : 5f), ProjectileType<GranLaser>(), 2, 10f, Main.myPlayer);
                            }
                            attackcounter++;
                        }
                        if (attackcounter == 8)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            attackcounter = 0;
                            npc.velocity = Vector2.Zero;
                            teleportposition = Vector2.Zero;
                        }
                    }
                    break;
                case 1: //minions
                    {
                        if (!AliveCheck(player)) { break; }

                        if (npc.ai[2] == 0)
                        {
                            direction = Main.rand.NextBool();
                            Direction = direction ? -1 : 1;
                            npc.ai[2]++;
                        }

                        if (npc.ai[1] > 5 && npc.ai[1] < 90)
                        {
                            if (++npc.ai[2] % 5 == 0)
                            {
                                Vector2 origin = new Vector2(player.Center.X + -300 * Direction, player.Center.Y);
                                float radius = 15;
                                int numLocations = 30;
                                for (int i = 0; i < 30; i++)
                                {
                                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                                    Vector2 dustvelocity = new Vector2(0f, 10f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                                    int dust = Dust.NewDust(position, 2, 2, 206, dustvelocity.X, dustvelocity.Y, 0, default, 2);
                                    Main.dust[dust].noGravity = true;
                                }
                            }
                        }

                        if (npc.ai[1] > 90)
                        {
                            npc.position = new Vector2(player.Center.X + -300 * Direction - 51, player.Center.Y -51);
                        }

                        if (++npc.ai[1] % 120 == 0 && npc.ai[1] < 360)
                        {
                            for (int i = -1; i < 1; i++)
                            {
                                NPC.NewNPC((int)(npc.Center.X + 150 + (300 * i)), (int)npc.Center.Y, NPCType<GraniteMinibossMinion>(), 0, 0, 0, changedPhase2 ? 1 : 0);
                            }
                            int count = 0;
                            for (int k = 0; k < 200; k++)
                            {
                                if (Main.npc[k].active && Main.npc[k].type == mod.NPCType("GraniteMinibossMinion"))
                                {
                                    if (count < 4)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        ((GraniteMinibossMinion)Main.npc[k].modNPC).kill = true;
                                    }
                                }
                            }
                        }

                        if (npc.ai[1] == 420)
                        {
                            npc.ai[0] = -1;
                            npc.ai[1] = 0;
                            npc.ai[2] = 0;
                            attackcounter = 0;
                            npc.velocity = Vector2.Zero;
                            teleportposition = Vector2.Zero;
                        }
                        break;
                    }
            }
            
            npc.spriteDirection = npc.direction;

            spritetimer++;
            if (spritetimer > 4)
            {
                frame++;
                spritetimer = 0;
            }
            if (frame > 7)
            {
                frame = 0;
            }
        }

        private bool AliveCheck(Player player)
        {
            if (!player.active || player.dead)
            {
                npc.TargetClosest();
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    if (npc.timeLeft > 25)
                    {
                        npc.timeLeft = 25;
                        npc.velocity = Vector2.UnitY * -7;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = mod.GetTexture("NPCs/Bosses/GraniteMini/AngryStone_Glow");
            spriteBatch.Draw(texture, new Vector2(npc.Center.X - Main.screenPosition.X - 1, npc.Center.Y - Main.screenPosition.Y), npc.frame, Color.White, npc.rotation, npc.frame.Size() / 2f, npc.scale, npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = frameHeight * frame;
            if (Main.player[npc.target].Center.X < npc.Center.X && dashing == false)
            {
                npc.spriteDirection = -1;
            }
            while (dashing == true && npc.spriteDirection != spritedirectionstore)
            {
                npc.spriteDirection = spritedirectionstore;
            }
        }
    }
}