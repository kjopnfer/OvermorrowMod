using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;     
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 1f, 0f);
            if (Projectile.soundDelay == 0 && Projectile.ai[1] > 10)
            {
                Projectile.soundDelay = 16;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            Player player = Main.player[Projectile.owner];
            player.itemAnimation = 10;
            player.itemTime = 10;

            if (RunOnce)
            {
                if (Projectile.ai[1] == 1)
                {
                    ReverseMovement = true;
                }

                Projectile.spriteDirection = player.direction;
                RotationDirection = Projectile.spriteDirection;

                Projectile.ai[1] = 0;
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
                Projectile.position = ModUtils.Bezier(player.Center, player.Center, cp1, cp2, Utils.Clamp(ReverseMovement ? Projectile.ai[0]-- : Projectile.ai[0]++, 0, 120) / 120f); ;
            }
            else
            {
                Projectile.position = Vector2.Lerp(Projectile.position, player.Center, Projectile.ai[0]++ / 30f);
            }

            Projectile.timeLeft = 5;

            Projectile.rotation += 0.37f * RotationDirection;

            if (Projectile.ai[1]++ >= 60f || HitTile)
            {
                if (Projectile.getRect().Intersects(player.getRect()))
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];
            if (target.type != ModContent.NPCType<IorichHarvesterCrystal>())
            {
                player.GetModPlayer<OvermorrowModPlayer>().ScytheHitCount++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color26 = Color.LightGreen;
            Texture2D texture2D16 = ModContent.Request<Texture2D>(AssetDirectory.Melee + "IorichHarvester/IorichHarvesterProjectile_Afterimage").Value;

            int num154 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y2 = num154 * Projectile.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, TextureAssets.Projectile[Projectile.type].Value.Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D16, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color27, num165, origin2, Projectile.scale, SpriteEffects.None, 0f);
            }

            return base.PreDraw(ref lightColor);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            if (!HitTile) Projectile.ai[0] = 0;

            HitTile = true;
            Projectile.netUpdate = true;

            SoundEngine.PlaySound(SoundID.Dig, (int)Projectile.position.X, (int)Projectile.position.Y);
            return false;
        }
    }
}