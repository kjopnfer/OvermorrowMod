using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Censers
{
    public class CeremonyHead : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Censers/Chain";
        private float activeRadius;
        private float deactiveRadius;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceremonial Censer");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.timeLeft = 26;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];

            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Fire, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 1, new Color(), 1f);


            if (owner.channel)
            {
                //owner.reuseDelay = 2;
                activeRadius = MathHelper.SmoothStep(30, 65, Utils.Clamp(projectile.ai[0], 0f, 105) / 105);

                projectile.timeLeft = 60;
                if (projectile.ai[0] < 25) projectile.ai[0]++;

                projectile.Center = owner.Center + Vector2.One.RotatedBy(-projectile.ai[0] / 7f) * activeRadius;
                projectile.rotation = projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(35);
                
                owner.ChangeDir(-projectile.direction);
                owner.heldProj = projectile.whoAmI;
                owner.itemTime = 30;
                owner.itemAnimation = 30;
            }
            else
            {
                deactiveRadius = MathHelper.Lerp(0, activeRadius, projectile.timeLeft / 60f);

                projectile.Center = owner.Center + Vector2.One.RotatedBy(-projectile.ai[0] / 7f) * deactiveRadius;
                projectile.rotation = projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(35);
            }



            projectile.ai[0]++;

            if (++projectile.ai[1] % 20 == 0 && projectile.ai[0] > 45)
            {
                Projectile.NewProjectile(projectile.Center, -projectile.DirectionTo(owner.Center), ProjectileID.ToxicCloud, 15, 0f, projectile.owner);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = player.Center;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                    direction = 1;

                player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }

            float CHAIN_LENGTH = 8;
            float distance = Vector2.Distance(player.Center, projectile.Center);

            float iterations = distance / CHAIN_LENGTH;

            Vector2 midPoint1 = player.Center + new Vector2(25, 25).RotatedBy(projectile.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2);
            Vector2 midPoint2 = projectile.Center - new Vector2(-25, 25).RotatedBy(projectile.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2);
        
            for (int i = 0; i < iterations; i++)
            {
                float progress = i / iterations;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));


                //Vector2 position = ModUtils.Bezier(player.Center, player.Center + new Vector2(75, 0), player.Center + new Vector2(25, 25), player.Center + new Vector2(50, 50), progress);
                //Vector2 position = ModUtils.Bezier(player.Center, projectile.Center, player.Center + new Vector2(50, 50), projectile.Center - new Vector2(50, 50), progress);
                Vector2 position = ModUtils.Bezier(player.Center, projectile.Center, midPoint1, midPoint2, progress);
                spriteBatch.Draw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

            }

            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                drawPosition += remainingVectorToPlayer * 8 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                //spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
