using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ArtOfBallistics : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 34;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OldAccessoryPlayer>().ArtOfBallistics = true;
            player.GetCritChance(DamageClass.Ranged) += 4;
        }

        public static void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            if (!player.GetModPlayer<OldAccessoryPlayer>().ArtOfBallistics) return;
            if (projectile.DamageType != DamageClass.Ranged) return;

            if (projectile.penetrate != -1)
                projectile.penetrate += 2;

        }

        public static void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            var accessoryPlayer = player.GetModPlayer<OldAccessoryPlayer>();
            if (!accessoryPlayer.ArtOfBallistics || projectile.DamageType != DamageClass.Ranged)
                return;

            var globalProjectile = projectile.GetGlobalProjectile<GlobalProjectiles>();
            globalProjectile.ArtOfBallisticsHit++;

            if (globalProjectile.ArtOfBallisticsHit > 1)
                CombatText.NewText(target.Hitbox, new Color(20, 166, 159), $"+{globalProjectile.ArtOfBallisticsHit}", true);
        }

        public static void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            var accessoryPlayer = player.GetModPlayer<OldAccessoryPlayer>();

            if (!accessoryPlayer.ArtOfBallistics || projectile.DamageType != DamageClass.Ranged)
                return;

            var globalProjectile = projectile.GetGlobalProjectile<GlobalProjectiles>();

            projectile.CritChance += 15 * globalProjectile.ArtOfBallisticsHit;
            modifiers.SourceDamage += (1.05f * globalProjectile.ArtOfBallisticsHit);
        }
    }
}