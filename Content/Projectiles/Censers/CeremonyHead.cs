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
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 26;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.oldVelocity.X * 0.2f, Projectile.oldVelocity.Y * 0.2f, 1, new Color(), 1f);


            if (owner.channel)
            {
                //owner.reuseDelay = 2;
                activeRadius = MathHelper.SmoothStep(30, 65, Utils.Clamp(Projectile.ai[0], 0f, 105) / 105);

                Projectile.timeLeft = 60;
                if (Projectile.ai[0] < 25) Projectile.ai[0]++;

                Projectile.Center = owner.Center + Vector2.One.RotatedBy(-Projectile.ai[0] / 7f) * activeRadius;
                Projectile.rotation = Projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(35);
                
                owner.ChangeDir(-Projectile.direction);
                owner.heldProj = Projectile.whoAmI;
                owner.itemTime = 30;
                owner.itemAnimation = 30;
            }
            else
            {
                deactiveRadius = MathHelper.Lerp(0, activeRadius, Projectile.timeLeft / 60f);

                Projectile.Center = owner.Center + Vector2.One.RotatedBy(-Projectile.ai[0] / 7f) * deactiveRadius;
                Projectile.rotation = Projectile.DirectionTo(owner.Center).ToRotation() + MathHelper.PiOver2 + MathHelper.ToRadians(35);
            }



            Projectile.ai[0]++;

            if (++Projectile.ai[1] % 20 == 0 && Projectile.ai[0] > 45)
            {
                Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center, -Projectile.DirectionTo(owner.Center), ProjectileID.ToxicCloud, 15, 0f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var player = Main.player[Projectile.owner];

            Vector2 mountedCenter = player.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>(ChainTexturePath).Value;

            var drawPosition = Projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (Projectile.alpha == 0)
            {
                int direction = -1;

                if (Projectile.Center.X < mountedCenter.X)
                    direction = 1;

                player.itemRotation = (float)Math.Atan2(remainingVectorToPlayer.Y * direction, remainingVectorToPlayer.X * direction);
            }

            float CHAIN_LENGTH = 8;
            float distance = Vector2.Distance(player.Center, Projectile.Center);

            float iterations = distance / CHAIN_LENGTH;

            Vector2 midPoint1 = player.Center + new Vector2(25, 25).RotatedBy(Projectile.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2);
            Vector2 midPoint2 = Projectile.Center - new Vector2(-25, 25).RotatedBy(Projectile.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2);
        
            for (int i = 0; i < iterations; i++)
            {
                float progress = i / iterations;
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));


                //Vector2 position = ModUtils.Bezier(player.Center, player.Center + new Vector2(75, 0), player.Center + new Vector2(25, 25), player.Center + new Vector2(50, 50), progress);
                //Vector2 position = ModUtils.Bezier(player.Center, projectile.Center, player.Center + new Vector2(50, 50), projectile.Center - new Vector2(50, 50), progress);
                Vector2 position = ModUtils.Bezier(player.Center, Projectile.Center, midPoint1, midPoint2, progress);
                Main.EntitySpriteDraw(chainTexture, position - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

            }

            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                drawPosition += remainingVectorToPlayer * 8 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                //Main.EntitySpriteDraw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}
