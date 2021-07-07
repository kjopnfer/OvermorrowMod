using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    class WaterStaffProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lacusite Bolt");
            Main.projFrames[projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 45;
            projectile.tileCollide = true;
            projectile.magic = true;
        }
        int timer = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            knockback = 0;
        }
        int direction = 1;

        List<Projectile> owned = new List<Projectile>();
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, -2.5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, 33, dustvelocity.X, dustvelocity.Y, 0, default, 1);
                Main.dust[dust].noGravity = false;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;

            Texture2D texture = mod.GetTexture("Projectiles/Magic/WaterStaffProj_Glow");
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f,
                    projectile.position.Y - Main.screenPosition.Y + projectile.height - drawRectangle.Height * 0.5f
                ),
                drawRectangle,
                Color.White,
                projectile.rotation,
                new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2),
                projectile.scale,
                projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                0f
            );
        }
    }
}
