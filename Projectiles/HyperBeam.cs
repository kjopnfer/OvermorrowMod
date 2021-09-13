using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;

namespace OvermorrowMod.Projectiles
{
    public class HyperBeam : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Effects/TrailTextures/Trail3";
        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = false;
        }
        public override void AI()
        {
            /*Player player = Main.player[projectile.owner];
            bool channel = player.channel && !player.CCed && !player.noItems;*/
            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
            projectile.velocity = projectile.velocity.RotatedBy(projectile.ai[0]);
            projectile.rotation = projectile.velocity.ToRotation();
            // base value ill use is like uh, 12.5% each
            float prog = (240f - (float)projectile.timeLeft) / 240;
            projectile.scale = MathHelper.Clamp((float)Math.Sin(prog * Math.PI) * 4, 0, 1);
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool CanDamage() => projectile.scale == 1f;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            PrimitivePacket packet = new PrimitivePacket();
            float range = 1000f;
            Vector2 start = projectile.Center;
            Vector2 end = projectile.Center + projectile.velocity * range;
            float baseSize = projectile.width;
            for (float i = 0; i < 99; i += 0.5f)
            {
                float progress1 = (float)i / (float)100;
                float progress2 = (float)(i + 1) / (float)100;
                Vector2 pos1 = Vector2.Lerp(start, end, progress1);
                Vector2 pos2 = Vector2.Lerp(start, end, progress2);
                float size1 = Size(baseSize, progress1) * projectile.scale;
                float size2 = Size(baseSize, progress2) * projectile.scale;
                Vector2 offset1 = Vector2.Normalize(end - start).RotatedBy(Math.PI / 2) * size1;
                Vector2 offset2 = Vector2.Normalize(end - start).RotatedBy(Math.PI / 2) * size2;
                Color color1 = ColorFunction(progress1);
                Color color2 = ColorFunction(progress1);
                /*packet.Add(pos1 + offset1, color1, new Vector2(progress1 + Main.GlobalTime, 1));
                packet.Add(pos1 - offset1, color1, new Vector2(progress1 + Main.GlobalTime, 0));
                packet.Add(pos2 + offset2, color2, new Vector2(progress2 + Main.GlobalTime, 1));*/

                packet.Add(pos2 + offset2, color2, new Vector2(progress2 + Main.GlobalTime, 1));
                packet.Add(pos2 - offset2, color2, new Vector2(progress2 + Main.GlobalTime, 0));
                packet.Add(pos1 - offset1, color1, new Vector2(progress1 + Main.GlobalTime, 0));
            }
            PrimitivePacket.SetTexture(0, Main.projectileTexture[projectile.type]);
            packet.Pass = "Texturized";
            packet.Send();
            return false;
        }
        public Color ColorFunction(float progress)
        {
            return SmoothStep(Color.Pink, Color.LightPink, progress);
        }
        public Color SmoothStep(Color col1, Color col2, float progress)
        {
            return new Color(MathHelper.SmoothStep(col1.R, col2.R, progress), MathHelper.SmoothStep(col1.G, col2.G, progress), MathHelper.SmoothStep(col1.B, col2.B, progress));
        }
        /*public float Size(float startSize, float progress)
        {
            float size = 0f;
            if (progress < 0.7f) size = MathHelper.SmoothStep(0, startSize, progress * (1f/0.7f));
            else
            {
                size = MathHelper.SmoothStep(startSize, 0, (progress - 0.7f) * (1f / 0.3f));
            }
            return size;
        }*/

        public float Size(float startSize, float progress)
        {
            float size = 0f;
            if (progress < 0.3f) size = MathHelper.SmoothStep(0, startSize, progress * (1f / 0.3f));
            else
            {
                size = MathHelper.SmoothStep(startSize, 0, (progress - 0.3f) * (1f / 0.7f));
            }
            return size;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float width = projectile.width;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * 1000f, width, ref _);
        }
    }
}