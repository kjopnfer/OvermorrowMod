using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using OvermorrowMod.Particles;
using OvermorrowMod.WardenClass;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WardenClass
{
    public class SoulEssence : OrbitingProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.White;
        public float TrailSize(float progress) => 3;
        public Type TrailType()
        {
            return typeof(SoulTrail);
        }

        public SoulEssence() : base(0)
        {
        }

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Essence");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 2;

            Period = 300;
            PeriodFast = 100;
            ProjectileSpeed = 8;
            OrbitingRadius = 100;
            CurrentOrbitingRadius = 300;
        }

        public override void AI()
        {
            //Making player variable "p" set as the projectile's owner
            entity = Main.player[projectile.owner];

            RelativeVelocity = entity.velocity;
            OrbitCenter = entity.Center;

            var modPlayer = WardenDamagePlayer.ModPlayer((Player)entity);

            // Make sure the projectile does not naturally expire while active
            if (modPlayer.soulList.Count > 0)
            {
                projectile.timeLeft = 2;
            }

            if (((Player)entity).dead)
            {
                projectile.Kill();
                return;
            }

            projectile.rotation += 0.04f;
            projectile.ai[1]++;

            base.AI();
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_644");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(182, 255, 231), projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(MathHelper.Lerp(0.3f, 0.5f, (float)Math.Sin(projectile.ai[1] / 10f)), 1f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(182, 255, 231), projectile.rotation, drawOrigin, new Vector2(MathHelper.Lerp(0.3f, 0.5f, (float)Math.Sin(projectile.ai[1] / 10f)), 1f), SpriteEffects.None, 0);
            
        }

        public override void Kill(int timeLeft)
        {
            if (ProjState == OrbitingProjectileState.Spawning || ProjState == OrbitingProjectileState.Moving)
            {
                GeneratePositionsAfterKill();
            }

            Vector2 origin = projectile.Center;
            float radius = 5;
            int numLocations = 3;

            var player = (Player)entity;

            if (player.GetModPlayer<WardenDamagePlayer>().SaintRing && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DevouredSoul>(), 30, 0f, player.whoAmI);
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 0.5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * 2;

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Color.Cyan, 1, 0.5f, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }
        }

        public override void Attack()
        {
        }
    }
}