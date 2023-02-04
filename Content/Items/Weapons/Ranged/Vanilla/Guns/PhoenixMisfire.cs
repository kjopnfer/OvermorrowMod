using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class PhoenixMisfire : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phoenix Burst");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override void OnSpawn(IEntitySource source)
        {

            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PhoenixMisfire"));

            Projectile.Center = Main.LocalPlayer.Center;

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            for (int i = 0; i < 32; i++)
            {
                //randomScale = Main.rand.NextFloat(0.15f, 0.35f);
                randomScale = Main.rand.NextFloat(5f, 8f);

                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(13, 18);
                Color color = Color.Orange;

                Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale, 0f, 0f, 1f);

                randomScale = Main.rand.NextFloat(3f, 6f);
                RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(9, 12);
                Particle.CreateParticle(Particle.ParticleType<RotatingEmber>(), Projectile.Center, Vector2.Normalize(RandomVelocity), Color.Orange, 1f, randomScale, 0f, 0f, -1f);

            }
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            if(AICounter < 20f) AICounter++;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = (int)MathHelper.Lerp(Projectile.width, 256, AICounter / 20f);
            hitbox.Height = (int)MathHelper.Lerp(Projectile.width, 256, AICounter / 20f);

            hitbox.X = (int)(Projectile.Center.X - hitbox.Width / 2f);
            hitbox.Y = (int)(Projectile.Center.Y - hitbox.Height / 2f);

            base.ModifyDamageHitbox(ref hitbox);
        }
    }
}