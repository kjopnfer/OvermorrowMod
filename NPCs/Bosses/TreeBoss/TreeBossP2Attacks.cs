using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.BossBags;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Placeable.Boss;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Items.Weapons.PreHardmode.Summoner;
using OvermorrowMod.Projectiles.Boss;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public partial class TreeBossP2 : ModNPC
    {
        private void Death()
        {
            // Death animation code
            if (npc.ai[3] > 0f)
            {
                npc.velocity = Vector2.Zero;

                if (npc.ai[2] > 0)
                {
                    npc.ai[2]--;

                    if (npc.ai[2] == 480)
                    {
                        BossText("I deem thee fit to inherit their powers.");
                    }

                    if (npc.ai[2] == 300)
                    {
                        BossText("Thou Dryad shalt guide thee.");
                    }

                    if (npc.ai[2] == 120)
                    {
                        BossText("Fare thee well.");
                    }
                }
                else
                {
                    npc.dontTakeDamage = true;
                    npc.ai[3]++; // Death timer
                    npc.velocity.X *= 0.95f;

                    if (npc.velocity.Y < 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.01f;
                    }

                    if (npc.velocity.X > 0.5f)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.01f;
                    }

                    if (npc.ai[3] > 120f)
                    {
                        npc.Opacity = 1f - (npc.ai[3] - 120f) / 60f;
                    }

                    if (Main.rand.NextBool(5) && npc.ai[3] < 120f)
                    {
                        // This dust spawn adapted from the Pillar death code in vanilla.
                        for (int dustNumber = 0; dustNumber < 6; dustNumber++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(npc.Left, npc.width, npc.height / 2, DustID.TerraBlade, 0f, 0f, 0, default(Color), 1f)];
                            dust.position = npc.Center + Vector2.UnitY.RotatedByRandom(4.1887903213500977) * new Vector2(npc.width * 1.5f, npc.height * 1.1f) * 0.8f * (0.8f + Main.rand.NextFloat() * 0.2f);
                            dust.velocity.X = 0f;
                            dust.velocity.Y = -Math.Abs(dust.velocity.Y - (float)dustNumber + npc.velocity.Y - 4f) * 3f;
                            dust.noGravity = true;
                            dust.fadeIn = 1f;
                            dust.scale = 1f + Main.rand.NextFloat() + (float)dustNumber * 0.3f;
                        }
                    }

                    if (npc.ai[3] % 30f == 1f)
                    {
                        //Main.PlaySound(4, npc.Center, 22);
                        Main.PlaySound(SoundID.Item25, npc.Center); // every half second while dying, play a sound
                    }

                    if (npc.ai[3] >= 180f)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 0);
                        npc.checkDead(); // This will trigger ModNPC.CheckDead the second time, causing the real death.
                    }
                }
                return;
            }
        }

        /*switch (npc.ai[0])
           {
               case -1: // case switching
                   {
                       if (movement == true)
                       {
                           if (changedPhase2 == true) { RandomCeiling = 4; }
                           else { RandomCeiling = 3; }
                           while (RandomCase == LastCase)
                           {
                               RandomCase = Main.rand.Next(1, RandomCeiling);
                           }
                           LastCase = RandomCase;
                           movement = false;
                           npc.ai[0] = RandomCase;
                       }
                       else
                       {
                           movement = true;
                           npc.ai[0] = 0;
                       }
                   }
                   break;
               case 0: // Follow player
                   if (npc.ai[0] == 0)
                   {
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

                       if (npc.ai[1] > (changedPhase2 ? 90 : 120))
                       {
                           npc.ai[0] = -1;
                           npc.ai[1] = 0;
                       }
                   }
                   break;
               case 1: // Shoot scythes
                   if (npc.ai[0] == 1)
                   {
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

                       if (npc.ai[1] % 90 == 0)
                       {
                           int shootSpeed = Main.rand.Next(8, 12);
                           Vector2 position = npc.Center;
                           Vector2 targetPosition = Main.player[npc.target].Center;
                           Vector2 direction = targetPosition - position;
                           direction.Normalize();
                           if (Main.netMode != NetmodeID.MultiplayerClient)
                           {
                               Projectile.NewProjectile(npc.Center, direction * shootSpeed, ModContent.ProjectileType<NatureScythe>(), npc.damage / 2, 3f, Main.myPlayer, 0, 0);
                           }
                       }

                       if (npc.ai[1] > 600)
                       {
                           npc.ai[0] = -1;
                           npc.ai[1] = 0;
                       }
                   }
                   break;
               case 2: // Absorb energy
                   npc.velocity = Vector2.Zero;

                   // Summon projectiles from off-screen that move towards the boss
                   if (npc.ai[1] % 20 == 0 && (energiesAbsorbed + energiesKilled) < 33 && npc.ai[1] <= 660)
                   {
                       for (int i = 0; i < 6; i++)
                       {
                           float randPositionX = npc.Center.X + Main.rand.Next(-10, 10) * 600;
                           float randPositionY = npc.Center.Y + Main.rand.Next(-10, 10) * 600;
                           npc.netUpdate = true;
                           if (Main.netMode != NetmodeID.MultiplayerClient)
                           {
                               NPC.NewNPC((int)randPositionX, (int)randPositionY, ModContent.NPCType<AbsorbEnergy>(), 0, 0f, npc.whoAmI, 0, npc.damage / 3, Main.myPlayer);
                           }
                       }
                   }

                   if (energiesKilled <= 5 && npc.ai[1] > 660) // punish
                   {
                       npc.ai[2] = 1;
                       Main.NewText("u suk");
                   }
                   else if (npc.ai[1] > 660) // else
                   {
                       npc.ai[2] = 1;
                   }

                   if (npc.ai[1] > 660 && npc.ai[3] == 1)
                   {
                       energiesAbsorbed = 0;
                       energiesKilled = 0;
                       npc.ai[0] = 4;
                       npc.ai[1] = 0;
                       npc.ai[2] = 0;
                   }
                   break;
               case 4: // Shoot nature blasts
                   npc.velocity = Vector2.Zero;

                   if (npc.ai[1] == 120)
                   {
                       int projectiles = Main.rand.Next((changedPhase2 ? 13 : 9), (changedPhase2 ? 18 : 13));
                       npc.netUpdate = true;

                       for (int i = 0; i < projectiles; i++)
                       {
                           if (Main.netMode != NetmodeID.MultiplayerClient)
                           {
                               Projectile.NewProjectile(npc.Center, new Vector2(7).RotatedBy(MathHelper.ToRadians((360 / projectiles) * i + i)), ModContent.ProjectileType<NatureBlast>(), 19, 2, Main.myPlayer);
                           }
                       }
                   }

                   if (npc.ai[1] > 240)
                   {
                       npc.ai[0] = -1;
                       npc.ai[1] = 0;
                   }
                   break;
               case 3: // scythes
                   {
                       Vector2 moveTo = player.Center;
                       moveTo.X += 50 * (npc.Center.X < moveTo.X ? -1 : 1);
                       var move = moveTo - npc.Center;
                       var speed = 1;

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


                       if (npc.ai[1] == 180)
                       {
                           for (int i = -2; i < 3; i++)
                           {
                               if (Main.netMode != NetmodeID.MultiplayerClient)
                               {
                                   Projectile.NewProjectile(new Vector2(player.Center.X + (250 * i), player.Center.Y - 200), Vector2.UnitY * 5, ModContent.ProjectileType<NatureScythe>(), 17, 1f, Main.myPlayer, 0, 1);
                                   Projectile.NewProjectile(new Vector2(player.Center.X + (250 * i), player.Center.Y + 200), -Vector2.UnitY * 5, ModContent.ProjectileType<NatureScythe>(), 17, 1f, Main.myPlayer, 0, 1);
                               }
                           }
                       }
                       if (npc.ai[1] > 540)
                       {
                           npc.ai[0] = 0;
                           npc.ai[1] = 0;
                       }
                       break;
                   }
           }*/
    }
}
