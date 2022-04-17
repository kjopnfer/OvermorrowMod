using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent;
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
            //ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            SwingTime = 80;
            holdOffset = 50f;
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.localNPCHitCooldown = SwingTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 4;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override float Lerp(float val)
        {
            return val == 1f ? 1f : (val == 0f
                ? 0f
                : (float)Math.Pow(2, val * 10f - 10f) / 2f);
        }

        float timer = 0;
        //int timer = 160;
        public override bool PreDraw(ref Color lightColor)
        {
            // TODO: Figure out what to do with these
            // spriteBatch.Reload(BlendState.Additive);

            Player player = Main.player[Projectile.owner];
            Texture2D slash = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
            float mult = Lerp(Utils.GetLerpValue(0f, SwingTime, Projectile.timeLeft));
            float alpha = (float)Math.Sin(mult * Math.PI);
            Vector2 pos = player.Center + Projectile.velocity * (80f - mult * 60f);
            //spriteBatch.Draw(slash, pos - Main.screenPosition, null, new Color(56, 38, 208) * alpha, Projectile.velocity.ToRotation() - MathHelper.PiOver2, slash.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            var off = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            Texture2D texture2D16 = ModContent.Request<Texture2D>(AssetDirectory.Melee + "Katana/Katana_Afterimage").Value;
            int frameHeight = texture2D16.Height / Main.projFrames[Projectile.type];
            var frame = new Rectangle(0, frameHeight * Projectile.frame, texture2D16.Width, frameHeight - 2);
            var orig = frame.Size() / 2f;

            var trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
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

                Main.spriteBatch.Draw(texture2D16, Projectile.oldPos[i] - Main.screenPosition + off, frame, color * (1f - fadeMult * i), Projectile.oldRot[i] + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale * (trailLength - i) / trailLength, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            }

            timer += 0.25f;

            //timer--;
            // spriteBatch.Reload(BlendState.AlphaBlend);

            // draws the main blade
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation + (Projectile.ai[1] == -1 ? 0 : MathHelper.PiOver2 * 3), orig, Projectile.scale, Projectile.ai[1] == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);


            return false;
        }
    }
}