using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects;
using OvermorrowMod.Projectiles.Boss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class BloodBullet : ModProjectile
    {
        private bool hit = false;
        private Vector2 velocitystash = Vector2.Zero;
        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.penetrate = -1;//2;//1;
            projectile.timeLeft = 600;
            projectile.friendly = true;
            projectile.hostile = false;
        }

        public override void AI() 
		{
            projectile.velocity = projectile.velocity * 1.0001f;
            if (hit)
            {
                projectile.damage = 0;
                velocitystash = Vector2.SmoothStep(velocitystash, Vector2.Zero, 0.1f);
                projectile.velocity = Vector2.Zero;
                projectile.ai[1] -= 0.01f;
                if (projectile.ai[1] <= 0)
                {
                    projectile.Kill();
                }

            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
                projectile.ai[1] = 1;
                Vector2.SmoothStep(projectile.velocity, Vector2.Zero, 0.01f);
                velocitystash = projectile.velocity;
                if (projectile.timeLeft < 240)
                {
                    hit = true;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if(target.life < projectile.damage - (target.defense * 1.1))
            {
                for (int i = 0; i < Main.rand.Next(3, 5); i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-5, -3), ProjectileType<BloodyBallGravity>(), 12, 10f, Main.myPlayer, 0, 10);
                    }
                }
            }
            hit = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hit = true;
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            PrimitivePacket packet = new PrimitivePacket();
            packet.Type = PrimitiveType.TriangleStrip;
            float range = Math.Abs(velocitystash.X) + Math.Abs(velocitystash.Y) + 6;
            Vector2 start = projectile.Center;
            Vector2 end = projectile.Center + ((-1 * velocitystash) * range);
            float baseSize = 6;
            for (float i = 0; i < 99; i += /*0.25f*/ /*0.1f*/ 0.125f)
            {
                float progress1 = (float)i / (float)100;
                Vector2 pos1 = Vector2.Lerp(start, end, progress1);
                Vector2 offset1 = ((Vector2.UnitX * projectile.ai[1]) * (baseSize * (1 - progress1))).RotatedBy(projectile.rotation);
                Vector2 offset2 = ((Vector2.UnitX * projectile.ai[1]) * (-baseSize * (1 - progress1))).RotatedBy(projectile.rotation);
                Vector2 offset3 = ((Vector2.UnitX * projectile.ai[1]) * (baseSize * (1 - progress1))).RotatedBy(projectile.rotation).RotatedBy(MathHelper.ToRadians(270));
                Color color1 = Color.Lerp(Color.DarkRed, Color.MediumVioletRed, progress1) * progress1;

                packet.Add(pos1 + offset1, color1, new Vector2(progress1 + Main.GlobalTime, 1));
                packet.Add(pos1 + offset2, color1, new Vector2(progress1 + Main.GlobalTime, 0));
                packet.Add(pos1 + offset3, color1, new Vector2(progress1 + Main.GlobalTime, 0.5f));
            }
            PrimitivePacket.SetTexture(0, Main.projectileTexture[projectile.type]);
            packet.Pass = "Texturized"; //"Basic";
            packet.Send();
            return false;
        }
    }
}