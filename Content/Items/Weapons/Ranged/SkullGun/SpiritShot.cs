using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.SkullGun
{
    public class SpiritShot : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.White;
        public float TrailSize(float progress) => 20;
        public Type TrailType()
        {
            return typeof(SixShooterTrail);
        }

        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Souls");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.DamageType = DamageClass.Ranged;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }

            Vector2 move = Vector2.Zero;
            float distance = 320f;
            bool target = false;

            if (projectile.ai[0] > 20)
            {
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                    {
                        Vector2 newMove = Main.npc[k].Center - projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
            }

            if (target)
            {
                AdjustMagnitude(ref move);
                projectile.velocity = (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }

            projectile.ai[0]++;
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D SoulTexture = ModContent.GetTexture("Terraria/Extra_89");

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, SoulTexture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(SoulTexture,
                projectile.Center - Main.screenPosition,
                null, new Color(196, 247, 258), projectile.rotation, SoulTexture.Size() / 2, projectile.scale * 1.2f, spriteEffects, 0f);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.MagnetSphere, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}