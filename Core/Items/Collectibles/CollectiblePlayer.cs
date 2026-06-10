using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Collectibles
{
    public class CollectiblePlayer : ModPlayer
    {
        private readonly Dictionary<CollectibleStat, float> totals = new();
        private readonly Dictionary<CollectibleEffect, float> effects = new();

        /// <summary>
        /// Number of consumed sources that lowered a player stat. Read by effects
        /// that reward taking on stat penalties, such as Rose.
        /// </summary>
        public int DecreasingSourceCount;

        public void Add(CollectibleStat stat, float amount)
        {
            totals.TryGetValue(stat, out float current);
            totals[stat] = current + amount;
        }

        public float Get(CollectibleStat stat) => totals.TryGetValue(stat, out float value) ? value : 0f;

        public void SetEffect(CollectibleEffect effect, float magnitude)
        {
            effects.TryGetValue(effect, out float current);
            effects[effect] = current + magnitude;
        }

        public float GetEffect(CollectibleEffect effect) => effects.TryGetValue(effect, out float value) ? value : 0f;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            totals.Clear();
            effects.Clear();
            DecreasingSourceCount = 0;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;

            float life = Get(CollectibleStat.MaxLife);
            float manaBonus = Get(CollectibleStat.MaxMana);

            if (life != 0f)
                health = new StatModifier(1f, 1f, life, 0f);
            if (manaBonus != 0f)
                mana = new StatModifier(1f, 1f, manaBonus, 0f);
        }

        public override void PostUpdateEquips()
        {
            Player.statDefense += (int)Get(CollectibleStat.DefenseFlat);

            float defensePercent = Get(CollectibleStat.DefensePercent);
            if (defensePercent != 0f)
                Player.statDefense += (int)(Player.statDefense * (defensePercent / 100f));

            float damagePercent = Get(CollectibleStat.DamagePercent);
            float critChance = Get(CollectibleStat.CritChance);

            float scaling = GetEffect(CollectibleEffect.DecreaseSourceScaling);
            if (scaling != 0f)
            {
                damagePercent += 2f * DecreasingSourceCount * scaling;
                critChance += 1f * DecreasingSourceCount * scaling;
            }

            if (damagePercent != 0f)
                Player.GetDamage(DamageClass.Generic) += damagePercent / 100f;

            float damageFlat = Get(CollectibleStat.DamageFlat);
            if (damageFlat != 0f)
                Player.GetDamage(DamageClass.Generic).Flat += damageFlat;

            if (critChance != 0f)
                Player.GetCritChance(DamageClass.Generic) += critChance;

            float moveSpeed = Get(CollectibleStat.MoveSpeedPercent);
            if (moveSpeed != 0f)
                Player.moveSpeed += moveSpeed / 100f;

            float knockback = Get(CollectibleStat.Knockback);
            if (knockback != 0f)
                Player.GetKnockback(DamageClass.Generic).Flat += knockback;

            float armorPen = Get(CollectibleStat.ArmorPenetration);
            if (armorPen != 0f)
                Player.GetArmorPenetration(DamageClass.Generic) += armorPen;

            float luck = Get(CollectibleStat.Luck);
            if (luck != 0f)
                Player.luck += luck;

            if (Player.statLifeMax2 < 1)
                Player.statLifeMax2 = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float chance = GetEffect(CollectibleEffect.ExtraCoinsOnHit);
            if (chance <= 0f) return;
            if (Main.rand.NextFloat() >= chance) return;

            Item.NewItem(Player.GetSource_OnHit(target), target.Hitbox, ItemID.SilverCoin, Main.rand.Next(1, 6));
        }
    }
}
