using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Items.Archives.Accessories;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    // TODO: This needs to go away
    public class OldAccessoryPlayer : ModPlayer
    {
        /// <summary>
        /// Used for calculating whether the player is in the NPC's aggro range.
        /// Higher values reduce the NPC's aggro range while increasing their alert threshold.
        /// </summary>
        public int AlertBonus = 0;

        /// <summary>
        /// Used for calculating how quickly the enemy loses aggro if the Player is their target.
        /// </summary>
        public float AggroLossBonus = 0;

        public bool ArtOfBallistics = false;
        public bool BlackPage = false;
        public bool CandlelitSanctuary = false;
        public bool WarriorsEpic = false;
        public bool WhitePage = false;

        public int WarriorsEpicCooldown { get; private set; } = 0;
        public bool WarriorsResolveTriggered { get; private set; } = false;

        public int CandleCharges { get; private set; } = 0;
        private int CandleCounter = 0;
        public override void ResetEffects()
        {
            AlertBonus = 0;
            AggroLossBonus = 0;

            if (!Player.HasBuff<WarriorsResolve>())
                WarriorsResolveTriggered = false;

            ArtOfBallistics = false;
            BlackPage = false;
            CandlelitSanctuary = false;
            WarriorsEpic = false;
            WhitePage = false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.IsWeaponType(WeaponType.Rapier))
            {
                // Add armor penetration equal to twice the projectile's damage
                modifiers.ArmorPenetration += proj.damage * 2;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0)
            {
                Content.Items.Accessories.WarriorsEpic.DrawEffects(Player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            }
        }

        public override void UpdateEquips()
        {
            Content.Items.Accessories.WarriorsEpic.WarriorsEpicEffect(Player);

            if (Player.HasBuff<WarriorsResolve>() && !WarriorsResolveTriggered)
            {
                Player.GetCritChance(DamageClass.Melee) += 100; // 100% crit chance bonus
            }
        }

        public override void PostUpdateBuffs()
        {

        }

        public override void PostUpdate()
        {
            DeathsDoorEffects();

            if (WarriorsEpicCooldown > 0) WarriorsEpicCooldown--;

            if (CandlelitSanctuary && CandleCharges < 3)
            {
                CandleCounter++;
                if (CandleCounter % ModUtils.SecondsToTicks(15) == 0)
                {
                    CandleCharges++;
                    CandleCounter = 0;

                    Projectile.NewProjectile(null, Player.Center, Vector2.Zero, ModContent.ProjectileType<CandleGain>(), 0, 0, Player.whoAmI);
                }
            }
        }


        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            CandleCounter = 0;
            if (CandleCharges > 0)
            {
                var damageReduction = 15 * CandleCharges;
                hurtInfo.Damage -= damageReduction;
                CandleCharges = 0;

                var item = ItemLoader.GetItem(ModContent.ItemType<CandlelitSanctuary>()).Item;
                Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(item, hurtInfo.DamageSource), Player.Center, Vector2.Zero, ModContent.ProjectileType<CandleBurst>(), damageReduction, 6f, Player.whoAmI);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            CandleCounter = 0;
            if (CandleCharges > 0)
            {
                var damageReduction = 15 * CandleCharges;
                hurtInfo.Damage -= damageReduction;
                CandleCharges = 0;

                var item = ItemLoader.GetItem(ModContent.ItemType<CandlelitSanctuary>()).Item;
                Projectile.NewProjectile(Player.GetSource_Accessory_OnHurt(item, hurtInfo.DamageSource), Player.Center, Vector2.Zero, ModContent.ProjectileType<CandleBurst>(), damageReduction, 6f, Player.whoAmI);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            OnKillEffects(target);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            OnKillEffects(target);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects(target);
            OnKillEffects(target);
        }



        private void OnHitEffects(NPC npc)
        {

        }

        private void OnKillEffects(NPC npc)
        {
            if (npc.life > 0) return;

            if (Player.HasBuff<WarriorsResolve>())
            {
                if (!WarriorsResolveTriggered)
                {
                    Player.Heal(40);
                    WarriorsResolveTriggered = true;
                }
            }
        }

        private void DeathsDoorEffects()
        {
            if (Player.statLife > Player.statLifeMax2 * 0.2f) return;

            if (WarriorsEpic && WarriorsEpicCooldown <= 0)
            {
                Player.AddBuff(ModContent.BuffType<WarriorsResolve>(), ModUtils.SecondsToTicks(10));
                WarriorsEpicCooldown = ModUtils.SecondsToTicks(30);
            }
        }
    }
}