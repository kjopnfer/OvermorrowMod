using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using OvermorrowMod.Buffs.Hexes;
using OvermorrowMod.Items.Accessories;
using OvermorrowMod.Items.Materials;
using OvermorrowMod.Items.Weapons.PreHardmode.Magic;
using OvermorrowMod.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.NPCs;
using OvermorrowMod.Projectiles.Accessory;
using OvermorrowMod.Projectiles.Hexes;
using OvermorrowMod.Projectiles.Melee;
using OvermorrowMod.WardenClass.Accessories;
using OvermorrowMod.WardenClass.Weapons.Artifacts;
using OvermorrowMod.WardenClass.Weapons.ChainWeapons;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod
{
    public class OvermorrowGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool Homingdie;
        public bool bleedingDebuff;
        public bool bleedingDebuff2;
        public bool FungiInfection;
        public int FungiTime;
        public int split;

        public override void ResetEffects(NPC npc)
        {
            bleedingDebuff = false;
            bleedingDebuff2 = false;
            FungiInfection = false;
        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.player;
            if (player.GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave)
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<CaveFish>(), .10f);
                pool.Add(ModContent.NPCType<SnapDragon>(), .15f);
                pool.Add(ModContent.NPCType<SalamanderHunter>(), .25f);
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave)
            {
                spawnRate = 140;
            }
        }

        public override void NPCLoot(NPC npc)
        {    
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    if (Main.rand.NextBool(5) && !Main.expertMode)
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SlimeArtifact>());
                    }
                    break;
                case NPCID.AngryBones:
                case NPCID.AngryBonesBig: 
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.DarkCaster:
                case NPCID.CursedSkull:
                    if (Main.rand.NextBool(100)) // 1% drop chance
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SoulFragment>());
                    }
                    break;
                case NPCID.CaveBat:
                    if (Main.rand.NextBool(20)) // 5% drop chance
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.ChainKnife);
                    }
                    break;
                case NPCID.BoneSerpentHead:
                    if (Main.rand.NextBool(20))
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SerpentTooth>());
                    }
                    break;
                case NPCID.EaterofSouls:
                    if (Main.rand.NextBool(75))
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<EatersBlade>());
                    }
                    break;
                case NPCID.Harpy:
                    if (Main.rand.NextBool(10)) // 10% drop chance
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<HarpyLeg>());
                    }
                    break;
                case NPCID.Drippler:
                case NPCID.BloodZombie:
                    if (Main.rand.NextBool(30)) // 3.33% drop chance
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<AncientCrystal>());
                    }

                    if (Main.rand.NextBool(3))
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<DrippingFlesh>(), Main.rand.Next(1, 3));
                    }
                    break;
                case NPCID.FungiBulb:
                    if (Main.rand.NextBool(6)) // 8.33% drop chance
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<FungiPiercer>());
                    }
                    break;
            }


            if (npc.type == NPCID.CaveBat || npc.type == NPCID.SmallSkeleton || npc.type == NPCID.BigSkeleton || npc.type == NPCID.SmallHeadacheSkeleton || npc.type == NPCID.BigHeadacheSkeleton || npc.type == NPCID.SmallMisassembledSkeleton || npc.type == NPCID.BigMisassembledSkeleton || npc.type == NPCID.SmallPantlessSkeleton || npc.type == NPCID.BigPantlessSkeleton || npc.type == NPCID.MotherSlime || npc.type == NPCID.Skeleton || npc.type == NPCID.GiantWormHead || npc.type == NPCID.RedSlime || npc.type == NPCID.UndeadMiner || npc.type == NPCID.Harpy || npc.type == NPCID.ManEater || npc.type == NPCID.SnowFlinx || npc.type == NPCID.SpikedIceSlime || npc.type == NPCID.WalkingAntlion || npc.type == NPCID.FlyingAntlion || npc.type == NPCID.GreekSkeleton || npc.type == NPCID.GraniteGolem || npc.type == NPCID.GraniteFlyer || npc.type == NPCID.Salamander)
            {
                if (Main.rand.Next(200) < 4)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MegaBuster>());
                }
            }


            if (Homingdie)
            {
                Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0, 0, ModContent.ProjectileType<HeroHoming>(), npc.damage * 3, 2, Main.player[npc.target].whoAmI);
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

            if (projectile.magic && owner.GetModPlayer<OvermorrowModPlayer>().MarbleTrail)
            {
                if (Main.rand.Next(20) == 0)
                {
                    owner.AddBuff(ModContent.BuffType<WindBuff>(), 600);
                }
            }

            if (npc.HasHex(Hex.HexType<CursedFlames>()))
            {
                if (Main.rand.NextBool(10))
                {
                    Projectile.NewProjectile(npc.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<CursedBall>(), 24, 2f, owner.whoAmI);
                }
            }

            if (npc.HasHex(Hex.HexType<LesserIchor>()))
            {
                if (Main.rand.NextBool(8))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(npc.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<IchorStream>(), 12, 2f, owner.whoAmI);
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

            if (npc.HasHex(Hex.HexType<LesserIchor>()))
            {
                npc.defense -= 8;
            }
        }

        public override void AI(NPC npc)
        {
            if (FungiInfection)
            {
                FungiTime++;
                if (FungiTime > 24)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    Vector2 perturbedSpeed = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(360f));
                    Projectile.NewProjectile(npc.Center.X + Main.rand.Next(-75, 76) - npc.width / 2, npc.Center.Y + Main.rand.Next(-75, 76) - npc.height / 2, perturbedSpeed.X * speed, perturbedSpeed.Y * speed, ProjectileID.TruffleSpore, npc.defense + 5, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                    FungiTime = 0;
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (bleedingDebuff || bleedingDebuff2)
            {
                if (Main.rand.Next(4) < 3)
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustID.Blood, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default(Color), 1f);
                    //Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }
            }

            if (FungiInfection)
            {
                if (Main.rand.NextBool(10))
                {
                    int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width, npc.height, DustID.GlowingMushroom, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 0, default(Color), 1f);
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                }
            }

            if (npc.HasHex(Hex.HexType<CursedFlames>()))
            {
                drawColor = Color.LimeGreen;
            }

            if (npc.HasHex(Hex.HexType<LesserIchor>()))
            {
                drawColor = Color.Yellow;
            }
        }
    }
}