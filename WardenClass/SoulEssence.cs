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

            ProjectileSlot = 1;
            Period = 300;
            PeriodFast = 100;
            ProjectileSpeed = 8;
            OrbitingRadius = 100;
            CurrentOrbitingRadius = 300;
        }

        public override void AI()
        {
            //Making player variable "p" set as the projectile's owner
            player = Main.player[projectile.owner];

            RelativeVelocity = player.velocity;
            OrbitCenter = player.Center;

            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            // Make sure the projectile does not naturally expire while active
            if (modPlayer.soulList.Count > 0)
            {
                projectile.timeLeft = 2;
            }

            if (player.dead)
            {
                projectile.Kill();
                return;
            }

            projectile.rotation += 0.04f;
            projectile.ai[1]++;

            // Factors for calculations
            // double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
            // double rad = deg * (Math.PI / 180); //Convert degrees to radians
            // double dist = projectile.ai[0]; //Distance away from the player

            /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
			/distance for the desired distance away from the player minus the projectile's width   /
			/and height divided by two so the center of the projectile is at the right place.     */
            // projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            // projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
            /*if (projectile.knockBack == 1)
            {
                projectile.ai[1] += 4f;
            }
            else
            {
                projectile.ai[1] -= 4f;
            }*/

            base.AI();

            // projectile.rotation = (float)rad;
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
            if (Proj_State == 1 || Proj_State == 2)
            {
                GeneratePositionsAfterKill();
            }

            Vector2 origin = projectile.Center;
            float radius = 5;
            int numLocations = 3;

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