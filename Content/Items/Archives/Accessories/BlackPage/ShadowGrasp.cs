
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ShadowGrasp : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(1);
            Projectile.timeLeft = ModUtils.SecondsToTicks(6);
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float TargetID => ref Projectile.ai[1];
        public override void AI()
        {
            NPC npc = Main.npc[(int)TargetID];
            if (!npc.active) Projectile.Kill();

            Projectile.Center = npc.Center;

            AICounter++;
            if (AICounter >= 20)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame == 7)
                    {
                        SpawnSmoke();
                    }

                    if (Projectile.frame >= Main.projFrames[Projectile.type])
                    {
                        Projectile.frame = 0;
                        Projectile.Kill();

                    }
                }
            }
        }

        private void SpawnSmoke()
        {
            var smokeTextures = new Texture2D[] {
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_01", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_02", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_03", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_04", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_05", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_06", AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("OvermorrowMod/Assets/Textures/smoke_07", AssetRequestMode.ImmediateLoad).Value
            };

            Color smokeColor = Color.Black;
            Vector2 velocity = Vector2.UnitY * 3;
            float spreadAngle = 40f;
            float velocityScale = 0.55f;
            var count = 30;
            for (int i = 0; i < count; i++)
            {
                Vector2 particleVelocity = (velocity * Main.rand.NextFloat(0.025f, velocityScale))
                    .RotatedByRandom(MathHelper.ToRadians(spreadAngle));

                var smoke = new Gas(smokeTextures, ModUtils.SecondsToTicks(Main.rand.NextFloat(0.6f, 1.1f)), scaleOverride: Main.rand.NextFloat(0.3f, 0.6f))
                {
                    gasBehavior = GasBehavior.Grow,
                    driftsUpward = true,
                    rotatesOverTime = true,
                    scaleRate = 0.005f,
                    customAlpha = 0.85f
                };

                ParticleManager.CreateParticleDirect(smoke, Projectile.Center, particleVelocity, smokeColor, 1f, scale: 0.05f, 0f);
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(ref hitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            float alpha = 1f;
            if (AICounter < 20)
                alpha = MathHelper.Lerp(0f, 1f, AICounter / 20f);

            int frameHeight = texture.Height / Main.projFrames[Type];
            Rectangle drawRectangle = new Rectangle(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White * alpha, 0f, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 1);

            return false;
        }
    }
}