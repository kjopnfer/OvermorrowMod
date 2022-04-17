using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class SapphireProj : ModProjectile
    {
        private bool ComingBack = false;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.timeLeft = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }


        public override void AI()
        {

            Projectile.rotation += 0.43f;

            if (Projectile.timeLeft < 65)
            {
                Projectile.timeLeft = 10;
                ComingBack = true;
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
        public override bool PreDraw(ref Color lightColor)
        {
            if (ComingBack)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Projectiles/Magic/Gems/SapphireProj").Value;

                Vector2 drawOrigin = new Vector2(Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
                for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                    Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Vector2 eee = Projectile.Center;
            SoundEngine.PlaySound(SoundID.Item27, (int)eee.X, (int)eee.Y);
            ComingBack = true;
            return false;
        }
    }
}
