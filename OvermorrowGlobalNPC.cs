using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Projectiles.Accessory;
using OvermorrowMod.WardenClass.Accessories;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
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
        public bool bleedingDebuff2;

        public override void ResetEffects(NPC npc)
        {
            bleedingDebuff = false;
            bleedingDebuff2 = false;
        }

        public override void NPCLoot(NPC npc)
        {
            if (npc.type == NPCID.AngryBones || npc.type == NPCID.AngryBonesBig || npc.type == NPCID.AngryBonesBigHelmet || npc.type == NPCID.AngryBonesBigMuscle ||
                npc.type == NPCID.DarkCaster || npc.type == NPCID.CursedSkull)
            {
                int dropChance = Main.rand.Next(100);
                if (dropChance == 0) // 1% drop chance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SoulFragment>());
                }
            }

            if (npc.type == NPCID.FungiBulb)
            {
                int dropChance = Main.rand.Next(2);
                if (dropChance == 0) // 8.33% drop chance
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<FungiPiercer>());
                }
            }

            if (npc.type == NPCID.Harpy)
            {
                int dropChance = Main.rand.Next(10);
                if (dropChance == 0) // 10% drop chance
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

                int dropChance2 = Main.rand.Next(2); // 50% drop chance
                if (dropChance2 == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrippingFlesh>(), Main.rand.Next(1, 3));
                }
            }

            if (Main.netMode == NetmodeID.Server)
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

                            if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2 && modPlayer.ReaperBook)
                            {
                                if (Main.rand.Next(8) == 0) // 12.5% chance to gain Soul Essence on death
                                { 
                                    if (!XPPacket.Write(1, npc.target))
                                    {
                                        player.GetModPlayer<WardenDamagePlayer>().AddSoul(1);
                                    }
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
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                if (npc.type != NPCID.WaterSphere && npc.type != NPCID.ChaosBall && npc.type != NPCID.BurningSphere && npc.type != NPCID.SolarFlare && npc.type != NPCID.VileSpit)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (npc.playerInteraction[i])
                        {
                            Player player = Main.LocalPlayer;
                            var modPlayer = WardenDamagePlayer.ModPlayer(player);
                            var modPlayer2 = player.GetModPlayer<OvermorrowModPlayer>();

                            if (modPlayer.soulResourceCurrent < modPlayer.soulResourceMax2 && modPlayer.ReaperBook) // Warden Reaper Book
                            {
                                if (Main.rand.Next(8) == 0) // 12.5% chance to gain Soul Essence on death
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
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            Player owner = Main.player[projectile.owner];
            if (owner.GetModPlayer<OvermorrowModPlayer>().ArmBracer && (projectile.minion == true || projectile.magic == true))
            {
                if (Main.rand.Next(6) == 0 && owner.GetModPlayer<OvermorrowModPlayer>().sandCount < 10)
                {
                    Projectile.NewProjectile(owner.Center, Vector2.Zero, ModContent.ProjectileType<SandBallFriendly>(), 24, 2f, projectile.owner, Main.rand.Next(60, 95), Main.rand.Next(3, 6));
                    owner.GetModPlayer<OvermorrowModPlayer>().sandCount++;
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
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (bleedingDebuff || bleedingDebuff2)
            {
                // These lines zero out any positive lifeRegen. This is expected for all bad life regeneration effects.
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                if (bleedingDebuff2) // This is only applied in tandem with the bleedingDebuff anyways
                {
                    // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 4 + 4 life lost per second.
                    npc.lifeRegen -= 16;
                }
                else
                {
                    // lifeRegen is measured in 1/2 life per second. Therefore, this effect causes 4 life lost per second.
                    npc.lifeRegen -= 8;
                    // The damage visual value
                }

                damage = 1;
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (bleedingDebuff || bleedingDebuff2)
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

        // New method to apply buffs to NPCs, this is WIP
        public void AddNewBuff(NPC npc, int type, int time)
        {
            // Check to make sure that the NPC is not immune to the applied debuff
            if (npc.buffImmune[type])
            {
                return;
            }

            // The netcode to send for updating NPC buff
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.AddNPCBuff, -1, -1, null, npc.whoAmI, type, time, 0f, 0, 0, 0);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SendNPCBuffs, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
            }

            // This handles reapplying an existing buff on an NPC
            // Loop through the NPC's buffs
            for (int i = 0; i < npc.buffType.Length; i++)
            {
                // Check if the buff matches the type being applied
                if (npc.buffType[i] == type)
                {
                    // Reapply the debuff
                    if (!BuffLoader.ReApply(type, npc, time, i) && npc.buffTime[i] < time)
                    {
                        npc.buffTime[i] = time;
                    }
                    return;
                }
            }

            // While loop flag
            int num3 = -1;
            while (num3 == -1)
            {
                // Default break-out int
                int num2 = -1;

                // Loop through the NPC's buffs
                for (int i = 0; i < npc.buffType.Length; i++)
                {
                    // Check if the buff is NOT a debuff
                    if (!Main.debuff[npc.buffType[i]])
                    {
                        // Set the default break-out int to be the index
                        // This prevents the while loop from exiting
                        num2 = i;

                        // Break out of the for loop
                        break;
                    }
                }

                // Catch the default break-out int
                if (num2 == -1)
                {
                    // Exit from the while loop
                    return;
                }

                // Loop through the NPC's buffs, using the int obtained from the not debuff checker
                for (int i = num2; i < npc.buffType.Length; i++)
                {
                    // Break out of the while loop if the buff index is 0
                    if (npc.buffType[i] == 0)
                    {
                        // Set the loop value to the index, thus breaking out of the loop by end of loop
                        // This is because the value is no longer -1
                        num3 = i;
                        break;
                    }
                }

                // If the looping int is still valid, remove the buff passed by the buff check
                if(num3 == -1)
                {
                    npc.DelBuff(num2);
                }
            }

            // Apply the buff to the NPC
            npc.buffType[num3] = type;
            npc.buffTime[num3] = time;
        }
    }
}