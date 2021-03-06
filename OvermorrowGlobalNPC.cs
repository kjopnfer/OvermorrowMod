using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.WardenClass.Accessories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod
{
    public class OvermorrowGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool bleedingDebuff;

        public override void ResetEffects(NPC npc)
        {
            bleedingDebuff = false;
        }

        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.Harpy)
            {
                int dropChance = Main.rand.Next(10);
                if(dropChance == 0) // 10% drop chance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<HarpyLeg>());
                }
            }

            if (npc.type == NPCID.Drippler || npc.type == NPCID.BloodZombie)
            {
                int dropChance = Main.rand.Next(30);
                if (dropChance == 0) // 3.33% drop chance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AncientCrystal>());
                }

                int dropChance2 = Main.rand.Next(5); // 20% drop chance
                if(dropChance2 == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrippingFlesh>());
                }
            }

            if(Main.netMode == NetmodeID.Server)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (npc.playerInteraction[i])
                        {
                            Player player = Main.player[i];
                            var modPlayer = WardenDamagePlayer.ModPlayer(player);
                            var modPlayer2 = player.GetModPlayer<OvermorrowModPlayer>();

                            if (modPlayer.ReaperBook)
                            {
                                if (!XPPacket.Write(1, npc.target))
                                {
                                    player.GetModPlayer<WardenDamagePlayer>().AddSoul(1);
                                }
                            }

                            if (modPlayer2.DripplerEye)
                            {
                                if (modPlayer2.dripplerStack < 25)
                                {
                                    modPlayer2.dripplerStack++;
                                }
                            }
                        }
                    }
                }
            }else if(Main.netMode == NetmodeID.SinglePlayer)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    Player player = Main.LocalPlayer;
                    var modPlayer = WardenDamagePlayer.ModPlayer(player);
                    var modPlayer2 = player.GetModPlayer<OvermorrowModPlayer>();

                    if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2 && modPlayer.ReaperBook) // Warden Reaper Book
                    {
                        if (Main.rand.Next(4) == 0) // 25% chance to gain Soul Essence on death
                        {
                            modPlayer.soulResourceCurrent += 1;
                            modPlayer.soulList.Add(Projectile.NewProjectile(npc.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, player.whoAmI, Main.rand.Next(70, 95), 0f));
                            CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.DarkCyan, "Soul Essence Gained", true, false);
                        }
                    }

                    if (modPlayer2.DripplerEye)
                    {
                        if (modPlayer2.dripplerStack < 25)
                        {
                            modPlayer2.dripplerStack++;
                        }
                    }
                }
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (target.GetModPlayer<OvermorrowModPlayer>().mirrorBuff && !npc.dontTakeDamage)
            {
                if (damage < npc.life)
                {
                    npc.life -= damage;
                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.Purple, damage, false, false);
                }
                else
                {
                    npc.life = 1;
                }
            }
            base.OnHitPlayer(npc, target, damage, crit);
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (bleedingDebuff)
            {
                // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects.
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 4 life lost per second.
                npc.lifeRegen -= 8;

                // The damage visual value
                damage = 1;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (bleedingDebuff)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, 5, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default(Color), 1f);
                    //Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }
            }
        }

        /*public override void HitEffect(NPC npc, int hitDirection, double damage)
        {
            // There might be server shenanigans we'll see
            if (npc.life <= 0)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (npc.playerInteraction[i])
                        {
                            Player player = Main.player[i];
                            var modPlayer = WardenDamagePlayer.ModPlayer(player);
                            if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2 && modPlayer.ReaperBook)
                            {
                                if (Main.rand.Next(4) == 0) // 25% chance to gain Soul Essence on death
                                {
                                    modPlayer.soulResourceCurrent += 1;
                                    modPlayer.soulList.Add(Projectile.NewProjectile(npc.position, new Vector2(0, 0), mod.ProjectileType("SoulEssence"), 0, 0f, player.whoAmI, Main.rand.Next(70, 95), 0f));
                                    CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.DarkCyan, "Soul Essence Gained", true, false);
                                }
                            }
                        }
                    }
                }
            }
            /*if (Main.netMode == 2)
            {
                if (npc.life <= 0)
                {
                    if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                    {
                        for (int i = 0; i < Main.maxPlayers; i++)
                        {
                            if (npc.playerInteraction[i])
                            {
                                Player player = Main.player[i];
                                var modPlayer = WardenDamagePlayer.ModPlayer(player);
                                if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2)
                                {
                                    modPlayer.soulResourceCurrent += 1;
                                }
                                CombatText.NewText(new Rectangle((int)npc.position.X, (int)npc.position.Y + 50, npc.width, npc.height), Color.DarkCyan, "1 Soul Essence", true, false);
                            }
                        }
                    }
                }
            }
        }*/
    }
}