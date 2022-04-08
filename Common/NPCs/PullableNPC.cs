using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.NPCs
{
    public class PullableNPC : ModNPC
    {
        public bool Grappled = false;
        protected bool CanBeGrappled = true;
        protected Projectile GrappleProjectile = null;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pullable");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.dontTakeDamageFromHostiles = true;
            npc.friendly = false;
            npc.chaseable = false;
        }

        public override void AI()
        {
            if (GrappleProjectile != null)
            {
                if (!GrappleProjectile.active)
                {
                    GrappleProjectile = null;
                    Grappled = false;
                }
            }

            if (Grappled)
            {
                npc.Center = GrappleProjectile.Center;
            }

            if (CanBeGrappled)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && projectile.aiStyle == 7 && npc.Hitbox.Intersects(projectile.Hitbox))
                    {
                        if (!Grappled && !projectile.GetGlobalProjectile<OvermorrowGlobalProjectile>().RetractSlow)
                        {
                            for (int j = 0; j < 18; j++)
                            {
                                Particle.CreateParticle(Particle.ParticleType<Spark>(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(4, 6), Color.Yellow);
                            }

                            Particle.CreateParticle(Particle.ParticleType<Pulse>(), projectile.Center, Vector2.Zero, Color.Yellow, 0.5f, 0.5f);

                            projectile.GetGlobalProjectile<OvermorrowGlobalProjectile>().RetractSlow = true;
                            projectile.ai[0] = 1;

                            GrappleProjectile = projectile;
                            Grappled = true;
                        }
                    }
                }
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item)
        {
            if (item.hammer > 5)
            {
                return true;
            }

            return base.CanBeHitByItem(player, item);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }
    }
}