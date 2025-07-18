using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Accessories;
using Terraria;
using Terraria.DataStructures;
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

        public static void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            if (!player.GetModPlayer<OldAccessoryPlayer>().ArtOfBallistics) return;
            if (projectile.DamageType != DamageClass.Ranged) return;

            if (projectile.penetrate != -1)
                projectile.penetrate += 2;
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
                    Main.NewText("triggered");
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

        public static void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            var accessoryPlayer = player.GetModPlayer<OldAccessoryPlayer>();
            if (!accessoryPlayer.ArtOfBallistics || projectile.DamageType != DamageClass.Ranged)
                return;

            var globalProjectile = projectile.GetGlobalProjectile<GlobalProjectiles>();
            globalProjectile.NumberHits++;

            if (globalProjectile.NumberHits > 1)
                CombatText.NewText(target.Hitbox, new Color(20, 166, 159), $"+{globalProjectile.NumberHits}", true);
        }

        public static void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            var accessoryPlayer = player.GetModPlayer<OldAccessoryPlayer>();

            if (!accessoryPlayer.ArtOfBallistics || projectile.DamageType != DamageClass.Ranged)
                return;

            var globalProjectile = projectile.GetGlobalProjectile<GlobalProjectiles>();

            projectile.CritChance += 15 * globalProjectile.NumberHits;
            modifiers.SourceDamage += (1.05f * globalProjectile.NumberHits);
        }


    }
}