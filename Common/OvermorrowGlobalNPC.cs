using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Biomes;
using OvermorrowMod.Content.Buffs.Debuffs;
using OvermorrowMod.Content.Buffs.Hexes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Materials;
using OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns;
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

        public bool BearTrapped;
        public bool LightningMarked;

        public int FungiTime;

        public override void ResetEffects(NPC npc)
        {
            //BearTrapped = false;
            LightningMarked = false;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<PhoenixMarkBuff>()) modifiers.SourceDamage *= 1.25f;
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<PhoenixMarkBuff>()) modifiers.SourceDamage *= 1.2f;
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {   

        }

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;
            if (player.InModBiome(ModContent.GetInstance<WaterCaveBiome>()))
            {
                pool.Clear();
                //pool.Add(ModContent.NPCType<CaveFish>(), .10f);
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
            if (npc.HasBuff<FungalInfection>())
            {
                npc.defense -= 2 * npc.buffType.Length;
            }

            if (npc.HasHex(Hex.HexType<LesserIchor>()))
            {
                npc.defense -= 8;
            }

            if (npc.defense < 0) npc.defense = 0;
        }

        public override bool PreAI(NPC npc)
        {
            if (BearTrapped)
            {
                npc.position.X = npc.oldPosition.X;
                npc.velocity.X = 0;
                npc.frameCounter = 0;
                return false;
            }

            return base.PreAI(npc);
        }

        public override void AI(NPC npc)
        {
            if (npc.HasBuff<FungalInfection>())
            {
                FungiTime++;
                if (FungiTime % 24 == 0)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[npc.target].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 0f;
                    Vector2 perturbedSpeed = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(360f));
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + Main.rand.Next(-75, 76) - npc.width / 2, npc.Center.Y + Main.rand.Next(-75, 76) - npc.height / 2, perturbedSpeed.X * speed, perturbedSpeed.Y * speed, ProjectileID.TruffleSpore, npc.defense + 5, 0f, Main.myPlayer, npc.whoAmI, Main.myPlayer);
                }
            }
        }

        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.HasBuff<FungalInfection>())
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
