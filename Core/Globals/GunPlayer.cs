using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Weapons.Guns;
using System.Collections.Generic;
using System.Data;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class GunPlayer : ModPlayer
    {
        public int BulletArmorPenetration;

        public bool CowBoySet;
        public bool GraniteLauncher;

        public bool ChicagoBonusShots = false;
        public bool FarlanderPierce = false;
        public bool WildEyeCrit = false;

        public int MusketInaccuracy = 0;
        public int GraniteEnergyCount = 0;
        public float FarlanderCharge = 0;
        public int FarlanderSpeedBoost = 0;

        //public Dictionary<int, GraniteShard> ShardList = new Dictionary<int, GraniteShard>();

        /// <summary>
        /// Used to preserve data between guns whenever swapped to prevent reload skipping
        /// </summary>
        public Dictionary<int, HeldGunInfo> playerGunInfo = new Dictionary<int, HeldGunInfo>();

        public override void ResetEffects()
        {
            BulletArmorPenetration = 0;

            CowBoySet = false;
            GraniteLauncher = false;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            /*if (hit.Crit && proj.DamageType == DamageClass.Ranged)
            {
                if (Player.GetModPlayer<GunPlayer>().GraniteEnergyCount < 8)
                {
                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        float randomScale = Main.rand.NextFloat(0.1f, 0.15f);

                        Color color = Color.Cyan;
                        Particle.CreateParticle(Particle.ParticleType<Pulse>(), proj.Center, Vector2.Zero, color, 1, randomScale);
                    }

                    Projectile.NewProjectile(null, target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 3, ModContent.ProjectileType<GraniteEnergy>(), 0, 0f, Player.whoAmI);
                }
            }*/
        }
    }
}