using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Weapons.PreHardmode.Melee;
using OvermorrowMod.Content.Items.Weapons.PreHardmode.Ranged;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Projectiles.Accessory;
using OvermorrowMod.Projectiles.Hexes;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool FungiInfection;
        public int FungiTime;

        public override void ResetEffects(NPC npc)
        {
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
                case NPCID.AngryBones:
                case NPCID.AngryBonesBig: 
                case NPCID.AngryBonesBigHelmet:
                case NPCID.AngryBonesBigMuscle:
                case NPCID.DarkCaster:
                case NPCID.CursedSkull:
                    if (Main.rand.NextBool(100)) 
                    {
                        // Put a thing here idk
                    }
                    break;
                case NPCID.CaveBat:
                    if (Main.rand.NextBool(20))
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
                    if (Main.rand.NextBool(3))
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<MutatedFlesh>(), Main.rand.Next(1, 3));
                    }
                    break;
                case NPCID.Piranha:
                    if (Main.rand.NextBool(50))
                    {
                        Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Blackfish>());
                    }
                    break;
            }

            if (npc.type == NPCID.Zombie || npc.type == NPCID.BigRainZombie || npc.type == NPCID.BigSlimedZombie ||npc.type == NPCID.SlimedZombie || npc.type == NPCID.SmallSkeleton || npc.type == NPCID.BigSkeleton || npc.type == NPCID.SmallHeadacheSkeleton || npc.type == NPCID.BigHeadacheSkeleton || npc.type == NPCID.SmallMisassembledSkeleton || npc.type == NPCID.BigMisassembledSkeleton || npc.type == NPCID.SmallPantlessSkeleton || npc.type == NPCID.BigPantlessSkeleton || npc.type == NPCID.GoblinArcher || npc.type == NPCID.Skeleton || npc.type == NPCID.GoblinPeon || npc.type == NPCID.GoblinSorcerer || npc.type == NPCID.GoblinThief || npc.type == NPCID.GoblinWarrior || npc.type == NPCID.GoblinSummoner || npc.type == NPCID.ZombieEskimo || npc.type == NPCID.ZombieRaincoat)
            {
                if (Main.rand.NextFloat() < .01f) //Should be 1% drop chance...not sure
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<StaleBread>());
                }
            }

            if (npc.type == NPCID.MotherSlime)
            {
                if (Main.rand.NextFloat() < .07f)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Hammer>());
                }
            }
             
             
            if (npc.type == NPCID.BoneSerpentHead)
            {
                if (Main.rand.NextBool(50)) //2% percent drop 
                {
                     Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SearingSaber>());
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
                            var modPlayer = player.GetModPlayer<OvermorrowModPlayer>();

                            if (modPlayer.DripplerEye)
                            {
                                if (modPlayer.dripplerStack < 25)
                                {
                                    modPlayer.dripplerStack++;
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
                            var modPlayer = player.GetModPlayer<OvermorrowModPlayer>();

                            if (modPlayer.DripplerEye)
                            {
                                if (modPlayer.dripplerStack < 25)
                                {
                                    modPlayer.dripplerStack++;
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

            if (npc.HasHex(Hex.HexType<CursedFlames>()))
            {
                if (Main.rand.NextBool(10))
                {
                    if (Main.netMode != NetmodeID.Server && Main.myPlayer == projectile.owner)
                    {
                        Projectile.NewProjectile(npc.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<CursedBall>(), 24, 2f, owner.whoAmI);
                    }
                }
            }

            if (npc.HasHex(Hex.HexType<LesserIchor>()))
            {
                if (Main.rand.NextBool(8))
                {
                    if (Main.netMode != NetmodeID.Server && Main.myPlayer == projectile.owner)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Projectile.NewProjectile(npc.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<IchorStream>(), 12, 2f, owner.whoAmI);
                        }
                    }
                }
            }

        }

        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            if (target.GetModPlayer<OvermorrowModPlayer>().mirrorBuff && !target.immune)
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
            if (npc.HasHex(Hex.HexType<LesserIchor>()))
            {
                npc.defense -= 8;
            }

            if (npc.HasHex(Hex.HexType<SoulFlame>()))
            {
                npc.defense -= 5;
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