using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns;
using OvermorrowMod.Content.NPCs.CaveFish;
using OvermorrowMod.Content.NPCs.SalamanderHunter;
using OvermorrowMod.Content.NPCs.SnapDragon;
using OvermorrowMod.Content.Projectiles.Hexes;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool FungiInfection;
        public bool LightningMarked;

        public int FungiTime;

        public override void ResetEffects(NPC npc)
        {
            FungiInfection = false;
            LightningMarked = false;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (npc.HasBuff<PhoenixMarkBuff>()) damage += (int)(damage * 0.25f);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.HasBuff<PhoenixMarkBuff>()) damage += (int)(damage * 0.2f);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            Player owner = Main.player[projectile.owner];

            if (npc.HasHex(Hex.HexType<CursedFlames>()))
            {
                if (Main.rand.NextBool(10))
                {
                    if (Main.netMode != NetmodeID.Server && Main.myPlayer == projectile.owner)
                    {
                        Projectile.NewProjectile(projectile.GetSource_OnHit(npc), npc.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<CursedBall>(), 24, 2f, owner.whoAmI);
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
                            Projectile.NewProjectile(projectile.GetSource_OnHit(npc), npc.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 4, ModContent.ProjectileType<IchorStream>(), 12, 2f, owner.whoAmI);
                        }
                    }
                }
            }

        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (player.InModBiome(ModContent.GetInstance<WaterCaveBiome>()))
            {
                pool.Clear();
                pool.Add(ModContent.NPCType<CaveFish>(), .10f);
                pool.Add(ModContent.NPCType<SnapDragon>(), .15f);
                pool.Add(ModContent.NPCType<SalamanderHunter>(), .25f);
            }
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.InModBiome(ModContent.GetInstance<WaterCaveBiome>()))
            {
                spawnRate = 140;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
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
                    npcLoot.Add(ItemDropRule.Common(ItemID.ChainKnife, 20));
                    break;
                case NPCID.BoneSerpentHead:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SerpentTooth>(), 20));
                    break;
                case NPCID.Harpy:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<HarpyLeg>(), 10));
                    break;
                case NPCID.Drippler:
                case NPCID.BloodZombie:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MutatedFlesh>(), 3, 1, 3));
                    break;
                case NPCID.Zombie:
                case NPCID.BigRainZombie:
                case NPCID.BigSlimedZombie:
                case NPCID.SlimedZombie:
                case NPCID.SmallSkeleton:
                case NPCID.BigSkeleton:
                case NPCID.SmallHeadacheSkeleton:
                case NPCID.BigHeadacheSkeleton:
                case NPCID.SmallMisassembledSkeleton:
                case NPCID.BigMisassembledSkeleton:
                case NPCID.SmallPantlessSkeleton:
                case NPCID.BigPantlessSkeleton:
                case NPCID.GoblinArcher:
                case NPCID.Skeleton:
                case NPCID.GoblinPeon:
                case NPCID.GoblinSorcerer:
                case NPCID.GoblinThief:
                case NPCID.GoblinWarrior:
                case NPCID.GoblinSummoner:
                case NPCID.ZombieEskimo:
                case NPCID.ZombieRaincoat:
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<StaleBread>(), 100));
                    break;
            }

            base.ModifyNPCLoot(npc, npcLoot);
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
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + Main.rand.Next(-75, 76) - npc.width / 2, npc.Center.Y + Main.rand.Next(-75, 76) - npc.height / 2, perturbedSpeed.X * speed, perturbedSpeed.Y * speed, ProjectileID.TruffleSpore, npc.defense + 5, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
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
