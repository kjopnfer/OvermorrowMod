using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.Items.Weapons.Hardmode.BiomeWep
{
    public class IceChunk : ModProjectile
    {

        private int timer = 0;
        private const string ChainTexturePath = "OvermorrowMod/Items/Weapons/Hardmode/BiomeWep/Spine";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Axearang");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
            projectile.width = 92;
            projectile.height = 102;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 600;
            projectile.light = 1f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;

        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(44, 350);
        }


        public override void AI()
        {


            timer++;
            if(timer == 30)
            {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X + 6, value1.Y + 6, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X - 6, value1.Y + 6, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X + 6, value1.Y - 6, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X - 6, value1.Y - 6, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);


            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X + 6, value1.Y, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X, value1.Y + 6, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X, value1.Y - 6, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X - 6, value1.Y, mod.ProjectileType("FrostWaveFriend"), projectile.damage + 10, 1f, projectile.owner, 0f);
            timer = 0;
            }


            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 135, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 1, new Color(), 2.3f);
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

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 12 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
