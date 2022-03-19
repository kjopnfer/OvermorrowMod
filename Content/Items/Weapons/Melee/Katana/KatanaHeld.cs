using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee
{
    public class KatanaHeld : HeldSword
    {
        public override string Texture => AssetDirectory.Melee + "Katana/Katana";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Assassin's Katana");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 80;
            //ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            SwingTime = 80;
            holdOffset = 50f;
            base.SetDefaults();
            projectile.width = projectile.height = 50;
            projectile.friendly = true;
            projectile.localNPCHitCooldown = SwingTime;
            projectile.usesLocalNPCImmunity = true;
            projectile.extraUpdates = 4;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }
        public override float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 0f
                ? 0f
                : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        float timer = 0;
        //int timer = 160;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.Reload(BlendState.Additive);

            Player player = Main.player[projectile.owner];
            Texture2D slash = ModContent.GetTexture(AssetDirectory.Textures + "trace_01");
            float mult = Lerp(Utils.InverseLerp(0f, SwingTime, projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + projectile.velocity * (80f - mult * 60f);
            //spriteBatch.Draw(slash, pos - Main.screenPosition, null, new Color(56, 38, 208) * alpha, projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, projectile.scale, SpriteEffects.None, 0f);

            var off = new Vector2(projectile.width / 2f, projectile.height / 2f);
            Texture2D texture2D16 = ModContent.GetTexture(AssetDirectory.Melee + "Katana/Katana_Afterimage");
            int frameHeight = texture2D16.Height / Main.projFrames[projectile.type];
            var frame = new Rectangle(0, frameHeight * projectile.frame, texture2D16.Width, frameHeight - 2);
            var orig = frame.Size() / 2f;

            var trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            var fadeMult = 1f / trailLength;
            for (int i = 0; i < trailLength; i++)
            {
                // This was me trying to skip the index of the first recorded position, it didn't work when I increased the extraUpdates for some reason
                if (i == timer)
                {
                    continue;
                }

                //if (i == 39) continue;


                //var color = Color.Lerp(Color.Blue, Color.Purple, 1f - fadeMult * i);
                var color = i == timer ? Color.Green : new Color(56, 38, 208);

                Main.spriteBatch.Draw(texture2D16, projectile.oldPos[i] - Main.screenPosition + off, frame, color * (1f - fadeMult * i), projectile.oldRot[i] + (projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, projectile.scale * (trailLength - i) / trailLength, projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            timer += 0.25f;

            //timer--;
            spriteBatch.Reload(BlendState.AlphaBlend);

            // draws the main blade
            Texture2D texture = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation + (projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, projectile.scale, projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);


            return false;
        }
    }
}