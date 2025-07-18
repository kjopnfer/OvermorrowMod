using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ArtOfBallistics : OvermorrowAccessory
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 34;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void UpdateAccessoryEffects(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 4;
        }

        public int ArtOfBallisticsHit = 0;

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
            // Don't add penetration if the projectile already has infinite penetration
            definition.AddProjectileSpawnEffect(
                condition: (player, projectile, source) =>
                {
                    return projectile.DamageType == DamageClass.Ranged && projectile.penetrate != -1;
                },
                effect: (player, projectile, source) =>
                {
                    projectile.penetrate += 2;
                }
            );

            definition.AddProjectileStrikeEffect(
                condition: (player, projectile, target, hit, damageDone) =>
                {
                    return projectile.DamageType == DamageClass.Ranged;
                },
                effect: (player, projectile, target, hit, damageDone) =>
                {
                    var globalProjectile = projectile.GetGlobalProjectile<GlobalProjectiles>();
                    if (globalProjectile.NumberHits > 1)
                    {
                        CombatText.NewText(target.Hitbox, new Color(20, 166, 159), $"+{globalProjectile.NumberHits}", true);
                    }
                }
            );

            definition.AddProjectileModifyHitEffect(
                condition: (player, projectile, target, modifiers) =>
                {
                    return projectile.DamageType == DamageClass.Ranged;
                },
                effect: (player, projectile, target, modifiers) =>
                {
                    var globalProjectile = projectile.GetGlobalProjectile<GlobalProjectiles>();

                    projectile.CritChance += 15 * globalProjectile.NumberHits;
                    modifiers.SourceDamage += (1.05f * globalProjectile.NumberHits);
                }
            );
        }
    }
}