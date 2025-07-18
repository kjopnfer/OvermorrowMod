using OvermorrowMod.Core.Items;
using OvermorrowMod.Core.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GlobalPlayer : ModPlayer
    {
        ///// <summary>
        ///// Used for calculating whether the player is in the NPC's aggro range.
        ///// Higher values reduce the NPC's aggro range while increasing their alert threshold.
        ///// </summary>
        //public int AlertBonus = 0;

        ///// <summary>
        ///// Used for calculating how quickly the enemy loses aggro if the Player is their target.
        ///// </summary>
        //public float AggroLossBonus = 0;

        public override void ResetEffects()
        {
            //AlertBonus = 0;
            //AggroLossBonus = 0;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.IsWeaponType(WeaponType.Rapier))
            {
                // Add armor penetration equal to twice the projectile's damage
                modifiers.ArmorPenetration += proj.damage * 2;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            AccessoryKeywords.TriggerStrike(Player, target, hit, damageDone);

            if (target.life <= 0)
            {
                AccessoryKeywords.TriggerExecute(Player, target);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            AccessoryKeywords.TriggerStrike(Player, target, hit, damageDone);

            if (target.life <= 0)
            {
                AccessoryKeywords.TriggerExecute(Player, target);
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            AccessoryKeywords.TriggerStrike(Player, target, hit, damageDone);

            if (target.life <= 0)
            {
                AccessoryKeywords.TriggerExecute(Player, target);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            AccessoryKeywords.TriggerRetaliate(Player, null, hurtInfo);
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            AccessoryKeywords.TriggerRetaliate(Player, npc, hurtInfo);
        }

        public override void PostUpdate()
        {
            if ((float)Player.statLife / Player.statLifeMax2 >= 0.8f)
            {
                AccessoryKeywords.TriggerVigor(Player);
            }

            if ((float)Player.statLife / Player.statLifeMax2 <= 0.2f)
            {
                AccessoryKeywords.TriggerDeathsDoor(Player);
            }

            if ((float)Player.statMana / Player.statManaMax2 <= 0.2f)
            {
                AccessoryKeywords.TriggerMindDown(Player);
            }

            for (int i = 0; i < 10; i++)
            {
                Item hotbarItem = Player.inventory[i];
                if (!hotbarItem.IsAir)
                {
                    AccessoryKeywords.TriggerQuickslot(Player, hotbarItem);
                }
            }
        }
    }
}