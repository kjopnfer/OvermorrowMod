using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.IorichHarvester
{
    public class IorichHarvesterProjectile : ModProjectile
    {
        private float LaunchLength;
        private Vector2 AimDirection;
        private int RotationDirection = 1;
        private bool HitTile = false;
        private bool ReverseMovement;
        private bool RunOnce = true;

        private const int OFFSET = 50; // This is to try to make it move ontop the cursor and not from it
        public override string Texture => AssetDirectory.Melee + "IorichHarvester/IorichHarvester";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Iorich");

            // Afterimage effect
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;     
        }
        public override void SetDefaults()
        {
            projectile.width = 52;
            projectile.height = 52;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 1f, 0f);
            if (projectile.soundDelay == 0 && projectile.ai[1] > 10)
            {
                projectile.soundDelay = 16;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            Player player = Main.player[projectile.owner];
            player.itemAnimation = 10;
            player.itemTime = 10;

            if (RunOnce)
            {
                if (projectile.ai[1] == 1)
                {
                    ReverseMovement = true;
                }

                projectile.spriteDirection = player.direction;
                RotationDirection = projectile.spriteDirection;

                projectile.ai[1] = 0;
                LaunchLength = Vector2.Distance(Main.MouseWorld, player.Center);
                AimDirection = Vector2.Normalize(Main.MouseWorld - player.Center);

                if (LaunchLength > 500)
                {
                    LaunchLength = 500;
                }

                RunOnce = false;
            }

            if (!HitTile)
            {
                Vector2 cp1 = player.Center + new Vector2(LaunchLength + OFFSET, 300).RotatedBy(AimDirection.ToRotation());
                Vector2 cp2 = player.Center + new Vector2(LaunchLength + OFFSET, -300).RotatedBy(AimDirection.ToRotation());
                projectile.position = ModUtils.Bezier(player.Center, player.Center, cp1, cp2, Utils.Clamp(ReverseMovement ? projectile.ai[0]-- : projectile.ai[0]++, 0, 120) / 120f); ;
            }
            else
            {
                projectile.position = Vector2.Lerp(projectile.position, player.Center, projectile.ai[0]++ / 30f);
            }

            projectile.timeLeft = 5;

            projectile.rotation += 0.37f * RotationDirection;

            if (projectile.ai[1]++ >= 60f || HitTile)
            {
                if (projectile.getRect().Intersects(player.getRect()))
                {
                    projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            if (target.type != ModContent.NPCType<IorichHarvesterCrystal>())
            {
                player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount++;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color26 = Color.LightGreen;
            Texture2D texture2D16 = ModContent.GetTexture(AssetDirectory.Melee + "IorichHarvesterProjectile_Afterimage");

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D16, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }

            return base.PreDraw(spriteBatch, lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (!HitTile) projectile.ai[0] = 0;

            HitTile = true;
            projectile.netUpdate = true;

            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            return false;
        }
    }
}