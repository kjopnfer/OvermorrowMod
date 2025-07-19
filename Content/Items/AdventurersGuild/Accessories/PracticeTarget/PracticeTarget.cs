using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Items.Bows;
using OvermorrowMod.Content.Items.Archives.Weapons;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items.Accessories;
using OvermorrowMod.Core.Items.Bows;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.AdventurersGuild.Accessories
{
    public class PracticeTarget : OvermorrowAccessory, IBowModifier
    {
        public override string Texture => AssetDirectory.GuildItems + Name;

        public int ConsecutiveHits { get; private set; } = 0;
        public const int MaxStacks = 5;
        public const float BonusPerStack = 0.05f;
        protected override void SafeSetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public override void ResetVariables()
        {
            ConsecutiveHits = 0;
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            definition.AddProjectileStrikeEffect(
                condition: (player, projectile, target, hit, damageDone) =>
                {
                    return projectile.DamageType == DamageClass.Ranged && projectile.arrow;
                },
                effect: (player, projectile, target, hit, damageDone) =>
                {
                    var instance = GetInstance<PracticeTarget>(player);
                    if (instance != null && instance.ConsecutiveHits < MaxStacks)
                    {
                        instance.ConsecutiveHits++;

                        Projectile.NewProjectile(null, player.Center + new Vector2(-4, -32), Vector2.Zero, ModContent.ProjectileType<PracticeTargetIcon>(), 0, 0f, player.whoAmI);
                    }
                }
            );

            definition.AddProjectileKillEffect(
                condition: (player, projectile, timeLeft) =>
                {
                    bool hasHitTarget = projectile.GetGlobalProjectile<GlobalProjectiles>().NumberHits > 0;
                    if (projectile.ModProjectile is WispArrow wispArrow && wispArrow.PatronusFlag > 0)
                    {
                        // Do not reset the number of targets hit since this projectile kills itself prematurely
                        hasHitTarget = true;
                    }

                    return timeLeft > 0 && projectile.DamageType == DamageClass.Ranged && projectile.arrow && !hasHitTarget;
                },
                effect: (player, projectile, timeLeft) =>
                {
                    var instance = GetInstance<PracticeTarget>(player);
                    if (instance != null && instance.ConsecutiveHits > 0)
                    {
                        CombatText.NewText(player.Hitbox, Color.Red, "Draw Speed Reset!", false);

                        Projectile.NewProjectile(null, player.Center + new Vector2(-4, -32), Vector2.Zero, ModContent.ProjectileType<PracticeTargetIcon>(), 0, 0f, player.whoAmI, 0f, instance.ConsecutiveHits);
                        instance.ConsecutiveHits = 0;
                    }
                }
            );
        }

        public void ModifyBowStats(BowStats stats, Player player)
        {
            var instance = GetInstance<PracticeTarget>(player);
            if (instance != null && instance.ConsecutiveHits > 0)
            {
                float speedBonus = 1f + (instance.ConsecutiveHits * BonusPerStack);
                stats.ChargeSpeed *= speedBonus;
            }
        }

        public void OnPowerShot(HeldBow bow, Player player)
        {
        }

        public void OnArrowFired(HeldBow bow, Player player, Projectile arrow)
        {
        }
    }
}