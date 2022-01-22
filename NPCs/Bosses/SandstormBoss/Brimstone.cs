using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using OvermorrowMod.Effects.Prim;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class Brimstone : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 5000;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
            float progress = Utils.InverseLerp(0, 120, projectile.timeLeft);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.height, projectile.width, ref a);
        }
        public Color BeamColor = Color.Red;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // texture used for flash
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/Circle");
            // make the beam slightly change scale with time
            float mult = (1.1f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = projectile.scale * 2 * mult;
            // draw flash
            for (int i = 0; i < 25; i++)
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor * 0.04f * (25f - (float)i), 0, new Vector2(texture.Width, texture.Height) / 2, scale * ((float)i / 25f), SpriteEffects.None, 0f);
            PrimitivePacket packet = new PrimitivePacket();
            packet.Pass = "Texture";
            Vector2 start = projectile.Center;
            Vector2 end = projectile.Center + projectile.velocity * projectile.height;
            float width = projectile.width * projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Textures/Flames"));
            float off = -Main.GlobalTime % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * mult, BeamColor * 0.4f, new Vector2(0 + off, 0));
            packet.Add(start - offset * mult, BeamColor * 0.4f, new Vector2(0 + off, 1));
            packet.Add(end + offset * mult, BeamColor * 0.4f, new Vector2(1 + off, 0));

            packet.Add(start - offset * mult, BeamColor * 0.4f, new Vector2(0 + off, 1));
            packet.Add(end - offset * mult, BeamColor * 0.4f, new Vector2(1 + off, 1));
            packet.Add(end + offset * mult, BeamColor * 0.4f, new Vector2(1 + off, 0));
            packet.Send();
            PrimitivePacket packet2 = new PrimitivePacket();
            packet2.Pass = "Texture";
            PrimitivePacket.SetTexture(0,  ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail1"));
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1, 0));
            packet2.Send();
            return false;
        }
    }
}