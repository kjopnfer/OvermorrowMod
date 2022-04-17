using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.Bonerang
{
    public class Bonerang : ModProjectile
    {
        private int SavedDMG = 0;
        private int timer = 0;
        private bool ComingBack = false;
        private int flametimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bonerang");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
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
            timer++;
            if (timer == 1)
            {
                SavedDMG = Projectile.damage;
            }

            Projectile.rotation += 0.36f;

            if (Projectile.timeLeft < 80)
            {
                Projectile.timeLeft = 10;
                ComingBack = true;
            }



            if (ComingBack)
            {
                if (flametimer == 1)
                {
                    flametimer++;
                    int DMG = Projectile.damage / 2 - 3;
                    var source = Projectile.GetProjectileSource_FromThis();
                    Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, 7, -7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, Projectile.owner, 0f);
                    Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, 7, 7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, Projectile.owner, 0f);
                    Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, -7, 7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, Projectile.owner, 0f);
                    Projectile.NewProjectile(source, Projectile.Center.X, Projectile.Center.Y, -7, -7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, Projectile.owner, 0f);

                }
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
            flametimer++;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Vector2 eee = Projectile.Center;
            SoundEngine.PlaySound(SoundID.NPCHit, Projectile.position, 2);
            {
                ComingBack = true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = ModContent.Request<Texture2D>("Projectiles/Melee/Bonerang").Value;

            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SoundEngine.PlaySound(SoundID.NPCHit, Projectile.position, 2);
            ComingBack = true;
            flametimer++;
        }
    }
}
