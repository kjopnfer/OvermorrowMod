using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class PullabeNPC : ModNPC
    {
        protected bool Grappled = false;
        protected Projectile GrappleProjectile = null;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.Meteor3;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pullable");
        }

        public override void SetDefaults()
        {
            npc.width = npc.height = 64;
            npc.aiStyle = -1;
            npc.lifeMax = 100;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.friendly = false;
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

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.aiStyle == 7 && npc.Hitbox.Intersects(projectile.Hitbox))
                {
                    if (!Grappled)
                    {
                        for (int j = 0; j < 18; j++)
                        {
                            Particle.CreateParticle(Particle.ParticleType<Spark>(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(4, 6), Color.Yellow);
                        }

                        Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.Yellow, 1, 0.5f);

                        projectile.GetGlobalProjectile<OvermorrowGlobalProjectile>().RetractSlow = true;
                        projectile.ai[0] = 1;

                        GrappleProjectile = projectile;
                        Grappled = true;
                        Main.NewText("grapple collision");
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