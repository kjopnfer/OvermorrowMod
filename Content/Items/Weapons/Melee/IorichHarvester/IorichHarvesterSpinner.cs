using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.IorichHarvester
{
    public class IorichHarvesterSpinner : ModProjectile
    {
        private bool RunOnce = true;
        private float Radius;
        private int MAX_TIME = 600;

        public Player RotationCenter;
        public Vector2 OldPosition;
        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;
        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override string Texture => AssetDirectory.Boss + "TreeBoss/RuneSpinner";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rune Spinner");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = MAX_TIME;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.DirectionTo(RotationCenter.Center).ToRotation() + (MathHelper.PiOver2 * 3);

            if (RunOnce)
            {
                SoundEngine.PlaySound(SoundID.Item46, Projectile.Center);

                Radius = Projectile.ai[1];
                Projectile.ai[1] = 0;

                RunOnce = false;

                #region Dust Code
                Vector2 vector23 = Projectile.Center + Vector2.One * -20f;
                int num137 = 40;
                int num138 = num137;
                for (int num139 = 0; num139 < 4; num139++)
                {
                    int num140 = Dust.NewDust(vector23, num137, num138, DustID.GemEmerald, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num140].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                }

                for (int num141 = 0; num141 < 10; num141++)
                {
                    int num142 = Dust.NewDust(vector23, num137, num138, DustID.TerraBlade, 0f, 0f, 200, default(Color), 0.7f);
                    Main.dust[num142].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].noLight = true;
                    Dust dust = Main.dust[num142];
                    dust.velocity *= 3f;
                    dust = Main.dust[num142];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num142].position) * (2f + Main.rand.NextFloat() * 4f);
                    num142 = Dust.NewDust(vector23, num137, num138, DustID.TerraBlade, 0f, 0f, 100, default(Color), 0.25f);
                    Main.dust[num142].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * num137 / 2f;
                    dust = Main.dust[num142];
                    dust.velocity *= 2f;
                    Main.dust[num142].noGravity = true;
                    Main.dust[num142].fadeIn = 1f;
                    Main.dust[num142].color = Color.Crimson * 0.5f;
                    Main.dust[num142].noLight = true;
                    dust = Main.dust[num142];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num142].position) * 8f;
                }

                for (int num143 = 0; num143 < 10; num143++)
                {
                    int num144 = Dust.NewDust(vector23, num137, num138, DustID.TerraBlade, 0f, 0f, 0, default(Color), 0.7f);
                    Main.dust[num144].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(Projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num144].noGravity = true;
                    Main.dust[num144].noLight = true;
                    Dust dust = Main.dust[num144];
                    dust.velocity *= 3f;
                    dust = Main.dust[num144];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num144].position) * 2f;
                }

                for (int num145 = 0; num145 < 50; num145++)
                {
                    int num146 = Dust.NewDust(vector23, num137, num138, DustID.GemEmerald, 0f, 0f, 0, default(Color), 0.25f);
                    Main.dust[num146].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy(Projectile.velocity.ToRotation()) * num137 / 2f;
                    Main.dust[num146].noGravity = true;
                    Dust dust = Main.dust[num146];
                    dust.velocity *= 3f;
                    dust = Main.dust[num146];
                    dust.velocity += Projectile.DirectionTo(Main.dust[num146].position) * 3f;
                }
                #endregion
            }

            if (RotationCenter.HeldItem.type == ModContent.ItemType<IorichHarvester>())
            {
                Projectile.timeLeft = 5;
            }

            Projectile.Center = RotationCenter.Center + new Vector2(Radius, 0).RotatedBy(Projectile.ai[0]);

            Projectile.ai[0] += MathHelper.Lerp(0, 0.065f, Utils.Clamp(Projectile.ai[1], 0, 90f) / 90f);

            Projectile.ai[1]++;
            Projectile.localAI[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            OvermorrowModPlayer player = Main.player[Projectile.owner].GetModPlayer<OvermorrowModPlayer>();

            if (Projectile.timeLeft < MAX_TIME - 60)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Boss + "TreeBoss/RuneSpinner_Trail").Value;

                int num154 = TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type];
                int y2 = num154 * Projectile.frame;
                Rectangle drawRectangle = new Rectangle(0, y2, TextureAssets.Projectile[Projectile.type].Value.Width, num154);

                Vector2 origin2 = drawRectangle.Size() / 2f;
                var off = new Vector2(Projectile.width / 2f, Projectile.height / 2f);

                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
                {
                    Color color2 = player.ScytheHitCount >= 3 ? new Color(135, 255, 141) : new Color(119, 150, 116);
                    color2 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                    float num165 = Projectile.oldRot[i];
                    float scale = Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];

                    Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + off, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), Color.Lerp(Color.Transparent, color2, Utils.Clamp(player.ScytheHitCount, 0, 3) / 3f), num165, origin2, scale, SpriteEffects.None, 0f);
                }
            }

            return base.PreDraw(ref lightColor);
        }
    }
}