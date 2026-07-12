using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Projectiles.Archives;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Armor
{
    /// <summary>
    /// Drives the Relic Mantle: on a critical hit it may spawn an orbiting energy (a <see cref="RelicBolt"/>),
    /// each energy grants defense up to a cap, and double-tapping down launches every energy at the cursor.
    /// </summary>
    public class RelicArmorPlayer : ModPlayer
    {
        private const int DefensePerEnergy = 3;
        private const int MaxEnergyDefense = 15;
        private const float StrikeChance = 0.5f;

        public bool relicArmorEquipped;

        private bool prevDown;
        private int doubleTapWindow;

        public override void ResetEffects() => relicArmorEquipped = false;

        public override void PostUpdateEquips()
        {
            if (!relicArmorEquipped) return;
            Player.statDefense += Math.Min(CountEnergies() * DefensePerEnergy, MaxEnergyDefense);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) => TryStrike(hit);

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => TryStrike(hit);

        private void TryStrike(NPC.HitInfo hit)
        {
            if (!relicArmorEquipped || !hit.Crit || Player.whoAmI != Main.myPlayer) return;
            if (Main.rand.NextFloat() >= StrikeChance) return;

            Projectile.NewProjectile(Player.GetSource_Misc("RelicArmor"), Player.Center, Vector2.Zero,
                ModContent.ProjectileType<RelicBolt>(), 0, 0f, Player.whoAmI);
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer) return;

            bool down = Player.controlDown;
            if (down && !prevDown)
            {
                if (doubleTapWindow > 0)
                {
                    LaunchEnergies();
                    doubleTapWindow = 0;
                }
                else
                {
                    doubleTapWindow = 15;
                }
            }
            if (doubleTapWindow > 0) doubleTapWindow--;
            prevDown = down;
        }

        private void LaunchEnergies()
        {
            if (!relicArmorEquipped) return;

            // Launch a single energy (the oldest) per double-tap. Its damage is the defense right now,
            // before this energy is expended, so each successive shot deals a little less.
            RelicBolt next = null;
            Projectile nextProj = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Player.whoAmI && p.ModProjectile is RelicBolt bolt && !bolt.IsLaunched)
                {
                    if (nextProj == null || p.localAI[1] < nextProj.localAI[1])
                    {
                        next = bolt;
                        nextProj = p;
                    }
                }
            }

            next?.Launch(Main.MouseWorld, Player.statDefense);
        }

        private int CountEnergies()
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.owner == Player.whoAmI && p.ModProjectile is RelicBolt bolt && !bolt.IsLaunched)
                    count++;
            }
            return count;
        }
    }
}
