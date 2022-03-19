using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
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
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.timeLeft = 100;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            timer++;
            if (timer == 1)
            {
                SavedDMG = projectile.damage;
            }

            projectile.rotation += 0.36f;

            if (projectile.timeLeft < 80)
            {
                projectile.timeLeft = 10;
                ComingBack = true;
            }



            if (ComingBack)
            {
                if (flametimer == 1)
                {
                    flametimer++;
                    int DMG = projectile.damage / 2 - 3;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 7, -7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 7, 7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -7, 7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, -7, -7, ModContent.ProjectileType<BoneProj>(), DMG, 3f, projectile.owner, 0f);

                }
                float BetweenKill = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
                projectile.tileCollide = false;
                Vector2 position = projectile.Center;
                Vector2 targetPosition = Main.player[projectile.owner].Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 18;
                if (BetweenKill < 22)
                {
                    projectile.Kill();
                }
            }
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            flametimer++;
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Vector2 eee = projectile.Center;
            Main.PlaySound(SoundID.NPCHit, projectile.position, 2);
            {
                ComingBack = true;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D texture = mod.GetTexture("Projectiles/Melee/Bonerang");

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = projectile.GetAlpha(Color.White) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(SoundID.NPCHit, projectile.position, 2);
            ComingBack = true;
            flametimer++;
        }
    }
}
