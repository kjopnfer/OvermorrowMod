using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.BloodEyes
{
    public class BloodyEye : ModProjectile
    {
        private bool ComingBack = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Eyes");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation += .55f;

            if (Projectile.timeLeft < 65)
            {
                Projectile.timeLeft = 10;
                ComingBack = true;
            }

            if (Projectile.timeLeft > 98)
            {
                Projectile.tileCollide = false;
            }
            else if (!ComingBack)
            {
                Projectile.tileCollide = true;
            }

            if (ComingBack)
            {
                float BetweenKill = Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center);
                Projectile.tileCollide = false;
                Vector2 position = Projectile.Center;
                Vector2 targetPosition = Main.player[Projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                Projectile.velocity = direction * 18;
                if (BetweenKill < 22)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            ComingBack = true;
            
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Melee + "BloodEyes/BloodyEye_Trail").Value;

            Color color = Color.Red;
            int num154 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
            int y2 = num154 * Projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, TextureAssets.Projectile[Projectile.type].Value.Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;
            var off = new Vector2(Projectile.width / 2f, Projectile.height / 2f);


            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color2 = color;
                color2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];

                float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] - Main.screenPosition + off, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color2, num165, origin2, scale, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ComingBack = true;
        }
    }
}
